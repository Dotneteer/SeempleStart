using System;

namespace Seemplest.Core.Queue
{
    /// <summary>
    /// This class represents a message popped from a memory queue.
    /// </summary>
    public class MemoryPoppedMessage : MemoryQueuedMessage, IPoppedMessage
    {
        /// <summary>
        /// Receipt identifying the popped item.
        /// </summary>
        public string PopReceipt { get; set; }

        /// <summary>
        /// The time point when the popped message is visible again in the queue.
        /// </summary>
        public DateTime NextVisibleTime { get; set; }
    }
}