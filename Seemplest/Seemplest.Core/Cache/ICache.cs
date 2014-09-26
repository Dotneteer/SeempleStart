using System;
using System.Collections.Generic;

namespace Seemplest.Core.Cache
{
    /// <summary>
    /// This interface defines the responsibility of a cache object.
    /// </summary>
    public interface ICache<in TKey, TValue> : IDisposable
    {
        /// <summary>
        /// Checks whether the element with the specified key is in the cache.
        /// </summary>
        /// <param name="key">Object key</param>
        /// <returns>True, if the value is in the cache; otherwise, false</returns>
        bool Contains(TKey key);

        /// <summary>
        /// Gets the specified value from the cache.
        /// </summary>
        /// <param name="key">Object key</param>
        /// <returns>The object obtained from the cache</returns>
        /// <exception cref="KeyNotFoundException">
        /// Object with the specified key is not in the cache
        /// </exception>
        TValue GetValue(TKey key);

        /// <summary>
        /// Puts the object with the specified key into the cache.
        /// </summary>
        /// <param name="key">Object key</param>
        /// <param name="value">Object value</param>
        void SetValue(TKey key, TValue value);

        /// <summary>
        /// Removes an item from the cache.
        /// </summary>
        /// <param name="key">Object key</param>
        void Remove(TKey key);

        /// <summary>
        /// Deletes all items from the cache
        /// </summary>
        void Clear();
    }
}