using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using Seemplest.Core.Common;
using Seemplest.Core.TypeResolution;

namespace Seemplest.Core.Configuration.ResourceConnections
{
    /// <summary>
    /// This class provides configuration settings for resource providers.
    /// </summary>
    public class ResourceConnectionProviderSettings : ConfigurationSettingsBase
    {
        private const string PROVIDER = "Provider";
        private const string TYPE = "type";
        private readonly List<Type> _providers = new List<Type>();

        /// <summary>
        /// Root elements of the connection providers section
        /// </summary>
        public const string ROOT = "ConnectionProviders";

        /// <summary>
        /// Creates a new instance of this class
        /// </summary>
        public ResourceConnectionProviderSettings()
        {
        }

        /// <summary>
        /// Creates a new instance of this class from ths specified XML element
        /// </summary>
        /// <param name="element">XML element representing settings</param>
        public ResourceConnectionProviderSettings(XElement element)
        {
            ReadFromXml(element);
        }

        /// <summary>
        /// Creates a new instance using the specified collection of providers
        /// </summary>
        /// <param name="types"></param>
        public ResourceConnectionProviderSettings(IEnumerable<Type> types)
        {
            _providers = new List<Type>(types);
        }

        /// <summary>
        /// Gets a read-only collection of connection providers
        /// </summary>
        public ReadOnlyCollection<Type> Providers
        {
            get { return new ReadOnlyCollection<Type>(_providers); }
        }

        /// <summary>
        /// Writes the object to an XElement instance using the specified root element name.
        /// </summary>
        /// <param name="rootElement">Root element name</param>
        /// <returns>XElement representation of the object</returns>
        public override XElement WriteToXml(XName rootElement)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            return new XElement(rootElement,
                from provider in _providers
                select new XElement(PROVIDER,
                    new XAttribute(TYPE, provider.AssemblyQualifiedName)));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Parse the specified configuration settings
        /// </summary>
        /// <param name="element">Element holding configuration settings</param>
        protected override void ParseFromXml(XElement element)
        {
            element.ProcessItems(PROVIDER, item =>
            {
                var provider = item.TypeAttribute(TYPE, TypeResolver.Current);
                if (provider == null)
                {
                    throw new InvalidOperationException(
                        String.Format("Element '{0}' cannot be parsed as a type", item));
                }
                _providers.Add(provider);
            });
        }
    }
}