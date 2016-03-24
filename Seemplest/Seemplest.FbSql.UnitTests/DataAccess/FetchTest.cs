using System;
using System.Collections.Generic;
using System.Linq;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.Core.DataAccess.Mapping;
using Seemplest.FbSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.FbSql.UnitTests.DataAccess
{
    [TestClass]
    public class FetchTest
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
        public void NullReaderRaisesException()
        {
            // --- Act
            DataReaderMappingManager.Reset();
            DataReaderMappingManager.GetMapperFor<SampleRecord>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(FbException))]
        public void InvalidQueryRaisesException()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);

            // --- Act
            db.Fetch<SampleRecord>("this is a dummy SQL");
        }

        [TestMethod]
        public void SimpleFetchWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var rows = db.Fetch<SampleRecord>();

            // --- Assert
            rows.ShouldHaveCountOf(2);
        }

        [TestMethod]
        public void FetchWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var rows = db.Fetch<SampleRecord>(@"order by ""Id""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual(1);
            rows[0].Name.ShouldEqual("First");
            rows[1].Id.ShouldEqual(2);
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        public void QueryhWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var rows = db.Query<SampleRecord>(
                @"select ""Id"", ""Name"" from ""sample"" order by ""Id""").ToList();

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual(1);
            rows[0].Name.ShouldEqual("First");
            rows[1].Id.ShouldEqual(2);
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        public void FetchManagesNullValues()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values(1, null)");
            db.Execute(@"insert into ""sample"" values(2, null)");

            // --- Act
            var rows = db.Fetch<SampleRecord>(@"order by ""Id""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual(1);
            rows[0].Name.ShouldBeNull();
            rows[1].Id.ShouldEqual(2);
            rows[1].Name.ShouldBeNull();
        }

        [TestMethod]
        public void FetchWorksWithMissingClrField()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var rows = db.Fetch<SampleRecordWithMissingField>(
                @"select ""Id"", ""Name"" from ""sample"" order by ""Name""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Name.ShouldEqual("First");
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        public void FetchWorksWithMissingDataField()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var rows = db.Fetch<SampleRecordWithExtraField>(
                @"select ""Id"", ""Name"" from ""sample"" order by ""Id""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual(1);
            rows[0].Name.ShouldEqual("First");
            rows[0].Guid.ShouldEqual(Guid.Empty);
            rows[1].Id.ShouldEqual(2);
            rows[1].Name.ShouldEqual("Second");
            rows[1].Guid.ShouldEqual(Guid.Empty);
        }

        [TestMethod]
        public void FetchWorksWithNarrowingConversion()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var rows = db.Fetch<SampleRecordWithNarrowingConversion>(
                @"select ""Id"", ""Name"" from ""sample"" order by ""Id""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual((short)1);
            rows[0].Name.ShouldEqual("First");
            rows[1].Id.ShouldEqual((short)2);
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        public void FetchWorksWithWideningConversion()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var rows = db.Fetch<SampleRecordWithWideningConversion>(
                @"select ""Id"", ""Name"" from ""sample"" order by ""Id""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual(1L);
            rows[0].Name.ShouldEqual("First");
            rows[1].Id.ShouldEqual(2L);
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        public void FetchWorksWithTypeConversion()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var rows = db.Fetch<SampleRecordWithTypeConversion>(
                @"select ""Id"", ""Name"" from ""sample"" order by ""Id""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual("1");
            rows[0].Name.ShouldEqual("First");
            rows[1].Id.ShouldEqual("2");
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        public void FetchWorksWithIntegerBasedEnumValues()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var rows = db.Fetch<SampleRecordWithEnum>(@"order by ""Id""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual(FbDbType.BigInt);
            rows[0].Name.ShouldEqual("First");
            rows[1].Id.ShouldEqual(FbDbType.Binary);
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        public void FetchWorksWithNonIntegerBasedEnumValues()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var rows = db.Fetch<SampleRecordWithEnum>(@"order by ""Id""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual(FbDbType.BigInt);
            rows[0].Name.ShouldEqual("First");
            rows[1].Id.ShouldEqual(FbDbType.Binary);
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        public void FetchWorksWithEnumStringValues()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" varchar(20) not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values('BigInt', 'First')");
            db.Execute(@"insert into ""sample"" values('Binary', 'Second')");

            // --- Act
            var rows = db.Fetch<SampleRecordWithEnum>(@"order by ""Id""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual(FbDbType.BigInt);
            rows[0].Name.ShouldEqual("First");
            rows[1].Id.ShouldEqual(FbDbType.Binary);
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void InvalidMappingRaisesAnException()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" blob not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            db.Fetch<SampleRecordWithEnum>();
        }

        [TestMethod]
        public void SourceConverterWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values(1, '1, 2, 3, 4')");
            db.Execute(@"insert into ""sample"" values(2, '5, 6')");

            // --- Act
            var rows = db.Fetch<SampleRecordWithList>(@"order by ""Id""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual(1);
            rows[0].Name.ShouldHaveCountOf(4);
            rows[0].Name[0].ShouldEqual("1");
            rows[0].Name[1].ShouldEqual("2");
            rows[0].Name[2].ShouldEqual("3");
            rows[0].Name[3].ShouldEqual("4");
            rows[1].Id.ShouldEqual(2);
            rows[1].Name.ShouldHaveCountOf(2);
            rows[1].Name[0].ShouldEqual("5");
            rows[1].Name[1].ShouldEqual("6");
        }

        [TestMethod]
        public void UnusedSourceConverterIsSkipped()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var rows = db.Fetch<SampleRecordUnusedConverter>(@"order by ""Id""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Name.ShouldEqual("First");
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        public void FirstWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var row1 = db.First<SampleRecord>(@"order by ""Id""");
            var row2 = db.First<SampleRecord>(SqlExpression.New
                .Select<SampleRecord>().From<SampleRecord>().OrderBy<SampleRecord>(r => r.Id));

            // --- Assert
            row1.Id.ShouldEqual(1);
            row1.Name.ShouldEqual("First");
            row2.Id.ShouldEqual(1);
            row2.Name.ShouldEqual("First");
        }

        [TestMethod]
        public void FirstIntoWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var instance = new SampleRecord { Id = 3 };
            var row1 = db.FirstInto(instance, @"select ""Name"" from ""sample"" order by ""Id""");
            var row2 = db.FirstInto(instance, SqlExpression.CreateFrom(
                @"select ""Name"" from ""sample"" order by ""Id"""));

            // --- Assert
            row1.Id.ShouldEqual(3);
            row1.Name.ShouldEqual("First");
            row2.Id.ShouldEqual(3);
            row2.Name.ShouldEqual("First");
        }

        [TestMethod]
        public void FirstOrDefaultWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var row1 = db.FirstOrDefault<SampleRecord>(@"order by ""Id""");
            var row2 = db.FirstOrDefault<SampleRecord>(SqlExpression.CreateFrom(
                @"select * from ""sample"" order by ""Id"""));
            var row3 = db.FirstOrDefault<SampleRecord>(@"where ""Id"" = 3");
            var row4 = db.FirstOrDefault<SampleRecord>(SqlExpression.CreateFrom(
                @"select * from ""sample"" where ""Id"" = 3"));

            // --- Assert
            row1.Id.ShouldEqual(1);
            row1.Name.ShouldEqual("First");
            row2.Id.ShouldEqual(1);
            row2.Name.ShouldEqual("First");
            row3.ShouldBeNull();
            row4.ShouldBeNull();
        }

        [TestMethod]
        public void FirstOrDefaultIntoWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var instance = new SampleRecord { Id = 3 };
            var row1 = db.FirstOrDefaultInto(instance, 
                @"select ""Name"" from ""sample"" order by ""Id""");
            var row2 = db.FirstOrDefaultInto(instance, 
                SqlExpression.CreateFrom(@"select ""Name"" from ""sample"" order by ""Id"""));
            var row3 = db.FirstOrDefaultInto(instance, 
                @"select ""Name"" from ""sample"" where ""Id"" = 3");
            var row4 = db.FirstOrDefaultInto(instance, 
                SqlExpression.CreateFrom(@"select ""Name"" from ""sample"" where ""Id"" = 3"));

            // --- Assert
            row1.Id.ShouldEqual(3);
            row1.Name.ShouldEqual("First");
            row2.Id.ShouldEqual(3);
            row2.Name.ShouldEqual("First");
            row3.ShouldBeNull();
            row4.ShouldBeNull();
        }

        [TestMethod]
        public void SingleWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var row1 = db.Single<SampleRecord>(@"where ""Id"" = 2");
            var row2 = db.Single<SampleRecord>(new SqlExpression(
                @"select ""Id"", ""Name"" from ""sample"" where ""Id"" = 2"));

            // --- Assert
            row1.Id.ShouldEqual(2);
            row1.Name.ShouldEqual("Second");
            row2.Id.ShouldEqual(2);
            row2.Name.ShouldEqual("Second");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SingleRaisesExceptionWithZeroRecord()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            db.Single<SampleRecord>(@"where ""Id"" = 3");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SingleRaisesExceptionWithMoreThanOneRecord()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            db.Single<SampleRecord>(@"where ""Id"" < 10");
        }

        [TestMethod]
        public void SingleIntoWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            db.BeginTransaction();
            db.Execute(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            db.CompleteTransaction();
            db.Execute(@"insert into ""sample"" values(1, 'First')");
            db.Execute(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var instance = new SampleRecord { Id = 3 };
            var row1 = db.SingleInto(instance, @"select ""Name"" from ""sample"" where ""Id"" = 1");
            var row2 = db.SingleInto(instance, SqlExpression.CreateFrom(
                @"select ""Name"" from ""sample"" where ""Id"" = 1"));

            // --- Assert
            row1.Id.ShouldEqual(3);
            row1.Name.ShouldEqual("First");
            row2.Id.ShouldEqual(3);
            row2.Name.ShouldEqual("First");
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

        class SampleListConverter2 : DualConverterBase<int, List<string>>
        {
            public override List<string> ConvertFromDataType(int dataType)
            {
                return new List<string>(dataType);
            }

            public override int ConvertToDataType(List<string> clrObject)
            {
                return clrObject.Count;
            }
        }

        class FakeIntStringConverter : DualConverterBase<int, string>
        {
            public override string ConvertFromDataType(int dataType)
            {
                throw new NotImplementedException();
            }

            public override int ConvertToDataType(string clrObject)
            {
                throw new NotImplementedException();
            }
        }

        [TableName("sample")]
        internal class SampleRecord : DataRecord<SampleRecord>
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        [TableName("sample")]
        internal class SampleRecordWithMissingField : DataRecord<SampleRecordWithMissingField>
        {
            public string Name { get; set; }
        }

        [TableName("sample")]
        internal class SampleRecordWithExtraField : DataRecord<SampleRecordWithExtraField>
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Guid Guid { get; set; }
        }

        [TableName("sample")]
        internal class SampleRecordWithNarrowingConversion : DataRecord<SampleRecordWithNarrowingConversion>
        {
            public short Id { get; set; }
            public string Name { get; set; }
        }

        [TableName("sample")]
        internal class SampleRecordWithWideningConversion : DataRecord<SampleRecordWithWideningConversion>
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }

        [TableName("sample")]
        internal class SampleRecordWithTypeConversion : DataRecord<SampleRecordWithTypeConversion>
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        [TableName("sample")]
        internal class SampleRecordWithEnum : DataRecord<SampleRecordWithEnum>
        {
            public FbDbType Id { get; set; }
            public string Name { get; set; }
        }

        [TableName("sample")]
        internal class SampleRecordWithList : DataRecord<SampleRecordWithList>
        {
            public int Id { get; set; }

            [SourceConverter(typeof(int), typeof(SampleListConverter2))]
            [SourceConverter(typeof(string), typeof(SampleListConverter))]
            public List<string> Name { get; set; }
        }

        [TableName("sample")]
        internal class SampleRecordUnusedConverter : DataRecord<SampleRecordUnusedConverter>
        {
            public int Id { get; set; }

            [SourceConverter(typeof(int), typeof(FakeIntStringConverter))]
            public string Name { get; set; }
        }

    }
}
