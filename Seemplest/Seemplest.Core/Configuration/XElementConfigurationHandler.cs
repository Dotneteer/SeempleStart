using System.Configuration;
using System.Xml;
using System.Xml.Linq;

namespace Seemplest.Core.Configuration
{
    /// <summary>
    /// This section parses the embedded part of a configuration section and retrieves it as an
    /// <see cref="XDocument"/> instance.
    /// </summary>
    public class XElementConfigurationHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// Creates a configuration section handler.
        /// </summary>
        /// <returns>
        /// The created section handler object.
        /// </returns>
        /// <param name="parent">Parent object.</param>
        /// <param name="configContext">Configuration context object.</param>
        /// <param name="section">Section XML node.</param>
        public object Create(object parent, object configContext, XmlNode section)
        {
            return XElement.Parse(section.OuterXml);
        }
    }
}