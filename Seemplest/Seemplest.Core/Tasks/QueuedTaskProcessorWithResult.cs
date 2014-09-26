using System.Configuration;
using Seemplest.Core.Configuration.ResourceConnections;
using Seemplest.Core.Queue;

namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This class implements a task that uses messages coming from a queue and 
    /// processes them. The results are written back to a response queue.
    /// </summary>
    /// <typeparam name="TTask">Task type</typeparam>
    /// <typeparam name="TArgument">Type of argument handled by this processor</typeparam>
    /// <typeparam name="TResult">Type of result handled by this processor</typeparam>
    public class QueuedTaskProcessor<TTask, TArgument, TResult> :
        QueuedTaskProcessor<TTask, TArgument>,
        IQueuedTaskProcessorWithResult
        where TTask : class, ITask<TArgument, TResult>, new()
    {
        private const int DEFAULT_TIME_TO_LIVE = 3600;

        private string _responseQueueKey;
        private INamedQueue _responseQueue;

        /// <summary>
        /// Creates a new instance of this task processor
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
        /// Initializes this instance
        /// </summary>
        private void Init()
        {
            ResultTimeToLiveInSeconds = DEFAULT_TIME_TO_LIVE;
        }

        /// <summary>
        /// Gets or sets the default processing visibility timeout in seconds
        /// </summary>
        public int ResultTimeToLiveInSeconds { get; set; }

        /// <summary>
        /// Key of the queue storing task responses processed by this queue
        /// </summary>
        public string ResponseQueueKey
        {
            get { return _responseQueueKey; }
            set
            {
                if (_responseQueueKey == value) return;
                _responseQueueKey = value;
                _responseQueue = ResourceConnectionFactory.CreateResourceConnection<INamedQueue>(value);
                if (_responseQueue == null)
                {
                    throw new ConfigurationErrorsException("No response queue is defined.");
                }
            }
        }

        /// <summary>
        /// Override this method to process a task
        /// </summary>
        /// <param name="task">Task executed</param>
        protected override void OnTaskProcessed(TTask task)
        {
            base.OnTaskProcessed(task);
            var resultMessage = task.ResultConverter.ConvertToResult(task.Result);
            _responseQueue.PutMessage(resultMessage, ResultTimeToLiveInSeconds);
        }
    }
}