using System;

namespace Seemplest.Core.ServiceObjects
{
    /// <summary>
    /// This interface defines the responsibility of a factory that creates service objects.
    /// </summary>
    public interface IContextedServiceFactory
    {
        /// <summary>
        /// Creates a servic object of the specified type.
        /// </summary>
        /// <param name="serviceType">Service object type</param>
        /// <returns>Service object instance</returns>
        object CreateService(Type serviceType);

        /// <summary>
        /// Creates a service object of the specified type.
        /// </summary>
        /// <typeparam name="TService">Service object type</typeparam>
        /// <returns>Service object instance</returns>
        TService CreateService<TService>()
            where TService: class, IServiceObject;
    }
}