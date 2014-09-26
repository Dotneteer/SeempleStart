using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Seemplest.Core.Queue
{
    /// <summary>
    /// This class implements a named queue provider that works within the memory.
    /// </summary>
    public sealed class MemoryNamedQueueProvider : INamedQueueProvider
    {
        /// <summary>Stores the queues</summary>
        private readonly NamedQueueCollection _queues = new NamedQueueCollection();

        /// <summary>
        /// Gets the named queue instance.
        /// </summary>
        /// <param name="name">Queue name</param>
        /// <returns>Queue instance if the named queue exists; otherwise, null</returns>
        public INamedQueue GetQueue(string name)
        {
            lock (_queues)
            {
                return _queues.Contains(name) ? _queues[name] : null;
            }
        }

        /// <summary>
        /// Gets the list of exisiting queues.
        /// </summary>
        /// <returns>Collection of existing queue instances.</returns>
        public IEnumerable<INamedQueue> ListQueues()
        {
            return new List<MemoryNamedQueue>(_queues);
        }

        /// <summary>
        /// Creates a new queue with the specified name.
        /// </summary>
        /// <param name="name">Queue name</param>
        /// <returns>The newly created queue instance.</returns>
        public INamedQueue CreateQueue(string name)
        {
            lock (_queues)
            {
                if (_queues.Contains(name))
                {
                    throw new ArgumentException(
                        string.Format("The queue with the name {0} already exists.", name));
                }
                var newQueue = new MemoryNamedQueue(name);
                _queues.Add(newQueue);
                return newQueue;
            }
        }

        /// <summary>
        /// Deletes the specified queue.
        /// </summary>
        /// <param name="name">name of the queue to delete.</param>
        public void DeleteQueue(string name)
        {
            lock (_queues)
            {
                if (!_queues.Contains(name)) return;
                var item = _queues[name];
                _queues.Remove(item);
                item.DeleteQueue();
            }
        }

        /// <summary>
        /// This class represents the collection of named queues.
        /// </summary>
        private class NamedQueueCollection : KeyedCollection<string, MemoryNamedQueue>
        {
            /// <summary>
            /// When implemented in a derived class, extracts the key from the specified element.
            /// </summary>
            /// <returns>
            /// The key for the specified element.
            /// </returns>
            /// <param name="item">The element from which to extract the key.</param>
            protected override string GetKeyForItem(MemoryNamedQueue item)
            {
                return item.Name;
            }
        }
    }
}