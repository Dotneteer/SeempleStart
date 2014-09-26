using System;
using System.Xml;
using System.Xml.Linq;
using Seemplest.Core.Common;

namespace Seemplest.Core.Configuration
{
    /// <summary>
    /// This class is intended to be the base class of a type describing
    /// configuration settings.
    /// </summary>
    public abstract class ConfigurationSettingsBase: IXElementRepresentable
    {
        /// <summary>
        /// Creates a new instance of this class to be used as a default base constructor
        /// in derived classes.
        /// </summary>
        protected ConfigurationSettingsBase()
        {
        }

        /// <summary>
        /// Creates a new instance of this class and initializes it from the specified XML element.
        /// </summary>
        /// <param name="element">XML element to initialize this instance from.</param>
        protected ConfigurationSettingsBase(XElement element)
        {
            if (element == null) throw new ArgumentNullException("element");
            ReadFromXml(element);
        }
        /// <summary>
        /// Writes the object to an XElement instance using the specified root element name.
        /// </summary>
        /// <param name="rootElement">Root element name</param>
        /// <returns>XElement representation of the object</returns>
        public abstract XElement WriteToXml(XName rootElement);

        /// <summary>
        /// Reads the configuration elements from the specified element
        /// </summary>
        /// <param name="element">Element to read the configuration from</param>
        public void ReadFromXml(XElement element)
        {
            try
            {
                ParseFromXml(element);
            }
            catch (Exception ex)
            {
                throw new XmlException(
                    "An exception has been caught when reading an XML configuration setting", ex);
            }    
        }

        /// <summary>
        /// Parse the specified configuration settings
        /// </summary>
        /// <param name="element">Element holding configuration settings</param>
        protected abstract void ParseFromXml(XElement element);

        /// <summary>
        /// Clones this configutration element to the specified type.
        /// </summary>
        /// <typeparam name="T">Destination setting type</typeparam>
        /// <returns>Clone of the settings class</returns>
        protected T Clone<T>()
            where T : class, IXElementRepresentable
        {
            var parameters = new object[] { WriteToXml("Temp") };
            var clone = (T)Activator.CreateInstance(typeof (T), parameters);
            return clone;
        }
    }
}
