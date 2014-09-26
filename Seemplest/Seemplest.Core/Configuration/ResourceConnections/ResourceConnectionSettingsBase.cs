using System.Collections.Generic;
using System.Xml.Linq;
using Seemplest.Core.Common;

namespace Seemplest.Core.Configuration.ResourceConnections
{
    /// <summary>
    /// This class is intended to be the base class of all resource connection settings.
    /// </summary>
    public abstract class ResourceConnectionSettingsBase : ConfigurationSettingsBase, IResourceConnectionSettings
    {
        private const string NAME = "name";

        /// <summary>
        /// Gets the name of the resource connection
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Writes the object to an XElement instance using the specified root element name.
        /// </summary>
        /// <param name="rootElement">Root element name</param>
        /// <returns>XElement representation of the object</returns>
        public override XElement WriteToXml(XName rootElement)
        {
            return new XElement(rootElement,
                new XAttribute(NAME, Name),
                GetAdditionalSettings());
        }

        /// <summary>
        /// Adds additional XElement and XAttribute settings to the XML representation
        /// </summary>
        /// <returns></returns>
        public virtual List<XObject> GetAdditionalSettings()
        {
            return new List<XObject>();
        }

        /// <summary>
        /// Parse the specified configuration settings
        /// </summary>
        /// <param name="element">Element holding configuration settings</param>
        protected override void ParseFromXml(XElement element)
        {
            Name = element.StringAttribute(NAME);
        }
    }
}