using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Seemplest.Core.Configuration.ResourceConnections;
using Seemplest.Core.Queue;
using Seemplest.Core.WindowsEventLog;

namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This class implements a task that uses messages coming from a queue and 
    /// processes them.
    /// </summary>
    /// <typeparam name="TTask">Task type</typeparam>
    /// <typeparam name="TArgument">Type of argument handled by this processor</typeparam>
    public class QueuedTaskProcessor<TTask, TArgument> :
        TaskProcessorBase,
        IQueuedTaskProcessor
        where TTask : class, ITask<TArgument>, new()
    {
        private const int VISIBILITY_TIMEOUT_IN_SECONDS = 60;
        private const int MAX_TIMEOUT_VALUE = 16;
        private const int LAST_TIMEOUT_VALUE = 1;

        private string _requestQueueKey;
        private INamedQueue _requestQueue;
        private IEnumerable<IPoppedMessage> _messagesObtained;
        private TimeSpan _lastPeekWaitTime = TimeSpan.FromSeconds(LAST_TIMEOUT_VALUE);
        private int _defaultQueryPeekWaitTime;

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        public QueuedTaskProcessor()
        {
            Init();
        }

        /// <summary>
        /// Creates a new instance of task processor bound to the specified execution context.
        /// </summary>
        /// <param name="context">Task execution context</param>
        public QueuedTaskProcessor(ITaskExecutionContext context)
            : base(context)
        {
            Init();
        }

        /// <summary>
        /// Initializes an instance of this class
        /// </summary>
        private void Init()
        {
            VisibilityTimeoutInSeconds = VISIBILITY_TIMEOUT_IN_SECONDS;
            DoNotPeekWhenCheckingTasks = false;
            MaxMessagesReadFromQueue = 1;
            MaxDequeuCountBeforeDrop = 3;
        }

        /// <summary>Gets or sets the maximum time to wait before a peek operation.</summary>
        /// <remarks>By default it is 16 seconds.</remarks>
        protected TimeSpan MaxPeekWaitTime = TimeSpan.FromSeconds(MAX_TIMEOUT_VALUE);

        /// <summary>The last peek time, by default 1 second</summary>

        /// <summary>
        /// Gets or sets the default processing visibility timeout in seconds
        /// </summary>
        public int VisibilityTimeoutInSeconds { get; set; }

        /// <summary>
        /// Single the flag indicating in no peek operation should be used, but messages
        /// should be instantly obtained from the queue.
        /// </summary>
        /// <remarks>True value indicates no peek operation.</remarks>
        public bool DoNotPeekWhenCheckingTasks { get; set; }

        /// <summary>
        /// Gets the maximum number dequeue counter for messages before dropping them.
        /// </summary>
        /// <remarks>
        /// If the dequeue counter of a message exceeds this value, we drop the message 
        /// instead of processing.
        /// </remarks>
        public int MaxDequeuCountBeforeDrop { get; set; }

        /// <summary>
        /// Maximum number of messages read from the queue at once.
        /// </summary>
        public int MaxMessagesReadFromQueue { get; set; }

        /// <summary>
        /// Gets or sets the frequency of the query that checks whether there is any
        /// new task to execute.
        /// </summary>
        public override int TaskSleepInMilliseconds
        {
            get { return PeekPolicy == null ? NextPeekTimeInMilliseconds() : PeekPolicy.NextPeekTimeInMilliseconds(); }
            set
            {
                if (value > MaxPeekWaitTime.TotalMilliseconds)
                {
                    _defaultQueryPeekWaitTime = (int)MaxPeekWaitTime.TotalMilliseconds;
                    _lastPeekWaitTime = MaxPeekWaitTime;
                }
                else
                {
                    _defaultQueryPeekWaitTime = value;
                    _lastPeekWaitTime = TimeSpan.FromMilliseconds(value / 2.0);
                }
            }
        }

        /// <summary>
        /// Checks wether there is any task to execute
        /// </summary>
        /// <returns>
        /// True, if there is any task that can be executed; otherwise, false.
        /// </returns>
        protected override bool HasAnyTaskToExecute()
        {
            if (DoNotPeekWhenCheckingTasks)
            {
                _messagesObtained = _requestQueue
                    .GetMessages(MaxMessagesReadFromQueue, VisibilityTimeoutInSeconds);
                return _messagesObtained.Any();
            }
            return (_requestQueue.PeekMessages(1).Any());
        }

        /// <summary>
        /// Processes tasks that are ready to run.
        /// </summary>
        protected override void ProcessTasks()
        {
            if (!DoNotPeekWhenCheckingTasks)
            {
                // --- We've just peeked the queue, however have not obtained messages.
                _messagesObtained = _requestQueue
                    .GetMessages(MaxMessagesReadFromQueue, VisibilityTimeoutInSeconds);
            }

            // --- Messages are ready to be processed
            foreach (var message in _messagesObtained)
            {
                if (ShouldStopTaskProcessing) return;
                if (MaxDequeuCountBeforeDrop > 0 && message.DequeueCount > MaxDequeuCountBeforeDrop)
                {
                    // --- Drop this message (probably poisoning message)
                    _requestQueue.DeleteMessage(message);
                    // --- The message procession failed, log this failure.
                    WindowsEventLogger.Log<PoisoningMessageFound>(
                        "Message with id {0} could not be successfully dequed and processed " +
                            "after {1} attempts.\nQueue name: {2}\nMessage: {3}", message.Id, MaxDequeuCountBeforeDrop,
                            _requestQueue.Name, message.MessageText);
                    continue;
                }

                try
                {
                    if (ProcessMessage(message)) return;
                }
                catch (OperationCanceledException ex)
                {
                    // --- The message procession canceled, log this failure.
                    WindowsEventLogger.Log<TaskExecutionInterrupted>(
                        "Task execution interrupted while processing message {0}. Queue name: {1}. Abort message {2}",
                            message.Id, _requestQueue.Name, ex.ToString());
                }
                catch (Exception ex)
                {
                    // --- The message procession failed, log this failure.
                    WindowsEventLogger.Log<TaskExecutionFailed>(
                        "Task execution failed while processing message {0}. Queue name: {1}. Error message {2}",
                            message.Id, _requestQueue.Name, ex.ToString());
                }
            }
        }

        /// <summary>
        /// Processes a single message
        /// </summary>
        /// <param name="message">Message to process</param>
        /// <returns></returns>
        private bool ProcessMessage(IPoppedMessage message)
        {
            if (ShouldStopTaskProcessing) return true;
            var task = new TTask();

            // --- Set input arguments
            task.Argument = task.ArgumentConverter.ConvertToArgument(message.MessageText);

            // --- Prepare task to run
            var cancel = false;
            OnTaskProcessing(task, ref cancel);

            var stopwatch = new Stopwatch();
            try
            {
                // --- Setup with graceful cancellation
                if (ShouldStopTaskProcessing) return true;
                task.Setup(Context);

                // --- Run with graceful cancellation
                CancellationToken.ThrowIfCancellationRequested();
                task.Run();
                OnTaskProcessed(task);
                NumTasksPmc.Increment();
                NumTasksPerSecondPmc.Increment();

                // --- At this point the message should be removed from the queue
                _requestQueue.DeleteMessage(message);
                return false;
            }
            catch
            {
                NumFailuresPmc.Increment();
                NumFailuresPerSecondPmc.Increment();
                throw;
            }
            finally
            {
                LastProcessTimePmc.RawValue = (int)stopwatch.ElapsedMilliseconds;
                task.Dispose();
            }
        }

        /// <summary>
        /// Override this method for prepare the specified task.
        /// </summary>
        /// <param name="task">Task to process</param>
        /// <param name="cancel">Set true to cancel this task.</param>
        protected virtual void OnTaskProcessing(TTask task, ref bool cancel)
        {
        }

        /// <summary>
        /// Override this method to process a task
        /// </summary>
        /// <param name="task"></param>
        protected virtual void OnTaskProcessed(TTask task)
        {
        }

        /// <summary>
        /// Gets or sets the policy used to peek the queue.
        /// </summary>
        public IQueuePeekPolicy PeekPolicy { get; set; }

        /// <summary>
        /// Resource key of the queue storing task requests processed by this queue
        /// </summary>
        public string RequestQueueKey
        {
            get { return _requestQueueKey; }
            set
            {
                if (_requestQueueKey == value) return;
                _requestQueueKey = value;
                _requestQueue = ResourceConnectionFactory.CreateResourceConnection<INamedQueue>(value);
                if (_requestQueue == null)
                {
                    throw new ConfigurationErrorsException("No request queue is defined.");
                }
            }
        }

        /// <summary>
        /// Gets the timespan from the last peek of the request queue to make the next
        /// peek, in milliseconds.
        /// </summary>
        /// <returns>
        /// Number of milliseconds from the last peek of the request queue to make the next
        /// peek.
        /// </returns>
        public int NextPeekTimeInMilliseconds()
        {
            if (_requestQueue == null)
                return (int)_lastPeekWaitTime.TotalMilliseconds;

            if (!WasAnyTaskExecuted)
            {
                if (_lastPeekWaitTime < MaxPeekWaitTime)
                    _lastPeekWaitTime = _lastPeekWaitTime + _lastPeekWaitTime;

                if (_lastPeekWaitTime > MaxPeekWaitTime)
                    _lastPeekWaitTime = MaxPeekWaitTime;
            }
            else
                _lastPeekWaitTime = TimeSpan.FromMilliseconds(_defaultQueryPeekWaitTime);

            return (int)_lastPeekWaitTime.TotalMilliseconds;
        }
    }
}