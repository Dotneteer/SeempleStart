namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This interface defines the responsibility of an object that can process 
    /// messages in a queue, and put back the result into another queue.
    /// </summary>
    public interface IQueuedTaskProcessorWithResult :
        IQueuedTaskProcessor
    {
        /// <summary>
        /// Key of the queue storing task responses processed by this queue
        /// </summary>
        string ResponseQueueKey { get; set; }
    }
}