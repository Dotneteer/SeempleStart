using System;

namespace Seemplest.Core.DataAccess.Attributes
{
    /// <summary>
    /// This attribute defines a source converter to use when a source type should be converted
    /// to the type of the property decorated with this attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TargetConverterAttribute : ImoAttributeBase
    {
        /// <summary>
        /// Gets the type of converter
        /// </summary>
        public Type ConverterType { get; private set; }

        /// <summary>
        /// Initializes this attribute with the specified target and converter types
        /// </summary>
        /// <param name="converterType">Converter type</param>
        public TargetConverterAttribute(Type converterType)
        {
            ConverterType = converterType;
        }
    }
}