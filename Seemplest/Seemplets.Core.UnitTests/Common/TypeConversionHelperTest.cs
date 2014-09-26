using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Common;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Common
{
    [TestClass]
    public class TypeConversionHelperTest
    {
        [TestMethod]
        public void ByteArrayToStringWorksAsExpected()
        {
            // --- Arrange
            var byteArr = new byte[] {0xAE, 0x23, 0x1F};

            // --- Act/Assert
            TypeConversionHelper.ByteArrayToString(byteArr).ShouldEqual("0xAE231F");
        }

        [TestMethod]
        public void CanConvertToStringWorksAsExpected()
        {
            // --- Arrange
            TypeConverter conv1;
            TypeConverter conv2;

            // --- Act
            var canConvert1 = TypeConversionHelper.CanConvertToString(new MyType(), out conv1);
            var canConvert2 = TypeConversionHelper.CanConvertToString(123, out conv2);

            // --- Assert
            canConvert1.ShouldBeTrue();
            conv1.ShouldNotBeNull();
            canConvert2.ShouldBeTrue();
            conv2.ShouldNotBeNull();
        }

        // ReSharper disable ClassNeverInstantiated.Local
        class MyType { }
        // ReSharper restore ClassNeverInstantiated.Local
    }
}
