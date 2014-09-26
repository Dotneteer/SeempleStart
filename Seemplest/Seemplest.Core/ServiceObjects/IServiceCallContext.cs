using System;

namespace Seemplest.Core.ServiceObjects
{
    /// <summary>
    /// This interface represents the call context
    /// </summary>
    public interface IServiceCallContext: ICloneable
    {
        /// <summary>
        /// Gets the value of the call context item with the specified type
        /// </summary>
        /// <typeparam name="TItem">Type of call context item</typeparam>
        /// <returns>Call context item value</returns>
        TItem Get<TItem>() 
            where TItem: IServiceCallContextItem;

        /// <summary>
        /// Sets the value of a call context item with the specified type and value
        /// </summary>
        /// <typeparam name="TItem">Type of call context item</typeparam>
        /// <param name="value">Call context item value</param>
        void Set<TItem>(TItem value)
            where TItem : IServiceCallContextItem;

        /// <summary>
        /// Removes the specified item from the call context.
        /// </summary>
        /// <typeparam name="TItem">Type of call context item</typeparam>
        void Remove<TItem>();

        /// <summary>
        /// Gets the call context item by its specified key
        /// </summary>
        /// <param name="key">Call context item key</param>
        /// <returns>Call context item value</returns>
        object GetByKey(string key);

        /// <summary>
        /// Sets the service context item by its key and value
        /// </summary>
        /// <param name="key">Call context item key</param>
        /// <param name="value">Call context item value</param>
        void SetByKey(string key, object value);

        /// <summary>
        /// Removes the call context item by its specified key
        /// </summary>
        /// <param name="key">Call context item key</param>
        void RemoveByKey(string key);

        /// <summary>
        /// Removes all items from the context
        /// </summary>
        void Clear();
    }
}