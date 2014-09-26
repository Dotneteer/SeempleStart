namespace SeemplestBlocks.Core.AppConfig
{
    /// <summary>
    /// This interface defines the operation that read and write configuration values
    /// </summary>
    public interface IConfigurationHandler : IConfigurationReader
    {
        /// <summary>
        /// Stores the specified configuration value
        /// </summary>
        /// <param name="category">Configuration item category</param>
        /// <param name="key">Configuration key</param>
        /// <param name="value">Configuration value</param>
        void SetConfigurationValue(string category, string key, string value);

        /// <summary>
        /// Checks whether the configuration value changes since the last check
        /// </summary>
        /// <param name="category">Configuration item category</param>
        /// <param name="key">Configuration key</param>
        /// <returns>True, if the configuration vale has changed; otherwise, false</returns>
        bool ChangedSinceLastCheck(string category, string key);

        /// <summary>
        /// Gets the current configuration version
        /// </summary>
        /// <returns>Configuration version key</returns>
        int? GetCurrentConfigurationVersion();
    }
}