using System;

namespace Seemplest.Core.Configuration.ResourceConnections
{
    /// <summary>
    /// This class is responsible for creating resource connections 
    /// in the application.
    /// </summary>
    public static class ResourceConnectionFactory
    {
        /// <summary>
        /// Gets the default configuration section name
        /// </summary>
        public const string DEFAULT_SECTION_NAME = "ResourceConnections";

        /// <summary>
        /// Gets the current resource connection factory object
        /// </summary>
        public static IResourceConnectionFactory Current { get; private set; }

        /// <summary>
        /// This event is raised when the configuration of ResourceConnectionFactory
        /// has been changed.
        /// </summary>
        public static event EventHandler<ConfigurationChangedEventArgs<IResourceConnectionFactory>> ConfigurationChanged;

        /// <summary>
        /// Initializes ResourceConnectionFactory to use a default factory object.
        /// </summary>
        static ResourceConnectionFactory()
        {
            Reset();
        }

        /// <summary>
        /// Resets the resource connection provider to its original state
        /// </summary>
        public static void Reset()
        {
            if (AppConfigurationManager.IsSectionDefined(DEFAULT_SECTION_NAME))
            {
                Configure(DEFAULT_SECTION_NAME);
            }
        }

        /// <summary>
        /// Sets the current resource connection factory object to the specified one.
        /// </summary>
        /// <param name="provider">
        /// Resource connection factory object to be used as current
        /// </param>
        public static void Configure(IResourceConnectionFactory provider)
        {
            var oldValue = Current;
            Current = provider;
            OnConfigurationChanged(
                new ConfigurationChangedEventArgs<IResourceConnectionFactory>(oldValue, Current));
        }

        /// <summary>
        /// Sets the current resource connection factory object to a 
        /// <see cref="DefaultResourceConnectionFactory"/>
        /// instance with the specified settings.
        /// </summary>
        /// <param name="settings">Configuration settings to be used.</param>
        public static void Configure(ResourceConnectionFactorySettings settings)
        {
            var oldValue = Current;
            Current = new DefaultResourceConnectionFactory(settings);
            OnConfigurationChanged(
                new ConfigurationChangedEventArgs<IResourceConnectionFactory>(oldValue, Current));
        }

        /// <summary>
        /// Uses the specified application configuration section to set the current 
        /// resource connection provider registry object.
        /// </summary>
        /// <param name="configurationSectionName"></param>
        public static void Configure(string configurationSectionName = null)
        {
            configurationSectionName = configurationSectionName ?? DEFAULT_SECTION_NAME;
            var settings = AppConfigurationManager.GetSettings<ResourceConnectionFactorySettings>(configurationSectionName);
            Configure(settings);
        }

        /// <summary>
        /// Creates a connection with the specified connection type
        /// </summary>
        /// <typeparam name="TConnection">Connection object to create</typeparam>
        /// <param name="name">Resource connection name</param>
        /// <returns>Connection instance</returns>
        public static TConnection CreateResourceConnection<TConnection>(string name)
        {
            return Current.CreateResourceConnection<TConnection>(name);
        }

        /// <summary>
        /// Raises the <see cref="ConfigurationChanged"/> event
        /// </summary>
        /// <param name="e">Configuration change event arguments</param>
        private static void OnConfigurationChanged(ConfigurationChangedEventArgs<IResourceConnectionFactory> e)
        {
            var handler = ConfigurationChanged;
            if (handler != null) handler(null, e);
        }
    }
}