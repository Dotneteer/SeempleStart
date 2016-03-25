using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
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
    public class DeleteAyncTest
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
        public void TestReturning()
        {
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" timestamp)");
            db.CompleteTransaction();
            using (
                var conn =
                    new FbConnection(
                        @"User=SYSDBA;Password=masterkey;DataSource=localhost;Database=C:\Temp\SeemplestTest.KSFDB;"))
            {
                conn.Open();
                var cmd = new FbCommand(
                    @"execute block returns (""Id"" int, ""Name"" timestamp) as begin insert into ""sample"" (""Id"", ""Name"") values(1, '02/29/2016 03:20:22.443') returning ""Id"", ""Name"" into :""Id"", :""Name""; suspend; end", conn);
                var reader = cmd.ExecuteReader();
                var row = reader.Read();
                row.ShouldBeTrue();
            }
            //db.Insert(new SampleRecord { Id = 1, Name = "First" });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task DeleteAsyncWithNullRaisesException()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();

            // --- Act
            await db.DeleteAsync<SampleRecord>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task DeleteAsyncWithNoPkRaisesException()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            var record = new SampleRecordWithNoPk { Name = "First" };

            // --- Act
            await db.DeleteAsync(record);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SimplePocoRaisesExceptionWithDeleteAsync()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            var record = new SimplePoco { Id = 1, Name = "First" };
            await db.InsertAsync(record);

            // --- Act
            await db.DeleteAsync(record);
        }

        [TestMethod]
        public async Task DeleteAsyncWorksWithSimpleRecord()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.InsertAsync(new SampleRecord { Id = 1, Name = "First" });
            await db.InsertAsync(new SampleRecord { Id = 2, Name = "First" });

            // --- Act
            var rows1 = await db.FetchAsync<SampleRecord>(@"order by ""Id""");
            await db.DeleteAsync(rows1[0]);
            var rows2 = await db.FetchAsync<SampleRecord>(@"order by ""Id""");
            await db.DeleteAsync(rows2[0]);
            var rows3 = await db.FetchAsync<SampleRecord>(@"order by ""Id""");

            // --- Assert
            rows1.ShouldHaveCountOf(2);
            rows2.ShouldHaveCountOf(1);
            rows3.ShouldHaveCountOf(0);
        }

        [TestMethod]
        [ExpectedException(typeof(TaskCanceledException))]
        public async Task DeleteAsyncWorksWithCancellation()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.InsertAsync(new SampleRecord { Id = 1, Name = "First" });
            var cts = new CancellationTokenSource();

            // --- Act
            var rows1 = await db.FetchAsync<SampleRecord>(cts.Token, @"order by ""Id""");
            cts.Cancel();
            await db.DeleteAsync(rows1[0], cts.Token);
        }

        [TestMethod]
        public async Task DeleteAsyncWorksWithMultiplePk()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id1"" int not null, ""Id2"" int not null, ""Name"" varchar(50), ""Description"" varchar(50))");
            await db.CompleteTransactionAsync();
            var record = new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "First" };
            await db.InsertAsync(record);

            // --- Act
            var back1 = await db.FirstOrDefaultAsync<SampleRecordWithMultiplePk>(
                @"where ""Id1"" = @0 and ""Id2"" = @1", record.Id1, record.Id2);
            await db.DeleteAsync(record);
            var back2 = await db.FirstOrDefaultAsync<SampleRecordWithMultiplePk>(
                @"where ""Id1"" = @0 and ""Id2"" = @1", record.Id1, record.Id2);

            // --- Assert
            back1.ShouldNotBeNull();
            back2.ShouldBeNull();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task DeleteAsyncFailsInReadOnlyMode()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN, SqlOperationMode.ReadOnly);

            // --- Act
            await db.DeleteAsync(new SampleRecord { Id = 1, Name = "First" });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task DeleteByIdAsyncFailsInReadOnlyMode()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN, SqlOperationMode.ReadOnly);

            // --- Act
            await db.DeleteByIdAsync<SampleRecord>(1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task DeleteByIdAsyncFailsWithNoPkValues()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);

            // --- Act
            // ReSharper disable RedundantCast
            await db.DeleteByIdAsync<SampleRecord>((IEnumerable<object>)null);
            // ReSharper restore RedundantCast
        }

        [TestMethod]
        public async Task DeleteByIdAsyncWorksWithSimpleRecord()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.InsertAsync(new SampleRecord { Id = 1, Name = "First" });
            await db.InsertAsync(new SampleRecord { Id = 2, Name = "First" });

            // --- Act
            var rows1 = await db.FetchAsync<SampleRecord>(@"order by ""Id""");
            var del1 = await db.DeleteByIdAsync<SampleRecord>(1);
            var rows2 = await db.FetchAsync<SampleRecord>(@"order by ""Id""");
            var del2 = await db.DeleteByIdAsync<SampleRecord>(2);
            var rows3 = await db.FetchAsync<SampleRecord>(@"order by ""Id""");

            // --- Assert
            rows1.ShouldHaveCountOf(2);
            del1.ShouldBeTrue();
            rows2.ShouldHaveCountOf(1);
            del2.ShouldBeTrue();
            rows3.ShouldHaveCountOf(0);
        }

        [TestMethod]
        [ExpectedException(typeof(TaskCanceledException))]
        public async Task DeleteByIdAsyncWorksWithCancellation()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.InsertAsync(new SampleRecord { Id = 1, Name = "First" });
            var cts = new CancellationTokenSource();

            // --- Act
            cts.Cancel();
            await db.DeleteByIdAsync<SampleRecord>(cts.Token, 1);
        }

        [TestMethod]
        public async Task DeleteByIdAsyncWorksWithMultiplePk()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id1"" int not null, ""Id2"" int not null, ""Name"" varchar(50), ""Description"" varchar(50))");
            db.CompleteTransaction();
            await db.InsertAsync(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 1, Name = "First" });
            await db.InsertAsync(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "Second" });

            // --- Act
            var rows1 = await db.FetchAsync<SampleRecordWithMultiplePk>();
            var del1 = await db.DeleteByIdAsync<SampleRecordWithMultiplePk>(1, 1);
            var rows2 = await db.FetchAsync<SampleRecordWithMultiplePk>();
            var del2 = await db.DeleteByIdAsync<SampleRecordWithMultiplePk>(1, 2);
            var rows3 = await db.FetchAsync<SampleRecordWithMultiplePk>();

            // --- Assert
            rows1.ShouldHaveCountOf(2);
            del1.ShouldBeTrue();
            rows2.ShouldHaveCountOf(1);
            del2.ShouldBeTrue();
            rows3.ShouldHaveCountOf(0);
        }

        [TableName("sample")]
        class SampleRecord : DataRecord<SampleRecord>
        {
            private int _id;
            private string _name;

            [PrimaryKey]
            public int Id
            {
                // ReSharper disable once UnusedMember.Local
                get { return _id; }
                set { _id = Modify(value, "Id"); }
            }

            public string Name
            {
                // ReSharper disable once UnusedMember.Local
                get { return _name; }
                set { _name = Modify(value, "Name"); }
            }
        }

        [TableName("sample")]
        class SampleRecordWithNoPk : DataRecord<SampleRecordWithNoPk>
        {
            // ReSharper disable UnusedMember.Local
            public int Id { get; set; }
            // ReSharper restore UnusedMember.Local
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public string Name { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }

        [TableName("sample")]
        class SimplePoco
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public int Id { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public string Name { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }

        [TableName("sample")]
        class SampleRecordWithMultiplePk : DataRecord<SampleRecordWithMultiplePk>
        {
            private int _id1;
            private int _id2;
            private string _name;
            private string _description;

            [PrimaryKey]
            public int Id1
            {
                get { return _id1; }
                set { _id1 = Modify(value, "Id1"); }
            }

            [PrimaryKey(1)]
            public int Id2
            {
                get { return _id2; }
                set { _id2 = Modify(value, "Id2"); }
            }

            public string Name
            {
                // ReSharper disable UnusedMember.Local
                get { return _name; }
                // ReSharper restore UnusedMember.Local
                set { _name = Modify(value, "Name"); }
            }

            // ReSharper disable UnusedMember.Local
            public string Description
            // ReSharper restore UnusedMember.Local
            {
                get { return _description; }
                set { _description = Modify(value, "Description"); }
            }
        }
    }
}
