using System;

namespace Seemplest.Core.DataAccess.Attributes
{
    /// <summary>
    /// This attribute marks a property as one representing a primary key column in a datatable. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : ImoAttributeBase
    {
        /// <summary>
        /// Gets the order of this property in the primary key.
        /// </summary>
        public int Order { get; private set; }

        /// <summary>
        /// Creates a new instance of this attribute with the specified order.
        /// </summary>
        /// <param name="order"></param>
        public PrimaryKeyAttribute(int order = 0)
        {
            Order = order;
        }
    }
}