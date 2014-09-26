using System;
using System.Xml.Linq;
using Seemplest.Core.Common;

namespace Seemplest.Core.Configuration
{
    /// <summary>
    /// Stores a typed property of a configuration setting
    /// </summary>
    public class TypedPropertySettings : PropertySettings
    {
        private const string TYPE = "type";

        /// <summary>
        /// Gets the type of the property
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Creates an empty setting element
        /// </summary>
        public TypedPropertySettings()
        {
        }

        /// <summary>
        /// Creates a new instance of this class and initializes it from the specified XML element.
        /// </summary>
        /// <param name="element">XML element to initialize this instance from.</param>
        public TypedPropertySettings(XElement element)
            : base(element)
        {
        }

        /// <summary>
        /// Creates an instance with the specified arguments
        /// </summary>
        /// <param name="name">Property name</param>
        /// <param name="value">Property value</param>
        /// <param name="type">Property type</param>
        public TypedPropertySettings(string name, string value, Type type)
            : base(name, value)
        {
            Type = type;
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
                new XAttribute(VALUE, Value),
                new XAttribute(TYPE, Type));
        }

        /// <summary>
        /// Parse the specified configuration settings
        /// </summary>
        /// <param name="element">Element holding configuration settings</param>
        protected override void ParseFromXml(XElement element)
        {
            Name = element.StringAttribute(NAME);
            Value = element.StringAttribute(VALUE);
            Type = element.TypeAttribute(TYPE);
        }
    }
}