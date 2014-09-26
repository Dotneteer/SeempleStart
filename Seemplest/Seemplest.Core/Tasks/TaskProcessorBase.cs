using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Seemplest.Core.PerformanceCounters;
using Seemplest.Core.WindowsEventLog;

namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This abstract class implements the basic behaviour of a task processor.
    /// </summary>
    public abstract class TaskProcessorBase : ITaskProcessor
    {
        private const int DEFAULT_SLEEP_TIME_SLICE = 100;
        private const int DEFAULT_STOP_TIMEOUT_SECONDS = 10;

        private bool _isStopRequested;
        private Task _executorTask;

        protected PerformanceCounterHandle NumTasksPmc;
        protected PerformanceCounterHandle NumTasksPerSecondPmc;
        protected PerformanceCounterHandle NumFailuresPmc;
        protected PerformanceCounterHandle NumFailuresPerSecondPmc;
        protected PerformanceCounterHandle LastProcessTimePmc;

        /// <summary>
        /// Gets or sets the name of task processor
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the execution context of the task
        /// </summary>
        public ITaskExecutionContext Context { get; private set; }

        /// <summary>
        /// Gets or sets the frequency of the query that checks whether there is any
        /// new task to execute.
        /// </summary>
        public virtual int TaskSleepInMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets the timeout to wait while running tasks stop.
        /// </summary>
        public TimeSpan StopTimeout { get; set; }

        /// <summary>
        /// Gets or sets the flag indicating if the process is stopped.
        /// </summary>
        // ReSharper disable ConvertToAutoProperty
        public bool IsStopRequested
        // ReSharper restore ConvertToAutoProperty
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return _isStopRequested; }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { _isStopRequested = value; }
        }

        /// <summary>
        /// Gest the cancellation token this task processor uses.
        /// </summary>
        public CancellationToken CancellationToken
        {
            get { return Context.CancellationTokenSource.Token; }
        }

        /// <summary>
        /// Stores the last return value of HasAnyTaskToExecute
        /// </summary>
        protected bool WasAnyTaskExecuted { get; set; }

        /// <summary>
        /// Creates a new instance of this class
        /// </summary>
        protected TaskProcessorBase()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            TaskSleepInMilliseconds = DEFAULT_SLEEP_TIME_SLICE;
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
            StopTimeout = TimeSpan.FromSeconds(DEFAULT_STOP_TIMEOUT_SECONDS);
            IsStopRequested = false;
        }

        /// <summary>
        /// Creates a new task processor and binds it to the specified context.
        /// </summary>
        /// <param name="context">Task execution context</param>
        protected TaskProcessorBase(ITaskExecutionContext context)
            : this()
        {
            SetContext(context);
        }

        /// <summary>
        /// Sets the execution context of the task processor
        /// </summary>
        /// <param name="context">Task execution context</param>
        public void SetContext(ITaskExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            Context = context;
        }

        /// <summary>
        /// Starts processing tasks.
        /// </summary>
        public virtual void Start()
        {
            IsStopRequested = false;
            EnsurePerformanceCounters();
            _executorTask = new Task(RunAsync);
            _executorTask.Start();
            WindowsEventLogger.Log<TaskProcessorStarted>(
                String.Format("Task processor '{0}' has been started.", GetType()));
        }

        /// <summary>
        /// Stops processing any new task and requests cancelling tasks already 
        /// under progress.
        /// </summary>
        public virtual void Stop()
        {
            if (_executorTask == null) return;
            IsStopRequested = true;
            Context.CancellationTokenSource.Cancel();
            var stoppedInTime = _executorTask.Wait(StopTimeout);
            if (!stoppedInTime)
                WindowsEventLogger.Log<TaskProcessorStoppedWithTimeout>(
                    String.Format("Task processor '{0}' has been stopped, but did not " +
                    "stop within timeout.", GetType()));
            else
            {
                WindowsEventLogger.Log<TaskProcessorStopped>(
                    String.Format("Task processor '{0}' has been stopped.", GetType()));
                _executorTask.Dispose();
                _executorTask = null;
            }
        }

        /// <summary>
        /// The body of the task processor
        /// </summary>
        public virtual void RunAsync()
        {
            while (true)
            {
                // --- Complete the task when cancellation is requested
                if (ShouldStopTaskProcessing) return;

                // --- Check for tasks to execute
                WasAnyTaskExecuted = HasAnyTaskToExecute();
                if (WasAnyTaskExecuted && !IsStopRequested)
                    ProcessTasks();

                // --- Sleep for a while, according to TaskSleepInMilliseconds
                var remainingSleepTime = TaskSleepInMilliseconds;
                while (remainingSleepTime > 0)
                {
                    var sleepPeriod = remainingSleepTime > DEFAULT_SLEEP_TIME_SLICE
                        ? DEFAULT_SLEEP_TIME_SLICE
                        : remainingSleepTime;
                    if (ShouldStopTaskProcessing) return;
                    Thread.Sleep(sleepPeriod);
                    remainingSleepTime -= sleepPeriod;
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Gets the flag indicating whether the task processing should be stopped either
        /// because the task processor has been stopped or cancelled.
        /// </summary>
        protected bool ShouldStopTaskProcessing
        {
            get { return CancellationToken.IsCancellationRequested || IsStopRequested; }
        }

        /// <summary>
        /// Checks wether there is any task to execute
        /// </summary>
        /// <returns>
        /// True, if there is any task that can be executed; otherwise, false.
        /// </returns>
        protected abstract bool HasAnyTaskToExecute();

        /// <summary>
        /// Processes tasks that are ready to run.
        /// </summary>
        protected abstract void ProcessTasks();

        /// <summary>
        /// Obtains all performance counter handles used by the task processor.
        /// </summary>
        protected virtual void EnsurePerformanceCounters()
        {
            if (NumTasksPmc == null)
                NumTasksPmc = PmcManager.GetCounter<TasksProcessedPmc>(Name);
            if (NumTasksPerSecondPmc == null)
                NumTasksPerSecondPmc = PmcManager.GetCounter<TasksProcessedPerSecondsPmc>(Name);
            if (NumFailuresPmc == null)
                NumFailuresPmc = PmcManager.GetCounter<TasksFailedPmc>(Name);
            if (NumFailuresPerSecondPmc == null)
                NumFailuresPerSecondPmc = PmcManager.GetCounter<TasksFailedPerSecondsPmc>(Name);
            if (LastProcessTimePmc == null)
                LastProcessTimePmc = PmcManager.GetCounter<LastTaskProcessingTimePmc>(Name);
        }
    }
}