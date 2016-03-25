using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.FbSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.FbSql.UnitTests.DataAccess
{
    [TestClass]
    public class InsertAsyncTest
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
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task InsertAsyncWithNullRaisesException()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.InsertAsync<SampleRecord>(null);
        }

        [TestMethod]
        public async Task InsertAsyncWorksWithSimpleRecord()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.InsertAsync(new SampleRecord { Id = 1, Name = "First" });
            await db.InsertAsync(new SampleRecord { Id = 2, Name = "Second" });

            // --- Act
            var rows = await db.FetchAsync<SampleRecord>(@"order by ""Id""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual(1);
            rows[0].Name.ShouldEqual("First");
            rows[1].Id.ShouldEqual(2);
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        public async Task InsertAsyncWorksWithCalculated()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name1"" varchar(50), ""Name2"" varchar(50), 
                         ""Name3"" computed by (""Name1"" || ""Name2"") )");
            await db.CompleteTransactionAsync();
            await db.InsertAsync(new SampleRecordWithCalculation { Id = 1, Name1 = "1", Name2 = "2" });

            // --- Act
            var rows = await db.FetchAsync<SampleRecordWithCalculation>();

            // --- Assert
            rows.ShouldHaveCountOf(1);
            rows[0].Id.ShouldEqual(1);
            rows[0].Name1.ShouldEqual("1");
            rows[0].Name2.ShouldEqual("2");
            rows[0].Name3.ShouldEqual("12");
        }

        [TestMethod]
        public async Task InsertAsyncWorksWithEmptyFields()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int, ""Name1"" varchar(50), 
                         ""Name2"" computed by (""Name1"") )");
            await db.CompleteTransactionAsync();
            await db.InsertAsync(new SampleRecordWithNulls());

            // --- Act
            var rows = await db.FetchAsync<SampleRecordWithNulls>();

            // --- Assert
            rows.ShouldHaveCountOf(1);
            rows[0].Id.ShouldBeNull();
            rows[0].Name1.ShouldBeNull();
            rows[0].Name2.ShouldBeNull();
        }

        [TestMethod]
        public async Task InsertAsyncWorksWithDate()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Date"" timestamp not null)");
            await db.CompleteTransactionAsync();
            await db.InsertAsync(new SampleRecordWithDate { Id = 1, Date = DateTime.MinValue });
            await db.InsertAsync(new SampleRecordWithDate { Id = 2, Date = DateTime.Now });

            // --- Act
            var rows = await db.FetchAsync<SampleRecordWithDate>(@"order by ""Id""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Date.Year.ShouldEqual(1754);
            rows[1].Date.Year.ShouldEqual(DateTime.Now.Year);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task InsertAsyncFailsInReadOnlyMode()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN, SqlOperationMode.ReadOnly);

            // --- Act
            await db.InsertAsync(new SampleRecord { Id = 1, Name = "First" });
        }

        [TableName("sample")]
        class SampleRecord : DataRecord<SampleRecord>
        {
            private int _id;
            private string _name;

            [PrimaryKey]
            public int Id
            {
                get { return _id; }
                set { _id = Modify(value, "Id"); }
            }

            public string Name
            {
                get { return _name; }
                set { _name = Modify(value, "Name"); }
            }
        }

        [TableName("sample")]
        class SampleRecordWithCalculation : DataRecord<SampleRecordWithCalculation>
        {
            public int Id { get; set; }
            public string Name1 { get; set; }
            public string Name2 { get; set; }

            [Calculated]
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public string Name3 { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }

        [TableName("sample")]
        class SampleRecordWithNulls : DataRecord<SampleRecordWithNulls>
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public int? Id { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local

            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public string Name1 { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local

            [Calculated]
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public string Name2 { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }

        [TableName("sample")]
        class SampleRecordWithDate : DataRecord<SampleRecordWithDate>
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public int Id { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
            public DateTime Date { get; set; }
        }
    }
}
