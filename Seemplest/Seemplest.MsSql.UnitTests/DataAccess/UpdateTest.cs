using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Common;
using Seemplest.Core.DataAccess;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.MsSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.MsSql.UnitTests.DataAccess
{
    [TestClass]
    public class UpdateTest
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
        public void UpdateWithNullRaisesException()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");

            // --- Act
            db.Update<SampleRecord>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UpdateWithNoPkRaisesException()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");
            var record = new SampleRecordWithNoPk { Name = "First" };

            // --- Act
            db.Update(record);
        }

        [TestMethod]
        public void SimpleUpdateWorksAsExpected()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");
            var record = new SampleRecord { Name = "First" };
            db.Insert(record);

            // --- Act
            record.Name = "NewFirst";
            db.Update(record);

            // --- Assert
            var back = db.FirstOrDefault<SampleRecord>("where Id = @0", record.Id);
            back.ShouldNotBeNull();
            back.Id.ShouldEqual(record.Id);
            back.Name.ShouldEqual("NewFirst");
        }

        [TestMethod]
        public void SimpleUpdateWorksWithMultiplePk()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id1 int not null, Id2 int not null, Name varchar(50) null, Description varchar(50) null)");
            var record = new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "First" };
            db.Insert(record);

            // --- Act
            record.Name = "NewFirst";
            db.Update(record);

            // --- Assert
            var back = db.FirstOrDefault<SampleRecordWithMultiplePk>("where Id1 = @0 and Id2 = @1", record.Id1, record.Id2);
            back.ShouldNotBeNull();
            back.Id1.ShouldEqual(record.Id1);
            back.Id2.ShouldEqual(record.Id2);
            back.Name.ShouldEqual("NewFirst");
        }

        [TestMethod]
        public void SimpleUpdateWorksWithVersion()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null, Version rowversion not null)");
            var record = new SampleRecordWithVersion { Id = 1, Name = "First" };
            db.Insert(record);

            // --- Act
            record.Name = "NewFirst";
            db.Update(record);

            // --- Assert
            var back = db.FirstOrDefault<SampleRecordWithVersion>("where Id = @0", record.Id);
            back.ShouldNotBeNull();
            back.Id.ShouldEqual(record.Id);
            back.Name.ShouldEqual("NewFirst");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SimplePocoRaisesExceptionWithUpdate()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");
            var record = new SimplePoco { Id = 1, Name = "First" };
            db.Insert(record);

            // --- Act
            record.Name = "NewFirst";
            db.Update(record);
        }

        [TestMethod]
        [ExpectedException(typeof(DBConcurrencyException))]
        public void SimpleUpdateWorksWithModifiedVersion1()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null, Version rowversion not null)");
            var record = new SampleRecordWithVersion { Id = 1, Name = "First" };
            db.Insert(record);

            // --- Act
            record.Name = "NewFirst";
            record.Version = new byte[0];
            db.Update(record);
        }

        [TestMethod]
        public void SimpleUpdateWorksWithModifiedVersion2()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN, rowVersionHandling: SqlRowVersionHandling.IgnoreConcurrencyIssues);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null, Version rowversion not null)");
            var record = new SampleRecordWithVersion { Id = 1, Name = "First" };
            db.Insert(record);

            // --- Act
            record.Name = "NewFirst";
            record.Version = new byte[0];
            db.Update(record);
            var backRecord = db.FirstOrDefault<SampleRecordWithVersion>("where Id=@0", 1);

            // --- Assert
            backRecord.ShouldNotBeNull();
            backRecord.Name.ShouldEqual("First");
        }

        [TestMethod]
        public void SimpleUpdateWorksWithModifiedVersion3()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN, rowVersionHandling: SqlRowVersionHandling.DoNotUseVersions);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null, Version rowversion not null)");
            var record = new SampleRecordWithVersion { Id = 1, Name = "First" };
            db.Insert(record);

            // --- Act
            record.Name = "NewFirst";
            record.Version = new byte[0];
            db.Update(record);
            var backRecord = db.FirstOrDefault<SampleRecordWithVersion>("where Id=@0", 1);

            // --- Assert
            backRecord.ShouldNotBeNull();
            backRecord.Name.ShouldEqual("NewFirst");
        }

        [TestMethod]
        [ExpectedException(typeof(DBConcurrencyException))]
        public void UpdateConcurrencyIsCaught()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null, Version rowversion not null)");
            var record1 = new SampleRecordWithVersion { Id = 1, Name = "First" };
            db.Insert(record1);
            var record2 = db.FirstOrDefault<SampleRecordWithVersion>("where Id = @0", 1);

            // --- Act
            record2.Name = "Through Record2";
            db.Update(record2);
            record1.Name = "Through Record1";
            db.Update(record1);
        }

        [TestMethod]
        public void SimpleUpdateWorksWithMultipleModifications()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id1 int not null, Id2 int not null, Name varchar(50) null, Description varchar(50) null)");
            var record = new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "First" };
            db.Insert(record);

            // --- Act
            record.Name = "NewFirst";
            record.Description = "Description";
            db.Update(record);

            // --- Assert
            var back = db.FirstOrDefault<SampleRecordWithMultiplePk>("where Id1 = @0 and Id2 = @1", record.Id1, record.Id2);
            back.ShouldNotBeNull();
            back.Id1.ShouldEqual(record.Id1);
            back.Id2.ShouldEqual(record.Id2);
            back.Name.ShouldEqual("NewFirst");
            back.Description.ShouldEqual("Description");
        }

        [TestMethod]
        public void UpdateWithGetPreviousWorksAsExpected()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null, Version rowversion not null)");
            var record = new SampleRecordWithVersion { Id = 1, Name = "First" };
            db.Insert(record);

            // --- Act
            record.Name = "NewFirst";
            var prevRecord = db.UpdateAndGetPrevious(record);

            // --- Assert
            var current = db.FirstOrDefault<SampleRecordWithVersion>("where Id = @0", record.Id);
            current.ShouldNotBeNull();
            prevRecord.Id.ShouldEqual(record.Id);
            prevRecord.Name.ShouldEqual("First");
            current.Name.ShouldEqual("NewFirst");
            TypeConversionHelper.ByteArrayToString(prevRecord.Version)
                .ShouldNotEqual(TypeConversionHelper.ByteArrayToString(current.Version));
        }

        [TestMethod]
        public void UpdateWithGetCurrentWorksAsExpected()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null, Version rowversion not null)");
            var record = new SampleRecordWithVersion { Id = 1, Name = "First" };
            db.Insert(record);

            // --- Act
            record.Name = "NewFirst";
            var currentRecord = db.UpdateAndGetCurrent(record);

            // --- Assert
            var back = db.FirstOrDefault<SampleRecordWithVersion>("where Id = @0", record.Id);
            back.ShouldNotBeNull();
            currentRecord.Id.ShouldEqual(record.Id);
            currentRecord.Name.ShouldEqual("NewFirst");
            back.Name.ShouldEqual("NewFirst");
            TypeConversionHelper.ByteArrayToString(currentRecord.Version)
                .ShouldEqual(TypeConversionHelper.ByteArrayToString(back.Version));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UpdateFailsInReadOnlyMode()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN, SqlOperationMode.ReadOnly);

            // --- Act
            db.Update(new SampleRecord { Id = 1, Name = "First" });
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
                // ReSharper disable UnusedMember.Local
                set { _id = Modify(value, "Id"); }
                // ReSharper restore UnusedMember.Local
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
                set { _version = Modify(value, "Version"); }
            }
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
