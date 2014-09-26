using System;

namespace Seemplest.Core.DataAccess.DataServices
{
    /// <summary>
    /// This class is intended to be the base class of all data access factories
    /// </summary>
    public abstract class DataAccessFactoryBase: IDataAccessFactory
    {
        /// <summary>
        /// Creates a data operation context with 
        /// </summary>
        /// <param name="serviceType">Type of data operation service</param>
        /// <param name="mode">Operation mode of the data context</param>
        public abstract IDataAccessOperation CreateContext(Type serviceType, 
            SqlOperationMode mode = SqlOperationMode.ReadWrite);

        /// <summary>
        /// Creates a context for the specified data access service with the specified operation mode
        /// </summary>
        /// <typeparam name="TService">Data access service type</typeparam>
        /// <param name="mode">Operation mode</param>
        /// <returns>Service object configured for the specified operation mode</returns>
        public abstract IDataAccessOperation CreateContext<TService>(SqlOperationMode mode = SqlOperationMode.ReadWrite)
            where TService : IDataAccessOperation;
    }
}