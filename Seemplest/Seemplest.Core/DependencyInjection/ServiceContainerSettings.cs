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
    /// This class represents the settings of a service container.
    /// </summary>
    public sealed class ServiceContainerSettings: ConfigurationSettingsBase
    {
        private const string NAME = "name";
        private const string PARENT = "parent";
        private const string MAP = "Map";

        private readonly List<MappingSettings> _mappings = new List<MappingSettings>();
        private readonly ITypeResolver _typeResolver;

        /// <summary>
        /// Gets the name of the container
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the name of the optional parent container
        /// </summary>
        public string ParentName { get; private set; }

        /// <summary>
        /// Gets the list of mappings in this container
        /// </summary>
        public IReadOnlyList<MappingSettings> Mappings
        {
            get { return new ReadOnlyCollection<MappingSettings>(_mappings);}
        }

        /// <summary>
        /// Creates a new instance with the specified mappings
        /// </summary>
        /// <param name="name">Container name</param>
        /// <param name="parentName">Name of the parent container</param>
        /// <param name="mappings">Mappings of the container</param>
        public ServiceContainerSettings(string name, string parentName = null, IEnumerable<MappingSettings> mappings = null)
        {
            Name = name;
            ParentName = parentName;
            if (mappings == null) return;
            _mappings = new List<MappingSettings>(mappings);    
        }

        /// <summary>
        /// Creates a new instance of this class and initializes it from the specified XML element
        /// </summary>
        /// <param name="element">XML element to initialize this instance from.</param>
        public ServiceContainerSettings(XElement element)
            : base(element)
        {
        }

        /// <summary>
        /// Creates a new instance of this class and initializes it from the specified XML element
        /// with the specified type resolver.
        /// </summary>
        /// <param name="element">XML element to initialize this instance from.</param>
        /// <param name="resolver">Type resolver</param>
        public ServiceContainerSettings(XElement element, ITypeResolver resolver)
            : base(element)
        {
            _typeResolver = resolver;
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
                ParentName == null ? null : new XAttribute(PARENT, ParentName),
                from mapping in _mappings
                select mapping.WriteToXml(MAP));
        }

        /// <summary>
        /// Parse the specified configuration settings
        /// </summary>
        /// <param name="element">Element holding configuration settings</param>
        protected override void ParseFromXml(XElement element)
        {
            Name = element.StringAttribute(NAME);
            ParentName = element.OptionalStringAttribute(PARENT, null);
            element.ProcessItems(MAP, item => _mappings.Add(new MappingSettings(item, _typeResolver)));
        }
    }
}