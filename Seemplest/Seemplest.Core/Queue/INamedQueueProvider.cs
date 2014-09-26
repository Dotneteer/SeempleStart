using System.Collections.Generic;

namespace Seemplest.Core.Queue
{
    /// <summary>
    /// This interface is responsible for managing named queues.
    /// </summary>
    public interface INamedQueueProvider
    {
        /// <summary>
        /// Gets the named queue instance.
        /// </summary>
        /// <param name="name">Queue name</param>
        /// <returns>Queue instance if the named queue exists; otherwise, null</returns>
        INamedQueue GetQueue(string name);

        /// <summary>
        /// Gets the list of exisiting queues.
        /// </summary>
        /// <returns>Collection of existing queue instances.</returns>
        IEnumerable<INamedQueue> ListQueues();

        /// <summary>
        /// Creates a new queue with the specified name.
        /// </summary>
        /// <param name="name">Queue name</param>
        /// <returns>The newly created queue instance.</returns>
        INamedQueue CreateQueue(string name);

        /// <summary>
        /// Deletes the specified queue.
        /// </summary>
        /// <param name="name">name of the queue to delete.</param>
        void DeleteQueue(string name);
    }
}