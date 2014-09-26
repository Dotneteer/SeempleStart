using System;
using Seemplest.Core.DependencyInjection;

namespace Seemplest.Core.ServiceObjects
{
    /// <summary>
    /// This abstract class is intended to be the base class of simple service object 
    /// factory types.
    /// </summary>
    public abstract class ContextedServiceFactoryBase : IContextedServiceFactory
    {
        /// <summary>
        /// Gets the service locator used by this factory
        /// </summary>
        public IServiceLocator ServiceLocator { get; private set; }

        /// <summary>
        /// Initializes this factory with the specified service locator
        /// </summary>
        /// <param name="serviceLocator">Service locator used by this service</param>
        protected ContextedServiceFactoryBase(IServiceLocator serviceLocator)
        {
            ServiceLocator = serviceLocator;
        }

        /// <summary>
        /// Creates a servic object of the specified type.
        /// </summary>
        /// <param name="serviceType">Service object type</param>
        /// <returns>Service object instance</returns>
        public object CreateService(Type serviceType)
        {
            // --- Obtain the service object
            var serviceObject = ServiceLocator.GetService(serviceType);
            if (serviceObject == null) return null;
            var srvObjIntf = serviceObject as IServiceObject;
            if (srvObjIntf != null)
            {
                srvObjIntf.SetServiceLocator(ServiceLocator);
                var context = CreateServiceCallContext();
                srvObjIntf.SetCallContext(context);
            }
            return serviceObject;
        }

        /// <summary>
        /// Creates a servic object of the specified type.
        /// </summary>
        /// <typeparam name="TService">Service object type</typeparam>
        /// <returns>Service object instance</returns>
        public TService CreateService<TService>() 
            where TService : class, IServiceObject
        {
            return CreateService(typeof (TService)) as TService;
        }

        /// <summary>
        /// Creates a call context for the service object
        /// </summary>
        /// <returns>Service call context instance</returns>
        protected abstract IServiceCallContext CreateServiceCallContext();
    }
}