using System;

namespace Seemplest.Core.DataAccess.Attributes
{
    /// <summary>
    /// This attribute sets the maximum text length on an IMO.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxTextLengthAttribute : ImoAttributeBase
    {
        /// <summary>
        /// Gets the name of the underlying schema.
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// Initializes the attribute with the spacified value.
        /// </summary>
        /// <param name="value"></param>
        public MaxTextLengthAttribute(int value)
        {
            Value = value;
        }
    }
}