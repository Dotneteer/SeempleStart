using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.MsSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.MsSql.UnitTests.DataAccess
{
    [TestClass]
    public class FetchMultipleAsyncTest
    {
        const string DB_CONN = "connStr=Seemplest";

        [TestCleanup]
        public void Cleanup()
        {
            var db = new SqlDatabase(DB_CONN);
            db.Execute(
                @"if exists (select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'sample')
                  drop table sample");
        }

        [TestMethod]
        public async Task FetchMultipleAsyncWorksWith2Sets()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");
            await db.ExecuteAsync("insert into sample values(3, 'Third')");

            // --- Act
            var rows = await db.FetchMultipleAsync<SampleRecord, SampleRecord>(
                @"select * from sample where Id = 1
                  select * from sample where Id > 1");
            var set1 = rows.Item1;
            var set2 = rows.Item2;

            // --- Assert
            set1.ShouldHaveCountOf(1);
            set2.ShouldHaveCountOf(2);
        }

        [TestMethod]
        public async Task FetchMultipleAsyncWorksWith3Sets()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");
            await db.ExecuteAsync("insert into sample values(3, 'Third')");
            await db.ExecuteAsync("insert into sample values(4, 'Fourth')");
            await db.ExecuteAsync("insert into sample values(5, 'Fifth')");

            // --- Act
            var rows = await db.FetchMultipleAsync<SampleRecord, SampleRecord, SampleRecord>(
                @"select * from sample where Id = 1
                  select * from sample where Id = 2
                  select Name, Id from sample where Id > 2");
            var set1 = rows.Item1;
            var set2 = rows.Item2;
            var set3 = rows.Item3;

            // --- Assert
            set1.ShouldHaveCountOf(1);
            set2.ShouldHaveCountOf(1);
            set3.ShouldHaveCountOf(3);
        }

        [TestMethod]
        public async Task FetchMultipleAsyncWorksWith4Sets()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");
            await db.ExecuteAsync("insert into sample values(3, 'Third')");
            await db.ExecuteAsync("insert into sample values(4, 'Fourth')");
            await db.ExecuteAsync("insert into sample values(5, 'Fifth')");
            await db.ExecuteAsync("insert into sample values(6, 'Sixth')");
            await db.ExecuteAsync("insert into sample values(7, 'Seventh')");

            // --- Act
            var rows = await db.FetchMultipleAsync<SampleRecord, SampleRecord, SampleRecord, SampleRecord>(
                @"select * from sample where Id = 1
                  select * from sample where Id = 2 or Id = 3
                  select Name, Id from sample where Id >=4 and Id <= 6
                  select * from sample where Id > 6");
            var set1 = rows.Item1;
            var set2 = rows.Item2;
            var set3 = rows.Item3;
            var set4 = rows.Item4;

            // --- Assert
            set1.ShouldHaveCountOf(1);
            set2.ShouldHaveCountOf(2);
            set3.ShouldHaveCountOf(3);
            set4.ShouldHaveCountOf(1);
        }

        [TestMethod]
        public async Task FetchMultipleWorksWithMissingSet()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");
            await db.ExecuteAsync("insert into sample values(3, 'Third')");
            await db.ExecuteAsync("insert into sample values(4, 'Fourth')");
            await db.ExecuteAsync("insert into sample values(5, 'Fifth')");
            await db.ExecuteAsync("insert into sample values(6, 'Sixth')");
            await db.ExecuteAsync("insert into sample values(7, 'Seventh')");

            // --- Act
            var rows = await db.FetchMultipleAsync<SampleRecord, SampleRecord, SampleRecord, SampleRecord>(
                @"select * from sample where Id = 1
                  select * from sample where Id = 2 or Id = 3");
            var set1 = rows.Item1;
            var set2 = rows.Item2;
            var set3 = rows.Item3;
            var set4 = rows.Item4;

            // --- Assert
            set1.ShouldHaveCountOf(1);
            set2.ShouldHaveCountOf(2);
            set3.ShouldHaveCountOf(0);
            set4.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public async Task FetchMultipleAsyncWorksWithExtraSets()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");
            await db.ExecuteAsync("insert into sample values(3, 'Third')");
            await db.ExecuteAsync("insert into sample values(4, 'Fourth')");
            await db.ExecuteAsync("insert into sample values(5, 'Fifth')");
            await db.ExecuteAsync("insert into sample values(6, 'Sixth')");
            await db.ExecuteAsync("insert into sample values(7, 'Seventh')");

            // --- Act
            var rows = await db.FetchMultipleAsync<SampleRecord, SampleRecord, SampleRecord, SampleRecord>(
                @"select * from sample where Id = 1
                  select * from sample where Id = 2 or Id = 3
                  select Name, Id from sample where Id >=4 and Id <= 6
                  select * from sample where Id > 6
                  select * from sample");
            var set1 = rows.Item1;
            var set2 = rows.Item2;
            var set3 = rows.Item3;
            var set4 = rows.Item4;

            // --- Assert
            set1.ShouldHaveCountOf(1);
            set2.ShouldHaveCountOf(2);
            set3.ShouldHaveCountOf(3);
            set4.ShouldHaveCountOf(1);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public async Task FetchMultipleAsyncRaisesExceptionWithInvalidSql()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");
            await db.ExecuteAsync("insert into sample values(3, 'Third')");

            // --- Act
            await db.FetchMultipleAsync<SampleRecord, SampleRecord>("dummy sql");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public async Task FetchMultipleAsyncRaisesExceptionWithInvalidConversion()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");
            await db.ExecuteAsync("insert into sample values(3, 'Third')");

            // --- Act
            await db.FetchMultipleAsync<SampleRecord, SampleRecord1>(
                @"select * from sample where Id = 1
                  select * from sample where Id = 2 or Id = 3
                  select Name, Id from sample where Id >=4 and Id <= 6");
        }

        [TestMethod]
        public async Task FetchMultipleAsyncExpressionWorksWith2Sets()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");
            await db.ExecuteAsync("insert into sample values(3, 'Third')");

            // --- Act
            var rows = await db.FetchMultipleAsync<SampleRecord, SampleRecord>(SqlExpression.CreateFrom(
                @"select * from sample where Id = 1
                  select * from sample where Id > 1"));
            var set1 = rows.Item1;
            var set2 = rows.Item2;

            // --- Assert
            set1.ShouldHaveCountOf(1);
            set2.ShouldHaveCountOf(2);
        }

        [TestMethod]
        public async Task FetchMultipleAsyncExpressionWorksWith3Sets()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");
            await db.ExecuteAsync("insert into sample values(3, 'Third')");
            await db.ExecuteAsync("insert into sample values(4, 'Fourth')");
            await db.ExecuteAsync("insert into sample values(5, 'Fifth')");

            // --- Act
            var rows = await db.FetchMultipleAsync<SampleRecord, SampleRecord, SampleRecord>(SqlExpression.CreateFrom(
                @"select * from sample where Id = 1
                  select * from sample where Id = 2
                  select Name, Id from sample where Id > 2"));
            var set1 = rows.Item1;
            var set2 = rows.Item2;
            var set3 = rows.Item3;

            // --- Assert
            set1.ShouldHaveCountOf(1);
            set2.ShouldHaveCountOf(1);
            set3.ShouldHaveCountOf(3);
        }

        [TestMethod]
        public async Task FetchMultipleAsyncExpressionWorksWith4Sets()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");
            await db.ExecuteAsync("insert into sample values(3, 'Third')");
            await db.ExecuteAsync("insert into sample values(4, 'Fourth')");
            await db.ExecuteAsync("insert into sample values(5, 'Fifth')");
            await db.ExecuteAsync("insert into sample values(6, 'Sixth')");
            await db.ExecuteAsync("insert into sample values(7, 'Seventh')");

            // --- Act
            var rows = await db.FetchMultipleAsync<SampleRecord, SampleRecord, SampleRecord, SampleRecord>(
                SqlExpression.CreateFrom(
                @"select * from sample where Id = 1
                  select * from sample where Id = 2 or Id = 3
                  select Name, Id from sample where Id >=4 and Id <= 6
                  select * from sample where Id > 6"));
            var set1 = rows.Item1;
            var set2 = rows.Item2;
            var set3 = rows.Item3;
            var set4 = rows.Item4;

            // --- Assert
            set1.ShouldHaveCountOf(1);
            set2.ShouldHaveCountOf(2);
            set3.ShouldHaveCountOf(3);
            set4.ShouldHaveCountOf(1);
        }

        [TestMethod]
        public async Task FetchMultipleAsyncWorksWithMapping2Sets()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");
            await db.ExecuteAsync("insert into sample values(3, 'Third')");

            // --- Act
            var rows = await db.FetchMultipleAsync<SampleRecord, SampleRecord, int>(SqlExpression.CreateFrom(
                @"select * from sample where Id = 1
                  select * from sample where Id > 1"), (l1, l2) => l1.Count + l2.Count);

            // --- Assert
            rows.ShouldEqual(3);
        }

        [TestMethod]
        public async Task FetchMultipleAsyncWorksWithMapping3Sets()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");
            await db.ExecuteAsync("insert into sample values(3, 'Third')");
            await db.ExecuteAsync("insert into sample values(4, 'Fourth')");
            await db.ExecuteAsync("insert into sample values(5, 'Fifth')");

            // --- Act
            var rows = await db.FetchMultipleAsync<SampleRecord, SampleRecord, SampleRecord, int>(SqlExpression.CreateFrom(
                @"select * from sample where Id = 1
                  select * from sample where Id = 2
                  select Name, Id from sample where Id > 2"), (l1, l2, l3) => l1.Count + l2.Count + l3.Count);

            // --- Assert
            rows.ShouldEqual(5);
        }

        [TestMethod]
        public async Task FetchMultipleWorksWithMapping4Sets()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");
            await db.ExecuteAsync("insert into sample values(3, 'Third')");
            await db.ExecuteAsync("insert into sample values(4, 'Fourth')");
            await db.ExecuteAsync("insert into sample values(5, 'Fifth')");
            await db.ExecuteAsync("insert into sample values(6, 'Sixth')");
            await db.ExecuteAsync("insert into sample values(7, 'Seventh')");

            // --- Act
            var rows = await db.FetchMultipleAsync<SampleRecord, SampleRecord, SampleRecord, SampleRecord, int>(
                SqlExpression.CreateFrom(
                @"select * from sample where Id = 1
                  select * from sample where Id = 2 or Id = 3
                  select Name, Id from sample where Id >=4 and Id <= 6
                  select * from sample where Id > 6"),
                  (l1, l2, l3, l4) => l1.Count + l2.Count + l3.Count + l4.Count);

            // --- Assert
            rows.ShouldEqual(7);
        }

        [TableName("sample")]
        internal class SampleRecord : DataRecord<SampleRecord>
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        [TableName("sample")]
        internal class SampleRecord1 : DataRecord<SampleRecord1>
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
    }
}
