using FirebirdSql.Data.FirebirdClient;

namespace Seemplest.FbSql.DataAccess
{
    /// <summary>
    /// This interface defines the responsibility of a mapper that maps a specified value to
    /// a <see cref="FbParameter"/> instance.
    /// </summary>
    public interface ISqlParameterMapper
    {
        /// <summary>
        /// Maps the specified value to a <see cref="FbParameter"/> instance
        /// </summary>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="value">The value to map</param>
        /// <returns>
        /// The new <see cref="FbParameter"/> instance; or null if mapping cannot be done.
        /// </returns>
        FbParameter MapParameterValue(string parameterName, object value);
    }
}