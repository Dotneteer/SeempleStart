using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using Seemplest.Core.Common;

namespace Seemplest.Core.Configuration
{
    /// <summary>
    /// This class is a generic collection of setting items
    /// </summary>
    public abstract class SettingsCollectionBase<TSettings, TKeyedSettingsCollection> : ConfigurationSettingsBase, IEnumerable<TSettings>
        where TSettings: ConfigurationSettingsBase, new()
        where TKeyedSettingsCollection: KeyedCollectionWithDictionary<string, TSettings>, new()
    {
        private readonly TKeyedSettingsCollection _properties = new TKeyedSettingsCollection();

        /// <summary>
        /// Creates a new instance of this class and initializes it from the specified XML element.
        /// </summary>
        /// <param name="element">XML element to initialize this instance from.</param>
        protected SettingsCollectionBase(XElement element)
        {
            ReadFromXml(element);
        }

        /// <summary>
        /// Gets the XName used for a collection item;
        /// </summary>
        public abstract XName SettingItemXmlName { get; }

        /// <summary>
        /// Creates a new instance of this class with the elements passed in the argument, 
        /// using the specified root XML name
        /// </summary>
        /// <param name="collection">Initial values</param>
        protected SettingsCollectionBase(IEnumerable<TSettings> collection = null)
        {
            if (collection == null) return;
            foreach (var item in collection) _properties.Add(item);
        }

        /// <summary>
        /// Gets the dictionary of propertysSettings
        /// </summary>
        public IReadOnlyDictionary<string, TSettings> ItemDictionary
        {
            get { return _properties.ItemDictionary; }
        }

        /// <summary>
        /// Writes the object to an XElement instance using the specified root element name.
        /// </summary>
        /// <param name="rootElement">Root element name</param>
        /// <returns>XElement representation of the object</returns>
        public override XElement WriteToXml(XName rootElement)
        {
            return new XElement(rootElement,
                from property in _properties
                select property.WriteToXml(SettingItemXmlName));
        }

        /// <summary>
        /// Parse the specified configuration settings
        /// </summary>
        /// <param name="element">Element holding configuration settings</param>
        protected override void ParseFromXml(XElement element)
        {
            _properties.Clear();
            element.ProcessItems(SettingItemXmlName, item =>
                {
                    var settingItem = new TSettings();
                    settingItem.ReadFromXml(item);
                    _properties.Add(settingItem);
                });
        }

        /// <summary>
        /// Gets the element specified with the index.
        /// </summary>
        /// <param name="index">Property index</param>
        /// <returns>The specified element</returns>
        public TSettings this[int index]
        {
            get { return _properties[index]; }
        }

        /// <summary>
        /// Gets the element specified with the key.
        /// </summary>
        /// <param name="key">Property name (key)</param>
        /// <returns>The specified element</returns>
        public TSettings this[string key]
        {
            get { return _properties[key]; }
        }

        /// <summary>
        /// Gets the count of properties in this collection.
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
        public IEnumerator<TSettings> GetEnumerator()
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