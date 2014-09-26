using System;
using System.Collections.Generic;

namespace Seemplest.Core.ServiceObjects
{
    /// <summary>
    /// This class provides an implementation of a service call context
    /// </summary>
    public class ServiceCallContext : IServiceCallContext
    {
        // --- Stores the service context items
        private readonly Dictionary<string, object> _contextItems = 
            new Dictionary<string, object>();

        /// <summary>
        /// Gets the value of the call context item with the specified type
        /// </summary>
        /// <typeparam name="TItem">Type of call context item</typeparam>
        /// <returns>Call context item value</returns>
        public TItem Get<TItem>() where TItem : IServiceCallContextItem
        {
            object contextItem;
            return _contextItems.TryGetValue(GetItemKey(typeof (TItem)), out contextItem)
                       ? (TItem)contextItem
                       : default(TItem);
        }

        /// <summary>
        /// Sets the value of a call context item with the specified type and value
        /// </summary>
        /// <typeparam name="TItem">Type of call context item</typeparam>
        /// <param name="value">Call context item value</param>
        public void Set<TItem>(TItem value) where TItem : IServiceCallContextItem
        {
            _contextItems[GetItemKey(typeof(TItem))] = value;
        }

        /// <summary>
        /// Removes the specified item from the call context.
        /// </summary>
        /// <typeparam name="TItem">Type of call context item</typeparam>
        public void Remove<TItem>()
        {
            _contextItems.Remove(GetItemKey(typeof (TItem)));
        }

        /// <summary>
        /// Gets the call context item by its specified key
        /// </summary>
        /// <param name="key">Call context item key</param>
        /// <returns>Call context item value</returns>
        public object GetByKey(string key)
        {
            object contextItem;
            return _contextItems.TryGetValue(key, out contextItem)
                       ? contextItem
                       : null;
        }

        /// <summary>
        /// Sets the service context item by its key and value
        /// </summary>
        /// <param name="key">Call context item key</param>
        /// <param name="value">Call context item value</param>
        public void SetByKey(string key, object value)
        {
            _contextItems[key] = value;
        }

        /// <summary>
        /// Removes the call context item by its specified key
        /// </summary>
        /// <param name="key">Call context item key</param>
        public void RemoveByKey(string key)
        {
            _contextItems.Remove(key);
        }

        /// <summary>
        /// Removes all items from the context
        /// </summary>
        public void Clear()
        {
            _contextItems.Clear();
        }

        /// <summary>
        /// Clones this service call context
        /// </summary>
        /// <returns>A clone of this service call context</returns>
        object ICloneable.Clone()
        {
            var clone = new ServiceCallContext();
            foreach (var item in _contextItems)
            {
                var clonedValue = item.Value;
                if (!clonedValue.GetType().IsValueType)
                {
                    var cloneable = item.Value as ICloneable;
                    if (cloneable != null)
                    {
                        clonedValue = cloneable.Clone();
                    }
                }
                clone._contextItems.Add(item.Key, clonedValue);
            }
            return clone;
        }

        /// <summary>
        /// Gets a string key for the specified type
        /// </summary>
        private static string GetItemKey(Type type)
        {
            return string.Format("__type__:{0}", type.AssemblyQualifiedName);
        }
    }
}