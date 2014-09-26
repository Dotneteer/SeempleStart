namespace Seemplest.Core.Configuration.ResourceConnections
{
    /// <summary>
    /// This interface defines a resource connection provider
    /// </summary>
    public interface IResourceConnectionProvider : IResourceConnectionSettings
    {
        /// <summary>
        /// Creates a new resource connection object from the settings.
        /// </summary>
        /// <returns>Newly created resource object</returns>
        object GetResourceConnectionFromSettings();
    }
}