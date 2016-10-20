using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.FbSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.FbSql.UnitTests.DataAccess
{
    [TestClass]
    public class PagingTest
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
        public void PageWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            for (var i = 1; i <= 13; i++)
            {
                db.Insert(new SampleRecord { Id = i, Name = "Record " + i });
            }

            // --- Act
            var sqlExpr = SqlExpression.New.Select<SampleRecord>().From<SampleRecord>().OrderBy("Id");
            var page1 = db.Page<SampleRecord>(0, 6, sqlExpr);
            var page2 = db.Page<SampleRecord>(1, 6, sqlExpr);
            var page3 = db.Page<SampleRecord>(2, 6, sqlExpr);
            var page4 = db.Page<SampleRecord>(3, 6, sqlExpr);

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
        public void SkipTakeWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            for (var i = 1; i <= 13; i++)
            {
                db.Insert(new SampleRecord { Id = i, Name = "Record " + i });
            }

            // --- Act
            var sqlExpr = SqlExpression.New.Select<SampleRecord>().From<SampleRecord>().OrderBy(@"""Id""");
            var rows1 = db.SkipTake<SampleRecord>(0, 3, sqlExpr);
            var rows2 = db.SkipTake<SampleRecord>(2, 4, sqlExpr);
            var rows3 = db.SkipTake<SampleRecord>(13, 2, sqlExpr);

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
        public void SkipTakeWorksWithoutOrderBy()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            for (var i = 1; i <= 13; i++)
            {
                db.Insert(new SampleRecord { Id = i, Name = "Record " + i });
            }

            // --- Act
            var sqlExpr = SqlExpression.New.Select<SampleRecord>().From<SampleRecord>();
            var rows1 = db.SkipTake<SampleRecord>(0, 3, sqlExpr);
            var rows2 = db.SkipTake<SampleRecord>(2, 4, sqlExpr);
            var rows3 = db.SkipTake<SampleRecord>(13, 2, sqlExpr);

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
        public void SkipTakeWorksWithDistinct()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            for (var i = 1; i <= 13; i++)
            {
                db.Insert(new SampleRecord { Id = i, Name = "Record " + i });
            }

            // --- Act
            var sqlExpr = SqlExpression.New.Select<SampleRecord>().Distinct.From<SampleRecord>();
            var rows1 = db.SkipTake<SampleRecord>(0, 3, sqlExpr);
            var rows2 = db.SkipTake<SampleRecord>(2, 4, sqlExpr);
            var rows3 = db.SkipTake<SampleRecord>(13, 2, sqlExpr);

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
        public void PageAutomaticallyAddsSelectAndFrom()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            for (var i = 1; i <= 13; i++)
            {
                db.Insert(new SampleRecord { Id = i, Name = "Record " + i });
            }

            // --- Act
            var sqlExpr = SqlExpression.New.Where(@"""Id"" > 0");
            var page1 = db.Page<SampleRecord>(0, 6, sqlExpr);
            var page2 = db.Page<SampleRecord>(1, 6, sqlExpr);
            var page3 = db.Page<SampleRecord>(2, 6, sqlExpr);
            var page4 = db.Page<SampleRecord>(3, 6, sqlExpr);

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
        public void PageAutomaticallyAddsSelect()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            for (var i = 1; i <= 13; i++)
            {
                db.Insert(new SampleRecord { Id = i, Name = "Record " + i });
            }

            // --- Act
            var sqlExpr = SqlExpression.New.From<SampleRecord>().Where(@"""Id"" > 0");
            var page1 = db.Page<SampleRecord>(0, 6, sqlExpr);
            var page2 = db.Page<SampleRecord>(1, 6, sqlExpr);
            var page3 = db.Page<SampleRecord>(2, 6, sqlExpr);
            var page4 = db.Page<SampleRecord>(3, 6, sqlExpr);

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
        public void InvalidSqlInPageRaisesException()
        {
            // --- Assert
            var db = new FbDatabase(DB_CONN);

            // --- Act
            db.Page<SampleRecord>(0, 1, new SqlExpression("select Dummy"));
        }


        [TestMethod]
        public void PageWorksWithZeroRecord()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            for (var i = 1; i <= 13; i++)
            {
                db.Insert(new SampleRecord { Id = i, Name = "Record " + i });
            }

            // --- Act
            var sqlExpr = @"select * from ""sample"" group by ""Id"", ""Name"" having sum(""Id"") > 100000";
            var page = db.Page<SampleRecord>(0, 6, sqlExpr);

            // --- Assert
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
