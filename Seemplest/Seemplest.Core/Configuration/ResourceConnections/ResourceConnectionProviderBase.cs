namespace Seemplest.Core.Configuration.ResourceConnections
{
    /// <summary>
    /// This class is intended to be the base class of all resource connection providers.
    /// </summary>
    public abstract class ResourceConnectionProviderBase :
        ResourceConnectionSettingsBase,
        IResourceConnectionProvider
    {
        /// <summary>
        /// Creates a new resource connection object from the settings.
        /// </summary>
        /// <returns>Newly created resource object</returns>
        public abstract object GetResourceConnectionFromSettings();
    }
}