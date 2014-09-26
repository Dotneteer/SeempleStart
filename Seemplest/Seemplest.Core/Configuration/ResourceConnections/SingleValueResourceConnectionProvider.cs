using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using Seemplest.Core.Common;

namespace Seemplest.Core.Configuration.ResourceConnections
{
    /// <summary>
    /// This class implements a resource connection with a single value.
    /// </summary>
    public abstract class SingleValueResourceConnectionProvider<TValue> : ResourceConnectionProviderBase
    {
        private const string VALUE = "value";

        /// <summary>
        /// Gets the value of the resource connection
        /// </summary>
        public TValue Value { get; private set; }

        /// <summary>
        /// Creates a new instance with the specified name and Value.
        /// </summary>
        /// <param name="name">Setting name</param>
        /// <param name="value">Setting value</param>
        protected SingleValueResourceConnectionProvider(string name, TValue value)
        {
            Value = value;
            Name = name;
        }

        /// <summary>
        /// Creates a new instance from the specified XML element
        /// </summary>
        /// <param name="element"></param>
        protected SingleValueResourceConnectionProvider(XElement element)
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            ReadFromXml(element);
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        /// <summary>
        /// Adds additional XElement and XAttribute settings to the XML representation
        /// </summary>
        /// <returns>Additional settings</returns>
        public override List<XObject> GetAdditionalSettings()
        {
            var settings = base.GetAdditionalSettings();
            settings.Add(new XAttribute(VALUE, Value));
            return settings;
        }

        /// <summary>
        /// Parse the specified configuration settings
        /// </summary>
        /// <param name="element">Element holding configuration settings</param>
        protected override void ParseFromXml(XElement element)
        {
            base.ParseFromXml(element);
            Value = ConvertValueFrom(element.StringAttribute(VALUE));
        }

        /// <summary>
        /// Obtains value of TValue converted from a string.
        /// </summary>
        /// <param name="valueString">String to convert from</param>
        /// <returns>TValue representation</returns>
        protected virtual TValue ConvertValueFrom(string valueString)
        {
            var converter = TypeDescriptor.GetConverter(typeof(TValue));
            return (TValue)converter.ConvertFromString(valueString);
        }
    }
}