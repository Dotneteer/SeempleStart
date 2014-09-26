using Seemplest.Core.Common;

namespace Seemplest.Core.Configuration.ResourceConnections
{
    /// <summary>
    /// Defines the responsibility of a resource connection settings object
    /// </summary>
    public interface IResourceConnectionSettings : IXElementRepresentable
    {
        /// <summary>
        /// Gets the name of the resource connection
        /// </summary>
        string Name { get; }
    }
}