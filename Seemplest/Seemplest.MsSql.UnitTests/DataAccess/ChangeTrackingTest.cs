using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.Core.DataAccess.Tracking;
using Seemplest.Core.Exceptions;
using Seemplest.MsSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.MsSql.UnitTests.DataAccess
{
    [TestClass]
    public class ChangeTrackingTest
    {
        const string DB_CONN = "connStr=Seemplest";

        private const string CREATE_SAMPLE_TABLE =
            @"create table sample
              (
                Id1 int not null, 
                Id2 int not null, 
                Name varchar(50) null, 
                Description varchar(50) null 
                constraint pk_sample primary key clustered(Id1, Id2)
              )";

        private const string CREATE_SAMPLE_TABLE_1 =
            @"create table sample1
              (
                Id int not null, 
                Name varchar(50) null, 
                constraint pk_sample1 primary key clustered(Id)
              )";

        [TestCleanup]
        public void Cleanup()
        {
            var db = new SqlDatabase(DB_CONN);
            db.Execute(
                @"if exists (select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'sample')
                  drop table sample");
            db.Execute(
                @"if exists (select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'sample1')
                  drop table sample1");
        }

        [TestMethod]
        public void SqlDatabaseCanBeCreatedWithTracking()
        {
            // --- Act
            // ReSharper disable ObjectCreationAsStatement
            new SqlDatabase(DB_CONN, SqlOperationMode.Tracked);
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestMethod]
        [ExpectedException(typeof(TrackingAbortedException))]
        public void TrackingCompletedFailureRaisesException()
        {
            // --- Act
            using (var db = new SqlDatabase(DB_CONN, SqlOperationMode.Tracked))
            {
                db.TrackingCompleted += (sender, e) => { e.TrackingException = new ArgumentNullException(); };
                db.SingleById<SampleRecord>(1, 2);
            }
        }


        [TestMethod]
        public void FetchIsCaughtByTracking()
        {
            // --- Arrange
            using (var prepdb = new SqlDatabase(DB_CONN))
            {
                prepdb.Execute(CREATE_SAMPLE_TABLE);
                prepdb.Insert(new SampleRecord { Id1 = 1, Id2 = 2, Name = "First" });
            }

            // --- Act
            SqlDatabaseChangeSet changeSet = null;
            using (var db = new SqlDatabase(DB_CONN, SqlOperationMode.Tracked))
            {
                db.TrackingCompleted += (sender, e) => { changeSet = e.ChangeSet; };
                db.SingleById<SampleRecord>(1, 2);
            }

            // --- Assert
            var tableChangeSet = changeSet["[dbo].[sample]"];
            var recordChangeSet = tableChangeSet[new PrimaryKeyValue(new List<object> {1, 2})];
            recordChangeSet.State.ShouldEqual(ChangedRecordState.Attached);
            recordChangeSet.IssueList.ShouldHaveCountOf(0);
            recordChangeSet.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public void MultipleFetchIsCaughtByTracking()
        {
            // --- Arrange
            using (var prepdb = new SqlDatabase(DB_CONN))
            {
                prepdb.Execute(CREATE_SAMPLE_TABLE);
                prepdb.Insert(new SampleRecord { Id1 = 1, Id2 = 2, Name = "First" });
            }

            // --- Act
            SqlDatabaseChangeSet changeSet = null;
            using (var db = new SqlDatabase(DB_CONN, SqlOperationMode.Tracked))
            {
                db.TrackingCompleted += (sender, e) => { changeSet = e.ChangeSet; };
                db.SingleById<SampleRecord>(1, 2);
                db.SingleById<SampleRecord>(1, 2);
            }

            // --- Assert
            var tableChangeSet = changeSet["[dbo].[sample]"];
            var recordChangeSet = tableChangeSet[new PrimaryKeyValue(new List<object> { 1, 2 })];
            recordChangeSet.State.ShouldEqual(ChangedRecordState.Attached);
            recordChangeSet.IssueList.ShouldHaveCountOf(0);
            recordChangeSet.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public void InsertIsCaughtByTracking()
        {
            // --- Arrange
            using (var prepdb = new SqlDatabase(DB_CONN))
            {
                prepdb.Execute(CREATE_SAMPLE_TABLE);
            }

            // --- Act
            SqlDatabaseChangeSet changeSet = null;
            using (var db = new SqlDatabase(DB_CONN, SqlOperationMode.Tracked))
            {
                db.TrackingCompleted += (sender, e) => { changeSet = e.ChangeSet; };
                db.Insert(new SampleRecord { Id1 = 1, Id2 = 2, Name = "First" });
            }

            // --- Assert
            var tableChangeSet = changeSet["[dbo].[sample]"];
            var recordChangeSet = tableChangeSet[new PrimaryKeyValue(new List<object> { 1, 2 })];
            recordChangeSet.State.ShouldEqual(ChangedRecordState.Inserted);
            recordChangeSet.IssueList.ShouldHaveCountOf(0);
            recordChangeSet.ShouldHaveCountOf(3);
            recordChangeSet["Id1"].PreviousValue.ShouldBeNull();
            recordChangeSet["Id1"].NewValue.ShouldEqual(1);
            recordChangeSet["Id2"].PreviousValue.ShouldBeNull();
            recordChangeSet["Id2"].NewValue.ShouldEqual(2);
            recordChangeSet["Name"].PreviousValue.ShouldBeNull();
            recordChangeSet["Name"].NewValue.ShouldEqual("First");
        }

        [TestMethod]
        public void UpdateAfterFetchIsCaughtByTracking()
        {
            // --- Arrange
            using (var prepdb = new SqlDatabase(DB_CONN))
            {
                prepdb.Execute(CREATE_SAMPLE_TABLE);
                prepdb.Insert(new SampleRecord { Id1 = 1, Id2 = 2, Name = "First" });
            }

            // --- Act
            SqlDatabaseChangeSet changeSet = null;
            using (var db = new SqlDatabase(DB_CONN, SqlOperationMode.Tracked))
            {
                db.TrackingCompleted += (sender, e) => { changeSet = e.ChangeSet; };
                var record = db.SingleById<SampleRecord>(1, 2);
                record.Description = "New description";
                db.Update(record);
            }

            // --- Assert
            var tableChangeSet = changeSet["[dbo].[sample]"];
            var recordChangeSet = tableChangeSet[new PrimaryKeyValue(new List<object> { 1, 2 })];
            recordChangeSet.State.ShouldEqual(ChangedRecordState.Updated);
            recordChangeSet.IssueList.ShouldHaveCountOf(0);
            recordChangeSet.ShouldHaveCountOf(1);
            recordChangeSet["Description"].PreviousValue.ShouldBeNull();
            recordChangeSet["Description"].NewValue.ShouldEqual("New description");
        }

        [TestMethod]
        public void MultipleUpdateAfterFetchIsCaughtByTracking()
        {
            // --- Arrange
            using (var prepdb = new SqlDatabase(DB_CONN))
            {
                prepdb.Execute(CREATE_SAMPLE_TABLE);
                prepdb.Insert(new SampleRecord { Id1 = 1, Id2 = 2, Name = "First" });
            }

            // --- Act
            SqlDatabaseChangeSet changeSet = null;
            using (var db = new SqlDatabase(DB_CONN, SqlOperationMode.Tracked))
            {
                db.TrackingCompleted += (sender, e) => { changeSet = e.ChangeSet; };
                var record = db.SingleById<SampleRecord>(1, 2);
                record.Description = "New description";
                db.Update(record);
                record.Name = "New name";
                db.Update(record);
            }

            // --- Assert
            var tableChangeSet = changeSet["[dbo].[sample]"];
            var recordChangeSet = tableChangeSet[new PrimaryKeyValue(new List<object> { 1, 2 })];
            recordChangeSet.State.ShouldEqual(ChangedRecordState.Updated);
            recordChangeSet.IssueList.ShouldHaveCountOf(0);
            recordChangeSet.ShouldHaveCountOf(2);
            recordChangeSet["Description"].PreviousValue.ShouldBeNull();
            recordChangeSet["Description"].NewValue.ShouldEqual("New description");
            recordChangeSet["Name"].PreviousValue.ShouldEqual("First");
            recordChangeSet["Name"].NewValue.ShouldEqual("New name");
        }

        [TestMethod]
        public void UpdateAfterInsertIsCaughtByTracking()
        {
            // --- Arrange
            using (var prepdb = new SqlDatabase(DB_CONN))
            {
                prepdb.Execute(CREATE_SAMPLE_TABLE);
            }

            // --- Act
            SqlDatabaseChangeSet changeSet = null;
            using (var db = new SqlDatabase(DB_CONN, SqlOperationMode.Tracked))
            {
                db.TrackingCompleted += (sender, e) => { changeSet = e.ChangeSet; };
                var record = new SampleRecord { Id1 = 1, Id2 = 2, Name = "First" };
                db.Insert(record);
                record.Description = "New description";
                db.Update(record);
            }

            // --- Assert
            var tableChangeSet = changeSet["[dbo].[sample]"];
            var recordChangeSet = tableChangeSet[new PrimaryKeyValue(new List<object> { 1, 2 })];
            recordChangeSet.State.ShouldEqual(ChangedRecordState.Inserted);
            recordChangeSet.IssueList.ShouldHaveCountOf(0);
            recordChangeSet.ShouldHaveCountOf(4);
            recordChangeSet["Id1"].PreviousValue.ShouldBeNull();
            recordChangeSet["Id1"].NewValue.ShouldEqual(1);
            recordChangeSet["Id2"].PreviousValue.ShouldBeNull();
            recordChangeSet["Id2"].NewValue.ShouldEqual(2);
            recordChangeSet["Name"].PreviousValue.ShouldBeNull();
            recordChangeSet["Name"].NewValue.ShouldEqual("First");
            recordChangeSet["Description"].PreviousValue.ShouldBeNull();
            recordChangeSet["Description"].NewValue.ShouldEqual("New description");
        }

        [TestMethod]
        public void MultipleUpdateAfterInsertIsCaughtByTracking()
        {
            // --- Arrange
            using (var prepdb = new SqlDatabase(DB_CONN))
            {
                prepdb.Execute(CREATE_SAMPLE_TABLE);
            }

            // --- Act
            SqlDatabaseChangeSet changeSet = null;
            using (var db = new SqlDatabase(DB_CONN, SqlOperationMode.Tracked))
            {
                db.TrackingCompleted += (sender, e) => { changeSet = e.ChangeSet; };
                var record = new SampleRecord { Id1 = 1, Id2 = 2, Name = "First" };
                db.Insert(record);
                record.Description = "New description";
                db.Update(record);
                record.Name = "New name";
                db.Update(record);
            }

            // --- Assert
            var tableChangeSet = changeSet["[dbo].[sample]"];
            var recordChangeSet = tableChangeSet[new PrimaryKeyValue(new List<object> { 1, 2 })];
            recordChangeSet.State.ShouldEqual(ChangedRecordState.Inserted);
            recordChangeSet.IssueList.ShouldHaveCountOf(0);
            recordChangeSet.ShouldHaveCountOf(4);
            recordChangeSet["Id1"].PreviousValue.ShouldBeNull();
            recordChangeSet["Id1"].NewValue.ShouldEqual(1);
            recordChangeSet["Id2"].PreviousValue.ShouldBeNull();
            recordChangeSet["Id2"].NewValue.ShouldEqual(2);
            recordChangeSet["Name"].PreviousValue.ShouldBeNull();
            recordChangeSet["Name"].NewValue.ShouldEqual("New name");
            recordChangeSet["Description"].PreviousValue.ShouldBeNull();
            recordChangeSet["Description"].NewValue.ShouldEqual("New description");
        }

        [TestMethod]
        public void DeleteAfterFetchIsCaughtByTracking()
        {
            // --- Arrange
            using (var prepdb = new SqlDatabase(DB_CONN))
            {
                prepdb.Execute(CREATE_SAMPLE_TABLE);
                prepdb.Insert(new SampleRecord { Id1 = 1, Id2 = 2, Name = "First" });
            }

            // --- Act
            SqlDatabaseChangeSet changeSet = null;
            using (var db = new SqlDatabase(DB_CONN, SqlOperationMode.Tracked))
            {
                db.TrackingCompleted += (sender, e) => { changeSet = e.ChangeSet; };
                var record = db.SingleById<SampleRecord>(1, 2);
                db.Delete(record);
            }

            // --- Assert
            var tableChangeSet = changeSet["[dbo].[sample]"];
            var recordChangeSet = tableChangeSet[new PrimaryKeyValue(new List<object> { 1, 2 })];
            recordChangeSet.State.ShouldEqual(ChangedRecordState.Deleted);
            recordChangeSet.IssueList.ShouldHaveCountOf(0);
            recordChangeSet.ShouldHaveCountOf(3);
            recordChangeSet["Id1"].PreviousValue.ShouldEqual(1);
            recordChangeSet["Id1"].NewValue.ShouldBeNull();
            recordChangeSet["Id2"].PreviousValue.ShouldEqual(2);
            recordChangeSet["Id2"].NewValue.ShouldBeNull();
            recordChangeSet["Name"].PreviousValue.ShouldEqual("First");
            recordChangeSet["Name"].NewValue.ShouldBeNull();
        }

        [TestMethod]
        public void DeleteAfterInsertIsCaughtByTracking()
        {
            // --- Arrange
            using (var prepdb = new SqlDatabase(DB_CONN))
            {
                prepdb.Execute(CREATE_SAMPLE_TABLE);
            }

            // --- Act
            SqlDatabaseChangeSet changeSet = null;
            using (var db = new SqlDatabase(DB_CONN, SqlOperationMode.Tracked))
            {
                db.TrackingCompleted += (sender, e) => { changeSet = e.ChangeSet; };
                db.Insert(new SampleRecord { Id1 = 1, Id2 = 2, Name = "First" });
                var record = db.SingleById<SampleRecord>(1, 2);
                db.Delete(record);
            }

            // --- Assert
            changeSet.ContainsKey("[dbo].[sample]").ShouldBeFalse();
        }

        [TestMethod]
        public void DeleteAfterInsertAndUpdateIsCaughtByTracking()
        {
            // --- Arrange
            using (var prepdb = new SqlDatabase(DB_CONN))
            {
                prepdb.Execute(CREATE_SAMPLE_TABLE);
            }

            // --- Act
            SqlDatabaseChangeSet changeSet = null;
            using (var db = new SqlDatabase(DB_CONN, SqlOperationMode.Tracked))
            {
                db.TrackingCompleted += (sender, e) => { changeSet = e.ChangeSet; };
                db.Insert(new SampleRecord { Id1 = 1, Id2 = 2, Name = "First" });
                var record = db.SingleById<SampleRecord>(1, 2);
                record.Name = "New name";
                record.Description = "New description";
                db.Update(record);
                db.Delete(record);
            }

            // --- Assert
            changeSet.ContainsKey("[dbo].[sample]").ShouldBeFalse();
        }

        [TestMethod]
        public void DeleteAfterUpdateIsCaughtByTracking()
        {
            // --- Arrange
            using (var prepdb = new SqlDatabase(DB_CONN))
            {
                prepdb.Execute(CREATE_SAMPLE_TABLE);
                prepdb.Insert(new SampleRecord { Id1 = 1, Id2 = 2, Name = "First" });
            }

            // --- Act
            SqlDatabaseChangeSet changeSet = null;
            using (var db = new SqlDatabase(DB_CONN, SqlOperationMode.Tracked))
            {
                db.TrackingCompleted += (sender, e) => { changeSet = e.ChangeSet; };
                var record = db.SingleById<SampleRecord>(1, 2);
                record.Name = "New name";
                record.Description = "New description";
                db.Update(record);
                db.Delete(record);
            }

            // --- Assert
            var tableChangeSet = changeSet["[dbo].[sample]"];
            var recordChangeSet = tableChangeSet[new PrimaryKeyValue(new List<object> { 1, 2 })];
            recordChangeSet.State.ShouldEqual(ChangedRecordState.Deleted);
            recordChangeSet.IssueList.ShouldHaveCountOf(0);
            recordChangeSet.ShouldHaveCountOf(4);
            recordChangeSet["Id1"].PreviousValue.ShouldEqual(1);
            recordChangeSet["Id1"].NewValue.ShouldBeNull();
            recordChangeSet["Id2"].PreviousValue.ShouldEqual(2);
            recordChangeSet["Id2"].NewValue.ShouldBeNull();
            recordChangeSet["Name"].PreviousValue.ShouldEqual("New name");
            recordChangeSet["Name"].NewValue.ShouldBeNull();
            recordChangeSet["Description"].PreviousValue.ShouldEqual("New description");
            recordChangeSet["Description"].NewValue.ShouldBeNull();
        }

        [TestMethod]
        public void ReattachingAfterInsertRaisesAnIssue()
        {
            // --- Arrange
            using (var prepdb = new SqlDatabase(DB_CONN))
            {
                prepdb.Execute(CREATE_SAMPLE_TABLE);
            }

            // --- Act
            SqlDatabaseChangeSet changeSet = null;
            using (var db = new SqlDatabase(DB_CONN, SqlOperationMode.Tracked))
            {
                db.TrackingCompleted += (sender, e) => { changeSet = e.ChangeSet; };
                db.Insert(new SampleRecord { Id1 = 1, Id2 = 2, Name = "First" });
                db.SingleById<SampleRecord>(1, 2);
            }

            // --- Assert
            var tableChangeSet = changeSet["[dbo].[sample]"];
            var recordChangeSet = tableChangeSet[new PrimaryKeyValue(new List<object> { 1, 2 })];
            recordChangeSet.State.ShouldEqual(ChangedRecordState.Inserted);
            recordChangeSet.IssueList.ShouldHaveCountOf(1);
            recordChangeSet.IssueList[0].Description.ShouldContainIgnoringCase("has already been attached");
            recordChangeSet.ShouldHaveCountOf(3);
            recordChangeSet["Id1"].PreviousValue.ShouldBeNull();
            recordChangeSet["Id1"].NewValue.ShouldEqual(1);
            recordChangeSet["Id2"].PreviousValue.ShouldBeNull();
            recordChangeSet["Id2"].NewValue.ShouldEqual(2);
            recordChangeSet["Name"].PreviousValue.ShouldBeNull();
            recordChangeSet["Name"].NewValue.ShouldEqual("First");
        }

        [TestMethod]
        public void SecondInsertRaisesAnIssue()
        {
            // --- Arrange
            using (var prepdb = new SqlDatabase(DB_CONN))
            {
                prepdb.Execute(CREATE_SAMPLE_TABLE);
            }

            // --- Act
            SqlDatabaseChangeSet changeSet = null;
            using (var db = new SqlDatabase(DB_CONN, SqlOperationMode.Tracked))
            {
                db.TrackingCompleted += (sender, e) => { changeSet = e.ChangeSet; };
                db.Insert(new SampleRecord { Id1 = 1, Id2 = 2, Name = "First" });
                db.Execute("delete from sample where Id1=1 and Id2=2");
                db.Insert(new SampleRecord { Id1 = 1, Id2 = 2, Name = "First" });
            }

            // --- Assert
            var tableChangeSet = changeSet["[dbo].[sample]"];
            var recordChangeSet = tableChangeSet[new PrimaryKeyValue(new List<object> { 1, 2 })];
            recordChangeSet.State.ShouldEqual(ChangedRecordState.Inserted);
            recordChangeSet.IssueList.ShouldHaveCountOf(1);
            recordChangeSet.IssueList[0].Description.ShouldContainIgnoringCase("has already been attached");
            recordChangeSet.ShouldHaveCountOf(3);
            recordChangeSet["Id1"].PreviousValue.ShouldBeNull();
            recordChangeSet["Id1"].NewValue.ShouldEqual(1);
            recordChangeSet["Id2"].PreviousValue.ShouldBeNull();
            recordChangeSet["Id2"].NewValue.ShouldEqual(2);
            recordChangeSet["Name"].PreviousValue.ShouldBeNull();
            recordChangeSet["Name"].NewValue.ShouldEqual("First");
        }

        [TestMethod]
        public void UpdateWithoutAttachRaisesAnIssue()
        {
            // --- Arrange
            using (var prepdb = new SqlDatabase(DB_CONN))
            {
                prepdb.Execute(CREATE_SAMPLE_TABLE);
                prepdb.Insert(new SampleRecord { Id1 = 1, Id2 = 2, Name = "First" });
            }

            // --- Act
            SqlDatabaseChangeSet changeSet = null;
            using (var db = new SqlDatabase(DB_CONN, SqlOperationMode.Tracked))
            {
                db.TrackingCompleted += (sender, e) => { changeSet = e.ChangeSet; };
                var record = new SampleRecord();
                ((IDataRecord)record).SignLoaded();
                record.Id1 = 1;
                record.Id2 = 2;
                record.Name = "First";
                db.Update(record);
            }

            // --- Assert
            var tableChangeSet = changeSet["[dbo].[sample]"];
            var recordChangeSet = tableChangeSet[new PrimaryKeyValue(new List<object> { 1, 2 })];
            recordChangeSet.State.ShouldEqual(ChangedRecordState.Updated);
            recordChangeSet.IssueList.ShouldHaveCountOf(1);
            recordChangeSet.IssueList[0].Description.ShouldContainIgnoringCase("has not been attached");
            recordChangeSet.ShouldHaveCountOf(3);
            recordChangeSet["Id1"].PreviousValue.ShouldBeNull();
            recordChangeSet["Id1"].NewValue.ShouldEqual(1);
            recordChangeSet["Id2"].PreviousValue.ShouldBeNull();
            recordChangeSet["Id2"].NewValue.ShouldEqual(2);
            recordChangeSet["Name"].PreviousValue.ShouldBeNull();
            recordChangeSet["Name"].NewValue.ShouldEqual("First");
        }

        [TestMethod]
        public void UpdateAfterDeleteRaisesAnIssue()
        {
            // --- Arrange
            using (var prepdb = new SqlDatabase(DB_CONN))
            {
                prepdb.Execute(CREATE_SAMPLE_TABLE);
                prepdb.Insert(new SampleRecord { Id1 = 1, Id2 = 2, Name = "First" });
            }

            // --- Act
            SqlDatabaseChangeSet changeSet = null;
            using (var db = new SqlDatabase(DB_CONN, SqlOperationMode.Tracked))
            {
                db.TrackingCompleted += (sender, e) => { changeSet = e.ChangeSet; };
                db.DeleteById<SampleRecord>(1, 2);
                var record = new SampleRecord();
                ((IDataRecord)record).SignLoaded();
                record.Id1 = 1;
                record.Id2 = 2;
                record.Name = "First";
                db.Update(record);
            }

            // --- Assert
            var tableChangeSet = changeSet["[dbo].[sample]"];
            var recordChangeSet = tableChangeSet[new PrimaryKeyValue(new List<object> { 1, 2 })];
            recordChangeSet.State.ShouldEqual(ChangedRecordState.Deleted);
            recordChangeSet.IssueList.ShouldHaveCountOf(2);
            recordChangeSet.IssueList[1].Description.ShouldContainIgnoringCase("has already been deleted");
            recordChangeSet.ShouldHaveCountOf(3);
            recordChangeSet["Id1"].PreviousValue.ShouldEqual(1);
            recordChangeSet["Id1"].NewValue.ShouldBeNull();
            recordChangeSet["Id2"].PreviousValue.ShouldEqual(2);
            recordChangeSet["Id2"].NewValue.ShouldBeNull();
            recordChangeSet["Name"].PreviousValue.ShouldEqual("First");
            recordChangeSet["Name"].NewValue.ShouldBeNull();
        }

        [TestMethod]
        public void PrimaryKeyValueWorksWithByteArray()
        {
            // --- Act
            var pkValueKey = new PrimaryKeyValue(new List<object>
                {
                    new byte[] {1, 2, 3}
                }).KeyString;

            // --- Assert
            pkValueKey.ShouldEqual("[0x010203]");
        }

        [TestMethod]
        public void EliminateNonChangedTablesWorksAsExpected()
        {
            // --- Arrange
            using (var prepdb = new SqlDatabase(DB_CONN))
            {
                prepdb.Execute(CREATE_SAMPLE_TABLE);
                prepdb.Execute(CREATE_SAMPLE_TABLE_1);
            }

            // --- Act
            SqlDatabaseChangeSet changeSet = null;
            using (var db = new SqlDatabase(DB_CONN, SqlOperationMode.Tracked))
            {
                db.TrackingCompleted += (sender, e) => { changeSet = e.ChangeSet; };
                db.Insert(new SampleRecord { Id1 = 1, Id2 = 2, Name = "First" });
                db.Insert(new SampleRecord1 { Id = 2, Name = "Sample1" });
                db.DeleteById<SampleRecord>(1, 2);
            }

            // --- Assert
            changeSet.ContainsKey("[dbo].[sample]").ShouldBeFalse();
            changeSet.ContainsKey("[dbo].[sample1]").ShouldBeTrue();
        }

        [TestMethod]
        public void TrackingWorksWithTransaction()
        {
            // --- Arrange
            using (var prepdb = new SqlDatabase(DB_CONN))
            {
                prepdb.Execute(CREATE_SAMPLE_TABLE);
                prepdb.Insert(new SampleRecord { Id1 = 1, Id2 = 2, Name = "First" });
            }

            // --- Act
            SqlDatabaseChangeSet changeSet = null;
            using (var db = new SqlDatabase(DB_CONN, SqlOperationMode.Tracked))
            {
                db.TrackingCompleted += (sender, e) => { changeSet = e.ChangeSet; };
                db.BeginTransaction();
                var record = db.SingleById<SampleRecord>(1, 2);
                record.Description = "New description";
                db.Update(record);
                record.Name = "New name";
                db.Update(record);
                db.CompleteTransaction();
            }

            // --- Assert
            var tableChangeSet = changeSet["[dbo].[sample]"];
            var recordChangeSet = tableChangeSet[new PrimaryKeyValue(new List<object> { 1, 2 })];
            recordChangeSet.State.ShouldEqual(ChangedRecordState.Updated);
            recordChangeSet.IssueList.ShouldHaveCountOf(0);
            recordChangeSet.ShouldHaveCountOf(2);
            recordChangeSet["Description"].PreviousValue.ShouldBeNull();
            recordChangeSet["Description"].NewValue.ShouldEqual("New description");
            recordChangeSet["Name"].PreviousValue.ShouldEqual("First");
            recordChangeSet["Name"].NewValue.ShouldEqual("New name");
        }

        [TestMethod]
        public void TrackingWorksWithNestedTransactions()
        {
            // --- Arrange
            using (var prepdb = new SqlDatabase(DB_CONN))
            {
                prepdb.Execute(CREATE_SAMPLE_TABLE);
                prepdb.Insert(new SampleRecord { Id1 = 1, Id2 = 2, Name = "First" });
            }

            // --- Act
            SqlDatabaseChangeSet changeSet = null;
            using (var db = new SqlDatabase(DB_CONN, SqlOperationMode.Tracked))
            {
                db.TrackingCompleted += (sender, e) => { changeSet = e.ChangeSet; };
                db.BeginTransaction();
                var record = db.SingleById<SampleRecord>(1, 2);
                record.Description = "New description";
                db.BeginTransaction();
                db.Update(record);
                db.CompleteTransaction();
                record.Name = "New name";
                db.BeginTransaction();
                db.Update(record);
                db.CompleteTransaction();
                db.CompleteTransaction();
            }

            // --- Assert
            var tableChangeSet = changeSet["[dbo].[sample]"];
            var recordChangeSet = tableChangeSet[new PrimaryKeyValue(new List<object> { 1, 2 })];
            recordChangeSet.State.ShouldEqual(ChangedRecordState.Updated);
            recordChangeSet.IssueList.ShouldHaveCountOf(0);
            recordChangeSet.ShouldHaveCountOf(2);
            recordChangeSet["Description"].PreviousValue.ShouldBeNull();
            recordChangeSet["Description"].NewValue.ShouldEqual("New description");
            recordChangeSet["Name"].PreviousValue.ShouldEqual("First");
            recordChangeSet["Name"].NewValue.ShouldEqual("New name");
        }

        [TestMethod]
        public void TrackingWorksWithAbortedTransaction()
        {
            // --- Arrange
            using (var prepdb = new SqlDatabase(DB_CONN))
            {
                prepdb.Execute(CREATE_SAMPLE_TABLE);
                prepdb.Insert(new SampleRecord { Id1 = 1, Id2 = 2, Name = "First" });
            }

            // --- Act
            SqlDatabaseChangeSet changeSet = null;
            using (var db = new SqlDatabase(DB_CONN, SqlOperationMode.Tracked))
            {
                db.TrackingCompleted += (sender, e) => { changeSet = e.ChangeSet; };
                db.BeginTransaction();
                var record = db.SingleById<SampleRecord>(1, 2);
                record.Description = "New description";
                db.Update(record);
                record.Name = "New name";
                db.Update(record);
                db.AbortTransaction();
            }

            // --- Assert
            changeSet.ContainsKey("[dbo].[sample]").ShouldBeFalse();
        }

        [TestMethod]
        public void TrackingWorksWithAbortedNestedTransactions()
        {
            // --- Arrange
            using (var prepdb = new SqlDatabase(DB_CONN))
            {
                prepdb.Execute(CREATE_SAMPLE_TABLE);
                prepdb.Insert(new SampleRecord { Id1 = 1, Id2 = 2, Name = "First" });
            }

            // --- Act
            SqlDatabaseChangeSet changeSet = null;
            using (var db = new SqlDatabase(DB_CONN, SqlOperationMode.Tracked))
            {
                db.TrackingCompleted += (sender, e) => { changeSet = e.ChangeSet; };
                db.BeginTransaction();
                var record = db.SingleById<SampleRecord>(1, 2);
                record.Description = "New description";
                db.BeginTransaction();
                db.Update(record);
                db.AbortTransaction();
                record.Name = "New name";
                db.BeginTransaction();
                db.Update(record);
                db.CompleteTransaction();
                db.CompleteTransaction();
            }

            // --- Assert
            changeSet.ContainsKey("[dbo].[sample]").ShouldBeFalse();
        }

        [TestMethod]
        public void TrackingWorksWithMultipleTransactions()
        {
            // --- Arrange
            using (var prepdb = new SqlDatabase(DB_CONN))
            {
                prepdb.Execute(CREATE_SAMPLE_TABLE);
                prepdb.Insert(new SampleRecord { Id1 = 1, Id2 = 2, Name = "First" });
            }

            // --- Act
            SqlDatabaseChangeSet changeSet = null;
            using (var db = new SqlDatabase(DB_CONN, SqlOperationMode.Tracked))
            {
                db.TrackingCompleted += (sender, e) => { changeSet = e.ChangeSet; };
                var record = db.SingleById<SampleRecord>(1, 2);
                record.Description = "New description";
                db.BeginTransaction();
                db.Update(record);
                db.AbortTransaction();
                record.Name = "New name";
                db.BeginTransaction();
                db.Insert(new SampleRecord { Id1 = 2, Id2 = 3, Name = "Second" });
                db.CompleteTransaction();
            }

            // --- Assert
            var tableChangeSet = changeSet["[dbo].[sample]"];
            var recordChangeSet = tableChangeSet[new PrimaryKeyValue(new List<object> { 2, 3 })];
            recordChangeSet.State.ShouldEqual(ChangedRecordState.Inserted);
            recordChangeSet.IssueList.ShouldHaveCountOf(0);
            recordChangeSet.ShouldHaveCountOf(3);
            recordChangeSet["Id1"].PreviousValue.ShouldBeNull();
            recordChangeSet["Id1"].NewValue.ShouldEqual(2);
            recordChangeSet["Id2"].PreviousValue.ShouldBeNull();
            recordChangeSet["Id2"].NewValue.ShouldEqual(3);
            recordChangeSet["Name"].PreviousValue.ShouldBeNull();
            recordChangeSet["Name"].NewValue.ShouldEqual("Second");
        }

        [TableName("sample")]
        class SampleRecord : DataRecord<SampleRecord>
        {
            private int _id1;
            private int _id2;
            private string _name;
            private string _description;

            [PrimaryKey]
            public int Id1
            {
                // ReSharper disable UnusedMember.Local
                get { return _id1; }
                // ReSharper restore UnusedMember.Local
                set { _id1 = Modify(value, "Id1"); }
            }

            [PrimaryKey(1)]
            public int Id2
            {
                // ReSharper disable UnusedMember.Local
                get { return _id2; }
                // ReSharper restore UnusedMember.Local
                set { _id2 = Modify(value, "Id2"); }
            }

            public string Name
            {
                // ReSharper disable UnusedMember.Local
                get { return _name; }
                // ReSharper restore UnusedMember.Local
                set { _name = Modify(value, "Name"); }
            }

            public string Description
            {
                // ReSharper disable UnusedMember.Local
                get { return _description; }
                // ReSharper restore UnusedMember.Local
                set { _description = Modify(value, "Description"); }
            }
        }

        [TableName("sample1")]
        class SampleRecord1 : DataRecord<SampleRecord1>
        {
            private int _id;
            private string _name;

            [PrimaryKey]
            public int Id
            {
                // ReSharper disable UnusedMember.Local
                get { return _id; }
                // ReSharper restore UnusedMember.Local
                set { _id = Modify(value, "Id"); }
            }

            public string Name
            {
                // ReSharper disable UnusedMember.Local
                get { return _name; }
                // ReSharper restore UnusedMember.Local
                set { _name = Modify(value, "Name"); }
            }
        }

    }
}
