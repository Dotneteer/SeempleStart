using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace Seemplest.MsSql.DataAccess
{
    /// <summary>
    /// This is the default mapper to use for mapping SQL parameters
    /// </summary>
    public class DefaultSqlParameterMapper : ISqlParameterMapper
    {
        private readonly ISqlParameterMapper _parentMapper;

        /// <summary>
        /// Initializes this instance to use the specified parent mapper object
        /// </summary>
        /// <param name="parentMapper">Parent mapper object</param>
        public DefaultSqlParameterMapper(ISqlParameterMapper parentMapper = null)
        {
            _parentMapper = parentMapper;
        }

        /// <summary>
        /// Maps the specified value to a <see cref="SqlParameter"/> instance
        /// </summary>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="value">The value to map</param>
        /// <returns>
        /// The new <see cref="SqlParameter"/> instance; or null if mapping cannot be done.
        /// </returns>
        public SqlParameter MapParameterValue(string parameterName, object value)
        {
            return DoMapping(parameterName, value) 
                ?? ( _parentMapper == null 
                    ? null 
                    : _parentMapper.MapParameterValue(parameterName, value));
        }

        /// <summary>
        /// Override this method in derived classes to provide
        /// your own mapping.
        /// </summary>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="value">The value to map</param>
        /// <returns></returns>
        protected virtual SqlParameter DoMapping(string parameterName, object value)
        {
            if (value == null) return new SqlParameter(parameterName, DBNull.Value);
            var parameter = new SqlParameter(parameterName, value);

            // --- Manage strings
            var stringType = value as string;
            if (stringType != null)
            {
                if (stringType.Length > 4000) parameter.SqlDbType = SqlDbType.NText;
                return parameter;
            }

            // --- Manage byte arrays
            var byteArr = value as byte[];
            if (byteArr != null)
            {
                // --- Handle long byte arrays
                if (byteArr.Length > 8000) parameter.SqlDbType = SqlDbType.Image;
                return parameter;
            }

            // --- Handle char as a single-character string
            if (value is char)
            {
                parameter.SqlDbType = SqlDbType.NChar;
                parameter.Size = 1;
                parameter.Value = new String((char)value, 1);
                return parameter;
            }

            // --- Handle UInt16 as integer
            if (value is UInt16)
            {
                parameter.SqlDbType = SqlDbType.Int;
                parameter.Value = (int) ((UInt16) value);
                return parameter;
            }

            // --- Handle UInt32 as big integer
            if (value is UInt32)
            {
                parameter.SqlDbType = SqlDbType.BigInt;
                parameter.Value = (long) ((UInt32) value);
                return parameter;
            }

            // --- Handle UInt64 as big integer
            if (value is UInt64)
            {
                parameter.SqlDbType = SqlDbType.BigInt;
                parameter.Value = (long)((UInt64)value);
                return parameter;
            }

            // --- Handle SByte as integer
            if (value is sbyte)
            {
                parameter.SqlDbType = SqlDbType.Int;
                parameter.Value = (int) ((sbyte) value);
                return parameter;
            }

            // --- Handle enumerable types
            if (value.GetType().IsEnum)
            {
                parameter.SqlDbType = SqlDbType.Int;
                parameter.Value = (int) value;
                return parameter;
            }

            // --- Hanlde XObject types
            var xobj = value as XObject;
            if (xobj != null)
            {
                parameter.SqlDbType = SqlDbType.Xml;
                parameter.Value = xobj.ToString();
                return parameter;
            }

            // --- In case of other types check if SqlDbType is valid or not
            try
            {
                // --- This line will raise an ArgumentException in case of mapping failure
                // ReSharper disable UnusedVariable
                var type = parameter.SqlDbType;
                // ReSharper restore UnusedVariable
                return parameter;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}