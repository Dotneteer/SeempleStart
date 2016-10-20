using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.FbSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.FbSql.UnitTests.DataAccess
{
    [TestClass]
    public class SqlExpressionTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SelectWithNullFails()
        {
            // --- Act    
            SqlExpression.New.Select(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SelectWithNoColumnsFails()
        {
            // --- Act    
            SqlExpression.New.Select(new object[] { });
        }

        [TestMethod]
        public void CreateFromWorks()
        {
            // --- Arrange
            const string EXPR = "select @0 from x";
            const int ARG = 23;

            // --- Act
            var expression = SqlExpression.CreateFrom(EXPR, new List<object> { ARG });

            // --- Assert
            expression.SqlText.ShouldEqual(EXPR);
            expression.Arguments.ShouldHaveCountOf(1);
            expression.Arguments[0].ShouldEqual(ARG);

        }

        [TestMethod]
        public void SelectWithColumnNamesWorks()
        {
            // --- Act
            var clause = SqlExpression.New.Select("a", "b", "c");

            // --- Assert
            clause.SqlText.ShouldEqualIgnoringCase("select \"a\", \"b\", \"c\"");
            clause.Arguments.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public void SelectWithDataRecordWorks()
        {
            // --- Act
            var clause = SqlExpression.New.Select<Record1>();

            // --- Assert
            clause.SqlText.ShouldEqualIgnoringCase("select \"Id\", \"DisplayName\", \"Description\"");
        }

        [TestMethod]
        public void SelectWithExpressionsWorks()
        {
            // --- Act
            var clause = SqlExpression.New.Select<Record1>(r => r.Id, r => r.Description, r => r.Name);

            // --- Assert
            clause.SqlText.ShouldEqualIgnoringCase("select \"Id\", \"Description\", \"DisplayName\"");
        }

        [TestMethod]
        public void DistinctWorksAsExpected()
        {
            // --- Act
            var expr1 = SqlExpression.New.Select("a", "b").Distinct;
            var expr2 = new SqlExpression("select all \"c\", \"d\"").Distinct;

            // --- Assert
            expr1.SqlText.ShouldEqual("select distinct \"a\", \"b\"");
            expr2.SqlText.ShouldEqual("select distinct \"c\", \"d\"");
        }

        [TestMethod]
        public void AllWorksAsExpected()
        {
            // --- Act
            var expr1 = SqlExpression.New.Select("a", "b").All;
            var expr2 = new SqlExpression("select distinct \"c\", \"d\"").All;

            // --- Assert
            expr1.SqlText.ShouldEqual("select all \"a\", \"b\"");
            expr2.SqlText.ShouldEqual("select all \"c\", \"d\"");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromWithNullFails()
        {
            // --- Act    
            SqlExpression.New.Select("a").From(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FromWithNoTablesFails()
        {
            // --- Act    
            SqlExpression.New.Select("a").From(new object[] { });
        }

        [TestMethod]
        public void FromWithTableNamesWorks()
        {
            // --- Act
            var clause = SqlExpression.New.Select("*").From("a", "b");

            // --- Assert
            clause.SqlText.ShouldEqualIgnoringCase("select *\nfrom \"a\", \"b\"");
            clause.Arguments.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public void FromWithDataRecordWorks()
        {
            // --- Act
            var clause1 = SqlExpression.New.Select("a", "b").From<Record1>();
            var clause2 = SqlExpression.New.Select("a", "b").From<Record1, Record2>();
            var clause3 = SqlExpression.New.Select("a", "b").From<Record1, Record2, Record3>();
            var clause4 = SqlExpression.New.Select("a", "b").From<Record1, Record2, Record3, Record4>();

            // --- Assert
            clause1.SqlText.ShouldEqualIgnoringCase("select \"a\", \"b\"\nfrom \"Record1\"");
            clause2.SqlText.ShouldEqualIgnoringCase("select \"a\", \"b\"\nfrom \"Record1\", \"Record2\"");
            clause3.SqlText.ShouldEqualIgnoringCase("select \"a\", \"b\"\nfrom \"Record1\", \"Record2\", \"Record3\"");
            clause4.SqlText.ShouldEqualIgnoringCase("select \"a\", \"b\"\nfrom \"Record1\", \"Record2\", \"Record3\", \"Record4\"");
        }

        [TestMethod]
        public void WhereWorksAsExpected()
        {
            // --- Act
            var clause1 = SqlExpression.New.Select("a", "b").From("x").Where("x.a = x.b");
            var clause2 = SqlExpression.New.Select("a", "b").From("x").Where("@0 = @1", new List<object> { 1, 2 });
            var clause3 = SqlExpression.New.Select("a", "b").From("x").Where("@0 = @1", 1, 2);
            var clause4 = SqlExpression.New.Select("a", "b").From("x").Where("x.a = x.b", null);
            var clause5 = SqlExpression.New.Select("a", "b").From("x").Where(true, "@0 = @1", new List<object> { 1, 2 });
            var clause6 = SqlExpression.New.Select("a", "b").From("x").Where(true, "@0 = @1", 1, 2);
            var clause7 = SqlExpression.New.Select("a", "b").From("x").Where(false, "@0 = @1", 1, 2);
            var clause8 = SqlExpression.New.Select("a", "b").From("x").Where(false, "@0 = @1", new List<object> { 1, 2 });
            var clause9 = SqlExpression.New.Select("a", "b").From("x").Where(true, "x.a = x.b");
            var clause10 = SqlExpression.New.Select("a", "b").From("x").Where(false, "x.a = x.b", null);

            // --- Assert
            clause1.SqlText.ShouldEqual("select \"a\", \"b\"\nfrom \"x\"\nwhere x.a = x.b");
            clause2.SqlText.ShouldEqual("select \"a\", \"b\"\nfrom \"x\"\nwhere @0 = @1");
            clause2.Arguments.ShouldHaveCountOf(2);
            clause2.Arguments[0].ShouldEqual(1);
            clause2.Arguments[1].ShouldEqual(2);
            clause3.SqlText.ShouldEqual("select \"a\", \"b\"\nfrom \"x\"\nwhere @0 = @1");
            clause3.Arguments.ShouldHaveCountOf(2);
            clause3.Arguments[0].ShouldEqual(1);
            clause3.Arguments[1].ShouldEqual(2);
            clause4.SqlText.ShouldEqual("select \"a\", \"b\"\nfrom \"x\"\nwhere x.a = x.b");
            clause5.SqlText.ShouldEqual("select \"a\", \"b\"\nfrom \"x\"\nwhere @0 = @1");
            clause5.Arguments.ShouldHaveCountOf(2);
            clause5.Arguments[0].ShouldEqual(1);
            clause5.Arguments[1].ShouldEqual(2);
            clause6.SqlText.ShouldEqual("select \"a\", \"b\"\nfrom \"x\"\nwhere @0 = @1");
            clause6.Arguments.ShouldHaveCountOf(2);
            clause6.Arguments[0].ShouldEqual(1);
            clause6.Arguments[1].ShouldEqual(2);
            clause7.SqlText.ShouldEqual("select \"a\", \"b\"\nfrom \"x\"");
            clause8.SqlText.ShouldEqual("select \"a\", \"b\"\nfrom \"x\"");
            clause9.SqlText.ShouldEqual("select \"a\", \"b\"\nfrom \"x\"\nwhere x.a = x.b");
            clause10.SqlText.ShouldEqual("select \"a\", \"b\"\nfrom \"x\"");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OrderByFailsWithNullColumn()
        {
            // --- Act
            SqlExpression.New.Select("a").From("x").OrderBy(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OrderByFailsWithNoColumn()
        {
            // --- Act
            SqlExpression.New.Select("a").From("x").OrderBy(new List<object>());
        }

        [TestMethod]
        public void OrderByWorksAsExpected()
        {
            // --- Act
            var clause1 = SqlExpression.New.Select("a").From("x").OrderBy("a");
            var clause2 = SqlExpression.New.Select("a", "b").From("x").OrderBy("a", "b");
            var clause3 = SqlExpression.New.Select("a").From("x").OrderBy<Record1>(r => r.Id);
            var clause4 = SqlExpression.New.Select("a").From("x").OrderBy<Record1>(r => r.Id, r => r.Name);

            // --- Assert
            clause1.SqlText.ShouldEqual("select \"a\"\nfrom \"x\"\norder by \"a\"");
            clause2.SqlText.ShouldEqual("select \"a\", \"b\"\nfrom \"x\"\norder by \"a\", \"b\"");
            clause3.SqlText.ShouldEqual("select \"a\"\nfrom \"x\"\norder by \"Id\"");
            clause4.SqlText.ShouldEqual("select \"a\"\nfrom \"x\"\norder by \"Id\", \"DisplayName\"");
        }

        [TestMethod]
        public void ProcessParametersWorksAsExpected()
        {
            // --- Act
            var clause1 = new SqlExpression("@0, @1", 1, 2);
            var clause2 = new SqlExpression("@0, @1", new object[] { 1, 2 });
            const string PARAM1 = "param1";
            var clause3 = new SqlExpression("@0, @1, @2", PARAM1, 2, PARAM1);
            var param2 = new object[] { 2, "hello", true };
            var clause4 = new SqlExpression("@0, @1", 1, param2);
            var param3 = new object[] { "hello" };
            var clause5 = new SqlExpression("@0, @1", 1, param3);
            var param4 = new object[] { };
            var clause6 = new SqlExpression("@0, @1", 1, param4);
            var param5 = new object[] { PARAM1, 3, PARAM1 };
            var clause7 = new SqlExpression("@0, @1", param5, 1);
            var clause8 = new SqlExpression("@0, @1", PARAM1, param5);

            // --- Assert
            clause1.SqlText.ShouldEqual("@0, @1");
            clause1.Arguments.ShouldHaveCountOf(2);
            clause1.Arguments[0].ShouldEqual(1);
            clause1.Arguments[1].ShouldEqual(2);
            clause2.SqlText.ShouldEqual("@0, @1");
            clause2.Arguments.ShouldHaveCountOf(2);
            clause2.Arguments[0].ShouldEqual(1);
            clause2.Arguments[1].ShouldEqual(2);
            clause3.SqlText.ShouldEqual("@0, @1, @0");
            clause3.Arguments.ShouldHaveCountOf(2);
            clause3.Arguments[0].ShouldEqual(PARAM1);
            clause3.Arguments[1].ShouldEqual(2);
            clause4.SqlText.ShouldEqual("@0, @1, @2, @3");
            clause4.Arguments.ShouldHaveCountOf(4);
            clause4.Arguments[0].ShouldEqual(1);
            clause4.Arguments[1].ShouldEqual(2);
            clause4.Arguments[2].ShouldEqual("hello");
            clause4.Arguments[3].ShouldEqual(true);
            clause5.SqlText.ShouldEqual("@0, @1");
            clause5.Arguments.ShouldHaveCountOf(2);
            clause5.Arguments[0].ShouldEqual(1);
            clause5.Arguments[1].ShouldEqual("hello");
            clause6.SqlText.ShouldEqual("@0, select 1 where 1 = 0");
            clause6.Arguments.ShouldHaveCountOf(1);
            clause6.Arguments[0].ShouldEqual(1);
            clause7.SqlText.ShouldEqual("@0, @1, @0, @2");
            clause7.Arguments.ShouldHaveCountOf(3);
            clause7.Arguments[0].ShouldEqual(PARAM1);
            clause7.Arguments[1].ShouldEqual(3);
            clause7.Arguments[2].ShouldEqual(1);
            clause8.SqlText.ShouldEqual("@0, @0, @1, @0");
            clause8.Arguments.ShouldHaveCountOf(2);
            clause8.Arguments[0].ShouldEqual(PARAM1);
            clause8.Arguments[1].ShouldEqual(3);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ProcessParameterWithWrongIndexRaisesException()
        {
            // --- Act
            // ReSharper disable once UnusedVariable
            var result = SqlExpression.CreateFrom("@2, @0", 1, 2).SqlText;
        }

        [TestMethod]
        public void ProcessParameterWithNameWorks()
        {
            // --- Act
            var clause1 = new SqlExpression("@Id, @Name", new Record1 { Id = 23, Name = "hello" });
            var clause2 = new SqlExpression("@Field, @Name", new Record1 { Id = 23, Name = "hello" },
                new Record5 { Field = "field" });

            // --- Assert
            clause1.SqlText.ShouldEqual("@0, @1");
            clause1.Arguments.ShouldHaveCountOf(2);
            clause1.Arguments[0].ShouldEqual(23);
            clause1.Arguments[1].ShouldEqual("hello");
            clause2.SqlText.ShouldEqual("@0, @1");
            clause2.Arguments.ShouldHaveCountOf(2);
            clause2.Arguments[0].ShouldEqual("field");
            clause2.Arguments[1].ShouldEqual("hello");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessParameterWithUnknownNameFails()
        {
            // ReSharper disable once UnusedVariable
            var result = new SqlExpression("@Id, @What", new Record1 { Id = 23, Name = "hello" }).SqlText;
        }

        [TestMethod]
        public void SqlExpressionMergesMultipleWhereClauses()
        {
            // --- Act
            var clause = SqlExpression.New.Select("a", "b").From("x").Where("a = b").Where("c = d");

            // --- Assert
            clause.SqlText.ShouldEqual("select \"a\", \"b\"\nfrom \"x\"\nwhere a = b\nand c = d");
        }

        [TestMethod]
        public void SqlExpressionMergesMultipleOrderByClauses()
        {
            // --- Act
            var clause = SqlExpression.New.Select("a", "b").From("x").OrderBy("a").OrderBy("b");

            // --- Assert
            clause.SqlText.ShouldEqual("select \"a\", \"b\"\nfrom \"x\"\norder by \"a\"\n, \"b\"");
        }

        [TestMethod]
        public void CompleteSelectWorksAsExpected()
        {
            // --- Act
            var clause1 = SqlExpression.New.Where("Id = 0");
            var completeClause1 = clause1.CompleteSelect<Record1>();
            var clause2 = SqlExpression.New.Select<Record1>().From<Record1>().Where("Id = 0");
            var completeClause2 = clause2.CompleteSelect<Record1>();

            // --- Assert
            completeClause1.SqlText.ShouldEqual("select \"Id\", \"DisplayName\", \"Description\"\nfrom \"Record1\"\nwhere Id = 0");
            completeClause2.SqlText.ShouldEqual("select \"Id\", \"DisplayName\", \"Description\"\nfrom \"Record1\"\nwhere Id = 0");
        }

        [TableName("Record1")]
        class Record1 : DataRecord<Record1>
        {
            // ReSharper disable UnusedMember.Local
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public int Id { get; set; }
            [ColumnName("DisplayName")]
            public string Name { get; set; }
            public string Description { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
            // ReSharper restore UnusedMember.Local
        }

        [TableName("Record2")]
        class Record2 : DataRecord<Record2>
        {
            // ReSharper disable UnusedMember.Local
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
            // ReSharper restore UnusedMember.Local
        }

        [TableName("Record3")]
        class Record3 : DataRecord<Record3>
        {
            // ReSharper disable UnusedMember.Local
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
            // ReSharper restore UnusedMember.Local
        }

        [SchemaName("user2")]
        [TableName("Record4")]
        class Record4 : DataRecord<Record4>
        {
            // ReSharper disable UnusedMember.Local
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
            // ReSharper restore UnusedMember.Local
        }

        [TableName("Record5")]
        class Record5 : DataRecord<Record5>
        {
            // ReSharper disable UnusedMember.Local
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public string Field { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
            // ReSharper restore UnusedMember.Local
        }
    }
}
