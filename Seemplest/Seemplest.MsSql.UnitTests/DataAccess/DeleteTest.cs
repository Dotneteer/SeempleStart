﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.MsSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.MsSql.UnitTests.DataAccess
{
    [TestClass]
    public class DeleteTest
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
        public void DeleteWithNullRaisesException()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");

            // --- Act
            db.Delete<SampleRecord>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeleteWithNoPkRaisesException()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");
            var record = new SampleRecordWithNoPk { Name = "First" };

            // --- Act
            db.Delete(record);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SimplePocoRaisesExceptionWithDelete()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");
            var record = new SimplePoco { Id = 1, Name = "First" };
            db.Insert(record);

            // --- Act
            db.Delete(record);
        }

        [TestMethod]
        public void DeleteWorksWithSimpleRecord()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");
            db.Insert(new SampleRecord { Id = 1, Name = "First" });
            db.Insert(new SampleRecord { Id = 2, Name = "First" });

            // --- Act
            var rows1 = db.Fetch<SampleRecord>("order by Id");
            db.Delete(rows1[0]);
            var rows2 = db.Fetch<SampleRecord>("order by Id");
            db.Delete(rows2[0]);
            var rows3 = db.Fetch<SampleRecord>("order by Id");

            // --- Assert
            rows1.ShouldHaveCountOf(2);
            rows2.ShouldHaveCountOf(1);
            rows3.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public void DeleteWorksWithMultiplePk()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id1 int not null, Id2 int not null, Name varchar(50) null, Description varchar(50) null)");
            var record = new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "First" };
            db.Insert(record);

            // --- Act
            var back1 = db.FirstOrDefault<SampleRecordWithMultiplePk>("where Id1 = @0 and Id2 = @1", record.Id1, record.Id2);
            db.Delete(record);
            var back2 = db.FirstOrDefault<SampleRecordWithMultiplePk>("where Id1 = @0 and Id2 = @1", record.Id1, record.Id2);

            // --- Assert
            back1.ShouldNotBeNull();
            back2.ShouldBeNull();
        }

        [TestMethod]
        public void DeleteAndGetPreviousWorks()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");
            var record1 = new SampleRecord {Id = 1, Name = "First"};
            var record2 = new SampleRecord { Id = 2, Name = "Second" };
            db.Insert(record1);
            db.Insert(record2);

            // --- Act
            var prev1 = db.DeleteAndGetPrevious(record1);
            var prev2 = db.DeleteAndGetPrevious(record2);
            var prev3 = db.DeleteAndGetPrevious(record1);

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
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeleteFailsInReadOnlyMode()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN, SqlOperationMode.ReadOnly);

            // --- Act
            db.Delete(new SampleRecord { Id = 1, Name = "First" });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeleteByIdFailsInReadOnlyMode()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN, SqlOperationMode.ReadOnly);

            // --- Act
            db.DeleteById<SampleRecord>(1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeleteByIdFailsWithNoPkValues()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            // ReSharper disable RedundantCast
            db.DeleteById<SampleRecord>((IEnumerable<object>)null);
            // ReSharper restore RedundantCast
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeleteByIdAnfGetPreviousFailsInReadOnlyMode()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN, SqlOperationMode.ReadOnly);

            // --- Act
            db.DeleteByIdAndGetPrevious<SampleRecord>(1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeleteByIdAndGetPreviousFailsWithNoPkValues()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);

            // --- Act
            // ReSharper disable RedundantCast
            db.DeleteByIdAndGetPrevious<SampleRecord>((IEnumerable<object>)null);
            // ReSharper restore RedundantCast
        }

        [TestMethod]
        public void DeleteByIdWorksWithSimpleRecord()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");
            db.Insert(new SampleRecord { Id = 1, Name = "First" });
            db.Insert(new SampleRecord { Id = 2, Name = "First" });

            // --- Act
            var rows1 = db.Fetch<SampleRecord>("order by Id");
            var del1 = db.DeleteById<SampleRecord>(1);
            var rows2 = db.Fetch<SampleRecord>("order by Id");
            var del2 = db.DeleteById<SampleRecord>(2);
            var rows3 = db.Fetch<SampleRecord>("order by Id");

            // --- Assert
            rows1.ShouldHaveCountOf(2);
            del1.ShouldBeTrue();
            rows2.ShouldHaveCountOf(1);
            del2.ShouldBeTrue();
            rows3.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public void DeleteByIdWorksWithMultiplePk()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id1 int not null, Id2 int not null, Name varchar(50) null, Description varchar(50))");
            db.Insert(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 1, Name = "First" });
            db.Insert(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "Second" });

            // --- Act
            var rows1 = db.Fetch<SampleRecordWithMultiplePk>();
            var del1 = db.DeleteById<SampleRecordWithMultiplePk>(1, 1);
            var rows2 = db.Fetch<SampleRecordWithMultiplePk>();
            var del2 = db.DeleteById<SampleRecordWithMultiplePk>(1, 2);
            var rows3 = db.Fetch<SampleRecordWithMultiplePk>();

            // --- Assert
            rows1.ShouldHaveCountOf(2);
            del1.ShouldBeTrue();
            rows2.ShouldHaveCountOf(1);
            del2.ShouldBeTrue();
            rows3.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public void DeleteByIdAndGetPreviousWorks()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id int not null, Name varchar(50) null)");
            db.Insert(new SampleRecord { Id = 1, Name = "First" });
            db.Insert(new SampleRecord { Id = 2, Name = "Second" });

            // --- Act
            var prev1 = db.DeleteByIdAndGetPrevious<SampleRecord>(1);
            var prev2 = db.DeleteByIdAndGetPrevious<SampleRecord>(2);
            var prev3 = db.DeleteByIdAndGetPrevious<SampleRecord>(1);

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
        public void DeleteByIdAndGetPreviousWorksWithMultiplePk()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"create table sample(Id1 int not null, Id2 int not null, Name varchar(50) null, Description varchar(50))");
            db.Insert(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 1, Name = "First" });
            db.Insert(new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "Second" });

            // --- Act
            var rows1 = db.Fetch<SampleRecordWithMultiplePk>();
            var del1 = db.DeleteByIdAndGetPrevious<SampleRecordWithMultiplePk>(1, 1);
            var rows2 = db.Fetch<SampleRecordWithMultiplePk>();
            var del2 = db.DeleteByIdAndGetPrevious<SampleRecordWithMultiplePk>(1, 2);
            var rows3 = db.Fetch<SampleRecordWithMultiplePk>();
            var del3 = db.DeleteByIdAndGetPrevious<SampleRecordWithMultiplePk>(1, 3);

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
                set { _id = Modify(value, "Id");  }
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
