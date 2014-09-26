using System;

namespace Seemplest.Core.Queue
{
    /// <summary>
    /// This interface defines the behavior of a message popped from a queue.
    /// </summary>
    public interface IPoppedMessage : IQueuedMessage
    {
        /// <summary>
        /// The time point when the popped message is visible again in the queue.
        /// </summary>
        DateTime NextVisibleTime { get; }

        /// <summary>
        /// Receipt identifying the popped item.
        /// </summary>
        string PopReceipt { get; }
    }
}