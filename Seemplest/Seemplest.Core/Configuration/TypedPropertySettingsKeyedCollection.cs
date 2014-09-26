using System.Collections.ObjectModel;

namespace Seemplest.Core.Configuration
{
    /// <summary>
    /// Stores properties internally
    /// </summary>
    public class TypedPropertySettingsKeyedCollection : KeyedCollectionWithDictionary<string, TypedPropertySettings>
    {
        /// <summary>
        /// When implemented in a derived class, extracts the key from the specified element.
        /// </summary>
        /// <returns>
        /// The key for the specified element.
        /// </returns>
        /// <param name="item">The element from which to extract the key.</param>
        protected override string GetKeyForItem(TypedPropertySettings item)
        {
            return item.Name;
        }
    }
}