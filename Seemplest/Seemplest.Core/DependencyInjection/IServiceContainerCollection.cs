using System.Collections.Generic;

namespace Seemplest.Core.DependencyInjection
{
    /// <summary>
    /// This interface defines the behavior of service container collection
    /// </summary>
    public interface IServiceContainerCollection
    {
        /// <summary>
        /// Checks if the registry has a container with the specified name.
        /// </summary>
        /// <param name="name">Container name</param>
        /// <returns>True, if the registry has the specified container; otherwise, false.</returns>
        bool HasContainer(string name);

        /// <summary>
        /// Gets the service container with the specified name.
        /// </summary>
        /// <param name="name">Service container name</param>
        /// <returns>Service container instance if found; otherwise, null</returns>
        /// <remarks>Use null or empty string as a name to obtain the default container.</remarks>
        IServiceContainer this[string name] { get; }

        /// <summary>
        /// Gets the container names used within this registry
        /// </summary>
        IReadOnlyList<string> ContainerNames { get; }

        /// <summary>
        /// Gets the number of containers within this registry
        /// </summary>
        int ContainerCount { get; }
    }
}