using System;
using FirebirdSql.Data.FirebirdClient;

namespace Seemplest.FbSql.DataAccess
{
    /// <summary>
    /// This is the default mapper to use for mapping SQL parameters
    /// </summary>
    public class DefaultSqlParameterMapper : ISqlParameterMapper
    {
        /// <summary>
        /// Maps the specified value to a <see cref="FbParameter"/> instance
        /// </summary>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="value">The value to map</param>
        /// <returns>
        /// The new <see cref="FbParameter"/> instance; or null if mapping cannot be done.
        /// </returns>
        public FbParameter MapParameterValue(string parameterName, object value)
        {
            return DoMapping(parameterName, value);
        }

        /// <summary>
        /// Override this method in derived classes to provide
        /// your own mapping.
        /// </summary>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="value">The value to map</param>
        /// <returns></returns>
        protected virtual FbParameter DoMapping(string parameterName, object value)
        {
            if (value == null) return new FbParameter(parameterName, DBNull.Value);
            var parameter = new FbParameter(parameterName, value);

            // --- Manage strings
            var stringType = value as string;
            if (stringType != null)
            {
                if (stringType.Length > 32765) parameter.FbDbType = FbDbType.Text;
                return parameter;
            }

            // --- Manage byte arrays
            var byteArr = value as byte[];
            if (byteArr != null)
            {
                // --- Handle long byte arrays
                if (byteArr.Length > 8000) parameter.FbDbType = FbDbType.Binary;
                return parameter;
            }

            // --- Handle char as a single-character string
            if (value is char)
            {
                parameter.FbDbType = FbDbType.Char;
                parameter.Size = 1;
                parameter.Value = new string((char)value, 1);
                return parameter;
            }

            // --- Handle UInt16 as integer
            if (value is ushort)
            {
                parameter.FbDbType = FbDbType.Integer;
                parameter.Value = (int)((ushort)value);
                return parameter;
            }

            // --- Handle UInt32 as big integer
            if (value is uint)
            {
                parameter.FbDbType = FbDbType.BigInt;
                parameter.Value = (long)((uint)value);
                return parameter;
            }

            // --- Handle UInt64 as big integer
            if (value is ulong)
            {
                parameter.FbDbType = FbDbType.BigInt;
                parameter.Value = (long)((ulong)value);
                return parameter;
            }

            // --- Handle SByte as integer
            if (value is sbyte)
            {
                parameter.FbDbType = FbDbType.Integer;
                parameter.Value = (int)((sbyte)value);
                return parameter;
            }

            // --- Handle enumerable types
            if (value.GetType().IsEnum)
            {
                parameter.FbDbType = FbDbType.Integer;
                parameter.Value = (int)value;
                return parameter;
            }

            // --- In case of other types check if SqlDbType is valid or not
            try
            {
                // --- This line will raise an ArgumentException in case of mapping failure
                // ReSharper disable UnusedVariable
                var type = parameter.FbDbType;
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