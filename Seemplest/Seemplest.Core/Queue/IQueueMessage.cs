using System;

namespace Seemplest.Core.Queue
{
    /// <summary>
    /// This interface represents the content of a message put in a queue.
    /// </summary>
    public interface IQueuedMessage : IMessage
    {
        /// <summary>
        /// Gets the identifier of the message
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the time of message insertion
        /// </summary>
        DateTime InsertionTime { get; }

        /// <summary>
        /// Gets the time of message expiration
        /// </summary>
        DateTime ExpirationTime { get; }

        /// <summary>
        /// Gets the count indicating how many times the message has been dequeued.
        /// </summary>
        int DequeueCount { get; }
    }
}