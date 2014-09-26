using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace Seemplest.Core.Configuration
{
    /// <summary>
    /// Stores a collection of PropertySettings items using Name and Value only
    /// </summary>
    public sealed class TypedPropertySettingsCollection :
        SettingsCollectionBase<TypedPropertySettings, TypedPropertySettingsKeyedCollection>
    {

        /// <summary>
        /// Creates a new instance of this class and initializes it from the specified XML element.
        /// </summary>
        /// <param name="element">XML element to initialize this instance from.</param>
        [ExcludeFromCodeCoverage]
        public TypedPropertySettingsCollection(XElement element)
            : base(element)
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the elements passed in the argument, 
        /// using the specified root XML name
        /// </summary>
        /// <param name="collection">Initial values</param>
        [ExcludeFromCodeCoverage]
        public TypedPropertySettingsCollection(IEnumerable<TypedPropertySettings> collection = null)
            : base(collection)
        {
        }

        /// <summary>
        /// Gets the XName used for a collection item;
        /// </summary>
        public override XName SettingItemXmlName
        {
            get { return "Property"; }
        }
    }
}