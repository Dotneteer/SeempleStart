using System;
using System.Configuration;
using System.Data;
using System.Security.Cryptography;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.FbSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.FbSql.UnitTests.DataAccess
{
    [TestClass]
    public class FbDatabaseTest
    {
        const string DB_CONN = "connStr=Seemplest";

        [TestCleanup]
        public void Cleanup()
        {
            var db = new FbDatabase(DB_CONN);
            db.Execute(
                @"EXECUTE BLOCK AS BEGIN
                  if (exists(select 1 from rdb$relations where rdb$relation_name = 'sample')) then 
                  execute statement 'drop table ""sample"";';
                  END");
        }

        [TestMethod]
        public void SqlDatabaseCreationWorksWithConnectionName()
        {
            // --- Arrange
            var connStr = ConfigurationManager.ConnectionStrings["Seemplest"].ConnectionString;

            // --- Act
            var db = new FbDatabase(DB_CONN);

            // --- Assert
            db.ConnectionOrName.ShouldEqual(DB_CONN);
            db.ConnectionString.ShouldEqual(connStr);
        }

        [TestMethod]
        public void SqlDatabaseCreationWorksWithConnectionString()
        {
            // --- Arrange
            var connStr = ConfigurationManager.ConnectionStrings["Seemplest"].ConnectionString;

            // --- Act
            var db = new FbDatabase(connStr);

            // --- Assert
            db.ConnectionOrName.ShouldEqual(connStr);
            db.ConnectionString.ShouldEqual(connStr);
        }

        [TestMethod]
        public void OpenAndCloseSharedConnectionWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);

            // --- Act
            db.OpenSharedConnection();
            var connCount1 = db.SharedConnectionDepth;
            db.OpenSharedConnection();
            var connCount2 = db.SharedConnectionDepth;
            var connState = db.Connection.State;
            db.CloseSharedConnection();
            var connCount3 = db.SharedConnectionDepth;
            db.CloseSharedConnection();
            var connCount4 = db.SharedConnectionDepth;
            db.CloseSharedConnection();

            // --- Assert
            connCount1.ShouldEqual(1);
            connCount2.ShouldEqual(2);
            connState.ShouldEqual(ConnectionState.Open);
            connCount3.ShouldEqual(1);
            connCount4.ShouldEqual(0);
        }

        [TestMethod]
        public void ExecuteWithSqlStringWorks()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();

            // --- Act
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");
            var rows = db.ExecuteScalar<int>(@"select count(*) from ""sample""");
            db.Execute(@"drop table ""sample""");

            // --- Assert
            rows.ShouldEqual(2);
        }

        [TestMethod]
        public void ExecuteScalarWorksWithNullResult()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();

            // --- Act
            var rows = db.ExecuteScalar<int?>(@"select ""Name"" from ""sample""");

            // --- Assert
            rows.ShouldBeNull();
        }

        [TestMethod]
        public void ExecuteScalarWorksWithDbNullResult()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();

            // --- Act
            var value1 = db.ExecuteScalar<int?>(@"select max(""Id"") from ""sample""");
            var value2 = db.ExecuteScalar<int?>(@"select count(*) from ""sample""");

            // --- Assert
            value1.ShouldBeNull();
            value2.ShouldNotBeNull();
        }

        [TestMethod]
        public void CompleteTransactionWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();

            // --- Act
            db.BeginTransaction();
            var count1 = db.TransactionDepth;
            db.BeginTransaction();
            var count2 = db.TransactionDepth;
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");
            db.CompleteTransaction();
            var count3 = db.TransactionDepth;
            db.CompleteTransaction();
            var count4 = db.TransactionDepth;
            var rows = db.ExecuteScalar<int>(@"select count(*) from ""sample""");

            // --- Assert
            count1.ShouldEqual(1);
            count2.ShouldEqual(2);
            count3.ShouldEqual(1);
            count4.ShouldEqual(0);
            rows.ShouldEqual(2);
        }

        [TestMethod]
        public void AbortTransactionWorksAsExpected1()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();

            // --- Act
            db.BeginTransaction();
            var count1 = db.TransactionDepth;
            db.BeginTransaction();
            var count2 = db.TransactionDepth;
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");
            db.AbortTransaction();
            var count3 = db.TransactionDepth;
            db.CompleteTransaction();
            var count4 = db.TransactionDepth;
            var rows = db.ExecuteScalar<int>(@"select count(*) from ""sample""");

            // --- Assert
            count1.ShouldEqual(1);
            count2.ShouldEqual(2);
            count3.ShouldEqual(1);
            count4.ShouldEqual(0);
            rows.ShouldEqual(0);
        }

        [TestMethod]
        public void AbortTransactionWorksAsExpected2()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();

            // --- Act
            db.BeginTransaction();
            var count1 = db.TransactionDepth;
            db.BeginTransaction();
            var count2 = db.TransactionDepth;
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");
            db.CompleteTransaction();
            var count3 = db.TransactionDepth;
            db.AbortTransaction();
            var count4 = db.TransactionDepth;
            var rows = db.ExecuteScalar<int>(@"select count(*) from ""sample""");

            // --- Assert
            count1.ShouldEqual(1);
            count2.ShouldEqual(2);
            count3.ShouldEqual(1);
            count4.ShouldEqual(0);
            rows.ShouldEqual(0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CompleteTransactionFailsWithNoTransaction()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);

            // --- Act
            db.CompleteTransaction();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AbortTransactionFailsWithNoTransaction()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);

            // --- Act
            db.AbortTransaction();
        }

        [TestMethod]
        public void TransactionIsolationWorks()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            var db1 = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();

            // --- Act
            db.BeginTransaction(IsolationLevel.ReadCommitted);
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");
            var rowsdb = db.ExecuteScalar<int>(@"select count(*) from ""sample""");
            db1.BeginTransaction();
            var rowsdb1 = db1.ExecuteScalar<int>(@"select count(*) from ""sample""");
            db1.CompleteTransaction();
            db.CompleteTransaction();
            var rowsFinal = db.ExecuteScalar<int>(@"select count(*) from ""sample""");

            // --- Assert
            rowsdb.ShouldEqual(2);
            rowsdb1.ShouldEqual(0);
            rowsFinal.ShouldEqual(2);
        }

        [TestMethod]
        [ExpectedException(typeof(FbException))]
        public void InvalidSqlRaisesExceptionInExecute()
        {
            var db = new FbDatabase(DB_CONN);
            db.Execute("dummy sql");
        }

        [TestMethod]
        [ExpectedException(typeof(FbException))]
        public void InvalidSqlRaisesExceptionInExecuteScalar()
        {
            var db = new FbDatabase(DB_CONN);
            db.ExecuteScalar<int>("dummy sql");
        }

        [TestMethod]
        public void ExecuteWorksWithParameters()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();

            // --- Act
            db.Execute(@"insert into ""sample"" values(@0, @1)", 1, "hello");
            var name = db.ExecuteScalar<string>(@"select ""Name"" from ""sample"" where ""Id""=1");

            // --- Assert
            name.ShouldEqual("hello");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ExecuteFailsWithInvalidParameters()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();

            // --- Act
            db.Execute(@"select ""Id"" from ""sample"" where ""Id""=@0", "hello");
        }

        [TestMethod]
        public void FormatCommandWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            var sqlCommand = new FbCommand("select @0");
            sqlCommand.Parameters.Add(new FbParameter("@0", 1));

            // --- Act
            var command1 = db.FormatCommand("select @0", new object[] { 1 });
            var command2 = db.FormatCommand("", new object[0]);
            var command3 = db.FormatCommand(null, new object[0]);
            var command4 = db.FormatCommand(null, null);
            var command5 = db.FormatCommand("select 1", null);
            var command6 = db.FormatCommand(sqlCommand);

            // --- Assert
            command1.ShouldEqual("select @0\n\t -> @0 [Int32] = \"1\"");
            command2.ShouldEqual(string.Empty);
            command3.ShouldEqual(string.Empty);
            command4.ShouldEqual(string.Empty);
            command5.ShouldEqual("select 1");
            command6.ShouldEqual("select @0\n\t -> @0 [Int32] = \"1\"");
        }

        [TestMethod]
        public void CommandTimeoutsUsedAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN) { OneTimeCommandTimeout = 10, CommandTimeout = 5 };

            // --- Act
            var timeout1 = db.OneTimeCommandTimeout;
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            var timeout2 = db.OneTimeCommandTimeout;
            db.CommandTimeout = 8;
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            var timeout3 = db.OneTimeCommandTimeout;
            var rows = db.ExecuteScalar<int>(@"select count(*) from ""sample""");

            // --- Assert
            rows.ShouldEqual(1);
            timeout1.ShouldEqual(10);
            timeout2.ShouldEqual(0);
            timeout3.ShouldEqual(0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExecuteFailsWhenDisabled()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN, executeMode: SqlDirectExecuteMode.Disable);

            // --- Act
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExecuteScalarFailsWhenDisabled()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN, executeMode: SqlDirectExecuteMode.Disable);

            // --- Act
            db.ExecuteScalar<int>(@"select count(*) from sample");
        }

        [TestMethod]
        public void DisposeRollsBackOpenTransaction()
        {
            // --- Arrange
            using (var prepdb = new FbDatabase(DB_CONN))
            {
                prepdb.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            }

            // --- Act
            SampleRecord record1;
            using (var db = new FbDatabase(DB_CONN))
            {
                db.BeginTransaction();
                db.Execute(@"insert into ""sample""( ""Id"", ""Name"") values(1, 'First')");
                record1 = db.FirstOrDefault<SampleRecord>(@"where ""Id"" = 1");
            }
            SampleRecord record2;
            using (var db = new FbDatabase(DB_CONN))
            {
                record2 = db.FirstOrDefault<SampleRecord>(@"where ""Id"" = 1");
            }

            // --- Assert
            record1.ShouldNotBeNull();
            record2.ShouldBeNull();
        }

        [TestMethod]
        public void DisposeClosesOpenConnection()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN, executeMode: SqlDirectExecuteMode.Disable);
            var disposed = false;
            db.OpenSharedConnection();
            db.Connection.Disposed += (sender, args) => disposed = true;

            // -- Act
            db.Dispose();

            // --- Assert
            disposed.ShouldBeTrue();
        }


        [TableName("sample")]
        internal class SampleRecord : DataRecord<SampleRecord>
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
