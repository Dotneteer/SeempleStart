using System;

namespace Seemplest.Core.DataAccess.Attributes
{
    /// <summary>
    /// This attribute sets the table name of an IMO within the underlying database.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute : ImoAttributeBase
    {
        /// <summary>
        /// Gets the name of the underlying table.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Initializes the attribute with the spacified value.
        /// </summary>
        /// <param name="value"></param>
        public TableNameAttribute(string value)
        {
            Value = value;
        }
    }
}