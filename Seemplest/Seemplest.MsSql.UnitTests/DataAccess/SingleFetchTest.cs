using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.Core.Exceptions;
using Seemplest.MsSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.MsSql.UnitTests.DataAccess
{
    [TestClass]
    public class SingleFetchTest
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
        public void SingleByIdRaisesExceptionWithNullValues()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            db.SingleById<SampleRecordWithMultiplePk>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SingleOrDefaultByIdRaisesExceptionWithNullValues()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            db.SingleOrDefaultById<SampleRecordWithMultiplePk>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExistsRaisesExceptionWithNullValues()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            db.Exists<SampleRecordWithMultiplePk>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SingleByIdRaisesExceptionWithNullElement()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            db.SingleById<SampleRecordWithMultiplePk>(new List<object>{ 1, null });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SingleOrDefaultByIdRaisesExceptionWithNullElement()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            db.SingleOrDefaultById<SampleRecordWithMultiplePk>(new List<object> { 1, null });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExistsRaisesExceptionWithNullElement()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            db.Exists<SampleRecordWithMultiplePk>(new List<object> { 1, null });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SingleByIdRaisesExceptionWithWrongNumberOfPkElements()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            db.SingleById<SampleRecordWithMultiplePk>(1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SingleOrDefaultByIdRaisesExceptionWithWrongNumberOfPkElements()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            db.SingleOrDefaultById<SampleRecordWithMultiplePk>(1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExistsRaisesExceptionWithWrongNumberOfPkElements()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            db.Exists<SampleRecordWithMultiplePk>(1);
        }

        [TestMethod]
        public void SingleByIdWorksAsExpected()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id1 int not null, Id2 int not null, Name varchar(50) null, Description varchar(50) null)");
            db.Insert(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "First" });
            db.Insert(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 3, Name = "Second" });

            // --- Act
            var record1 = db.SingleById<SampleRecordWithMultiplePk>(1, 2);
            var record2 = db.SingleById<SampleRecordWithMultiplePk>(new List<object> {1, 2});

            // --- Assert
            record1.ShouldNotBeNull();
            record1.Name.ShouldEqual("First");
            record2.ShouldNotBeNull();
            record2.Name.ShouldEqual("First");
        }

        [TestMethod]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void SingleByIdRaisesExceptionWhenNoRecordFound()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id1 int not null, Id2 int not null, Name varchar(50) null, Description varchar(50) null)");
            db.Insert(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "First" });
            db.Insert(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 3, Name = "Second" });

            // --- Act
            db.SingleById<SampleRecordWithMultiplePk>(2, 3);
        }

        [TestMethod]
        [ExpectedException(typeof(MultipleRecordFoundException))]
        public void SingleByIdRaisesExceptionWhenMultipleRecordFound()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id1 int not null, Id2 int not null, Name varchar(50) null, Description varchar(50) null)");
            db.Insert(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "First" });
            db.Insert(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "FirstA" });
            db.Insert(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 3, Name = "Second" });

            // --- Act
            db.SingleById<SampleRecordWithMultiplePk>(1, 2);
        }

        [TestMethod]
        public void SingleOrDefaultByIdWorksAsExpected()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id1 int not null, Id2 int not null, Name varchar(50) null, Description varchar(50) null)");
            db.Insert(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "First" });
            db.Insert(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 3, Name = "Second" });

            // --- Act
            var record1 = db.SingleOrDefaultById<SampleRecordWithMultiplePk>(1, 2);
            var record2 = db.SingleOrDefaultById<SampleRecordWithMultiplePk>(new List<object> { 2, 3 });

            // --- Assert
            record1.ShouldNotBeNull();
            record1.Name.ShouldEqual("First");
            record2.ShouldBeNull();
        }

        [TestMethod]
        [ExpectedException(typeof(MultipleRecordFoundException))]
        public void SingleOrDefaultByIdRaisesExceptionWhenMultipleRecordFound()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id1 int not null, Id2 int not null, Name varchar(50) null, Description varchar(50) null)");
            db.Insert(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "First" });
            db.Insert(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "FirstA" });
            db.Insert(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 3, Name = "Second" });

            // --- Act
            db.SingleOrDefaultById<SampleRecordWithMultiplePk>(1, 2);
        }

        [TestMethod]
        public void ExistsWorksAsExpected()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id1 int not null, Id2 int not null, Name varchar(50) null, Description varchar(50) null)");
            db.Insert(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "First" });
            db.Insert(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "FirstA" });
            db.Insert(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 3, Name = "Second" });

            // --- Act
            var found1 = db.Exists<SampleRecordWithMultiplePk>(1, 2);
            var found2 = db.Exists<SampleRecordWithMultiplePk>(1, 3);
            var found3 = db.Exists<SampleRecordWithMultiplePk>(new List<object> {1, 2});
            var notFound1 = db.Exists<SampleRecordWithMultiplePk>(2, 3);
            var notFound2 = db.Exists<SampleRecordWithMultiplePk>(new List<object> { 2, 3 });

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
