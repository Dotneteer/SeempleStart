using System;

namespace Seemplest.Core.DataAccess.Attributes
{
    /// <summary>
    /// This attribute defines a source converter to use when a source type should be converted
    /// to the type of the property decorated with this attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class SourceConverterAttribute : ImoAttributeBase
    {
        /// <summary>
        /// Gets the source type of conversion
        /// </summary>
        public Type SourceType { get; private set; }

        /// <summary>
        /// Gets the type of converter
        /// </summary>
        public Type ConverterType { get; private set; }

        /// <summary>
        /// Initializes this attribute with the specified source and converter types
        /// </summary>
        /// <param name="sourceType">Source type</param>
        /// <param name="converterType">Converter type</param>
        public SourceConverterAttribute(Type sourceType, Type converterType)
        {
            SourceType = sourceType;
            ConverterType = converterType;
        }
    }
}