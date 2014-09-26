using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Seemplest.Core.Configuration
{
    /// <summary>
    /// This abstract class represents a keyed collection can can retrieve its
    /// items as a read-only dictionary.
    /// </summary>
    /// <typeparam name="TKey">Type of key</typeparam>
    /// <typeparam name="TValue">Type of colection items</typeparam>
    public abstract class KeyedCollectionWithDictionary<TKey, TValue> : KeyedCollection<TKey, TValue>
    {
        [ExcludeFromCodeCoverage]
        protected KeyedCollectionWithDictionary()
        {
        }

        [ExcludeFromCodeCoverage]
        protected KeyedCollectionWithDictionary(IEqualityComparer<TKey> comparer) 
            : base(comparer)
        {
        }

        [ExcludeFromCodeCoverage]
        protected KeyedCollectionWithDictionary(IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold) 
            : base(comparer, dictionaryCreationThreshold)
        {
        }

        /// <summary>
        /// Gets the dictionary of items
        /// </summary>
        public IReadOnlyDictionary<TKey, TValue> ItemDictionary
        {
            get
            {
                return new ReadOnlyDictionary<TKey, TValue>(
                    Dictionary ?? new Dictionary<TKey, TValue>());
            }
        }
    }
}