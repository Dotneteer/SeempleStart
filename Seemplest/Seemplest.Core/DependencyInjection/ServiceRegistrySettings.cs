using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using Seemplest.Core.Configuration;
using Seemplest.Core.Common;
using Seemplest.Core.TypeResolution;

namespace Seemplest.Core.DependencyInjection
{
    /// <summary>
    /// This class represents the configuration of a service registry.
    /// </summary>
    public sealed class ServiceRegistrySettings: ConfigurationSettingsBase
    {
        private const string TYPE_RESOLVER = "typeResolver";
        private const string DEFAULT_CONTAINER = "defaultContainer";
        private const string CONTAINER = "Container";

        private readonly List<ServiceContainerSettings> _containers = 
            new List<ServiceContainerSettings>();

        /// <summary>
        /// Gets the type resolver used by the service registry
        /// </summary>
        public ITypeResolver Resolver { get; private set; }

        /// <summary>
        /// Gets the name of the default container
        /// </summary>
        public string DefaultContainer { get; private set; }

        /// <summary>
        /// Gets the list of containers held by this registry
        /// </summary>
        public IReadOnlyList<ServiceContainerSettings> Containers
        {
            get { return new ReadOnlyCollection<ServiceContainerSettings>(_containers);}    
        }

        /// <summary>
        /// Creates a new instance of this class and initializes it from the specified XML element.
        /// </summary>
        /// <param name="element">XML element to initialize this instance from.</param>
        public ServiceRegistrySettings(XElement element)
            : base(element)
        {
        }

        /// <summary>
        /// Creates a new instance of this class using the specified settings
        /// </summary>
        /// <param name="defaultContainer">Default container name</param>
        /// <param name="containers">Collection of containers</param>
        /// <param name="typeResolver">Type resolver to use</param>
        public ServiceRegistrySettings(string defaultContainer, 
            IEnumerable<ServiceContainerSettings> containers = null, ITypeResolver typeResolver = null)
        {
            Resolver = typeResolver;
            DefaultContainer = defaultContainer;
            if (containers != null) _containers = new List<ServiceContainerSettings>(containers);
        }

        /// <summary>
        /// Writes the object to an XElement instance using the specified root element name.
        /// </summary>
        /// <param name="rootElement">Root element name</param>
        /// <returns>XElement representation of the object</returns>
        public override XElement WriteToXml(XName rootElement)
        {
            return new XElement(rootElement,
                Resolver == null ? null : new XAttribute(TYPE_RESOLVER, Resolver),
                DefaultContainer == null ? null : new XAttribute(DEFAULT_CONTAINER, DefaultContainer),
                from container in _containers
                select container.WriteToXml(CONTAINER));
        }

        /// <summary>
        /// Parse the specified configuration settings
        /// </summary>
        /// <param name="element">Element holding configuration settings</param>
        protected override void ParseFromXml(XElement element)
        {
            var resolver = element.OptionalTypeAttribute(TYPE_RESOLVER);
            Resolver = resolver == null
                           ? null
                           : (ITypeResolver) Activator.CreateInstance(resolver);
            DefaultContainer = element.OptionalStringAttribute(DEFAULT_CONTAINER, null);
            element.ProcessItems(CONTAINER, item => _containers.Add(new ServiceContainerSettings(item)));
        }
    }
}