using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Seemplest.Core.Common
{
    /// <summary>
    /// This class provides several utilty functions to help in type conversion.
    /// </summary>
    public static class TypeConversionHelper
    {
        /// <summary>
        /// Converts the specified byte array to its hexadecimal representation.
        /// </summary>
        /// <param name="binary">Byte array to convert</param>
        /// <returns>String representation</returns>
        public static string ByteArrayToString(byte[] binary)
        {
            var sb = new StringBuilder("0x");
            foreach (var value in binary)
            {
                sb.AppendFormat("{0:X2}", value);
            }
            return sb.ToString();
        }

        /// <summary>
        /// This method checks whether the specified object can be converted to string.
        /// </summary>
        /// <param name="rawValue">Value to check</param>
        /// <param name="converter">Type converter object</param>
        /// <returns>
        /// True, if the type can be converted to string; otherwise, false;
        /// </returns>
        public static bool CanConvertToString(object rawValue, out TypeConverter converter)
        {
            var valueType = rawValue.GetType();
            converter = TypeDescriptor.GetConverter(valueType);
            return converter.CanConvertTo(typeof(String));
        }

        /// <summary>
        /// Converts an object to a typed string.
        /// </summary>
        /// <param name="rawValue">Value to convert</param>
        /// <param name="culture">Culture information</param>
        /// <returns>String representation of <paramref name="rawValue"/></returns>
        public static string TypedValueToString(object rawValue, CultureInfo culture)
        {
            var valueType = rawValue.GetType();
            // --- Any type that supports a type converter
            var converter = TypeDescriptor.GetConverter(valueType);

            // ReSharper disable AssignNullToNotNullAttribute
            var result = converter.CanConvertTo(typeof(String)) 
                                ? converter.ConvertToString(null, culture, rawValue) 
                                : rawValue.ToString();
            // ReSharper restore AssignNullToNotNullAttribute
            return result;
        }

        /// <summary>
        /// Converts an object ty a typed string using the invariant culture.
        /// </summary>
        /// <param name="rawValue">Value to convert</param>
        /// <returns>String representation of <paramref name="rawValue"/></returns>
        public static string TypedValueToString(object rawValue)
        {
            return TypedValueToString(rawValue, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a string to a typed value.
        /// </summary>
        /// <param name="sourceString">String representation of the value</param>
        /// <param name="targetType">Target type</param>
        /// <param name="culture">Culture information</param>
        /// <returns>Value converted to the target type</returns>
        public static object StringToTypedValue(
          string sourceString, Type targetType, CultureInfo culture)
        {
            object result = null;
            var converter = TypeDescriptor.GetConverter(targetType);
            // ReSharper disable AssignNullToNotNullAttribute
            if (converter.CanConvertTo(targetType))
                result = converter.ConvertTo(null, culture, sourceString, targetType);
            // ReSharper restore AssignNullToNotNullAttribute
            return result;
        }

        /// <summary>
        /// Converts a string to a typed value using the invariant culture.
        /// </summary>
        /// <param name="sourceString">String representation of the value</param>
        /// <param name="targetType">Target type</param>
        /// <returns>Value converted to the target type</returns>
        public static object StringToTypedValue(string sourceString, Type targetType)
        {
            return StringToTypedValue(sourceString, targetType, CultureInfo.InvariantCulture);
        }
    }
}
