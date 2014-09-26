using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.MsSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.MsSql.UnitTests.DataAccess
{
    [TestClass]
    public class PagingAsyncTest
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
        public async Task PageAsyncWorksAsExpected()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            for (var i = 1; i <= 13; i++)
            {
                await db.InsertAsync(new SampleRecord { Id = i, Name = "Record " + i });
            }

            // --- Act
            var sqlExpr = SqlExpression.New.Select<SampleRecord>().From<SampleRecord>().OrderBy("Id");
            var page1 = await db.PageAsync<SampleRecord>(0, 6, sqlExpr);
            var page2 = await db.PageAsync<SampleRecord>(1, 6, sqlExpr);
            var page3 = await db.PageAsync<SampleRecord>(2, 6, sqlExpr);
            var page4 = await db.PageAsync<SampleRecord>(3, 6, sqlExpr);

            // --- Assert
            page1.CurrentPage.ShouldEqual(0);
            page1.ItemsPerPage.ShouldEqual(6);
            page1.Items.ShouldHaveCountOf(6);
            page1.TotalItems.ShouldEqual(13);
            page1.TotalPages.ShouldEqual(3);

            page2.CurrentPage.ShouldEqual(1);
            page2.ItemsPerPage.ShouldEqual(6);
            page2.Items.ShouldHaveCountOf(6);
            page2.TotalItems.ShouldEqual(13);
            page2.TotalPages.ShouldEqual(3);

            page3.CurrentPage.ShouldEqual(2);
            page3.ItemsPerPage.ShouldEqual(6);
            page3.Items.ShouldHaveCountOf(1);
            page3.TotalItems.ShouldEqual(13);
            page3.TotalPages.ShouldEqual(3);

            page4.CurrentPage.ShouldEqual(3);
            page4.ItemsPerPage.ShouldEqual(6);
            page4.Items.ShouldHaveCountOf(0);
            page4.TotalItems.ShouldEqual(13);
            page4.TotalPages.ShouldEqual(3);
        }

        [TestMethod]
        public async Task SkipTakeAsyncWorksAsExpected()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            for (var i = 1; i <= 13; i++)
            {
                await db.InsertAsync(new SampleRecord { Id = i, Name = "Record " + i });
            }

            // --- Act
            var sqlExpr = SqlExpression.New.Select<SampleRecord>().From<SampleRecord>().OrderBy("Id");
            var rows1 = await db.SkipTakeAsync<SampleRecord>(0, 3, sqlExpr);
            var rows2 = await db.SkipTakeAsync<SampleRecord>(2, 4, sqlExpr);
            var rows3 = await db.SkipTakeAsync<SampleRecord>(13, 2, sqlExpr);

            // --- Assert
            rows1.ShouldHaveCountOf(3);
            rows1.Select(r => r.Id).ShouldContain(1);
            rows1.Select(r => r.Id).ShouldContain(2);
            rows1.Select(r => r.Id).ShouldContain(3);

            rows2.ShouldHaveCountOf(4);
            rows2.Select(r => r.Id).ShouldContain(3);
            rows2.Select(r => r.Id).ShouldContain(4);
            rows2.Select(r => r.Id).ShouldContain(5);
            rows2.Select(r => r.Id).ShouldContain(6);

            rows3.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public async Task SkipTakeAsyncWorksWithoutOrderBy()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            for (var i = 1; i <= 13; i++)
            {
                await db.InsertAsync(new SampleRecord { Id = i, Name = "Record " + i });
            }

            // --- Act
            var sqlExpr = SqlExpression.New.Select<SampleRecord>().From<SampleRecord>();
            var rows1 = await db.SkipTakeAsync<SampleRecord>(0, 3, sqlExpr);
            var rows2 = await db.SkipTakeAsync<SampleRecord>(2, 4, sqlExpr);
            var rows3 = await db.SkipTakeAsync<SampleRecord>(13, 2, sqlExpr);

            // --- Assert
            rows1.ShouldHaveCountOf(3);
            rows1.Select(r => r.Id).ShouldContain(1);
            rows1.Select(r => r.Id).ShouldContain(2);
            rows1.Select(r => r.Id).ShouldContain(3);

            rows2.ShouldHaveCountOf(4);
            rows2.Select(r => r.Id).ShouldContain(3);
            rows2.Select(r => r.Id).ShouldContain(4);
            rows2.Select(r => r.Id).ShouldContain(5);
            rows2.Select(r => r.Id).ShouldContain(6);

            rows3.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public async Task SkipTakeAsyncWorksWithDistinct()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            for (var i = 1; i <= 13; i++)
            {
                await db.InsertAsync(new SampleRecord { Id = i, Name = "Record " + i });
            }

            // --- Act
            var sqlExpr = SqlExpression.New.Select<SampleRecord>().Distinct.From<SampleRecord>();
            var rows1 = await db.SkipTakeAsync<SampleRecord>(0, 3, sqlExpr);
            var rows2 = await db.SkipTakeAsync<SampleRecord>(2, 4, sqlExpr);
            var rows3 = await db.SkipTakeAsync<SampleRecord>(13, 2, sqlExpr);

            // --- Assert
            rows1.ShouldHaveCountOf(3);
            rows1.Select(r => r.Id).ShouldContain(1);
            rows1.Select(r => r.Id).ShouldContain(2);
            rows1.Select(r => r.Id).ShouldContain(3);

            rows2.ShouldHaveCountOf(4);
            rows2.Select(r => r.Id).ShouldContain(3);
            rows2.Select(r => r.Id).ShouldContain(4);
            rows2.Select(r => r.Id).ShouldContain(5);
            rows2.Select(r => r.Id).ShouldContain(6);

            rows3.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public async Task PageAsyncAutomaticallyAddsSelectAndFrom()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            for (var i = 1; i <= 13; i++)
            {
                await db.InsertAsync(new SampleRecord { Id = i, Name = "Record " + i });
            }

            // --- Act
            var sqlExpr = SqlExpression.New.Where("Id > 0");
            var page1 = await db.PageAsync<SampleRecord>(0, 6, sqlExpr);
            var page2 = await db.PageAsync<SampleRecord>(1, 6, sqlExpr);
            var page3 = await db.PageAsync<SampleRecord>(2, 6, sqlExpr);
            var page4 = await db.PageAsync<SampleRecord>(3, 6, sqlExpr);

            // --- Assert
            page1.CurrentPage.ShouldEqual(0);
            page1.ItemsPerPage.ShouldEqual(6);
            page1.Items.ShouldHaveCountOf(6);
            page1.TotalItems.ShouldEqual(13);
            page1.TotalPages.ShouldEqual(3);

            page2.CurrentPage.ShouldEqual(1);
            page2.ItemsPerPage.ShouldEqual(6);
            page2.Items.ShouldHaveCountOf(6);
            page2.TotalItems.ShouldEqual(13);
            page2.TotalPages.ShouldEqual(3);

            page3.CurrentPage.ShouldEqual(2);
            page3.ItemsPerPage.ShouldEqual(6);
            page3.Items.ShouldHaveCountOf(1);
            page3.TotalItems.ShouldEqual(13);
            page3.TotalPages.ShouldEqual(3);

            page4.CurrentPage.ShouldEqual(3);
            page4.ItemsPerPage.ShouldEqual(6);
            page4.Items.ShouldHaveCountOf(0);
            page4.TotalItems.ShouldEqual(13);
            page4.TotalPages.ShouldEqual(3);
        }

        [TestMethod]
        public async Task PageAsyncAutomaticallyAddsSelect()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            for (var i = 1; i <= 13; i++)
            {
                await db.InsertAsync(new SampleRecord { Id = i, Name = "Record " + i });
            }

            // --- Act
            var sqlExpr = SqlExpression.New.From<SampleRecord>().Where("Id > 0");
            var page1 = await db.PageAsync<SampleRecord>(0, 6, sqlExpr);
            var page2 = await db.PageAsync<SampleRecord>(1, 6, sqlExpr);
            var page3 = await db.PageAsync<SampleRecord>(2, 6, sqlExpr);
            var page4 = await db.PageAsync<SampleRecord>(3, 6, sqlExpr);

            // --- Assert
            page1.CurrentPage.ShouldEqual(0);
            page1.ItemsPerPage.ShouldEqual(6);
            page1.Items.ShouldHaveCountOf(6);
            page1.TotalItems.ShouldEqual(13);
            page1.TotalPages.ShouldEqual(3);

            page2.CurrentPage.ShouldEqual(1);
            page2.ItemsPerPage.ShouldEqual(6);
            page2.Items.ShouldHaveCountOf(6);
            page2.TotalItems.ShouldEqual(13);
            page2.TotalPages.ShouldEqual(3);

            page3.CurrentPage.ShouldEqual(2);
            page3.ItemsPerPage.ShouldEqual(6);
            page3.Items.ShouldHaveCountOf(1);
            page3.TotalItems.ShouldEqual(13);
            page3.TotalPages.ShouldEqual(3);

            page4.CurrentPage.ShouldEqual(3);
            page4.ItemsPerPage.ShouldEqual(6);
            page4.Items.ShouldHaveCountOf(0);
            page4.TotalItems.ShouldEqual(13);
            page4.TotalPages.ShouldEqual(3);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task InvalidSqlInPageRaisesException()
        {
            // --- Assert
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            await db.PageAsync<SampleRecord>(0, 1, new SqlExpression("select Dummy"));
        }


        [TestMethod]
        public async Task PageAsyncWorksWithZeroRecord()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            for (var i = 1; i <= 13; i++)
            {
                await db.InsertAsync(new SampleRecord { Id = i, Name = "Record " + i });
            }

            // --- Act
            const string SQL_EXPR = "select * from sample group by Id, Name having sum(Id) > 100000";
            await db.PageAsync<SampleRecord>(0, 6, SQL_EXPR);
        }

        [TableName("sample")]
        internal class SampleRecord : DataRecord<SampleRecord>
        {
            [PrimaryKey]
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
