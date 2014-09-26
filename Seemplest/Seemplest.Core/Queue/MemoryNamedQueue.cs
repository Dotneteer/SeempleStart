using System;
using System.Collections.Generic;
using System.Linq;
using Seemplest.Core.Configuration;

namespace Seemplest.Core.Queue
{
    /// <summary>
    /// This class implements a named queue in the memory.
    /// </summary>
    /// <remarks>
    /// This queue is not optimized for performance, it is intended to use for
    /// test purposes, or only in situations where the queue lenght is expected 
    /// to be only a few dozen items.
    /// </remarks>
    public sealed class MemoryNamedQueue : INamedQueue
    {
        private readonly object _locker = new object();
        private readonly ICollection<MemoryPoppedMessage> _messages =
            new HashSet<MemoryPoppedMessage>();

        /// <summary>
        /// Gets the name of the queue instance.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// True, if this MemoryNamedQueue was deleted from its provider
        /// </summary>
        public bool IsDeleted { get; private set; }

        /// <summary>
        /// Initializes a new instance of the memory queue.
        /// </summary>
        public MemoryNamedQueue(string name)
        {
            Name = name;
            IsDeleted = false;
        }

        /// <summary>
        /// Deletes all messages of the queue, and marks that this queue is deleted from its provider
        /// </summary>
        internal void DeleteQueue()
        {
            Clear();
            IsDeleted = true;
        }

        /// <summary>
        /// Puts a new message to the queue.
        /// </summary>
        /// <param name="content">Message content</param>
        /// <param name="timeToLiveInSeconds">Time to live in seconds</param>
        public void PutMessage(string content, int timeToLiveInSeconds)
        {
            CheckIfDeleted();
            lock (_locker)
            {
                _messages.Add(new MemoryPoppedMessage
                {
                    MessageText = content,
                    ExpirationTime = EnvironmentInfo.GetCurrentDateTimeUtc().AddSeconds(timeToLiveInSeconds),
                    DequeueCount = 0,
                    InsertionTime = EnvironmentInfo.GetCurrentDateTimeUtc(),
                    Id = Guid.NewGuid().ToString(),
                    PopReceipt = null,
                    NextVisibleTime = DateTime.MinValue,
                });
            }
        }

        /// <summary>
        /// Gets messages from the queue.
        /// </summary>
        /// <param name="numberOfMessages">Number of messages to retrieve from the queue</param>
        /// <param name="visibilityTimeoutInSeconds">
        /// Timeot while messages are invisible for other GetMessages requests</param>
        /// <returns>The collection of messages obtained from the queue.</returns>
        public IEnumerable<IPoppedMessage> GetMessages(int numberOfMessages, int visibilityTimeoutInSeconds)
        {
            CheckIfDeleted();
            lock (_locker)
            {
                var now = EnvironmentInfo.GetCurrentDateTimeUtc();
                var result = (from m in _messages
                              where m.NextVisibleTime < now && m.ExpirationTime > now
                              orderby m.InsertionTime descending
                              select m).Take(numberOfMessages).ToList();
                var copyResult = new List<MemoryPoppedMessage>();
                foreach (var message in result)
                {
                    message.DequeueCount++;
                    message.PopReceipt = Guid.NewGuid().ToString();
                    message.NextVisibleTime = now.AddSeconds(visibilityTimeoutInSeconds);
                    var copyAdd = new MemoryPoppedMessage
                    {
                        DequeueCount = message.DequeueCount,
                        ExpirationTime = message.ExpirationTime,
                        Id = message.Id,
                        InsertionTime = message.InsertionTime,
                        MessageText = message.MessageText,
                        PopReceipt = message.PopReceipt,
                        NextVisibleTime = message.NextVisibleTime
                    };
                    copyResult.Add(copyAdd);
                }
                return copyResult;
            }
        }


        /// <summary>
        /// Checks the queue for messages without locking or removing the messages from the queue.
        /// </summary>
        /// <param name="numberOfMessages">Number of messages to peek it the queue</param>
        /// <returns>The collection of messages obtained from the queue.</returns>
        public IEnumerable<IQueuedMessage> PeekMessages(int numberOfMessages)
        {

            CheckIfDeleted();
            lock (_locker)
            {
                var now = EnvironmentInfo.GetCurrentDateTimeUtc();
                var result = (from m in _messages
                              where m.NextVisibleTime < now && m.ExpirationTime > now
                              orderby m.InsertionTime descending
                              select m).Take(numberOfMessages).ToList();
                return result;
            }
        }

        /// <summary>
        /// Deletes the specified message from the queue.
        /// </summary>
        /// <param name="poppedMessage">Message to delete from the queue</param>
        public void DeleteMessage(IPoppedMessage poppedMessage)
        {
            CheckIfDeleted();
            lock (_locker)
            {
                var message = _messages.Single(m => m.Id == poppedMessage.Id &&
                                                    m.PopReceipt == poppedMessage.PopReceipt &&
                                                    m.NextVisibleTime >= EnvironmentInfo.GetCurrentDateTimeUtc());
                if (message != null) _messages.Remove(message);
            }
        }

        /// <summary>
        /// Clears the entire content of the queue.
        /// </summary>
        public void Clear()
        {
            _messages.Clear();
        }

        /// <summary>
        /// Gets the approximate count of messages.
        /// </summary>
        /// <returns>Approximate count of messages</returns>
        public int GetApproximateMessageCount()
        {
            return _messages.Count;
        }

        /// <summary>
        /// If this queue is deleted from its provider, throws an InvalidOperationException
        /// </summary>
        private void CheckIfDeleted()
        {
            if (IsDeleted) throw new InvalidOperationException(
                String.Format("The MemoryNamedQueue {0} is already deleted.", Name));
        }
    }
}