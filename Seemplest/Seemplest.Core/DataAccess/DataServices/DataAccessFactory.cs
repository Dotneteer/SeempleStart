using System;
using Seemplest.Core.DependencyInjection;

namespace Seemplest.Core.DataAccess.DataServices
{
    /// <summary>
    /// This static class provides a factory that creates data operation objects
    /// </summary>
    public static class DataAccessFactory
    {
        // --- This factory object is used behind this static class
        private static IDataAccessFactory s_InternalFactory;

        /// <summary>
        /// Initializes the static members of this class
        /// </summary>
        static DataAccessFactory()
        {
            SetRegistry(ServiceManager.ServiceRegistry);
        }

        /// <summary>
        /// Gets the service registry to be used with this class
        /// </summary>
        public static IServiceRegistry ServiceRegistry { get; private set; }

        /// <summary>
        /// Set the service registry to be used with this class
        /// </summary>
        /// <param name="registry">The registry to be used with this class</param>
        public static void SetRegistry(IServiceRegistry registry)
        {
            if (ServiceRegistry == registry) return;
            if (registry == null) throw new ArgumentNullException("registry");
            ServiceRegistry = registry;
            s_InternalFactory = new DefaultDataAccessFactory(registry);
        }

        /// <summary>
        /// Creates a data operation context with 
        /// </summary>
        /// <param name="serviceType">Type of data operation service</param>
        /// <param name="mode">Operation mode of the data context</param>
        public static IDataAccessOperation CreateContext(Type serviceType,
            SqlOperationMode mode = SqlOperationMode.ReadWrite)
        {
            return s_InternalFactory.CreateContext(serviceType, mode);
        }

        /// <summary>
        /// Creates a context for the specified data access service with the specified operation mode
        /// </summary>
        /// <typeparam name="TService">Data access service type</typeparam>
        /// <param name="mode">Operation mode</param>
        /// <returns>Service object configured for the specified operation mode</returns>
        public static TService CreateContext<TService>(SqlOperationMode mode = SqlOperationMode.ReadWrite)
            where TService : IDataAccessOperation
        {
            return (TService)s_InternalFactory.CreateContext<TService>(mode);
        }

        /// <summary>
        /// Creates a context for the specified data access service with read only mode
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns>Service object configured for the specified operation mode</returns>
        public static TService CreateReadOnlyContext<TService>()
            where TService : IDataAccessOperation
        {
            return CreateContext<TService>(SqlOperationMode.ReadOnly);
        }

        /// <summary>
        /// Creates a context for the specified data access service with tracked mode
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns>Service object configured for the specified operation mode</returns>
        public static TService CreateTrackedContext<TService>()
            where TService : IDataAccessOperation
        {
            return CreateContext<TService>(SqlOperationMode.Tracked);
        }
    }
}