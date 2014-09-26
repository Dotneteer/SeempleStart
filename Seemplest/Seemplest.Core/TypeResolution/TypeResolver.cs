using System;
using Seemplest.Core.Configuration;

namespace Seemplest.Core.TypeResolution
{
    /// <summary>
    /// This class is responsible for managing type resolution in the application.
    /// </summary>
    public static class TypeResolver
    {
        /// <summary>
        /// Gets the default configuration section name
        /// </summary>
        public const string DEFAULT_SECTION_NAME = "TypeResolver";

        /// <summary>
        /// Gets the current type resolver object
        /// </summary>
        public static ITypeResolver Current { get; private set; }

        /// <summary>
        /// This event is raised when the configuration of TypeResolver has been changed.
        /// </summary>
        public static event EventHandler<ConfigurationChangedEventArgs<ITypeResolver>> ConfigurationChanged;

        /// <summary>
        /// Initializes TypeResolver to use a default type resolver.
        /// </summary>
        static TypeResolver()
        {
            Reset();
        }

        /// <summary>
        /// Resets the type resolver into its original state
        /// </summary>
        public static void Reset()
        {
            Configure();
        }

        /// <summary>
        /// Sets the current type resolver object to a <see cref="DefaultTypeResolver"/>
        /// instance with the specified settings.
        /// </summary>
        /// <param name="settings">Configuration settings to be used.</param>
        /// <param name="parentResolver">Parent type resolver</param>
        public static void Configure(TypeResolverConfigurationSettings settings, ITypeResolver parentResolver = null) 
        { 
            var oldValue = Current;
            Current = new DefaultTypeResolver(settings, parentResolver);
            OnConfigurationChanged(new ConfigurationChangedEventArgs<ITypeResolver>(oldValue, Current));
        }

        /// <summary>
        /// Sets the current type resolver object to the specified one.
        /// </summary>
        /// <param name="typeResolver">Type resolver object to be used as current</param>
        public static void Configure(ITypeResolver typeResolver) 
        { 
            var oldValue = Current;
            Current = typeResolver;
            OnConfigurationChanged(new ConfigurationChangedEventArgs<ITypeResolver>(oldValue, Current));
        }

        /// <summary>
        /// Uses the specified application configuration section to set the current type resolver object.
        /// </summary>
        /// <param name="configurationSectionName"></param>
        public static void Configure(string configurationSectionName = null) 
        { 
            configurationSectionName = configurationSectionName ?? DEFAULT_SECTION_NAME;
            var settings = AppConfigurationManager.GetSettings<TypeResolverConfigurationSettings>(configurationSectionName);
            Configure(settings);
        }

        /// <summary>
        /// Raises the <see cref="ConfigurationChanged"/> event
        /// </summary>
        /// <param name="e">Configuration change event arguments</param>
        private static void OnConfigurationChanged(ConfigurationChangedEventArgs<ITypeResolver> e)
        {
            var handler = ConfigurationChanged;
            if (handler != null) handler(null, e);
        }
    }
}