namespace Seemplest.Core.Configuration.ResourceConnections
{
    /// <summary>
    /// This interface defines factory operations for resource connections using the 
    /// registered providers.
    /// </summary>
    public interface IResourceConnectionFactory
    {
        /// <summary>
        /// Creates a connection with the specified connection type
        /// </summary>
        /// <typeparam name="TConnection">Connection object to create</typeparam>
        /// <param name="name">Resource connection name</param>
        /// <returns>Connection instance</returns>
        TConnection CreateResourceConnection<TConnection>(string name);
    }
}