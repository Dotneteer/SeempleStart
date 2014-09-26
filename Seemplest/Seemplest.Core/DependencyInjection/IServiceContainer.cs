using System;
using System.Collections.Generic;
using Seemplest.Core.Configuration;

namespace Seemplest.Core.DependencyInjection
{
    /// <summary>
    /// This class defines the responsibility of a service registry
    /// </summary>
    public interface IServiceContainer : IServiceLocator
    {
        /// <summary>
        /// Registers the specified object with its related lifetime manager, and the
        /// construction parameters used by the lifetime manager.
        /// </summary>
        /// <param name="serviceType">Type of service to register</param>
        /// <param name="serviceObjectType">Type of object implementing the service</param>
        /// <param name="parameters">Object construction parameters</param>
        /// <param name="properties">Object properties to inject</param>
        /// <param name="ltManager">Lifetime manager object</param>
        /// <param name="customContext"></param>
        void Register(Type serviceType, Type serviceObjectType, InjectedParameterSettingsCollection parameters = null,
            PropertySettingsCollection properties = null, ILifetimeManager ltManager = null, object customContext = null);

        /// <summary>
        /// Removes the specified service from the registry
        /// </summary>
        /// <param name="serviceType"></param>
        void RemoveService(Type serviceType);

        /// <summary>
        /// Gets the collection of registered services.
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<Type> GetRegisteredServices();

        /// <summary>
        /// Gets the name of the container.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the parent service container.
        /// </summary>
        IServiceContainer Parent { get; }
    }
}