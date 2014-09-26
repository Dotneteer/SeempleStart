using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.Core.Exceptions;
using Seemplest.MsSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.MsSql.UnitTests.DataAccess
{
    [TestClass]
    public class SingleFetchAsyncTest
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
        public async Task SingleByIdAsyncRaisesExceptionWithNullValues()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            await db.SingleByIdAsync<SampleRecordWithMultiplePk>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SingleOrDefaultByIdAsyncRaisesExceptionWithNullValues()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            await db.SingleOrDefaultByIdAsync<SampleRecordWithMultiplePk>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExistsAsyncRaisesExceptionWithNullValues()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            await db.ExistsAsync<SampleRecordWithMultiplePk>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SingleByIdAsyncRaisesExceptionWithNullElement()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            await db.SingleByIdAsync<SampleRecordWithMultiplePk>(new List<object> { 1, null });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SingleOrDefaultByIdAsyncRaisesExceptionWithNullElement()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            await db.SingleOrDefaultByIdAsync<SampleRecordWithMultiplePk>(new List<object> { 1, null });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ExistsAsyncRaisesExceptionWithNullElement()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            await db.ExistsAsync<SampleRecordWithMultiplePk>(new List<object> { 1, null });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SingleByIdAsyncRaisesExceptionWithWrongNumberOfPkElements()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            await db.SingleByIdAsync<SampleRecordWithMultiplePk>(1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SingleOrDefaultByIdAsyncRaisesExceptionWithWrongNumberOfPkElements()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            await db.SingleOrDefaultByIdAsync<SampleRecordWithMultiplePk>(1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ExistsAsyncRaisesExceptionWithWrongNumberOfPkElements()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            await db.ExistsAsync<SampleRecordWithMultiplePk>(1);
        }

        [TestMethod]
        public async Task SingleByIdAsyncWorksAsExpected()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id1 int not null, Id2 int not null, Name varchar(50) null, Description varchar(50) null)");
            await db.InsertAsync(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "First" });
            await db.InsertAsync(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 3, Name = "Second" });

            // --- Act
            var record1 = await db.SingleByIdAsync<SampleRecordWithMultiplePk>(1, 2);
            var record2 = await db.SingleByIdAsync<SampleRecordWithMultiplePk>(new List<object> { 1, 2 });

            // --- Assert
            record1.ShouldNotBeNull();
            record1.Name.ShouldEqual("First");
            record2.ShouldNotBeNull();
            record2.Name.ShouldEqual("First");
        }

        [TestMethod]
        [ExpectedException(typeof(RecordNotFoundException))]
        public async Task SingleByIdAsyncRaisesExceptionWhenNoRecordFound()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id1 int not null, Id2 int not null, Name varchar(50) null, Description varchar(50) null)");
            await db.InsertAsync(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "First" });
            await db.InsertAsync(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 3, Name = "Second" });

            // --- Act
            await db.SingleByIdAsync<SampleRecordWithMultiplePk>(2, 3);
        }

        [TestMethod]
        [ExpectedException(typeof(MultipleRecordFoundException))]
        public async Task SingleByIdAsyncRaisesExceptionWhenMultipleRecordFound()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id1 int not null, Id2 int not null, Name varchar(50) null, Description varchar(50) null)");
            await db.InsertAsync(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "First" });
            await db.InsertAsync(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "FirstA" });
            await db.InsertAsync(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 3, Name = "Second" });

            // --- Act
            await db.SingleByIdAsync<SampleRecordWithMultiplePk>(1, 2);
        }

        [TestMethod]
        public async Task SingleOrDefaultByIdAsyncWorksAsExpected()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id1 int not null, Id2 int not null, Name varchar(50) null, Description varchar(50) null)");
            await db.InsertAsync(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "First" });
            await db.InsertAsync(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 3, Name = "Second" });

            // --- Act
            var record1 = await db.SingleOrDefaultByIdAsync<SampleRecordWithMultiplePk>(1, 2);
            var record2 = await db.SingleOrDefaultByIdAsync<SampleRecordWithMultiplePk>(new List<object> { 2, 3 });

            // --- Assert
            record1.ShouldNotBeNull();
            record1.Name.ShouldEqual("First");
            record2.ShouldBeNull();
        }

        [TestMethod]
        [ExpectedException(typeof(MultipleRecordFoundException))]
        public async Task SingleOrDefaultByIdAsyncRaisesExceptionWhenMultipleRecordFound()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id1 int not null, Id2 int not null, Name varchar(50) null, Description varchar(50) null)");
            await db.InsertAsync(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "First" });
            await db.InsertAsync(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "FirstA" });
            await db.InsertAsync(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 3, Name = "Second" });

            // --- Act
            await db.SingleOrDefaultByIdAsync<SampleRecordWithMultiplePk>(1, 2);
        }

        [TestMethod]
        public async Task ExistsAsyncWorksAsExpected()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            await db.ExecuteAsync(@"create table sample(Id1 int not null, Id2 int not null, Name varchar(50) null, Description varchar(50) null)");
            await db.InsertAsync(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "First" });
            await db.InsertAsync(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "FirstA" });
            await db.InsertAsync(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 3, Name = "Second" });

            // --- Act
            var found1 = await db.ExistsAsync<SampleRecordWithMultiplePk>(1, 2);
            var found2 = await db.ExistsAsync<SampleRecordWithMultiplePk>(1, 3);
            var found3 = await db.ExistsAsync<SampleRecordWithMultiplePk>(new List<object> { 1, 2 });
            var notFound1 = await db.ExistsAsync<SampleRecordWithMultiplePk>(2, 3);
            var notFound2 = await db.ExistsAsync<SampleRecordWithMultiplePk>(new List<object> { 2, 3 });

            // --- Assert
            found1.ShouldBeTrue();
            found2.ShouldBeTrue();
            found3.ShouldBeTrue();
            notFound1.ShouldBeFalse();
            notFound2.ShouldBeFalse();
        }


        [TableName("sample")]
        internal class SampleRecordWithMultiplePk : DataRecord<SampleRecordWithMultiplePk>
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
                get { return _name; }
                set { _name = Modify(value, "Name"); }
            }

            public string Description
            {
                get { return _description; }
                set { _description = Modify(value, "Description"); }
            }
        }
    }
}
