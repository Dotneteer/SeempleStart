using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using Seemplest.Core.Configuration;

namespace Seemplest.Core.Common
{
    /// <summary>
    /// This class contains helper methods for managing application configuration settings
    /// </summary>
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Instantiates an object and sets its properties as provided.
        /// </summary>
        /// <param name="type">Type to instantiate</param>
        /// <param name="properties">Collection of properties to set up</param>
        /// <returns>The newly instantiated object</returns>
        public static object CreateInstance(Type type, PropertySettingsCollection properties)
        {
            // --- Instantiate the object
            var tempInstance = Activator.CreateInstance(type);
            InjectProperties(ref tempInstance, properties);
            return tempInstance;
        }

        /// <summary>
        /// Instantiates an object and sets its properties as provided.
        /// </summary>
        /// <param name="type">Type to instantiate</param>
        /// <param name="constructorParams">Collection of construction parameters</param>
        /// <param name="properties">Collection of properties to set up</param>
        /// <returns>The newly instantiated object</returns>
        public static object CreateInstance(Type type, ConstructorParameterSettingsCollection constructorParams,
            PropertySettingsCollection properties)
        {
            // --- Create the constructor parameter array
            var parameters = new object[constructorParams.Count];
            for (var i = 0; i < constructorParams.Count; i++)
            {
                var converter = TypeDescriptor.GetConverter(constructorParams[i].Type);
                parameters[i] = converter.ConvertFromString(constructorParams[i].Value);
            }

            // --- Instantiate the object
            var tempInstance = Activator.CreateInstance(type, parameters);
            InjectProperties(ref tempInstance, properties);
            return tempInstance;
        }

        /// <summary>
        /// Instantiates an object and sets its properties as provided.
        /// </summary>
        /// <param name="type">Type to instantiate</param>
        /// <param name="constructorParams">Collection of construction parameters</param>
        /// <param name="properties">Collection of properties to set up</param>
        /// <returns>The newly instantiated object</returns>
        public static object CreateInstance(Type type, object[] constructorParams,
            PropertySettingsCollection properties)
        {
            // --- Instantiate the object
            var tempInstance = Activator.CreateInstance(type, constructorParams);
            InjectProperties(ref tempInstance, properties);
            return tempInstance;
        }

        /// <summary>
        /// Sets the properties of an object as provided.
        /// </summary>
        /// <param name="instance">Object instance</param>
        /// <param name="properties">Object properties</param>
        public static void InjectProperties(ref object instance, PropertySettingsCollection properties)
        {
            if (properties == null) return;
            var type = instance.GetType();
            foreach (var parameter in properties)
            {
                // --- Set up the initial values of properties
                var propInfo = type.GetProperty(parameter.Name, BindingFlags.Instance | BindingFlags.Public);
                if (propInfo == null)
                {
                    // --- Undefined property used
                    throw new ConfigurationErrorsException(
                        string.Format("Type {0} does not have '{1}' public property.",
                                      type, parameter.Name));
                }
                object objectValue;
                if (propInfo.PropertyType.IsEnum)
                {
                    objectValue = Enum.Parse(propInfo.PropertyType, parameter.Value);
                }
                else
                {
                    objectValue = Convert.ChangeType(parameter.Value, propInfo.PropertyType,
                        CultureInfo.InvariantCulture);
                }
                propInfo.SetValue(instance, objectValue, null);
            }
        }

        /// <summary>
        /// Instantiates an object and sets its properties as provided.
        /// </summary>
        /// <param name="type">Type to instantiate</param>
        /// <param name="properties">Collection of properties to set up</param>
        /// <returns>The newly instantiated object</returns>
        public static object PrepareInstance(Type type, PropertySettingsCollection properties)
        {
            // --- Instantiate the object
            var tempInstance = Activator.CreateInstance(type);
            InjectProperties(ref tempInstance, properties);
            return tempInstance;
        }
    }
}