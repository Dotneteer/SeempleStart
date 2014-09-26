namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This interface defines the responsibility of an object that can process 
    /// messages in a queue.
    /// </summary>
    public interface IQueuedTaskProcessor : ITaskProcessor
    {
        /// <summary>
        /// Gets or sets the policy used to peek the queue.
        /// </summary>
        IQueuePeekPolicy PeekPolicy { get; set; }

        /// <summary>
        /// Resource key of the queue storing task requests processed by this queue
        /// </summary>
        string RequestQueueKey { get; set; }
    }
}