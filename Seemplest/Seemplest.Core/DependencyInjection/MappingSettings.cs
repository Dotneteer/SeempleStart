using System;
using System.Xml.Linq;
using Seemplest.Core.Configuration;
using Seemplest.Core.Common;
using Seemplest.Core.TypeResolution;

namespace Seemplest.Core.DependencyInjection
{
    public sealed class MappingSettings: ConfigurationSettingsBase
    {
        private const string FROM = "from";
        private const string TO = "to";
        private const string LIFETIME = "lifetime";
        private const string CONSTRUCT = "Construct";
        private const string PROPERTIES = "Properties";

        private readonly ITypeResolver _typeResolver;

        /// <summary>
        /// Gets the type to map the service object from
        /// </summary>
        public Type From { get; private set; }

        /// <summary>
        /// Gets the type of service object
        /// </summary>
        public Type To { get; private set; }

        /// <summary>
        /// Gets the optional type of lifetime manager
        /// </summary>
        public Type Lifetime { get; private set; }

        /// <summary>
        /// Gets the collection of injected parameters
        /// </summary>
        public InjectedParameterSettingsCollection Parameters { get; private set; }

        /// <summary>
        /// Gets the collection of properties
        /// </summary>
        public PropertySettingsCollection Properties { get; private set; }

        public MappingSettings(Type @from, Type to, 
            Type lifetime = null, 
            InjectedParameterSettingsCollection parameters = null, 
            PropertySettingsCollection properties = null)
        {
            From = @from;
            To = to;
            Lifetime = lifetime;
            Parameters = parameters;
            Properties = properties;
        }

        /// <summary>
        /// Creates a new instance of this class and initializes it from the specified XML element
        /// with the specified type resolver.
        /// </summary>
        /// <param name="element">XML element to initialize this instance from.</param>
        /// <param name="typeResolver">Type resolver</param>
        public MappingSettings(XElement element, ITypeResolver typeResolver = null)
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
            // ReSharper disable AssignNullToNotNullAttribute
            return new XElement(rootElement,
                new XAttribute(FROM, From.AssemblyQualifiedName),
                new XAttribute(TO, To.AssemblyQualifiedName),
                Lifetime == null ? null : new XAttribute(LIFETIME, Lifetime.AssemblyQualifiedName),
                Properties == null ? null : Properties.WriteToXml(PROPERTIES),
                Parameters == null ? null : Parameters.WriteToXml(CONSTRUCT));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Parse the specified configuration settings
        /// </summary>
        /// <param name="element">Element holding configuration settings</param>
        protected override void ParseFromXml(XElement element)
        {
            From = element.TypeAttribute(FROM, _typeResolver);
            To = element.TypeAttribute(TO, _typeResolver);
            Lifetime = element.OptionalTypeAttribute(LIFETIME, new LifeTimeTypeResolver(_typeResolver));
            element.ProcessOptionalElement(PROPERTIES, 
                item => Properties = new PropertySettingsCollection(item));
            element.ProcessOptionalElement(CONSTRUCT, 
                item => Parameters = new InjectedParameterSettingsCollection(item, _typeResolver));
        }

        /// <summary>
        /// This resolver is used to resolve life time manager type names
        /// </summary>
        private class LifeTimeTypeResolver: ChainedTypeResolverBase
        {
            /// <summary>
            /// Creates a type resolver using the specified type resolver instance as
            /// a fallback.
            /// </summary>
            /// <param name="parentResolver">Fallback type resolver</param>
            public LifeTimeTypeResolver(ITypeResolver parentResolver = null)
                : base(parentResolver)
            {
            }

            /// <summary>
            /// Resolves the specified name to a <see cref="Type"/> instance.
            /// </summary>
            /// <param name="name">Name to resolve</param>
            /// <returns><see cref="Type"/>This implementation always return null</returns>
            public override Type ResolveLocally(string name)
            {
                switch (name)
                {
                    case "PerCall": return typeof (PerCallLifetimeManager);
                    case "PerThread": return typeof(PerThreadLifetimeManager);
                    case "Singleton": return typeof(SingletonLifetimeManager);
                    default: return null;
                }
            }
        }
    }
}