using System;
using System.Configuration;
using System.Data.SqlClient;
using Seemplest.Core.Configuration.ResourceConnections;

namespace Seemplest.MsSql.DataAccess
{
    /// <summary>
    /// This class contains helper functions for managing SQL Server database operations
    /// </summary>
    public class SqlHelper
    {
        /// <summary>
        /// Creates an <see cref="SqlConnection"/> instance from the specified string
        /// </summary>
        /// <param name="nameOrConnectionString">
        /// String describing the SQL Server conection information
        /// </param>
        /// <returns>Newly created <see cref="SqlConnection"/> instance.</returns>
        /// <remarks>
        /// The <paramref name="nameOrConnectionString"/> can be a simple connection string. If it starts
        /// with 'name=', the right side is used as a ke to access the connection
        /// string information described in the standard application configuration file.
        /// </remarks>
        public static SqlConnection CreateSqlConnection(string nameOrConnectionString)
        {
            return new SqlConnection(GetConnectionString(nameOrConnectionString));
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
        /// with 'name=', the right side is used as a ke to access the connection
        /// string information described in the standard application configuration file.
        /// </remarks>
        public static string GetConnectionString(string nameOrConnectionString)
        {
            var parts = nameOrConnectionString.Split('=');
            if (parts.Length == 2)
            {
                var mode = parts[0];
                var connectionName = parts[1];
                if (String.Equals(mode, "name", StringComparison.InvariantCultureIgnoreCase))
                {
                    return ResourceConnectionFactory.CreateResourceConnection<SqlConnection>(connectionName).ConnectionString;
                }
                if (String.Equals(mode, "connStr", StringComparison.InvariantCultureIgnoreCase))
                {
                    var connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionName];
                    if (connectionStringSettings == null)
                    {
                        throw new ConfigurationErrorsException(
                            string.Format("ConnectionString with the given name ('{0}') does not exists!",
                                          connectionName));
                    }
                    return connectionStringSettings.ConnectionString;
                }
            }
            return nameOrConnectionString;
        }
    }
}