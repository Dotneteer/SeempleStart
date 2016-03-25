using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.FbSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.FbSql.UnitTests.DataAccess
{
    [TestClass]
    public class UpdateTest
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
        public void UpdateWithNullRaisesException()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();

            // --- Act
            db.Update<SampleRecord>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UpdateWithNoPkRaisesException()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            var record = new SampleRecordWithNoPk { Name = "First" };

            // --- Act
            db.Update(record);
        }

        [TestMethod]
        public void SimpleUpdateWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            var record = new SampleRecord { Name = "First" };
            db.Insert(record);

            // --- Act
            record.Name = "NewFirst";
            db.Update(record);

            // --- Assert
            var back = db.FirstOrDefault<SampleRecord>(@"where ""Id"" = @0", record.Id);
            back.ShouldNotBeNull();
            back.Id.ShouldEqual(record.Id);
            back.Name.ShouldEqual("NewFirst");
        }

        [TestMethod]
        public void SimpleUpdateWorksWithMultiplePk()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id1"" int not null, ""Id2"" int not null, ""Name"" varchar(50), ""Description"" varchar(50))");
            db.CompleteTransaction();
            var record = new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "First" };
            db.Insert(record);

            // --- Act
            record.Name = "NewFirst";
            db.Update(record);

            // --- Assert
            var back = db.FirstOrDefault<SampleRecordWithMultiplePk>(
                @"where ""Id1"" = @0 and ""Id2"" = @1", record.Id1, record.Id2);
            back.ShouldNotBeNull();
            back.Id1.ShouldEqual(record.Id1);
            back.Id2.ShouldEqual(record.Id2);
            back.Name.ShouldEqual("NewFirst");
        }

        [TestMethod]
        public void SimpleUpdateWorksWithMultipleModifications()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id1"" int not null, ""Id2"" int not null, ""Name"" varchar(50), ""Description"" varchar(50))");
            db.CompleteTransaction();
            var record = new SampleRecordWithMultiplePk { Id1 = 1, Id2 = 2, Name = "First" };
            db.Insert(record);

            // --- Act
            record.Name = "NewFirst";
            record.Description = "Description";
            db.Update(record);

            // --- Assert
            var back = db.FirstOrDefault<SampleRecordWithMultiplePk>(
                @"where ""Id1"" = @0 and ""Id2"" = @1", record.Id1, record.Id2);
            back.ShouldNotBeNull();
            back.Id1.ShouldEqual(record.Id1);
            back.Id2.ShouldEqual(record.Id2);
            back.Name.ShouldEqual("NewFirst");
            back.Description.ShouldEqual("Description");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UpdateFailsInReadOnlyMode()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN, SqlOperationMode.ReadOnly);

            // --- Act
            db.Update(new SampleRecord { Id = 1, Name = "First" });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SimplePocoRaisesExceptionWithUpdate()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            var record = new SimplePoco { Id = 1, Name = "First" };
            db.Insert(record);

            // --- Act
            record.Name = "NewFirst";
            db.Update(record);
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
