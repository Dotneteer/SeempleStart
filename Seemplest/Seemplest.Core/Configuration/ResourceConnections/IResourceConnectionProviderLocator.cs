using System;

namespace Seemplest.Core.Configuration.ResourceConnections
{
    /// <summary>
    /// This interface defines the operations of a resource connection locator
    /// </summary>
    public interface IResourceConnectionProviderLocator
    {
        /// <summary>
        /// Gets the resource connection by its name.
        /// </summary>
        /// <param name="name">Resource connection provider name</param>
        /// <returns>Resource connection object type</returns>
        Type GetResourceConnectionProvider(string name);
    }
}