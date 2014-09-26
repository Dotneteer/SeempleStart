using System;
using System.Xml.Linq;
using Seemplest.Core.Common;

namespace Seemplest.Core.Configuration
{
    /// <summary>
    /// This class represents the fundamental application configuration settings.
    /// </summary>
    public sealed class AppConfigurationSettings : ConfigurationSettingsBase
    {
        private const string INSTANCE_PREFIX = "instancePrefix";
        private const string INSTANCE_NAME = "instanceName";
        private const string PROVIDER = "provider";
        private const string CONSTRUCT = "Construct";
        private const string PROPERTIES = "Properties";

        /// <summary>
        /// Gets the instance prefix of this application
        /// </summary>
        public string InstancePrefix { get; private set; }

        /// <summary>
        /// Gest the instance name of this application
        /// </summary>
        public string InstanceName { get; private set; }

        /// <summary>
        /// Gets the application configuration provider
        /// </summary>
        public Type Provider { get; private set; }

        /// <summary>
        /// Gets the construction parameters
        /// </summary>
        public ConstructorParameterSettingsCollection ConstructorParameters { get; private set; }

        /// <summary>
        /// Gets the properties of the configuration provider
        /// </summary>
        public PropertySettingsCollection Properties { get; private set; }

        /// <summary>
        /// Creates an instance initializing it with the specified parameters.
        /// </summary>
        /// <param name="provider">Application configuration provider</param>
        /// <param name="constructorParams">Constructor parameters</param>
        /// <param name="properties">Configuration instance properties</param>
        /// <param name="instancePrefix">Application instance prefix</param>
        /// <param name="instanceName">Application instance name</param>
        public AppConfigurationSettings(Type provider, 
            ConstructorParameterSettingsCollection constructorParams = null, 
            PropertySettingsCollection properties = null, 
            string instancePrefix = null, 
            string instanceName = null)
        {
            InstancePrefix = instancePrefix;
            InstanceName = instanceName;
            Provider = provider;
            ConstructorParameters = constructorParams ?? new ConstructorParameterSettingsCollection();
            Properties = properties ?? new PropertySettingsCollection();
        }

        /// <summary>
        /// Creates a new instance of this class and initializes it from the specified XML element.
        /// </summary>
        /// <param name="element">XML element to initialize this instance from.</param>
        public AppConfigurationSettings(XElement element)
        {
            ConstructorParameters = new ConstructorParameterSettingsCollection();
            Properties = new PropertySettingsCollection();
            ReadFromXml(element);
        }

        /// <summary>
        /// Writes the object to an XElement instance using the specified root element name.
        /// </summary>
        /// <param name="rootElement">Root element name</param>
        /// <returns>XElement representation of the object</returns>
        public override XElement WriteToXml(XName rootElement)
        {
            return new XElement(rootElement,
                new XAttribute(INSTANCE_PREFIX, InstancePrefix ?? String.Empty),
                new XAttribute(INSTANCE_NAME, InstanceName ?? String.Empty),
                // ReSharper disable AssignNullToNotNullAttribute
                new XAttribute(PROVIDER, Provider == null ? String.Empty : Provider.AssemblyQualifiedName),
                // ReSharper restore AssignNullToNotNullAttribute
                ConstructorParameters.WriteToXml(CONSTRUCT),
                Properties.WriteToXml(PROPERTIES));
        }

        /// <summary>
        /// Parse the specified configuration settings
        /// </summary>
        /// <param name="element">Element holding configuration settings</param>
        protected override void ParseFromXml(XElement element)
        {
            InstancePrefix = element.OptionalStringAttribute(INSTANCE_PREFIX);
            InstanceName = element.OptionalStringAttribute(INSTANCE_NAME);
            var providerValue = element.OptionalStringAttribute(PROVIDER);
            Provider = String.IsNullOrWhiteSpace(providerValue)
                           ? typeof(AppConfigProvider)
                           : Type.GetType(providerValue);
            element.ProcessOptionalElement(CONSTRUCT, item => ConstructorParameters.ReadFromXml(item));
            element.ProcessOptionalElement(PROPERTIES, item => Properties.ReadFromXml(item));
        }
    }
}