using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using Seemplest.Core.Common;
using Seemplest.Core.TypeResolution;

namespace Seemplest.Core.Configuration
{
    /// <summary>
    /// Stores a collection of PropertySettings items using Name, Value, and Type
    /// </summary>
    public sealed class ConstructorParameterSettingsCollection :
        ConfigurationSettingsBase,
        IEnumerable<ConstructorParameterSettings>
    {
        private const string PARAM = "Param";

        private readonly List<ConstructorParameterSettings> _properties = new List<ConstructorParameterSettings>();
        private readonly ITypeResolver _typeResolver;

        /// <summary>
        /// Creates a new instance of this class and initializes it from the specified XML element with
        /// using the specified type resolver.
        /// </summary>
        /// <param name="element">XML element to initialize this instance from.</param>
        /// <param name="typeResolver">Type resolver to use</param>
        public ConstructorParameterSettingsCollection(XElement element, ITypeResolver typeResolver = null) 
            : base(element)
        {
            _typeResolver = typeResolver;
        }

        /// <summary>
        /// Creates a new instance of this class with the elements passed in the argument, 
        /// using the specified root XML name
        /// </summary>
        /// <param name="collection">Initial values</param>
        public ConstructorParameterSettingsCollection(IEnumerable<ConstructorParameterSettings> collection = null)
        {
            if (collection == null) return;
            _properties = new List<ConstructorParameterSettings>(collection);
        }

        /// <summary>
        /// Writes the object to an XElement instance.
        /// </summary>
        /// <returns>XElement representation of the object</returns>
        public override XElement WriteToXml(XName rootElement)
        {
            return new XElement(rootElement,
                from property in this
                select property.WriteToXml(PARAM));
        }

        /// <summary>
        /// Parse the specified configuration settings
        /// </summary>
        /// <param name="element">Element holding configuration settings</param>
        protected override void ParseFromXml(XElement element)
        {
            _properties.Clear();
            element.ProcessItems(PARAM, item => _properties.Add(
                new ConstructorParameterSettings(item, _typeResolver)));
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Specified element</returns>
        public ConstructorParameterSettings this[int index]
        {
            get { return _properties[index]; }
        }

        /// <summary>
        /// Gets the number of elements in this collection
        /// </summary>
        public int Count
        {
            get { return _properties.Count; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<ConstructorParameterSettings> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}