using System;
using System.Configuration;
using FirebirdSql.Data.FirebirdClient;

namespace Seemplest.FbSql.DataAccess
{
    /// <summary>
    /// This class contains helper functions for managing SQL Server database operations
    /// </summary>
    public class SqlHelper
    {
        /// <summary>
        /// Creates an <see cref="FbConnection"/> instance from the specified string
        /// </summary>
        /// <param name="nameOrConnectionString">
        /// String describing the SQL Server conection information
        /// </param>
        /// <returns>Newly created <see cref="FbConnection"/> instance.</returns>
        /// <remarks>
        /// The <paramref name="nameOrConnectionString"/> can be a simple connection string. If it starts
        /// with 'connStr=', the right side is used as a ke to access the connection
        /// string information described in the standard application configuration file.
        /// </remarks>
        public static FbConnection CreateSqlConnection(string nameOrConnectionString)
        {
            return new FbConnection(GetConnectionString(nameOrConnectionString));
        }

        /// <summary>
        /// Retrieves the connection string
        /// </summary>
        /// <param name="nameOrConnectionString">
        /// String describing the SQL Server conection information
        /// </param>
        /// <returns>Connection string information</returns>
        /// <remarks>
        /// The <paramref name="nameOrConnectionString"/> can be a simple connection string. If it starts
        /// with 'connStr=', the right side is used as a ke to access the connection
        /// string information described in the standard application configuration file.
        /// </remarks>
        public static string GetConnectionString(string nameOrConnectionString)
        {
            var parts = nameOrConnectionString.Split('=');
            if (parts.Length == 2)
            {
                var mode = parts[0];
                var connectionName = parts[1];
                if (string.Equals(mode, "connStr", StringComparison.InvariantCultureIgnoreCase))
                {
                    var connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionName];
                    if (connectionStringSettings == null)
                    {
                        throw new ConfigurationErrorsException(
                            $"ConnectionString with the given name ('{connectionName}') does not exists!");
                    }
                    return connectionStringSettings.ConnectionString;
                }
            }
            return nameOrConnectionString;
        }
    }
}
