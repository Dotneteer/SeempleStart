using System;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.MsSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.MsSql.UnitTests.DataAccess
{
    [TestClass]
    public class FetchMultipleTest
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
        public void FetchMultipleWorksWith2Sets()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");
            db.Execute("insert into sample values(1, 'First')");
            db.Execute("insert into sample values(2, 'Second')");
            db.Execute("insert into sample values(3, 'Third')");

            // --- Act
            var rows = db.FetchMultiple<SampleRecord, SampleRecord>(
                @"select * from sample where Id = 1
                  select * from sample where Id > 1");
            var set1 = rows.Item1;
            var set2 = rows.Item2;

            // --- Assert
            set1.ShouldHaveCountOf(1);
            set2.ShouldHaveCountOf(2);
        }

        [TestMethod]
        public void FetchMultipleWorksWith3Sets()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");
            db.Execute("insert into sample values(1, 'First')");
            db.Execute("insert into sample values(2, 'Second')");
            db.Execute("insert into sample values(3, 'Third')");
            db.Execute("insert into sample values(4, 'Fourth')");
            db.Execute("insert into sample values(5, 'Fifth')");

            // --- Act
            var rows = db.FetchMultiple<SampleRecord, SampleRecord, SampleRecord>(
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
        public void FetchMultipleWorksWith4Sets()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");
            db.Execute("insert into sample values(1, 'First')");
            db.Execute("insert into sample values(2, 'Second')");
            db.Execute("insert into sample values(3, 'Third')");
            db.Execute("insert into sample values(4, 'Fourth')");
            db.Execute("insert into sample values(5, 'Fifth')");
            db.Execute("insert into sample values(6, 'Sixth')");
            db.Execute("insert into sample values(7, 'Seventh')");

            // --- Act
            var rows = db.FetchMultiple<SampleRecord, SampleRecord, SampleRecord, SampleRecord>(
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
        public void FetchMultipleWorksWithMissingSet()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");
            db.Execute("insert into sample values(1, 'First')");
            db.Execute("insert into sample values(2, 'Second')");
            db.Execute("insert into sample values(3, 'Third')");
            db.Execute("insert into sample values(4, 'Fourth')");
            db.Execute("insert into sample values(5, 'Fifth')");
            db.Execute("insert into sample values(6, 'Sixth')");
            db.Execute("insert into sample values(7, 'Seventh')");

            // --- Act
            var rows = db.FetchMultiple<SampleRecord, SampleRecord, SampleRecord, SampleRecord>(
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
        public void FetchMultipleWorksWithExtraSets()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");
            db.Execute("insert into sample values(1, 'First')");
            db.Execute("insert into sample values(2, 'Second')");
            db.Execute("insert into sample values(3, 'Third')");
            db.Execute("insert into sample values(4, 'Fourth')");
            db.Execute("insert into sample values(5, 'Fifth')");
            db.Execute("insert into sample values(6, 'Sixth')");
            db.Execute("insert into sample values(7, 'Seventh')");

            // --- Act
            var rows = db.FetchMultiple<SampleRecord, SampleRecord, SampleRecord, SampleRecord>(
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
        public void FetchMultipleRaisesExceptionWithInvalidSql()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");
            db.Execute("insert into sample values(1, 'First')");
            db.Execute("insert into sample values(2, 'Second')");
            db.Execute("insert into sample values(3, 'Third')");

            // --- Act
            db.FetchMultiple<SampleRecord, SampleRecord>("dummy sql");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void FetchMultipleRaisesExceptionWithInvalidConversion()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");
            db.Execute("insert into sample values(1, 'First')");
            db.Execute("insert into sample values(2, 'Second')");
            db.Execute("insert into sample values(3, 'Third')");

            // --- Act
            db.FetchMultiple<SampleRecord, SampleRecord1>(
                @"select * from sample where Id = 1
                  select * from sample where Id = 2 or Id = 3
                  select Name, Id from sample where Id >=4 and Id <= 6");
        }

        [TestMethod]
        public void FetchMultipleExpressionWorksWith2Sets()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");
            db.Execute("insert into sample values(1, 'First')");
            db.Execute("insert into sample values(2, 'Second')");
            db.Execute("insert into sample values(3, 'Third')");

            // --- Act
            var rows = db.FetchMultiple<SampleRecord, SampleRecord>(SqlExpression.CreateFrom(
                @"select * from sample where Id = 1
                  select * from sample where Id > 1"));
            var set1 = rows.Item1;
            var set2 = rows.Item2;

            // --- Assert
            set1.ShouldHaveCountOf(1);
            set2.ShouldHaveCountOf(2);
        }

        [TestMethod]
        public void FetchMultipleExpressionWorksWith3Sets()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");
            db.Execute("insert into sample values(1, 'First')");
            db.Execute("insert into sample values(2, 'Second')");
            db.Execute("insert into sample values(3, 'Third')");
            db.Execute("insert into sample values(4, 'Fourth')");
            db.Execute("insert into sample values(5, 'Fifth')");

            // --- Act
            var rows = db.FetchMultiple<SampleRecord, SampleRecord, SampleRecord>(SqlExpression.CreateFrom(
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
        public void FetchMultipleExpressionWorksWith4Sets()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");
            db.Execute("insert into sample values(1, 'First')");
            db.Execute("insert into sample values(2, 'Second')");
            db.Execute("insert into sample values(3, 'Third')");
            db.Execute("insert into sample values(4, 'Fourth')");
            db.Execute("insert into sample values(5, 'Fifth')");
            db.Execute("insert into sample values(6, 'Sixth')");
            db.Execute("insert into sample values(7, 'Seventh')");

            // --- Act
            var rows = db.FetchMultiple<SampleRecord, SampleRecord, SampleRecord, SampleRecord>(
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
        public void FetchMultipleWorksWithMapping2Sets()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");
            db.Execute("insert into sample values(1, 'First')");
            db.Execute("insert into sample values(2, 'Second')");
            db.Execute("insert into sample values(3, 'Third')");

            // --- Act
            var rows = db.FetchMultiple<SampleRecord, SampleRecord, int>(SqlExpression.CreateFrom(
                @"select * from sample where Id = 1
                  select * from sample where Id > 1"), (l1, l2) => l1.Count + l2.Count);

            // --- Assert
            rows.ShouldEqual(3);
        }

        [TestMethod]
        public void FetchMultipleWorksWithMapping3Sets()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");
            db.Execute("insert into sample values(1, 'First')");
            db.Execute("insert into sample values(2, 'Second')");
            db.Execute("insert into sample values(3, 'Third')");
            db.Execute("insert into sample values(4, 'Fourth')");
            db.Execute("insert into sample values(5, 'Fifth')");

            // --- Act
            var rows = db.FetchMultiple<SampleRecord, SampleRecord, SampleRecord, int>(SqlExpression.CreateFrom(
                @"select * from sample where Id = 1
                  select * from sample where Id = 2
                  select Name, Id from sample where Id > 2"), (l1, l2, l3) => l1.Count + l2.Count + l3.Count);

            // --- Assert
            rows.ShouldEqual(5);
        }

        [TestMethod]
        public void FetchMultipleWorksWithMapping4Sets()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");
            db.Execute("insert into sample values(1, 'First')");
            db.Execute("insert into sample values(2, 'Second')");
            db.Execute("insert into sample values(3, 'Third')");
            db.Execute("insert into sample values(4, 'Fourth')");
            db.Execute("insert into sample values(5, 'Fifth')");
            db.Execute("insert into sample values(6, 'Sixth')");
            db.Execute("insert into sample values(7, 'Seventh')");

            // --- Act
            var rows = db.FetchMultiple<SampleRecord, SampleRecord, SampleRecord, SampleRecord, int>(
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
