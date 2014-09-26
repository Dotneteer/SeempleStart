using System;

namespace Seemplest.Core.Configuration.ResourceConnections
{
    /// <summary>
    /// This class is responsible for managing the registry of resource
    /// connection providers in the application.
    /// </summary>
    public static class ResourceConnectionProviderRegistry
    {
        /// <summary>
        /// Gets the default configuration section name
        /// </summary>
        public const string DEFAULT_SECTION_NAME = "ConnectionProviders";

        /// <summary>
        /// Gets the current resource connection provider object
        /// </summary>
        public static IResourceConnectionProviderRegistry Current { get; private set; }

        /// <summary>
        /// This event is raised when the configuration of ResourceConnectionProviderRegistry
        /// has been changed.
        /// </summary>
        public static event EventHandler<ConfigurationChangedEventArgs<IResourceConnectionProviderRegistry>> ConfigurationChanged;

        /// <summary>
        /// Initializes ResourceConnectionProviderRegistry to use a default registry.
        /// </summary>
        static ResourceConnectionProviderRegistry()
        {
            Configure();    
        }

        /// <summary>
        /// Resets the resource connection provider registry to its original state
        /// </summary>
        public static void Reset()
        {
            Configure();
        }

        /// <summary>
        /// Sets the current resource connection provider registry object to a 
        /// <see cref="DefaultResourceConnectionProviderRegistry"/>
        /// instance with the specified settings.
        /// </summary>
        /// <param name="settings">Configuration settings to be used.</param>
        public static void Configure(ResourceConnectionProviderSettings settings)
        {
            var oldValue = Current;
            Current = new DefaultResourceConnectionProviderRegistry(settings);
            OnConfigurationChanged(
                new ConfigurationChangedEventArgs<IResourceConnectionProviderRegistry>(oldValue, Current));
        }
        
        /// <summary>
        /// Sets the current resource connection provider registry object to the specified one.
        /// </summary>
        /// <param name="provider">
        /// Resource connection provider object to be used as current
        /// </param>
        public static void Configure(IResourceConnectionProviderRegistry provider)
        {
            var oldValue = Current;
            Current = provider;
            OnConfigurationChanged(
                new ConfigurationChangedEventArgs<IResourceConnectionProviderRegistry>(oldValue, Current));
        }

        /// <summary>
        /// Uses the specified application configuration section to set the current 
        /// resource connection provider registry object.
        /// </summary>
        /// <param name="configurationSectionName"></param>
        public static void Configure(string configurationSectionName = null)
        {
            configurationSectionName = configurationSectionName ?? DEFAULT_SECTION_NAME;
            var settings = AppConfigurationManager.GetSettings<ResourceConnectionProviderSettings>(configurationSectionName);
            Configure(settings);
        }

        /// <summary>
        /// Raises the <see cref="ConfigurationChanged"/> event
        /// </summary>
        /// <param name="e">Configuration change event arguments</param>
        private static void OnConfigurationChanged(ConfigurationChangedEventArgs<IResourceConnectionProviderRegistry> e)
        {
            var handler = ConfigurationChanged;
            if (handler != null) handler(null, e);
        }
    }
}