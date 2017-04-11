using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.MsSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.MsSql.UnitTests.DataAccess
{
    [TestClass]
    public class FetchFromSpAsyncTest
    {
        const string DB_CONN = "connStr=Seemplest";

        [TestCleanup]
        public void Cleanup()
        {
            var db = new SqlDatabase(DB_CONN);
            db.Execute(
                @"if exists (select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'sample')
                  drop table sample");
            db.Execute(
                @"if exists (select [ROUTINE_NAME] from [INFORMATION_SCHEMA].[ROUTINES]
                  where [ROUTINE_NAME] = 'SampleSp')
                  drop procedure [dbo].[SampleSp]");
        }

        [TestMethod]
        public async Task FetchFromSpAsyncWorksAsExpected()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync(@"CREATE PROCEDURE [dbo].[SampleSp] 
                                    	@@Par1 int, 
                                       	@@Par2 varchar(100)
                                    AS
                                    BEGIN
                                        select * from sample where Id >= @@Par1
                                        return 123
                                    END");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");

            // --- Act
            var result = await db.FetchFromSpAsync<SampleRecord>(
                new SqlExpression("[dbo].[SampleSp] @0, @1", 2, "haho"));

            // --- Assert
            var rows = result.Item1;
            var retVal = result.Item2;
            rows.ShouldHaveCountOf(1);
            rows[0].Id.ShouldEqual(2);
            rows[0].Name.ShouldEqual("Second");
            retVal.ShouldEqual(123);
        }

        [TestMethod]
        public async Task FetchFromSpAsyncWorksWithExec()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync(@"CREATE PROCEDURE [dbo].[SampleSp] 
                                    	@@Par1 int, 
                                       	@@Par2 varchar(100)
                                    AS
                                    BEGIN
                                        select * from sample where Id >= @@Par1
                                        return 123
                                    END");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");

            // --- Act
            var result = await db.FetchFromSpAsync<SampleRecord>(
                new SqlExpression("exEC [dbo].[SampleSp] @0, @1", 2, "haho"));

            // --- Assert
            var rows = result.Item1;
            var retVal = result.Item2;
            rows.ShouldHaveCountOf(1);
            rows[0].Id.ShouldEqual(2);
            rows[0].Name.ShouldEqual("Second");
            retVal.ShouldEqual(123);
        }

        [TestMethod]
        public async Task FetchFromSpAsyncWorksWithExecute()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync(@"CREATE PROCEDURE [dbo].[SampleSp] 
                                    	@@Par1 int, 
                                       	@@Par2 varchar(100)
                                    AS
                                    BEGIN
                                        select * from sample where Id >= @@Par1
                                        return 123
                                    END");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");

            // --- Act
            var result = await db.FetchFromSpAsync<SampleRecord>(
                new SqlExpression("execUTE [dbo].[SampleSp] @0, @1", 2, "haho"));

            // --- Assert
            var rows = result.Item1;
            var retVal = result.Item2;
            rows.ShouldHaveCountOf(1);
            rows[0].Id.ShouldEqual(2);
            rows[0].Name.ShouldEqual("Second");
            retVal.ShouldEqual(123);
        }

        [TestMethod]
        public async Task FetchFromSpAsyncWithSqlWorksAsExpected()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync(@"CREATE PROCEDURE [dbo].[SampleSp] 
                                    	@@Par1 int, 
                                       	@@Par2 varchar(100)
                                    AS
                                    BEGIN
                                        select * from sample where Id >= @@Par1
                                        return 123
                                    END");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");

            // --- Act
            var result = await db.FetchFromSpAsync<SampleRecord>("[dbo].[SampleSp] @0, @1", 2, "haho");

            // --- Assert
            var rows = result.Item1;
            var retVal = result.Item2;
            rows.ShouldHaveCountOf(1);
            rows[0].Id.ShouldEqual(2);
            rows[0].Name.ShouldEqual("Second");
            retVal.ShouldEqual(123);
        }

        [TestMethod]
        public async Task FetchFromSpAsyncWithSqlWorksWithExec()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync(@"CREATE PROCEDURE [dbo].[SampleSp] 
                                    	@@Par1 int, 
                                       	@@Par2 varchar(100)
                                    AS
                                    BEGIN
                                        select * from sample where Id >= @@Par1
                                        return 123
                                    END");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");

            // --- Act
            var result = await db.FetchFromSpAsync<SampleRecord>("exEC [dbo].[SampleSp] @0, @1", 2, "haho");

            // --- Assert
            var rows = result.Item1;
            var retVal = result.Item2;
            rows.ShouldHaveCountOf(1);
            rows[0].Id.ShouldEqual(2);
            rows[0].Name.ShouldEqual("Second");
            retVal.ShouldEqual(123);
        }

        [TestMethod]
        public async Task FetchFromSpAsyncWithSqlWorkWithExecute()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync(@"CREATE PROCEDURE [dbo].[SampleSp] 
                                    	@@Par1 int, 
                                       	@@Par2 varchar(100)
                                    AS
                                    BEGIN
                                        select * from sample where Id >= @@Par1
                                        return 123
                                    END");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");

            // --- Act
            var result = await db.FetchFromSpAsync<SampleRecord>("execUTE [dbo].[SampleSp] @0, @1", 2, "haho");

            // --- Assert
            var rows = result.Item1;
            var retVal = result.Item2;
            rows.ShouldHaveCountOf(1);
            rows[0].Id.ShouldEqual(2);
            rows[0].Name.ShouldEqual("Second");
            retVal.ShouldEqual(123);
        }

        [TestMethod]
        public async Task FetchFromSpAsyncWorksWith2ResultSet()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync(@"CREATE PROCEDURE [dbo].[SampleSp] 
                                    	@@Par1 int, 
                                       	@@Par2 varchar(100)
                                    AS
                                    BEGIN
                                        select * from sample where Id >= @@Par1
                                        select * from sample
                                        return 123
                                    END");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");
            await db.ExecuteAsync("insert into sample values(3, 'Third')");
            await db.ExecuteAsync("insert into sample values(4, 'Fourth')");
            await db.ExecuteAsync("insert into sample values(5, 'Fifth')");
            await db.ExecuteAsync("insert into sample values(6, 'Sixth')");
            await db.ExecuteAsync("insert into sample values(7, 'Seventh')");

            // --- Act
            var result = await db.FetchFromSpAsync<SampleRecord, SampleRecord>(
                "[dbo].[SampleSp] @0, @1", 2, "haho");

            // --- Assert
            var rows1 = result.Item1;
            var rows2 = result.Item2;
            var retVal = result.Item3;
            rows1.ShouldHaveCountOf(6);
            rows2.ShouldHaveCountOf(7);
            retVal.ShouldEqual(123);
        }

        [TestMethod]
        public async Task FetchFromSpAsyncWorksWith3ResultSet()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync(@"CREATE PROCEDURE [dbo].[SampleSp] 
                                    	@@Par1 int, 
                                       	@@Par2 varchar(100)
                                    AS
                                    BEGIN
                                        select * from sample where Id >= @@Par1
                                        select * from sample
                                        select * from sample where Id > 2
                                        return 123
                                    END");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");
            await db.ExecuteAsync("insert into sample values(3, 'Third')");
            await db.ExecuteAsync("insert into sample values(4, 'Fourth')");
            await db.ExecuteAsync("insert into sample values(5, 'Fifth')");
            await db.ExecuteAsync("insert into sample values(6, 'Sixth')");
            await db.ExecuteAsync("insert into sample values(7, 'Seventh')");

            // --- Act
            var result = await db.FetchFromSpAsync<SampleRecord, SampleRecord, SampleRecord>(
                "[dbo].[SampleSp] @0, @1", 2, "haho");

            // --- Assert
            var rows1 = result.Item1;
            var rows2 = result.Item2;
            var rows3 = result.Item3;
            var retVal = result.Item4;
            rows1.ShouldHaveCountOf(6);
            rows2.ShouldHaveCountOf(7);
            rows3.ShouldHaveCountOf(5);
            retVal.ShouldEqual(123);
        }

        [TestMethod]
        public async Task FetchFromSpAsyncWorksWith4ResultSet()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync(@"CREATE PROCEDURE [dbo].[SampleSp] 
                                    	@@Par1 int, 
                                       	@@Par2 varchar(100)
                                    AS
                                    BEGIN
                                        select * from sample where Id >= @@Par1
                                        select * from sample
                                        select * from sample where Id > 2
                                        select * from sample where Id > 3
                                        return 123
                                    END");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");
            await db.ExecuteAsync("insert into sample values(3, 'Third')");
            await db.ExecuteAsync("insert into sample values(4, 'Fourth')");
            await db.ExecuteAsync("insert into sample values(5, 'Fifth')");
            await db.ExecuteAsync("insert into sample values(6, 'Sixth')");
            await db.ExecuteAsync("insert into sample values(7, 'Seventh')");

            // --- Act
            var result = await db.FetchFromSpAsync<SampleRecord, SampleRecord, SampleRecord, SampleRecord>(
                "[dbo].[SampleSp] @0, @1", 2, "haho");

            // --- Assert
            var rows1 = result.Item1;
            var rows2 = result.Item2;
            var rows3 = result.Item3;
            var rows4 = result.Item4;
            var retVal = result.Item5;
            rows1.ShouldHaveCountOf(6);
            rows2.ShouldHaveCountOf(7);
            rows3.ShouldHaveCountOf(5);
            rows4.ShouldHaveCountOf(4);
            retVal.ShouldEqual(123);
        }

        [TestMethod]
        public async Task FetchFromSpAsyncWorksWith5ResultSet()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync(@"CREATE PROCEDURE [dbo].[SampleSp] 
                                    	@@Par1 int, 
                                       	@@Par2 varchar(100)
                                    AS
                                    BEGIN
                                        select * from sample where Id >= @@Par1
                                        select * from sample
                                        select * from sample where Id > 2
                                        select * from sample where Id > 3
                                        select * from sample where Id > 4
                                        return 123
                                    END");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");
            await db.ExecuteAsync("insert into sample values(3, 'Third')");
            await db.ExecuteAsync("insert into sample values(4, 'Fourth')");
            await db.ExecuteAsync("insert into sample values(5, 'Fifth')");
            await db.ExecuteAsync("insert into sample values(6, 'Sixth')");
            await db.ExecuteAsync("insert into sample values(7, 'Seventh')");

            // --- Act
            var result = await db.FetchFromSpAsync<SampleRecord, SampleRecord, SampleRecord, SampleRecord, SampleRecord>(
                "[dbo].[SampleSp] @0, @1", 2, "haho");

            // --- Assert
            var rows1 = result.Item1;
            var rows2 = result.Item2;
            var rows3 = result.Item3;
            var rows4 = result.Item4;
            var rows5 = result.Item5;
            var retVal = result.Item6;
            rows1.ShouldHaveCountOf(6);
            rows2.ShouldHaveCountOf(7);
            rows3.ShouldHaveCountOf(5);
            rows4.ShouldHaveCountOf(4);
            rows5.ShouldHaveCountOf(3);
            retVal.ShouldEqual(123);
        }

        [TestMethod]
        public async Task FetchFromSpAsyncWorksWith6ResultSet()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.ExecuteAsync(@"CREATE PROCEDURE [dbo].[SampleSp] 
                                    	@@Par1 int, 
                                       	@@Par2 varchar(100)
                                    AS
                                    BEGIN
                                        select * from sample where Id >= @@Par1
                                        select * from sample
                                        select * from sample where Id > 2
                                        select * from sample where Id > 3
                                        select * from sample where Id > 4
                                        select * from sample where Id > 5
                                        return 123
                                    END");
            await db.ExecuteAsync("insert into sample values(1, 'First')");
            await db.ExecuteAsync("insert into sample values(2, 'Second')");
            await db.ExecuteAsync("insert into sample values(3, 'Third')");
            await db.ExecuteAsync("insert into sample values(4, 'Fourth')");
            await db.ExecuteAsync("insert into sample values(5, 'Fifth')");
            await db.ExecuteAsync("insert into sample values(6, 'Sixth')");
            await db.ExecuteAsync("insert into sample values(7, 'Seventh')");

            // --- Act
            var result = await db.FetchFromSpAsync<SampleRecord, SampleRecord, SampleRecord, SampleRecord, SampleRecord, SampleRecord>(
                "[dbo].[SampleSp] @0, @1", 2, "haho");

            // --- Assert
            var rows1 = result.Item1;
            var rows2 = result.Item2;
            var rows3 = result.Item3;
            var rows4 = result.Item4;
            var rows5 = result.Item5;
            var rows6 = result.Item6;
            var retVal = result.Item7;
            rows1.ShouldHaveCountOf(6);
            rows2.ShouldHaveCountOf(7);
            rows3.ShouldHaveCountOf(5);
            rows4.ShouldHaveCountOf(4);
            rows5.ShouldHaveCountOf(3);
            rows6.ShouldHaveCountOf(2);
            retVal.ShouldEqual(123);
        }

        [TableName("sample")]
        internal class SampleRecord : DataRecord<SampleRecord>
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}