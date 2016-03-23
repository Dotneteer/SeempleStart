using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class FetchAsyncTest
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
        public void Dummy()
        {
        }


        [TestMethod]
        [ExpectedException(typeof(FbException))]
        public async Task InvalidAsyncQueryRaisesException()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);

            // --- Act
            await db.FetchAsync<SampleRecord>("this is a dummy SQL");
        }

        [TestMethod]
        public async Task SimpleFetchAsyncWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values(1, 'First')");
            await db.ExecuteAsync(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var rows = await db.FetchAsync<SampleRecord>();

            // --- Assert
            rows.ShouldHaveCountOf(2);
        }

        [TestMethod]
        public async Task FetchAsyncWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values(1, 'First')");
            await db.ExecuteAsync(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var rows = await db.FetchAsync<SampleRecord>(@"order by ""Id""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual(1);
            rows[0].Name.ShouldEqual("First");
            rows[1].Id.ShouldEqual(2);
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        public async Task QueryAsyncWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values(1, 'First')");
            await db.ExecuteAsync(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var rows = (await db.QueryAsync<SampleRecord>(@"select ""Id"", ""Name"" from ""sample"" order by ""Id""")).ToList();

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual(1);
            rows[0].Name.ShouldEqual("First");
            rows[1].Id.ShouldEqual(2);
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        public async Task FetchAsyncManagesNullValues()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values(1, null)");
            await db.ExecuteAsync(@"insert into ""sample"" values(2, null)");
            
            // --- Act
            var rows = await db.FetchAsync<SampleRecord>(@"order by ""Id""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual(1);
            rows[0].Name.ShouldBeNull();
            rows[1].Id.ShouldEqual(2);
            rows[1].Name.ShouldBeNull();
        }

        [TestMethod]
        public async Task FetchAsyncWorksWithMissingClrField()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values(1, 'First')");
            await db.ExecuteAsync(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var rows = await db.FetchAsync<SampleRecordWithMissingField>(
                @"select ""Id"", ""Name"" from ""sample"" order by ""Name""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Name.ShouldEqual("First");
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        public async Task FetchAsyncWorksWithMissingDataField()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values(1, 'First')");
            await db.ExecuteAsync(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var rows = await db.FetchAsync<SampleRecordWithExtraField>(
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
        public async Task FetchWorksWithNarrowingConversion()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values(1, 'First')");
            await db.ExecuteAsync(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var rows = await db.FetchAsync<SampleRecordWithNarrowingConversion>(
                @"select ""Id"", ""Name"" from ""sample"" order by ""Id""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual((short)1);
            rows[0].Name.ShouldEqual("First");
            rows[1].Id.ShouldEqual((short)2);
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        public async Task FetchAsyncWorksWithWideningConversion()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values(1, 'First')");
            await db.ExecuteAsync(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var rows = await db.FetchAsync<SampleRecordWithWideningConversion>(
                @"select ""Id"", ""Name"" from ""sample"" order by ""Id""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual(1L);
            rows[0].Name.ShouldEqual("First");
            rows[1].Id.ShouldEqual(2L);
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        public async Task FetchAsyncWorksWithTypeConversion()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values(1, 'First')");
            await db.ExecuteAsync(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var rows = await db.FetchAsync<SampleRecordWithTypeConversion>(
                @"select ""Id"", ""Name"" from ""sample"" order by ""Id""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual("1");
            rows[0].Name.ShouldEqual("First");
            rows[1].Id.ShouldEqual("2");
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        public async Task FetchAsyncWorksWithIntegerBasedEnumValues()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values(1, 'First')");
            await db.ExecuteAsync(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var rows = await db.FetchAsync<SampleRecordWithEnum>(@"order by ""Id""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual(FbDbType.BigInt);
            rows[0].Name.ShouldEqual("First");
            rows[1].Id.ShouldEqual(FbDbType.Binary);
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        public async Task FetchAsyncWorksWithNonIntegerBasedEnumValues()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values(1, 'First')");
            await db.ExecuteAsync(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var rows = await db.FetchAsync<SampleRecordWithEnum>(@"order by ""Id""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual(FbDbType.BigInt);
            rows[0].Name.ShouldEqual("First");
            rows[1].Id.ShouldEqual(FbDbType.Binary);
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        public async Task FetchAsyncWorksWithEnumStringValues()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" varchar(20) not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values('BigInt', 'First')");
            await db.ExecuteAsync(@"insert into ""sample"" values('Binary', 'Second')");

            // --- Act
            var rows = await db.FetchAsync<SampleRecordWithEnum>(@"order by ""Id""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Id.ShouldEqual(FbDbType.BigInt);
            rows[0].Name.ShouldEqual("First");
            rows[1].Id.ShouldEqual(FbDbType.Binary);
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public async Task InvalidMappingRaisesAnException()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" blob not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values(1, 'First')");
            await db.ExecuteAsync(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            await db.FetchAsync<SampleRecordWithEnum>(@"order by ""Id""");
        }

        [TestMethod]
        public async Task SourceConverterWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values(1, '1, 2, 3, 4')");
            await db.ExecuteAsync(@"insert into ""sample"" values(2, '5, 6')");

            // --- Act
            var rows = await db.FetchAsync<SampleRecordWithList>(@"order by ""Id""");

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
        public async Task UnusedSourceConverterIsSkipped()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values(1, 'First')");
            await db.ExecuteAsync(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var rows = await db.FetchAsync<SampleRecordUnusedConverter>(@"order by ""Id""");

            // --- Assert
            rows.ShouldHaveCountOf(2);
            rows[0].Name.ShouldEqual("First");
            rows[1].Name.ShouldEqual("Second");
        }

        [TestMethod]
        public async Task FirstAsyncWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values(1, 'First')");
            await db.ExecuteAsync(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var row1 = await db.FirstAsync<SampleRecord>(@"order by ""Id""");
            var row2 = await db.FirstAsync<SampleRecord>(SqlExpression.New
                .Select<SampleRecord>().From<SampleRecord>().OrderBy<SampleRecord>(r => r.Id));

            // --- Assert
            row1.Id.ShouldEqual(1);
            row1.Name.ShouldEqual("First");
            row2.Id.ShouldEqual(1);
            row2.Name.ShouldEqual("First");
        }

        [TestMethod]
        public async Task FirstAsyncIntoWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values(1, 'First')");
            await db.ExecuteAsync(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var instance = new SampleRecord { Id = 3 };
            var row1 = await db.FirstIntoAsync(instance, 
                @"select ""Name"" from ""sample"" order by ""Id""");
            var row2 = await db.FirstIntoAsync(instance, SqlExpression.CreateFrom(
                @"select ""Name"" from ""sample"" order by ""Id"""));

            // --- Assert
            row1.Id.ShouldEqual(3);
            row1.Name.ShouldEqual("First");
            row2.Id.ShouldEqual(3);
            row2.Name.ShouldEqual("First");
        }

        [TestMethod]
        public async Task FirstOrDefaultAsyncWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values(1, 'First')");
            await db.ExecuteAsync(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var row1 = await db.FirstOrDefaultAsync<SampleRecord>(@"order by ""Id""");
            var row2 = await db.FirstOrDefaultAsync<SampleRecord>(SqlExpression.CreateFrom(
                @"select * from ""sample"" order by ""Id"""));
            var row3 = await db.FirstOrDefaultAsync<SampleRecord>(@"where ""Id"" = 3");
            var row4 = await db.FirstOrDefaultAsync<SampleRecord>(SqlExpression.CreateFrom(
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
        public async Task FirstOrDefaultAsyncIntoWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values(1, 'First')");
            await db.ExecuteAsync(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var instance = new SampleRecord { Id = 3 };
            var row1 = await db.FirstOrDefaultIntoAsync(instance, 
                @"select ""Name"" from ""sample"" order by ""Id""");
            var row2 = await db.FirstOrDefaultIntoAsync(instance, 
                SqlExpression.CreateFrom(@"select ""Name"" from ""sample"" order by ""Id"""));
            var row3 = await db.FirstOrDefaultIntoAsync(instance, 
                @"select ""Name"" from ""sample"" where ""Id"" = 3");
            var row4 = await db.FirstOrDefaultIntoAsync(instance, 
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
        public async Task SingleAsyncWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values(1, 'First')");
            await db.ExecuteAsync(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var row1 = await db.SingleAsync<SampleRecord>(@"where ""Id"" = 2");
            var row2 = await db.SingleAsync<SampleRecord>(new SqlExpression(
                @"select ""Id"", ""Name"" from ""sample"" where ""Id"" = 2"));

            // --- Assert
            row1.Id.ShouldEqual(2);
            row1.Name.ShouldEqual("Second");
            row2.Id.ShouldEqual(2);
            row2.Name.ShouldEqual("Second");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SingleAsyncRaisesExceptionWithZeroRecord()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values(1, 'First')");
            await db.ExecuteAsync(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            await db.SingleAsync<SampleRecord>(@"where ""Id"" = 3");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SingleAsyncRaisesExceptionWithMoreThanOneRecord()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values(1, 'First')");
            await db.ExecuteAsync(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            await db.SingleAsync<SampleRecord>(@"where ""Id"" < 10");
        }

        [TestMethod]
        public async Task SingleIntoAsyncWorksAsExpected()
        {
            // --- Arrange
            var db = new FbDatabase(DB_CONN);
            await db.BeginTransactionAsync();
            await db.ExecuteAsync(@"create table ""sample"" (""Id"" int not null, ""Name"" varchar(50))");
            await db.CompleteTransactionAsync();
            await db.ExecuteAsync(@"insert into ""sample"" values(1, 'First')");
            await db.ExecuteAsync(@"insert into ""sample"" values(2, 'Second')");

            // --- Act
            var instance = new SampleRecord { Id = 3 };
            var row1 = await db.SingleIntoAsync(instance, 
                @"select ""Name"" from ""sample"" where ""Id"" = 1");
            var row2 = await db.SingleIntoAsync(instance, SqlExpression.CreateFrom(
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
