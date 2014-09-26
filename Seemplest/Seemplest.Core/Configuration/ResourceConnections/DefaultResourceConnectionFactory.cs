namespace Seemplest.Core.Configuration.ResourceConnections
{
    /// <summary>
    /// This class provides factory operations for resource connections using the 
    /// registered providers.
    /// </summary>
    public class DefaultResourceConnectionFactory : IResourceConnectionFactory
    {
        /// <summary>
        /// Gest the settings used by this factory
        /// </summary>
        public ResourceConnectionFactorySettings Settings { get; private set; }

        /// <summary>
        /// Creates a new factory using the specified settings
        /// </summary>
        /// <param name="settings">Factory settings</param>
        public DefaultResourceConnectionFactory(ResourceConnectionFactorySettings settings)
        {
            Settings = settings;
        }

        /// <summary>
        /// Creates a connection with the specified connection type
        /// </summary>
        /// <typeparam name="TConnection">Connection object to create</typeparam>
        /// <param name="name">Resource connection name</param>
        /// <returns></returns>
        public TConnection CreateResourceConnection<TConnection>(string name)
        {
            var provider = Settings.Providers[name];
            return (TConnection)provider.GetResourceConnectionFromSettings();
        }
    }
}