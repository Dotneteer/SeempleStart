using System.Configuration;

namespace SeemplestBlocks.Core.AppConfig
{
    /// <summary>
    /// This class uses the App.Config file to read configuration values
    /// </summary>
    public class AppConfigReader: IConfigurationReader
    {
        /// <summary>
        /// Reads the configuration value with the specified key
        /// </summary>
        /// <param name="category">Category of the configuration item</param>
        /// <param name="key">The key of the configuration item</param>
        /// <param name="value">Configuration value, provided, it is found</param>
        /// <returns>True, if the configuration value is found; otherwise, false</returns>
        public bool GetConfigurationValue(string category, string key, out string value)
        {
            var configKey = string.Format("{0}.{1}", category, key);
            var configValue = ConfigurationManager.AppSettings[configKey];
            value = null;
            if (configValue == null)
            {
                return false;
            }
            value = configValue;
            return true;
        }
    }
}