using System;

namespace SeemplestBlocks.Core.AppConfig
{
    /// <summary>
    /// This attribute defines the key of the configuration
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigurationKeyAttribute: Attribute
    {
        /// <summary>
        /// Key value
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Initializes the attribute's value
        /// </summary>
        /// <param name="value">Initial value</param>
        public ConfigurationKeyAttribute(string value)
        {
            Value = value;
        }
    }
}