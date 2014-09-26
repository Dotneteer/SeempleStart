using System;

namespace Seemplest.Core.ServiceObjects
{
    /// <summary>
    /// This class is intended to be the base class of all service context items
    /// </summary>
    public abstract class ServiceCallContextItemBase<T> : IServiceCallContextItem
    {
        private readonly T _itemValue;

        /// <summary>
        /// Gets the value of this context item
        /// </summary>
        public T Value
        {
            get { return _itemValue;  }
        }

        /// <summary>
        /// Initializes the value of this context item
        /// </summary>
        /// <param name="itemValue">Context item value</param>
        protected ServiceCallContextItemBase(T itemValue)
        {
            _itemValue = itemValue;
        }

        /// <summary>
        /// Gets the value of the context item.
        /// </summary>
        /// <returns>Context item value</returns>
        object IServiceCallContextItem.GetValue()
        {
            return _itemValue;
        }

        /// <summary>
        /// Creates a clone of this call context item
        /// </summary>
        /// <returns>The cloned instance of this context item</returns>
        object ICloneable.Clone()
        {
            var itemValue = _itemValue;
            if (!typeof (T).IsValueType)
            {
                var cloneable = _itemValue as ICloneable;
                if (cloneable != null)
                {
                    itemValue = (T)cloneable.Clone();
                }
            }
            var clone = Activator.CreateInstance(GetType(), itemValue);
            return clone;
        }
    }
}