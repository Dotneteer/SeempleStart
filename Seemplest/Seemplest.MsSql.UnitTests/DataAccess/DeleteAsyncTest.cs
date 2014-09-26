using System;
using System.Collections.Generic;
using System.Threading;
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
    public class DeleteAyncTest
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
        public async Task DeleteAsyncWithNullRaisesException()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");

            // --- Act
            await db.DeleteAsync<SampleRecord>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task DeleteAsyncWithNoPkRaisesException()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            var record = new SampleRecordWithNoPk { Name = "First" };

            // --- Act
            await db.DeleteAsync(record);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SimplePocoRaisesExceptionWithDeleteAsync()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            var record = new SimplePoco { Id = 1, Name = "First" };
            await db.InsertAsync(record);

            // --- Act
            await db.DeleteAsync(record);
        }

        [TestMethod]
        public async Task DeleteAsyncWorksWithSimpleRecord()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.InsertAsync(new SampleRecord { Id = 1, Name = "First" });
            await db.InsertAsync(new SampleRecord { Id = 2, Name = "First" });

            // --- Act
            var rows1 = await db.FetchAsync<SampleRecord>("order by Id");
            await db.DeleteAsync(rows1[0]);
            var rows2 = await db.FetchAsync<SampleRecord>("order by Id");
            await db.DeleteAsync(rows2[0]);
            var rows3 = await db.FetchAsync<SampleRecord>("order by Id");

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
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.InsertAsync(new SampleRecord { Id = 1, Name = "First" });
            var cts = new CancellationTokenSource();

            // --- Act
            var rows1 = await db.FetchAsync<SampleRecord>(cts.Token, "order by Id");
            cts.Cancel();
            await db.DeleteAsync(rows1[0], cts.Token);
        }

        [TestMethod]
        public async Task DeleteAsyncWorksWithMultiplePk()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id1 int not null, Id2 int not null, Name varchar(50) null, Description varchar(50) null)");
            var record = new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "First" };
            await db.InsertAsync(record);

            // --- Act
            var back1 = await db.FirstOrDefaultAsync<SampleRecordWithMultiplePk>("where Id1 = @0 and Id2 = @1", record.Id1, record.Id2);
            await db.DeleteAsync(record);
            var back2 = await db.FirstOrDefaultAsync<SampleRecordWithMultiplePk>("where Id1 = @0 and Id2 = @1", record.Id1, record.Id2);

            // --- Assert
            back1.ShouldNotBeNull();
            back2.ShouldBeNull();
        }

        [TestMethod]
        public async Task DeleteAndGetPreviousAsyncWorks()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            var record1 = new SampleRecord { Id = 1, Name = "First" };
            var record2 = new SampleRecord { Id = 2, Name = "Second" };
            await db.InsertAsync(record1);
            await db.InsertAsync(record2);

            // --- Act
            var prev1 = await db.DeleteAndGetPreviousAsync(record1);
            var prev2 = await db.DeleteAndGetPreviousAsync(record2);
            var prev3 = await db.DeleteAndGetPreviousAsync(record1);

            // --- Assert
            prev1.ShouldNotBeNull();
            prev1.Id.ShouldEqual(1);
            prev1.Name.ShouldEqual("First");
            prev2.ShouldNotBeNull();
            prev2.Id.ShouldEqual(2);
            prev2.Name.ShouldEqual("Second");
            prev3.ShouldBeNull();
        }

        [TestMethod]
        [ExpectedException(typeof(TaskCanceledException))]
        public async Task DeleteAndGetPreviousAsyncWorksWithCancellation()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            var record1 = new SampleRecord { Id = 1, Name = "First" };
            await db.InsertAsync(record1);
            var cts = new CancellationTokenSource();

            // --- Act
            cts.Cancel();
            await db.DeleteAndGetPreviousAsync(record1, cts.Token);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task DeleteAsyncFailsInReadOnlyMode()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN, SqlOperationMode.ReadOnly);

            // --- Act
            await db.DeleteAsync(new SampleRecord { Id = 1, Name = "First" });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task DeleteByIdAsyncFailsInReadOnlyMode()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN, SqlOperationMode.ReadOnly);

            // --- Act
            await db.DeleteByIdAsync<SampleRecord>(1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task DeleteByIdAsyncFailsWithNoPkValues()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            // ReSharper disable RedundantCast
            await db.DeleteByIdAsync<SampleRecord>((IEnumerable<object>)null);
            // ReSharper restore RedundantCast
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task DeleteByIdAndGetPreviousAsyncFailsInReadOnlyMode()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN, SqlOperationMode.ReadOnly);

            // --- Act
            await db.DeleteByIdAndGetPreviousAsync<SampleRecord>(1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task DeleteByIdAndGetPreviousAsyncFailsWithNoPkValues()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            // ReSharper disable RedundantCast
            await db.DeleteByIdAndGetPreviousAsync<SampleRecord>((IEnumerable<object>)null);
            // ReSharper restore RedundantCast
        }

        [TestMethod]
        public async Task DeleteByIdAsyncWorksWithSimpleRecord()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.InsertAsync(new SampleRecord { Id = 1, Name = "First" });
            await db.InsertAsync(new SampleRecord { Id = 2, Name = "First" });

            // --- Act
            var rows1 = await db.FetchAsync<SampleRecord>("order by Id");
            var del1 = await db.DeleteByIdAsync<SampleRecord>(1);
            var rows2 = await db.FetchAsync<SampleRecord>("order by Id");
            var del2 = await db.DeleteByIdAsync<SampleRecord>(2);
            var rows3 = await db.FetchAsync<SampleRecord>("order by Id");

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
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
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
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id1 int not null, Id2 int not null, Name varchar(50) null, Description varchar(50))");
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

        [TestMethod]
        public async Task DeleteByIdAndGetPreviousAsyncWorks()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id int not null, Name varchar(50) null)");
            await db.InsertAsync(new SampleRecord { Id = 1, Name = "First" });
            await db.InsertAsync(new SampleRecord { Id = 2, Name = "Second" });

            // --- Act
            var prev1 = await db.DeleteByIdAndGetPreviousAsync<SampleRecord>(1);
            var prev2 = await db.DeleteByIdAndGetPreviousAsync<SampleRecord>(2);
            var prev3 = await db.DeleteByIdAndGetPreviousAsync<SampleRecord>(1);

            // --- Assert
            prev1.ShouldNotBeNull();
            prev1.Id.ShouldEqual(1);
            prev1.Name.ShouldEqual("First");
            prev2.ShouldNotBeNull();
            prev2.Id.ShouldEqual(2);
            prev2.Name.ShouldEqual("Second");
            prev3.ShouldBeNull();
        }

        [TestMethod]
        public async Task DeleteByIdAndGetPreviousAsyncWorksWithMultiplePk()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id1 int not null, Id2 int not null, Name varchar(50) null, Description varchar(50))");
            await db.InsertAsync(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 1, Name = "First" });
            await db.InsertAsync(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "Second" });

            // --- Act
            var rows1 = await db.FetchAsync<SampleRecordWithMultiplePk>();
            var del1 = await db.DeleteByIdAndGetPreviousAsync<SampleRecordWithMultiplePk>(1, 1);
            var rows2 = await db.FetchAsync<SampleRecordWithMultiplePk>();
            var del2 = await db.DeleteByIdAndGetPreviousAsync<SampleRecordWithMultiplePk>(1, 2);
            var rows3 = await db.FetchAsync<SampleRecordWithMultiplePk>();
            var del3 = await db.DeleteByIdAndGetPreviousAsync<SampleRecordWithMultiplePk>(1, 3);

            // --- Assert
            rows1.ShouldHaveCountOf(2);
            del1.ShouldNotBeNull();
            rows2.ShouldHaveCountOf(1);
            del2.ShouldNotBeNull();
            rows3.ShouldHaveCountOf(0);
            del3.ShouldBeNull();
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
