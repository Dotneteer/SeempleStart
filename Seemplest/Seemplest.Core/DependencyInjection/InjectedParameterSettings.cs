using System;
using System.Xml.Linq;
using Seemplest.Core.Configuration;
using Seemplest.Core.Common;
using Seemplest.Core.TypeResolution;

namespace Seemplest.Core.DependencyInjection
{
    /// <summary>
    /// This class represents the configuration settings for a constructor parameter
    /// </summary>
    public sealed class InjectedParameterSettings: ConfigurationSettingsBase
    {
        private const string TYPE = "type";
        private const string VALUE = "value";
        private const string RESOLVE = "resolve";

        private readonly ITypeResolver _typeResolver;

        /// <summary>
        /// Gets the type name of the constructor parameter
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the value of the constructor parameter
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Signs if the construction parameter should be resolved from the service container.
        /// </summary>
        public bool Resolve { get; private set; }

        /// <summary>
        /// Creates a new instance of this class with the specified settings.
        /// </summary>
        /// <param name="type">Parameter type</param>
        /// <param name="value">Parameter value</param>
        /// <param name="resolve">True, if should be resolved from container</param>
        public InjectedParameterSettings(Type type, object value = null, bool resolve = false)
        {
            Type = type;
            Value = value;
            Resolve = resolve;
        }

        /// <summary>
        /// Creates a new instance of this class and initializes it from the specified XML element with
        /// the specified type resolver.
        /// </summary>
        /// <param name="element">XML element to initialize this instance from.</param>
        /// <param name="typeResolver">Type resolver</param>
        public InjectedParameterSettings(XElement element, ITypeResolver typeResolver = null)
            : base(element)
        {
            _typeResolver = typeResolver;
        }

        /// <summary>
        /// Writes the object to an XElement instance using the specified root element name.
        /// </summary>
        /// <param name="rootElement">Root element name</param>
        /// <returns>XElement representation of the object</returns>
        public override XElement WriteToXml(XName rootElement)
        {
            return new XElement(rootElement,
                new XAttribute(TYPE, Type),
                Value == null ? null : new XAttribute(VALUE, Value),
                Resolve ? new XAttribute(RESOLVE, true) : null);
        }

        /// <summary>
        /// Parse the specified configuration settings
        /// </summary>
        /// <param name="element">Element holding configuration settings</param>
        protected override void ParseFromXml(XElement element)
        {
            Type = element.TypeAttribute(TYPE, _typeResolver);
            Value = element.OptionalStringAttribute(VALUE, null);
            Resolve = element.OptionalBoolAttribute(RESOLVE);
        }
    }
}