using System;
using Seemplest.Core.DependencyInjection;

namespace Seemplest.Core.DataAccess.DataServices
{
    /// <summary>
    /// This class implements a default data access factory that creates data
    /// operation objects through the provided service registry
    /// </summary>
    public class DefaultDataAccessFactory : DataAccessFactoryBase
    {
        private readonly IServiceRegistry _serviceRegistry;

        /// <summary>
        /// Creates a new instance with the specified service registry
        /// </summary>
        /// <param name="serviceRegistry">Service registry instance</param>
        public DefaultDataAccessFactory(IServiceRegistry serviceRegistry)
        {
            if (serviceRegistry == null)
            {
                throw new ArgumentNullException("serviceRegistry");
            }
            _serviceRegistry = serviceRegistry;
        }

        /// <summary>
        /// Creates a data operation context with 
        /// </summary>
        /// <param name="serviceType">Type of data operation service</param>
        /// <param name="mode">Operation mode of the data context</param>
        public override IDataAccessOperation CreateContext(Type serviceType, 
            SqlOperationMode mode = SqlOperationMode.ReadWrite)
        {
            var instance = _serviceRegistry.GetService(serviceType) as IDataAccessOperation;
            if (instance == null) throw new InvalidOperationException(
                String.Format("Service type {0} cannot be resolved to an IDataAccessOperation instance",
                    serviceType));
            instance.SetOperationMode(mode);
            return instance;
        }

        /// <summary>
        /// Creates a context for the specified data access service with the specified operation mode
        /// </summary>
        /// <typeparam name="TService">Data access service type</typeparam>
        /// <param name="mode">Operation mode</param>
        /// <returns>Service object configured for the specified operation mode</returns>
        public override IDataAccessOperation CreateContext<TService>(SqlOperationMode mode = SqlOperationMode.ReadWrite)
        {
            return CreateContext(typeof (TService), mode);
        }
    }
}