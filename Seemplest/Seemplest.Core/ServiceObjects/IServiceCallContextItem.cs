using System;

namespace Seemplest.Core.ServiceObjects
{
    /// <summary>
    /// This interface dscribes the properties of an item that can be the element of the call context
    /// </summary>
    public interface IServiceCallContextItem: ICloneable
    {
        /// <summary>
        /// Gets the value of the context item.
        /// </summary>
        /// <returns>Context item value</returns>
        object GetValue();
    }
}