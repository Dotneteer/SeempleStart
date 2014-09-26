using System;

namespace Seemplest.Core.DataAccess.DataServices
{
    /// <summary>
    /// This interface defines the operations for a data access factory object
    /// </summary>
    public interface IDataAccessFactory
    {
        /// <summary>
        /// Creates a data operation context with 
        /// </summary>
        /// <param name="serviceType">Type of data operation service</param>
        /// <param name="mode">Operation mode of the data context</param>
        IDataAccessOperation CreateContext(Type serviceType, SqlOperationMode mode = SqlOperationMode.ReadWrite);

        /// <summary>
        /// Creates a data operation context with 
        /// </summary>
        /// <typeparam name="TService">Type of data operation service</typeparam>
        /// <param name="mode">Operation mode of the data context</param>
        IDataAccessOperation CreateContext<TService>(SqlOperationMode mode = SqlOperationMode.ReadWrite)
            where TService: IDataAccessOperation;
    }
}