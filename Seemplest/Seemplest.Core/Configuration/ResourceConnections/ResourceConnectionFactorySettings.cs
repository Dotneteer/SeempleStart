using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

namespace Seemplest.Core.Configuration.ResourceConnections
{
    /// <summary>
    /// This class holds seetings of a resource connection factory
    /// </summary>
    public class ResourceConnectionFactorySettings : ConfigurationSettingsBase
    {
        private readonly ResourceConnectionProviderCollection _providers =
            new ResourceConnectionProviderCollection();

        /// <summary>
        /// Gets the dictionary of providers registered with this factory
        /// </summary>
        public ReadOnlyDictionary<string, ResourceConnectionProviderBase> Providers
        {
            get { return _providers.ProviderDictionary; }
        }

        /// <summary>
        /// Creates a new instance of this class
        /// </summary>
        public ResourceConnectionFactorySettings()
            : this(new ResourceConnectionProviderCollection())
        {
        }

        /// <summary>
        /// Creates a new instance of this class
        /// </summary>
        /// <param name="providers">Resource connection provider definitions</param>
        public ResourceConnectionFactorySettings(ResourceConnectionProviderCollection providers)
        {
            if (providers == null) throw new ArgumentNullException("providers");
            _providers = providers;
        }

        /// <summary>
        /// Gets the settings from the specified XML element.
        /// </summary>
        /// <param name="element">XML element with settings</param>
        public ResourceConnectionFactorySettings(XElement element)
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
            return new XElement(rootElement,
                from provider in _providers.ProviderDictionary.Values
                select provider.WriteToXml(
                    DefaultResourceConnectionProviderRegistry.GetProviderName(provider.GetType())));
        }

        /// <summary>
        /// Parse the specified configuration settings
        /// </summary>
        /// <param name="element">Element holding configuration settings</param>
        protected override void ParseFromXml(XElement element)
        {
            foreach (var provider in element.Descendants())
            {
                // --- Obtain the appropriate provider instance
                var type = ResourceConnectionProviderRegistry.Current
                    .GetResourceConnectionProvider(provider.Name.LocalName);
                if (type == null)
                {
                    throw new ConfigurationErrorsException(
                        String.Format("{0} is an unknown resource connection provider type.",
                        provider.Name.LocalName));
                }
                try
                {
                    var instance = Activator.CreateInstance(type, new object[] { provider }) as ResourceConnectionProviderBase;
                    _providers.Add(instance);
                }
                catch (Exception ex)
                {
                    throw new ConfigurationErrorsException(String.Format(
                        "The registered {0} type does not support the correct provider.", type), ex);
                }
            }
        }
    }
}