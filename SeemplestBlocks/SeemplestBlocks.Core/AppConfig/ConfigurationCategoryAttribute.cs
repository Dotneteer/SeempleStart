using System;

namespace SeemplestBlocks.Core.AppConfig
{
    /// <summary>
    /// This attribute defines the configuration category
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurationCategoryAttribute : Attribute
    {
        /// <summary>
        /// Category value
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Initializes the attribute's value
        /// </summary>
        /// <param name="value">Initial value</param>
        public ConfigurationCategoryAttribute(string value)
        {
            Value = value;
        }
    }
}