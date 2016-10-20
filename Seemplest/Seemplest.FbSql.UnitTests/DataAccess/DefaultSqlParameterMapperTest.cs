using System;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.FbSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.FbSql.UnitTests.DataAccess
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
            var par9 = mapper.MapParameterValue("@0", new string('x', 32765));
            var par10 = mapper.MapParameterValue("@0", new string('x', 32766));
            var par11 = mapper.MapParameterValue("@0", (ushort)1234);
            var par12 = mapper.MapParameterValue("@0", (uint)12345);
            var par13 = mapper.MapParameterValue("@0", 123456UL);
            var par14 = mapper.MapParameterValue("@0", new byte[8000]);
            var par15 = mapper.MapParameterValue("@0", new byte[8001]);
            var par16 = mapper.MapParameterValue("@0", FbDbType.Binary);


            // --- Assert
            par1.Value.ShouldEqual(DBNull.Value);
            par2.FbDbType.ShouldEqual(FbDbType.SmallInt);
            par2.Value.ShouldEqual((byte)123);
            par3.FbDbType.ShouldEqual(FbDbType.SmallInt);
            par3.Value.ShouldEqual((short)1234);
            par4.FbDbType.ShouldEqual(FbDbType.Integer);
            par4.Value.ShouldEqual(12345);
            par5.FbDbType.ShouldEqual(FbDbType.BigInt);
            par5.Value.ShouldEqual(123456L);
            par6.FbDbType.ShouldEqual(FbDbType.Integer);
            par6.Value.ShouldEqual(-2);
            par7.FbDbType.ShouldEqual(FbDbType.Char);
            par7.Size.ShouldEqual(1);
            par7.Value.ShouldEqual("A");
            par8.FbDbType.ShouldEqual(FbDbType.VarChar);
            par8.Value.ShouldEqual("Hello");
            par9.FbDbType.ShouldEqual(FbDbType.VarChar);
            par9.Value.ShouldEqual(new string('x', 32765));
            par10.FbDbType.ShouldEqual(FbDbType.Text);
            par10.Value.ShouldEqual(new string('x', 32766));
            par11.FbDbType.ShouldEqual(FbDbType.Integer);
            par11.Value.ShouldEqual(1234);
            par12.FbDbType.ShouldEqual(FbDbType.BigInt);
            par12.Value.ShouldEqual(12345L);
            par13.FbDbType.ShouldEqual(FbDbType.BigInt);
            par13.Value.ShouldEqual(123456L);
            par14.FbDbType.ShouldEqual(FbDbType.Binary);
            par15.FbDbType.ShouldEqual(FbDbType.Binary);
            par16.FbDbType.ShouldEqual(FbDbType.Integer);
            par16.Value.ShouldEqual((int)FbDbType.Binary);
        }
    }
}
