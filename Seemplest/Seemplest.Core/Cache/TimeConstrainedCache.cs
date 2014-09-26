using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Seemplest.Core.Configuration;

namespace Seemplest.Core.Cache
{
    /// <summary>
    /// This class impements a cache where cached items may expire
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class TimeConstrainedCache<TKey, TValue> : ICache<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, Tuple<TValue, DateTime>> _cache = 
            new ConcurrentDictionary<TKey, Tuple<TValue, DateTime>>();

        /// <summary>
        /// Defines the expiration time span of cache items
        /// </summary>
        public TimeSpan ExpirationTimeSpan { get; private set; }

        /// <summary>
        /// Creates a cache instance with 5 seconds expiration time span
        /// </summary>
        public TimeConstrainedCache() : this(TimeSpan.FromSeconds(5))
        {
        }

        /// <summary>
        /// Creates a cache with the specified expiration time span
        /// </summary>
        /// <param name="expirationTimeSpan">Cache expiration time span</param>
        public TimeConstrainedCache(TimeSpan expirationTimeSpan)
        {
            ExpirationTimeSpan = expirationTimeSpan;
        }

        /// <summary>
        /// Resets the cache using the specified expiration time span
        /// </summary>
        /// <param name="expirationTimeSpan">Cache expiration time span</param>
        /// <remarks>This method clears the contents of the cache</remarks>
        public void ResetTo(TimeSpan expirationTimeSpan)
        {
            _cache.Clear();
            ExpirationTimeSpan = expirationTimeSpan;
        }

        /// <summary>
        /// Checks whether the element with the specified key is in the cache.
        /// </summary>
        /// <param name="key">Object key</param>
        /// <returns>True, if the value is in the cache; otherwise, false</returns>
        public bool Contains(TKey key)
        {
            return _cache.ContainsKey(key);
        }

        /// <summary>
        /// Gets the number of items stored in the cache
        /// </summary>
        public int Count
        {
            get { return _cache.Count; }
        }

        /// <summary>
        /// Gets the specified value from the cache.
        /// </summary>
        /// <param name="key">Object key</param>
        /// <returns>The object obtained from the cache</returns>
        /// <exception cref="KeyNotFoundException">
        /// Object with the specified key is not in the cache
        /// </exception>
        public TValue GetValue(TKey key)
        {
            Tuple<TValue, DateTime> outValue;
            var found = _cache.TryGetValue(key, out outValue);
            
            // --- Check if item is in the cache
            if (!found)
            {
                throw new KeyNotFoundException();
            }

            // --- Check if item is still valid
            if (outValue.Item2 > EnvironmentInfo.GetCurrentDateTimeUtc())
            {
                return outValue.Item1;
            }

            // --- Item expires
            _cache.TryRemove(key, out outValue);
            throw new KeyNotFoundException();
        }

        /// <summary>
        /// Tries to obtain the specified value from the cache
        /// </summary>
        /// <param name="key">Object key</param>
        /// <param name="value">The value, if found in the cahce</param>
        /// <returns>True, if the object has been found in the cache; otherwise, false</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            Tuple<TValue, DateTime> outValue;
            var found = _cache.TryGetValue(key, out outValue);
            value = default(TValue);

            // --- Check if item is in the cache
            if (!found)
            {
                return false;
            }

            // --- Check if item is still valid
            if (outValue.Item2 > EnvironmentInfo.GetCurrentDateTimeUtc())
            {
                value = outValue.Item1;
                return true;
            }

            // --- Item expires
            _cache.TryRemove(key, out outValue);
            return false;
        }

        /// <summary>
        /// Puts the object with the specified key into the cache.
        /// </summary>
        /// <param name="key">Object key</param>
        /// <param name="value">Object value</param>
        public void SetValue(TKey key, TValue value)
        {
            _cache[key] = new Tuple<TValue, DateTime>(value, EnvironmentInfo.GetCurrentDateTimeUtc() + ExpirationTimeSpan);
        }

        /// <summary>
        /// Removes an item from the cache.
        /// </summary>
        /// <param name="key">Object key</param>
        public void Remove(TKey key)
        {
            Tuple<TValue, DateTime> outValue;
            _cache.TryRemove(key, out outValue);
        }

        /// <summary>
        /// Deletes all items from the cache
        /// </summary>
        public void Clear()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}