using System;

namespace SeemplesTools.Deployment.Common
{
    /// <summary>
    /// Ez az osztály adatbázisokhoz kapcsolódó hasznos műveleteket tartalmaz.
    /// </summary>
    public static class DatabaseHelper
    {
        /// <summary>
        /// Előállítja a megadott adatbázis kapcsolati információját
        /// </summary>
        /// <param name="sqlInstance">Az SQL Server példány neve</param>
        /// <param name="sqlDatabase">Az adatbázis neve</param>
        /// <param name="userName">Az SQL felhasználó neve</param>
        /// <param name="password">Az SQL felhasználó jelszava</param>
        /// <returns>Az adatbázis kapcsolati információja</returns>
        public static string GetConnectionString(string sqlInstance, string sqlDatabase, string userName, string password)
        {
            if (userName == null)
            {
                return String.Format(
                    "Data Source={0};Initial Catalog={1};Integrated Security=SSPI;Pooling=false", sqlInstance, sqlDatabase);
            }
            return String.Format(
                "Server={0};Database={1};User ID={2};Password={3};Pooling=false", 
                sqlInstance, sqlDatabase, userName, password);
        }
    }
}
