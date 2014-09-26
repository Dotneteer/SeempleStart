using System.Collections.Generic;

namespace Seemplest.Core.Queue
{
    /// <summary>
    /// This interface defines the responsibilities of a queue.
    /// </summary>
    public interface IQueue
    {
        /// <summary>
        /// Puts a new message to the queue.
        /// </summary>
        /// <param name="content">Message content</param>
        /// <param name="timeToLiveInSeconds">Time to live in seconds</param>
        void PutMessage(string content, int timeToLiveInSeconds);

        /// <summary>
        /// Gets messages from the queue.
        /// </summary>
        /// <param name="numberOfMessages">Number of messages to retrieve from the queue</param>
        /// <param name="visibilityTimeoutInSeconds">
        /// Timeot while messages are invisible for other GetMessages requests</param>
        /// <returns>The collection of messages obtained from the queue.</returns>
        IEnumerable<IPoppedMessage> GetMessages(int numberOfMessages, int visibilityTimeoutInSeconds);

        /// <summary>
        /// Checks the queue for messages without locking or removing the messages from the queue.
        /// </summary>
        /// <param name="numberOfMessages">Number of messages to peek it the queue</param>
        /// <returns>The collection of messages obtained from the queue.</returns>
        IEnumerable<IQueuedMessage> PeekMessages(int numberOfMessages);

        /// <summary>
        /// Deletes the specified message from the queue.
        /// </summary>
        /// <param name="poppedMessage">Message to delete from the queue</param>
        void DeleteMessage(IPoppedMessage poppedMessage);

        /// <summary>
        /// Clears the entire content of the queue.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets the approximate count of messages.
        /// </summary>
        /// <returns>Approximate count of messages</returns>
        int GetApproximateMessageCount();
    }
}