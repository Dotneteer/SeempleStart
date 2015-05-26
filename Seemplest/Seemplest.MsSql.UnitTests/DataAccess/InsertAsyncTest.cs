using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.MsSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.MsSql.UnitTests.DataAccess
{
    [TestClass]
    public class InsertAsyncTest
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
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task InsertAsyncWithNullRaisesException()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.InsertAsync<SampleRecord>(null);
        }

        [TestMethod]
        public async Task InsertAsyncWorksWithSimpleRecord()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.InsertAsync(new SampleRecord { Id = 1, Name = "First" });
            await db.InsertAsync(new SampleRecord { Id = 2, Name = "Second" });

            // --- Act
            var rows = await db.FetchAsync<SampleRecord>("order by Id");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual(1);
            rows[0].Name.ShouldEqual("First");
            rows[1].Id.ShouldEqual(2);
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        public async Task InsertAsyncWorksWithIdentity()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int identity(1, 1) not null, Name varchar(50) null)");
            await db.InsertAsync(new SampleRecordWithIdentity { Name = "First" });
            await db.InsertAsync(new SampleRecordWithIdentity { Name = "Second" });

            // --- Act
            var rows = await db.FetchAsync<SampleRecord>("order by Id");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual(1);
            rows[0].Name.ShouldEqual("First");
            rows[1].Id.ShouldEqual(2);
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        public async Task InsertAsyncWorksWithGetTurnedOff()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int identity(1, 1) not null, Name varchar(50) null)");

            // --- Act
            var record1 = new SampleRecordWithIdentity { Name = "First" };
            await db.InsertAsync(record1);
            var record2 = new SampleRecordWithIdentity { Name = "Second" };
            await db.InsertAsync(record2, withGet: false);
            var count = await db.ExecuteScalarAsync<int>("select count(*) from sample");

            // --- Assert
            record1.Id.ShouldEqual(1);
            record2.Id.ShouldEqual(0);
            count.ShouldEqual(2);
        }

        [TestMethod]
        public async Task InsertAsyncWorksWithExplicitIdentity()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int identity(1, 1) not null, Name varchar(50) null)");
            await db.InsertAsync(new SampleRecordWithIdentity { Id = 23, Name = "First" }, true);

            // --- Act
            var rows = await db.FetchAsync<SampleRecord>();

            // --- Assert
            rows.ShouldHaveCountOf(1);
            rows[0].Id.ShouldEqual(23);
            rows[0].Name.ShouldEqual("First");
        }

        [TestMethod]
        public async Task InsertAsyncWorksWithVersion()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null, Version rowversion not null)");
            await db.InsertAsync(new SampleRecordWithVersion { Id = 1, Name = "First" });
            await db.InsertAsync(new SampleRecordWithVersion { Id = 2, Name = "Second" });

            // --- Act
            var rows = await db.FetchAsync<SampleRecordWithVersion>("order by Id");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual(1);
            rows[0].Name.ShouldEqual("First");
            rows[0].Version.ShouldNotBeNull();
            rows[1].Id.ShouldEqual(2);
            rows[1].Name.ShouldEqual("Second");
            rows[1].Version.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task InsertAsyncWorksWithCalculated()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name1 varchar(50) null, Name2 varchar(50) null, Name3 as Name1 + Name2)");
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
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int null, Name1 varchar(50) null, Name2 as Name1)");
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
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Date datetime not null)");
            await db.InsertAsync(new SampleRecordWithDate { Id = 1, Date = DateTime.MinValue });
            await db.InsertAsync(new SampleRecordWithDate { Id = 2, Date = DateTime.Now });

            // --- Act
            var rows = await db.FetchAsync<SampleRecordWithDate>("order by Id");

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
            var db = new SqlDatabase(DB_CONN, SqlOperationMode.ReadOnly);

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
        class SampleRecordWithIdentity : DataRecord<SampleRecordWithIdentity>
        {
            [AutoGenerated]
            public int Id { get; set; }
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public string Name { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }

        [TableName("sample")]
        class SampleRecordWithVersion : DataRecord<SampleRecordWithVersion>
        {
            private int _id;
            private string _name;
            private byte[] _version;

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

            [VersionColumn]
            public byte[] Version
            {
                get { return _version; }
                // ReSharper disable UnusedMember.Local
                set { _version = Modify(value, "Version"); }
                // ReSharper restore UnusedMember.Local
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
            [Calculated]
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public int? Id { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local

            [Calculated]
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
