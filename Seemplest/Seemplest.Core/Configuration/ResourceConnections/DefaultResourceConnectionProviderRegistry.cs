using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Seemplest.Core.Configuration.ResourceConnections
{
    /// <summary>
    /// This type is a default implementation of a resource connection registry.
    /// </summary>
    public class DefaultResourceConnectionProviderRegistry : IResourceConnectionProviderRegistry
    {
        private readonly Dictionary<string, Type> _connectionProviders =
            new Dictionary<string, Type>();

        /// <summary>
        /// Creates an empty registry
        /// </summary>
        public DefaultResourceConnectionProviderRegistry()
        {
        }

        /// <summary>
        /// Creates a new instance of this class using the specified settings
        /// </summary>
        public DefaultResourceConnectionProviderRegistry(ResourceConnectionProviderSettings settings)
        {
            foreach (var provider in settings.Providers)
            {
                RegisterResourceConnectionProvider(provider);
            }
        }

        /// <summary>
        /// Gets the resource connection by its name.
        /// </summary>
        /// <param name="name">Resource connection object name</param>
        /// <returns>Resource connection object</returns>
        public Type GetResourceConnectionProvider(string name)
        {
            Type result;
            _connectionProviders.TryGetValue(name, out result);
            return result;
        }

        /// <summary>
        /// Registers a resource connection type.
        /// </summary>
        /// <param name="type">Type representing the resource connection</param>
        /// <remarks>
        /// The type must implement the <see cref="IResourceConnectionSettings"/>
        /// interface.
        /// </remarks>
        public void RegisterResourceConnectionProvider(Type type)
        {
            // --- Check if type is a resource connection provider
            if (!typeof(IResourceConnectionProvider).IsAssignableFrom(type))
            {
                throw new ArgumentException(
                    String.Format("{0} does not implement IResourceConnectionProvider<object>.", type),
                    "type");
            }

            // --- Register the provider
            _connectionProviders.Add(GetProviderName(type), type);
        }

        /// <summary>
        /// Gets the name of the provider
        /// </summary>
        /// <param name="type">Provider type</param>
        /// <returns>Name of the provider</returns>
        public static string GetProviderName(Type type)
        {
            var name = type.Name;
            var attrs = type.GetCustomAttributes(typeof(DisplayNameAttribute), false) as DisplayNameAttribute[];
            // ReSharper disable PossibleNullReferenceException
            if (attrs.Length > 0)
            // ReSharper restore PossibleNullReferenceException
            {
                name = attrs[0].DisplayName;
            }
            return name;
        }
    }
}