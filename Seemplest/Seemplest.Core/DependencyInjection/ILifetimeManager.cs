using Seemplest.Core.Configuration;
using System;

namespace Seemplest.Core.DependencyInjection
{
    /// <summary>
    /// This interface defines the responsibilities of a lifetime manager.
    /// </summary>
    public interface ILifetimeManager
    {
        /// <summary>
        /// Gets or sets the type of the service supported by this lifetime manager
        /// </summary>
        Type ServiceType { get; set; }
        
        /// <summary>
        /// Gets or sets the type of the service object provided by this lifetime manager
        /// </summary>
        Type ServiceObjectType { get; set; }

        /// <summary>
        /// Gets or sets the construction parameters of this lifetime manager
        /// </summary>
        object[] ConstructionParameters { get; set; }

        /// <summary>
        /// Gets or sets the property values to set after construction
        /// </summary>
        PropertySettingsCollection Properties { get; set; }

        /// <summary>
        /// Gets or sets the custom context object of the lifetime manager
        /// </summary>
        object CustomContext { get; set; }

        /// <summary>
        /// Retrieve an object from the backing store associated with this Lifetime manager.
        /// </summary>
        /// <returns>
        /// The object retrieved by the lifetime manager.
        /// </returns>
        object GetObject();

        /// <summary>
        /// Resets the state of the lifetime manager
        /// </summary>
        void ResetState();
    }
}