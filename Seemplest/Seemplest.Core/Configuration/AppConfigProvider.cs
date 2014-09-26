using System;
using System.Configuration;
using System.Xml.Linq;
using Seemplest.Core.Common;

namespace Seemplest.Core.Configuration
{
    /// <summary>
    /// This class represents a configuration provider that uses the 
    /// application configuration file to obtain and write configuration settings
    /// </summary>
    public class AppConfigProvider : IConfigurationProvider
    {
        /// <summary>
        /// Gets a value with the specified key.
        /// </summary>
        /// <param name="settingKey">Key of application setting</param>
        /// <returns>Settings value</returns>
        public string GetValue(string settingKey)
        {
            return ConfigurationManager.AppSettings[settingKey];
        }

        /// <summary>
        /// Gets a compound setting object with the specified key.
        /// </summary>
        /// <typeparam name="TSetting">Type of the configuration setting</typeparam>
        /// <param name="settingKey">Key of application setting</param>
        /// <returns>XML element representing the setting</returns>
        public TSetting GetSetting<TSetting>(string settingKey)
            where TSetting : IXElementRepresentable
        {
            var section = ConfigurationManager.GetSection(settingKey);
            if (section == null)
            {
                throw new ConfigurationErrorsException(
                    String.Format("Configuration section '{0}' cannot be found",
                    settingKey));
            }
            var element = (XElement)section;
            return (TSetting)Activator.CreateInstance(typeof(TSetting), new object[] { element });
        }

        /// <summary>
        /// Sets a value with the specified key.
        /// </summary>
        /// <param name="settingKey">Key of application setting</param>
        /// <param name="value">Settings value</param>
        public void SetValue(string settingKey, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var setting = config.AppSettings.Settings[settingKey];
            if (setting != null)
            {
                setting.Value = value;
            }
            else
            {
                config.AppSettings.Settings.Add(settingKey, value);
            }
            config.Save();
        }

        /// <summary>
        /// Sets a compound setting object with the specified key.
        /// </summary>
        /// <typeparam name="TSetting">Type of the configuration setting</typeparam>
        /// <param name="settingKey">Key of application setting</param>
        /// <param name="value">Setting value</param>
        public void SetSetting<TSetting>(string settingKey, TSetting value) where TSetting : IXElementRepresentable
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.GetSection(settingKey).SectionInformation.SetRawXml(value.WriteToXml(settingKey).ToString());
            config.Save();
        }

        /// <summary>
        /// Checks whether the specified value is defined.
        /// </summary>
        /// <param name="settingKey">Key of application setting</param>
        /// <returns>True, if configuration value is defined; otherwise, false</returns>
        public bool IsValueDefined(string settingKey)
        {
            return ConfigurationManager.AppSettings[settingKey] != null;
        }

        /// <summary>
        /// Checks whether the specified configuration setting is defined.
        /// </summary>
        /// <param name="settingKey">Key of application setting</param>
        /// <returns>True, if configuration section is defined; otherwise, false</returns>
        public bool IsSettingDefined(string settingKey)
        {
            return ConfigurationManager.GetSection(settingKey) != null;
        }
    }
}