using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using Seemplest.Core.Common;
using Seemplest.Core.Configuration;

namespace Seemplest.Core.TypeResolution
{
    /// <summary>
    /// This class is used to describe configuration settings for a type resolver.
    /// </summary>
    public class TypeResolverConfigurationSettings : ConfigurationSettingsBase
    {
        private const string NAMESPACES = "Namespaces";
        private const string NAMESPACE = "Namespace";
        private const string NAME = "name";
        private const string ASSEMBLIES = "Assemblies";
        private const string ASSEMBLY = "Assembly";

        private readonly List<string> _assemblyNames = new List<string>();
        private readonly List<string> _namespaces = new List<string>();

        /// <summary>
        /// Default configuration section within app config files
        /// </summary>
        public const string ROOT = "TypeResolver";

        /// <summary>
        /// Gets the names of assemblies.
        /// </summary>
        public IReadOnlyList<string> AssemblyNames
        {
            get { return new ReadOnlyCollection<string>(_assemblyNames); }
        }

        /// <summary>
        /// Gets the names of namespaces
        /// </summary>
        public IReadOnlyList<string> Namespaces
        {
            get { return new ReadOnlyCollection<string>(_namespaces); }
        }

        /// <summary>
        /// Creates an instance of this class and initializes it from the specified input.
        /// </summary>
        /// <param name="asmList">List of assembly names</param>
        /// <param name="nsList">List of namespaces</param>
        public TypeResolverConfigurationSettings(IEnumerable<string> asmList,
            IEnumerable<string> nsList)
        {
            _assemblyNames = new List<string>(asmList);
            _namespaces = new List<string>(nsList);
        }

        /// <summary>
        /// Creates a new instance of this class and initializes it from the specified XML element.
        /// </summary>
        /// <param name="element">XML element to initialize this instance from.</param>
        public TypeResolverConfigurationSettings(XElement element) : base(element) { }

        /// <summary>
        /// Writes the object to an XElement instance using the specified root element name.
        /// </summary>
        /// <param name="rootElement">Root element name</param>
        /// <returns>XElement representation of the object</returns>
        public override XElement WriteToXml(XName rootElement)
        {
            return new XElement(rootElement,
                new XElement(ASSEMBLIES,
                    from asm in _assemblyNames
                    select new XElement(ASSEMBLY,
                        new XAttribute(NAME, asm))),
                new XElement(NAMESPACES,
                    from ns in _namespaces
                    select new XElement(NAMESPACE,
                        new XAttribute(NAME, ns))));
        }

        /// <summary>
        /// Parse the specified configuration settings
        /// </summary>
        /// <param name="element">Element holding configuration settings</param>
        protected override void ParseFromXml(XElement element)
        {
            element.ProcessContainerItems(ASSEMBLIES, ASSEMBLY,
                item => _assemblyNames.Add(item.StringAttribute(NAME)));
            element.ProcessContainerItems(NAMESPACES, NAMESPACE,
                item => _namespaces.Add(item.StringAttribute(NAME)));
        }
    }
}