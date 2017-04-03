using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.MsSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.MsSql.UnitTests.DataAccess
{
    [TestClass]
    public class DefaultSqlParameterMapperTest
    {
        [TestMethod]
        public void MappingWorksWithIntrinsicTypes()
        {
            // --- Arrange
            var mapper = new DefaultSqlParameterMapper();

            // --- Act
            var par1 = mapper.MapParameterValue("@0", null);
            var par2 = mapper.MapParameterValue("@0", (byte)123);
            var par3 = mapper.MapParameterValue("@0", (short)1234);
            var par4 = mapper.MapParameterValue("@0", 12345);
            var par5 = mapper.MapParameterValue("@0", 123456L);
            var par6 = mapper.MapParameterValue("@0", (sbyte)-2);
            var par7 = mapper.MapParameterValue("@0", 'A');
            var par8 = mapper.MapParameterValue("@0", "Hello");
            var par9 = mapper.MapParameterValue("@0", new string('x', 4000));
            var par10 = mapper.MapParameterValue("@0", new string('x', 4001));
            var par11 = mapper.MapParameterValue("@0", (ushort)1234);
            var par12 = mapper.MapParameterValue("@0", (uint)12345);
            var par13 = mapper.MapParameterValue("@0", 123456UL);
            var par14 = mapper.MapParameterValue("@0", new byte[8000]);
            var par15 = mapper.MapParameterValue("@0", new byte[8001]);
            var par16 = mapper.MapParameterValue("@0", new XElement("fruit", "apple"));
            var par17 = mapper.MapParameterValue("@0", SqlDbType.Image);


            // --- Assert
            par1.Value.ShouldEqual(DBNull.Value);
            par2.SqlDbType.ShouldEqual(SqlDbType.TinyInt);
            par2.Value.ShouldEqual((byte) 123);
            par3.SqlDbType.ShouldEqual(SqlDbType.SmallInt);
            par3.Value.ShouldEqual((short)1234);
            par4.SqlDbType.ShouldEqual(SqlDbType.Int);
            par4.Value.ShouldEqual(12345);
            par5.SqlDbType.ShouldEqual(SqlDbType.BigInt);
            par5.Value.ShouldEqual(123456L);
            par6.SqlDbType.ShouldEqual(SqlDbType.Int);
            par6.Value.ShouldEqual(-2);
            par7.SqlDbType.ShouldEqual(SqlDbType.NChar);
            par7.Size.ShouldEqual(1);
            par7.Value.ShouldEqual("A");
            par8.SqlDbType.ShouldEqual(SqlDbType.NVarChar);
            par8.Value.ShouldEqual("Hello");
            par9.SqlDbType.ShouldEqual(SqlDbType.NVarChar);
            par9.Value.ShouldEqual(new string('x', 4000));
            par10.SqlDbType.ShouldEqual(SqlDbType.NText);
            par10.Value.ShouldEqual(new string('x', 4001));
            par11.SqlDbType.ShouldEqual(SqlDbType.Int);
            par11.Value.ShouldEqual(1234);
            par12.SqlDbType.ShouldEqual(SqlDbType.BigInt);
            par12.Value.ShouldEqual(12345L);
            par13.SqlDbType.ShouldEqual(SqlDbType.BigInt);
            par13.Value.ShouldEqual(123456L);
            par14.SqlDbType.ShouldEqual(SqlDbType.VarBinary);
            par15.SqlDbType.ShouldEqual(SqlDbType.Image);
            par16.SqlDbType.ShouldEqual(SqlDbType.Xml);
            par17.SqlDbType.ShouldEqual(SqlDbType.Int);
            par17.Value.ShouldEqual((int)SqlDbType.Image);
        }

        [TestMethod]
        public void MappingUsesParentMapper()
        {
            // --- Arrange
            var mapper1 = new DefaultSqlParameterMapper(new DummyMapper());
            var mapper2 = new DefaultSqlParameterMapper();

            // --- Act
            var par1 = mapper1.MapParameterValue("@0", new SqlCommand("hello"));
            var par2 = mapper2.MapParameterValue("@0", new SqlCommand("hello"));

            // --- Assert
            par1.SqlDbType.ShouldEqual(SqlDbType.NVarChar);
            par1.Value.ToString().ShouldEqual("hello");
            par2.ShouldBeNull();
        }

        class DummyMapper : ISqlParameterMapper
        {
            public SqlParameter MapParameterValue(string parameterName, object value)
            {
                var command = value as SqlCommand;
                return command != null ? new SqlParameter(parameterName, command.CommandText) : null;
            }

            public SqlParameter MapParameterType(string parameterName, Type type)
            {
                var command = new SqlCommand();
                return new SqlParameter(parameterName, command.CommandText);
            }
        }
    }
}
