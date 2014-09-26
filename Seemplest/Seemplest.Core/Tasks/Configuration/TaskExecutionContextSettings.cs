using System.Collections.Generic;
using System.Xml.Linq;
using Seemplest.Core.Common;
using Seemplest.Core.Configuration;

namespace Seemplest.Core.Tasks.Configuration
{
    public class TaskExecutionContextSettings : ConfigurationSettingsBase
    {
        private const string PROPERTIES = "Properties";
        private const string PROVIDER_KEY = "providerKey";

        private readonly TypedPropertySettingsCollection _propertysSettings = new TypedPropertySettingsCollection();

        /// <summary>
        /// The default root element of these settings
        /// </summary>
        public const string ROOT = "Context";

        /// <summary>
        /// Gets the resource key of the queue provider used in this execution context
        /// </summary>
        public string ProviderKey { get; private set; }

        /// <summary>
        /// Gets the property dictionary of the execution context
        /// </summary>
        public IReadOnlyDictionary<string, TypedPropertySettings> Properties
        {
            get { return _propertysSettings.ItemDictionary; }
        }

        /// <summary>
        /// Creates a new instance of this class
        /// </summary>
        /// <param name="providerKey">Provider key</param>
        /// <param name="propertysSettings">Dictionary of propertysSettings</param>
        public TaskExecutionContextSettings(string providerKey,
            TypedPropertySettingsCollection propertysSettings = null)
        {
            ProviderKey = providerKey;
            _propertysSettings = propertysSettings ?? new TypedPropertySettingsCollection();
        }

        /// <summary>
        /// Creates a new instance of this class from the specified XML element.
        /// </summary>
        /// <param name="element">XML element</param>
        public TaskExecutionContextSettings(XElement element)
        {
            ReadFromXml(element);
        }

        /// <summary>
        /// Writes the object to an XElement instance using the specified root element name.
        /// </summary>
        /// <param name="rootElement">Root element name</param>
        /// <returns>XElement representation of the object</returns>
        public override XElement WriteToXml(XName rootElement)
        {
            return new XElement(
                rootElement,
                new XAttribute(PROVIDER_KEY, ProviderKey),
                _propertysSettings.Count == 0
                    ? null : _propertysSettings.WriteToXml(PROPERTIES));
        }

        /// <summary>
        /// Parse the specified configuration settings
        /// </summary>
        /// <param name="element">Element holding configuration settings</param>
        protected override void ParseFromXml(XElement element)
        {
            ProviderKey = element.StringAttribute(PROVIDER_KEY);
            element.ProcessOptionalElement(PROPERTIES, item => _propertysSettings.ReadFromXml(item));
        }

        /// <summary>
        /// Creates a clone of this setting instance.
        /// </summary>
        /// <returns>Cloned instance</returns>
        public TaskExecutionContextSettings Clone()
        {
            return Clone<TaskExecutionContextSettings>();
        }
    }
}