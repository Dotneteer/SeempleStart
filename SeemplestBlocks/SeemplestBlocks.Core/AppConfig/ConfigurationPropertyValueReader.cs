using System;
using System.ComponentModel;
using System.Diagnostics;
using Seemplest.Core.DependencyInjection;

namespace SeemplestBlocks.Core.AppConfig
{
    /// <summary>
    /// This class can be used to access the configuration values within the 
    /// classes defining configuration sets
    /// használható a konfigurációs értékek meghatározásához
    /// </summary>
    public sealed class ConfigurationPropertyValueReader
    {
        private readonly IConfigurationReader _valueReader;
        private readonly string _categoryName;
        private readonly Type _configurationSetType;

        /// <summary>
        /// Initializes the dependencies of the object
        /// </summary>
        public ConfigurationPropertyValueReader()
        {
            _valueReader = ServiceManager.GetService<IConfigurationReader>();
            var fr = new StackFrame(1, false);
            _configurationSetType = fr.GetMethod().DeclaringType;
            // ReSharper disable once PossibleNullReferenceException
            var attrs = _configurationSetType.GetCustomAttributes(typeof(ConfigurationCategoryAttribute), false)
                as ConfigurationCategoryAttribute[];
            _categoryName = attrs != null && attrs.Length > 0
                ? attrs[0].Value
                : _configurationSetType.Name;
        }

        /// <summary>
        /// Gets the configuration value of the specified property
        /// </summary>
        /// <typeparam name="T">Type of configuration value</typeparam>
        /// <param name="nameOfProperty">Property name</param>
        /// <param name="defaultValue">Default property value</param>
        /// <returns>Configuration value</returns>
        public T GetValue<T>(string nameOfProperty, T defaultValue = default(T))
        {
            string value;
            if (!_valueReader.GetConfigurationValue(_categoryName, nameOfProperty, out value))
            {
                return defaultValue;
            }

            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T) converter.ConvertFrom(value);
        }

        /// <summary>
        /// Reads the configuration value represented by the property from the getter method of
        /// which this operation is called.
        /// </summary>
        /// <typeparam name="T">Type of configuration value</typeparam>
        /// <returns>Configuration value</returns>
        /// <remarks>
        /// This operation uses the call stack to evaluate the configuration value
        /// </remarks>
        public T GetValue<T>()
        {
            StackFrame fr = null;
            for (var i = 0; i < 5; i++)
            {
                var tempFr = new StackFrame(i, false);
                if (tempFr.GetMethod().DeclaringType != _configurationSetType) continue;
                fr = tempFr;
                break;
            }
            if (fr != null)
            {
                var propName = RemovePrefixOfGetters(fr.GetMethod().Name);
                // ReSharper disable once PossibleNullReferenceException
                var prop = _configurationSetType.GetProperty(propName);
                if (prop != null)
                {
                    var attrs = prop.GetCustomAttributes(typeof(ConfigurationKeyAttribute), false)
                            as ConfigurationKeyAttribute[];
                    var name = attrs != null && attrs.Length > 0
                        ? attrs[0].Value
                        : RemovePrefixOfGetters(propName);
                    return GetValue<T>(name);
                }
            }
            return GetValue<T>("$$$None");
        }

        /// <summary>
        /// Removes prefix of getter and setter functions
        /// </summary>
        /// <param name="nameOfMethod">The name of the method</param>
        /// <returns>The nem of the Property</returns>
        private static string RemovePrefixOfGetters(string nameOfMethod)
        {
            // ReSharper disable StringIndexOfIsCultureSpecific.1
            return nameOfMethod.IndexOf("get_") == 0 
                ? nameOfMethod.Substring(4) : nameOfMethod;
            // ReSharper restore StringIndexOfIsCultureSpecific.1
        }
    }
}