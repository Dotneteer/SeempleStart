using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Seemplest.Core.Configuration.ResourceConnections
{
    /// <summary>
    /// This container stores a collection of resource connection provider objects.
    /// </summary>
    public class ResourceConnectionProviderCollection : KeyedCollection<string, ResourceConnectionProviderBase>
    {
        /// <summary>
        /// When implemented in a derived class, extracts the key from the specified element.
        /// </summary>
        /// <returns>
        /// The key for the specified element.
        /// </returns>
        /// <param name="item">The element from which to extract the key.</param>
        protected override string GetKeyForItem(ResourceConnectionProviderBase item)
        {
            return item.Name;
        }

        /// <summary>
        /// Gets the dictionary of providers
        /// </summary>
        public ReadOnlyDictionary<string, ResourceConnectionProviderBase> ProviderDictionary
        {
            get
            {
                return new ReadOnlyDictionary<string, ResourceConnectionProviderBase>(
                    Dictionary ?? new Dictionary<string, ResourceConnectionProviderBase>());
            }
        }
    }
}