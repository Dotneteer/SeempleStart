using System.Configuration;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.FbSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.FbSql.UnitTests.DataAccess
{
    [TestClass]
    public class SqlHelperTest
    {
        private const string DB_CONN_NAME = "connStr=Seemplest";
        private const string DB_CONN = @"User=SYSDBA;Password=masterkey;DataSource=localhost;Database=C:\Temp\SeemplestTest.KSFDB;";

        [TestMethod]
        public void GetConnectionStringWorksAsExpected()
        {
            // --- Act
            const string DUMMY_CONN = "dummy=Dummy";
            var connStr1 = SqlHelper.GetConnectionString(DB_CONN_NAME);
            var sqlConn1 = new FbConnection(connStr1);
            sqlConn1.Open();
            sqlConn1.Close();
            var connStr2 = SqlHelper.GetConnectionString(DB_CONN);
            var sqlConn2 = new FbConnection(connStr2);
            var connStr3 = SqlHelper.GetConnectionString(DUMMY_CONN);
            sqlConn2.Open();
            sqlConn2.Close();

            // --- Assert
            connStr3.ShouldEqual(DUMMY_CONN);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void GetConnectionStringFailsWithInvalidName()
        {
            SqlHelper.GetConnectionString("connStr=DummyName");
        }

        [TestMethod]
        public void CreateConnectionWorksAsExpected()
        {
            // --- Act
            var sqlConn1 = SqlHelper.CreateSqlConnection(DB_CONN_NAME);
            sqlConn1.Open();
            sqlConn1.Close();
            var sqlConn2 = SqlHelper.CreateSqlConnection(DB_CONN);
            sqlConn2.Open();
            sqlConn2.Close();
        }
    }
}
