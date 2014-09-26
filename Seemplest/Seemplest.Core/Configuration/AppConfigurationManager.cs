using System;
using System.Configuration;
using System.Xml.Linq;
using Seemplest.Core.Common;

namespace Seemplest.Core.Configuration
{
    /// <summary>
    /// This class is responsible for managing the application configuration.
    /// </summary>
    public static class AppConfigurationManager
    {
        /// <summary>
        /// Gets the default configuration section name
        /// </summary>
        // ReSharper disable ConvertToConstant.Local
        // ReSharper disable FieldCanBeMadeReadOnly.Local
        private static string s_DefaultSectionName = "AppConfiguration";
        // ReSharper restore FieldCanBeMadeReadOnly.Local
        // ReSharper restore ConvertToConstant.Local

        /// <summary>
        /// Gets the current application configuration provider settings
        /// </summary>
        public static AppConfigurationSettings ProviderSettings { get; private set; }

        /// <summary>
        /// Initializes the static members of this class.
        /// </summary>
        static AppConfigurationManager()
        {
            Reset();
        }

        /// <summary>
        /// Resets the configuration manager to its default state.
        /// </summary>
        /// <remarks>
        /// Examines the "AppConfiguration" section in the App.Config file to load the default 
        /// configuration provider. If it fails, creates an <see cref="AppConfigProvider"/>
        /// instance as the default configuration provider.
        /// </remarks>
        public static void Reset()
        {
            // --- Obtain default configuration from the application configuration file, if there is
            // --- any specification there
            try
            {
                Configure(s_DefaultSectionName);
            }
            catch (Exception)
            {
                Configure(new AppConfigProvider());
            }
        }

        /// <summary>
        /// This event is raised when the application connfiguration is changed.
        /// </summary>
        public static event EventHandler<ConfigurationChangedEventArgs<IConfigurationProvider>> ConfigurationProviderChanged;

        /// <summary>
        /// Sets the current application configuration provider according to the
        /// specified settings.
        /// </summary>
        /// <param name="settings">Application configuration provider settings</param>
        public static void Configure(AppConfigurationSettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            var oldValue = CurrentProvider;
            ProviderSettings = settings;
            CurrentProvider = ConfigurationHelper.CreateInstance(settings.Provider, 
                settings.ConstructorParameters, 
                settings.Properties) as IConfigurationProvider;
            OnOnConfigurationChanged(
                new ConfigurationChangedEventArgs<IConfigurationProvider>(oldValue, CurrentProvider));
        }

        /// <summary>
        /// Sets the current application configuration provider from the specified section
        /// of the application configuration file.
        /// </summary>
        /// <param name="configurationSectionName">Name of the configuration section</param>
        public static void Configure(string configurationSectionName = null)
        {
            configurationSectionName = configurationSectionName ?? s_DefaultSectionName;
            var config = ConfigurationManager.GetSection(configurationSectionName) as XElement;
            Configure(new AppConfigurationSettings(config));
        }

        /// <summary>
        /// Sets the current application configuration provider to the specified one
        /// </summary>
        /// <param name="provider">Application configuration provider instance</param>
        public static void Configure(IConfigurationProvider provider)
        {
            if (provider == null) throw new ArgumentNullException("provider");
            var oldValue = CurrentProvider;
            CurrentProvider = provider;
            ProviderSettings = new AppConfigurationSettings(provider.GetType());
            OnOnConfigurationChanged(
                new ConfigurationChangedEventArgs<IConfigurationProvider>(oldValue, CurrentProvider));
        }

        /// <summary>
        /// Gets the current application configuration provider
        /// </summary>
        public static IConfigurationProvider CurrentProvider { get; private set; }

        /// <summary>
        /// Gets a setting value with the specified key.
        /// </summary>
        /// <param name="settingKey">Key of application setting</param>
        /// <returns>Settings value</returns>
        public static object GetSettingValue(string settingKey)
        {
            return CurrentProvider.GetValue(settingKey);
        }

        /// <summary>
        /// Gets a setting value with the specified key.
        /// </summary>
        /// <typeparam name="TVal">Value type of configuration parameter</typeparam>
        /// <param name="settingKey">Key of application setting</param>
        /// <returns>Settings value</returns>
        public static TVal GetSettingValue<TVal>(string settingKey)
        {
            return (TVal)Convert.ChangeType(CurrentProvider.GetValue(settingKey), typeof(TVal));
        }

        /// <summary>
        /// Gets a compound setting object with the specified key.
        /// </summary>
        /// <typeparam name="TSetting">Type of the configuration setting</typeparam>
        /// <param name="settingKey">Key of application setting</param>
        /// <returns>XML element representing the setting</returns>
        public static TSetting GetSettings<TSetting>(string settingKey)
            where TSetting : IXElementRepresentable
        {
            return CurrentProvider.GetSetting<TSetting>(settingKey);
        }

        /// <summary>
        /// Checks if the specified configuration value is defined
        /// </summary>
        /// <param name="settingKey">Configuration value key</param>
        /// <returns>True, if configuration value is defined; otherwise, false</returns>
        public static bool IsSettingValueDefined(string settingKey)
        {
            return CurrentProvider.IsValueDefined(settingKey);
        }

        /// <summary>
        /// Checks if the specified configuration section is defined
        /// </summary>
        /// <param name="settingKey">Configuration section key</param>
        /// <returns>True, if configuration section is defined; otherwise, false</returns>
        public static bool IsSectionDefined(string settingKey)
        {
            return CurrentProvider.IsSettingDefined(settingKey);
        }

        /// <summary>
        /// Raises the configuration changed event
        /// </summary>
        /// <param name="e">Event arguments</param>
        private static void OnOnConfigurationChanged(ConfigurationChangedEventArgs<IConfigurationProvider> e)
        {
            var handler =
                ConfigurationProviderChanged;
            if (handler != null) handler(null, e);
        }
    }
}