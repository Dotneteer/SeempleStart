using System.Xml.Linq;
using Seemplest.Core.Common;

namespace Seemplest.Core.Configuration
{
    /// <summary>
    /// Stores a property of a configuration setting
    /// </summary>
    public class PropertySettings: ConfigurationSettingsBase
    {
        protected const string NAME = "name";
        protected const string VALUE = "value";

        /// <summary>
        /// Gets the name of the property
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the value of the property
        /// </summary>
        public string Value { get; protected set; }

        /// <summary>
        /// Creates an empty setting element
        /// </summary>
        public PropertySettings()
        {
        }

        /// <summary>
        /// Creates a new instance of this class and initializes it from the specified XML element.
        /// </summary>
        /// <param name="element">XML element to initialize this instance from.</param>
        public PropertySettings(XElement element) : base(element)
        {
        }

        /// <summary>
        /// Creates an instance with the specified arguments
        /// </summary>
        /// <param name="name">Property name</param>
        /// <param name="value">Property value</param>
        public PropertySettings(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Writes the object to an XElement instance using the specified root element name.
        /// </summary>
        /// <param name="rootElement">Root element name</param>
        /// <returns>XElement representation of the object</returns>
        public override XElement WriteToXml(XName rootElement)
        {
            return new XElement(rootElement,
                new XAttribute(NAME, Name),
                new XAttribute(VALUE, Value));
        }

        /// <summary>
        /// Parse the specified configuration settings
        /// </summary>
        /// <param name="element">Element holding configuration settings</param>
        protected override void ParseFromXml(XElement element)
        {
            Name = element.StringAttribute(NAME);
            Value = element.StringAttribute(VALUE);
        }
    }
}