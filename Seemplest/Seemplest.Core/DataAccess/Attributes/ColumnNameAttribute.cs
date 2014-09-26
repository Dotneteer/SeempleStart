using System;

namespace Seemplest.Core.DataAccess.Attributes
{
    /// <summary>
    /// This attribute sets the name of a column representing a property in the underlying
    /// data table-
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnNameAttribute : ImoAttributeBase
    {
        /// <summary>
        /// Gets the name of the underlying datatable column
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Initializes the attribute with the spacified value.
        /// </summary>
        /// <param name="value"></param>
        public ColumnNameAttribute(string value)
        {
            Value = value;
        }
    }
}