using System;
using System.Data.SqlClient;

namespace Seemplest.MsSql.DataAccess
{
    /// <summary>
    /// Event argument class with an SqlCommand object
    /// </summary>
    public class SqlCommandEventArgs : EventArgs
    {
        /// <summary>
        /// SqlCommand object
        /// </summary>
        public SqlCommand SqlCommand { get; private set; }

        /// <summary>
        /// Initializes the class
        /// </summary>
        /// <param name="sqlCommand">SqlCommand object</param>
        public SqlCommandEventArgs(SqlCommand sqlCommand)
        {
            SqlCommand = sqlCommand;
        }
    }
}