using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.Core.DataAccess.Mapping;
using Seemplest.FbSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.FbSql.UnitTests.DataAccess
{
    [TestClass]
    public class TargetConverterTest
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
        public void TargetConverterWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            var record = new SampleRecordWithList { Id = 1, Name = new List<string> { "1", "2", "3", "4" } };

            // --- Act
            db.Insert(record);
            record.Name = new List<string> { "5", "6" };
            db.Update(record);
            var back = db.FirstOrDefault<SampleRecordWithList>(@"where ""Id""=@0", 1);

            // --- Assert
            back.Name.ShouldHaveCountOf(2);
            back.Name[0].ShouldEqual("5");
            back.Name[1].ShouldEqual("6");
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void NullConverterRaisesException()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            var record = new SampleRecordWithNullConverter { Id = 1, Name = new List<string> { "1", "2", "3", "4" } };

            // --- Act
            db.Insert(record);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void InvalidConverterRaisesException()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            var record = new SampleRecordWithInvalidConverter { Id = 1, Name = new List<string> { "1", "2", "3", "4" } };

            // --- Act
            db.Insert(record);
        }

        class SampleListConverter : DualConverterBase<string, List<string>>
        {
            public override List<string> ConvertFromDataType(string dataType)
            {
                return new List<string>(dataType.Split(',').Select(s => s.Trim()));
            }

            public override string ConvertToDataType(List<string> clrObject)
            {
                return string.Join(",", clrObject);
            }
        }

        [TableName("sample")]
        internal class SampleRecordWithList : DataRecord<SampleRecordWithList>
        {
            private int _id;
            private List<string> _name;

            [PrimaryKey]
            public int Id
            {
                get { return _id; }
                set { _id = Modify(value, "Id"); }
            }

            [SourceConverter(typeof(string), typeof(SampleListConverter))]
            [TargetConverter(typeof(SampleListConverter))]
            public List<string> Name
            {
                get { return _name; }
                set { _name = Modify(value, "Name"); }
            }
        }

        [TableName("sample")]
        internal class SampleRecordWithNullConverter : DataRecord<SampleRecordWithNullConverter>
        {
            private int _id;
            private List<string> _name;

            [PrimaryKey]
            public int Id
            {
                get { return _id; }
                set { _id = Modify(value, "Id"); }
            }

            [SourceConverter(typeof(string), typeof(SampleListConverter))]
            [TargetConverter(null)]
            public List<string> Name
            {
                get { return _name; }
                set { _name = Modify(value, "Name"); }
            }
        }

        [TableName("sample")]
        internal class SampleRecordWithInvalidConverter : DataRecord<SampleRecordWithInvalidConverter>
        {
            private int _id;
            private List<string> _name;

            [PrimaryKey]
            public int Id
            {
                get { return _id; }
                set { _id = Modify(value, "Id"); }
            }

            [SourceConverter(typeof(string), typeof(SampleListConverter))]
            [TargetConverter(typeof(Guid))]
            public List<string> Name
            {
                get { return _name; }
                set { _name = Modify(value, "Name"); }
            }
        }
    }
}
