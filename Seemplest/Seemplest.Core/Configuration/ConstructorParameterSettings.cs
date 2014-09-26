using System;
using System.Xml.Linq;
using Seemplest.Core.Common;
using Seemplest.Core.TypeResolution;

namespace Seemplest.Core.Configuration
{
    /// <summary>
    /// Stores a property of a configuration setting
    /// </summary>
    public sealed class ConstructorParameterSettings: ConfigurationSettingsBase
    {
        private const string VALUE = "value";
        private const string TYPE = "type";

        private readonly ITypeResolver _typeResolver;

        /// <summary>
        /// Gets the type of the property
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the value of the property
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Creates a new instance of this class and initializes it from the specified XML element.
        /// </summary>
        /// <param name="element">XML element to initialize this instance from.</param>
        /// <param name="typeResolver">Optional type resolver</param>
        public ConstructorParameterSettings(XElement element, ITypeResolver typeResolver = null)
            : base(element)
        {
            _typeResolver = typeResolver;
        }

        /// <summary>
        /// Creates an instance with the specified arguments
        /// </summary>
        /// <param name="type">Property type</param>
        /// <param name="value">Property value</param>
        public ConstructorParameterSettings(Type type, string value)
        {
            Type = type;
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
                new XAttribute(VALUE, Value),
                new XAttribute(TYPE, Type));
        }

        /// <summary>
        /// Parse the specified configuration settings
        /// </summary>
        /// <param name="element">Element holding configuration settings</param>
        protected override void ParseFromXml(XElement element)
        {
            Value = element.StringAttribute(VALUE);
            Type = element.TypeAttribute(TYPE, _typeResolver);
        }
    }
}