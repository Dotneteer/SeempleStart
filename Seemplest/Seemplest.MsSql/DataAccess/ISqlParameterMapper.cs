using System;
using System.Data.SqlClient;

namespace Seemplest.MsSql.DataAccess
{
    /// <summary>
    /// This interface defines the responsibility of a mapper that maps a specified value to
    /// a <see cref="SqlParameter"/> instance.
    /// </summary>
    public interface ISqlParameterMapper
    {
        /// <summary>
        /// Maps the specified value to a <see cref="SqlParameter"/> instance
        /// </summary>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="value">The value to map</param>
        /// <returns>
        /// The new <see cref="SqlParameter"/> instance; or null if mapping cannot be done.
        /// </returns>
        SqlParameter MapParameterValue(string parameterName, object value);

        /// <summary>
        /// Maps the specified type to a <see cref="SqlParameter"/> instance
        /// </summary>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="type">The value to map</param>
        /// <returns>
        /// The new <see cref="SqlParameter"/> instance; or null if mapping cannot be done.
        /// </returns>
        SqlParameter MapParameterType(string parameterName, Type type);
    }
}