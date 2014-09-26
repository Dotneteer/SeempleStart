using Seemplest.Core.Common;

namespace Seemplest.Core.Configuration
{
    /// <summary>
    /// This interface describes the responsibilities of a provider that allows application
    /// components to access their configuration.
    /// </summary>
    public interface IConfigurationProvider
    {
        /// <summary>
        /// Gets a value with the specified key.
        /// </summary>
        /// <param name="settingKey">Key of application setting</param>
        /// <returns>Settings value</returns>
        string GetValue(string settingKey);

        /// <summary>
        /// Gets a compound setting object with the specified key.
        /// </summary>
        /// <typeparam name="TSetting">Type of the configuration setting</typeparam>
        /// <param name="settingKey">Key of application setting</param>
        /// <returns>XML element representing the setting</returns>
        TSetting GetSetting<TSetting>(string settingKey)
            where TSetting : IXElementRepresentable;

        /// <summary>
        /// Sets a value with the specified key.
        /// </summary>
        /// <param name="settingKey">Key of application setting</param>
        /// <param name="value">Settings value</param>
        void SetValue(string settingKey, string value);

        /// <summary>
        /// Sets a compound setting object with the specified key.
        /// </summary>
        /// <typeparam name="TSetting">Type of the configuration setting</typeparam>
        /// <param name="settingKey">Key of application setting</param>
        /// <param name="value">Setting value</param>
        void SetSetting<TSetting>(string settingKey, TSetting value)
            where TSetting : IXElementRepresentable;

        /// <summary>
        /// Checks whether the specified value is defined.
        /// </summary>
        /// <param name="settingKey">Key of application setting</param>
        /// <returns>True, if configuration value is defined; otherwise, false</returns>
        bool IsValueDefined(string settingKey);

        /// <summary>
        /// Checks whether the specified configuration setting is defined.
        /// </summary>
        /// <param name="settingKey">Key of application setting</param>
        /// <returns>True, if configuration section is defined; otherwise, false</returns>
        bool IsSettingDefined(string settingKey);
    }
}