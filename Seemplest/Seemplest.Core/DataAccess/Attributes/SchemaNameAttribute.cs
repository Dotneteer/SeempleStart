using System;

namespace Seemplest.Core.DataAccess.Attributes
{
    /// <summary>
    /// This attribute sets the schema name of an IMO within the underlying database.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SchemaNameAttribute : ImoAttributeBase
    {
        /// <summary>
        /// Gets the name of the underlying schema.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Initializes the attribute with the spacified value.
        /// </summary>
        /// <param name="value"></param>
        public SchemaNameAttribute(string value)
        {
            Value = value;
        }
    }
}