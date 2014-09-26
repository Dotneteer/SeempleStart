using System;

namespace Seemplest.Core.Queue
{
    /// <summary>
    /// This class implements a queued 
    /// </summary>
    public class MemoryQueuedMessage : IQueuedMessage
    {
        /// <summary>String describing a message</summary>
        public string MessageText { get; set; }

        /// <summary>
        /// Gets the identifier of the message
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets the time of message insertion
        /// </summary>
        public DateTime InsertionTime { get; set; }

        /// <summary>
        /// Gets the time of message expiration
        /// </summary>
        public DateTime ExpirationTime { get; set; }

        /// <summary>
        /// Gets the count indicating how many times the message has been dequeued.
        /// </summary>
        public int DequeueCount { get; set; }
    }
}