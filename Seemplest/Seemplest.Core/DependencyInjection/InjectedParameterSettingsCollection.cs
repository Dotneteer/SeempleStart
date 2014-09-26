using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using Seemplest.Core.Common;
using Seemplest.Core.Configuration;
using Seemplest.Core.TypeResolution;

namespace Seemplest.Core.DependencyInjection
{
    /// <summary>
    /// Stores a collection of PropertySettings items using Name and Value only
    /// </summary>
    public sealed class InjectedParameterSettingsCollection : 
        ConfigurationSettingsBase,
        IEnumerable<InjectedParameterSettings>
    {
        private const string PARAM = "Param";

        private readonly List<InjectedParameterSettings> _parameters = new List<InjectedParameterSettings>();
        private readonly ITypeResolver _typeResolver;

        /// <summary>
        /// Creates a new instance of this class and initializes it from the specified XML element
        /// with the specified type resolver.
        /// </summary>
        /// <param name="element">XML element to initialize this instance from.</param>
        /// <param name="typeResolver">Type resolver</param>
        public InjectedParameterSettingsCollection(XElement element, ITypeResolver typeResolver = null)
            : base(element)
        {
            _typeResolver = typeResolver;
        }

        /// <summary>
        /// Creates a new instance of this class with the elements passed in the argument, 
        /// using the specified root XML name
        /// </summary>
        /// <param name="collection">Initial values</param>
        public InjectedParameterSettingsCollection(IEnumerable<InjectedParameterSettings> collection)
        {
            foreach(var item in collection) _parameters.Add(item);
        }

        /// <summary>
        /// Writes the object to an XElement instance.
        /// </summary>
        /// <returns>XElement representation of the object</returns>
        public override XElement WriteToXml(XName rootElement)
        {
            return new XElement(rootElement,
                from param in this
                select param.WriteToXml(PARAM));
        }

        /// <summary>
        /// Parse the specified configuration settings
        /// </summary>
        /// <param name="element">Element holding configuration settings</param>
        protected override void ParseFromXml(XElement element)
        {
            _parameters.Clear();
            element.ProcessItems(PARAM, item => _parameters.Add(new InjectedParameterSettings(item, _typeResolver)));
        }

        /// <summary>
        /// Gets the count of injected parameters
        /// </summary>
        public int Count
        {
            get { return _parameters.Count; }
        }

        /// <summary>
        /// Gets the specified element of the collection
        /// </summary>
        /// <param name="index">Element index</param>
        /// <returns>Specified collection element</returns>
        public InjectedParameterSettings this[int index]
        {
            get { return _parameters[index];  }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<InjectedParameterSettings> GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}