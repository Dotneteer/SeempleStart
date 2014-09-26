namespace Seemplest.Core.Configuration
{
    /// <summary>
    /// Stores properties internally
    /// </summary>
    public class PropertySettingsKeyedCollection : KeyedCollectionWithDictionary<string, PropertySettings>
    {
        /// <summary>
        /// When implemented in a derived class, extracts the key from the specified element.
        /// </summary>
        /// <returns>
        /// The key for the specified element.
        /// </returns>
        /// <param name="item">The element from which to extract the key.</param>
        protected override string GetKeyForItem(PropertySettings item)
        {
            return item.Name;
        }
    }
}