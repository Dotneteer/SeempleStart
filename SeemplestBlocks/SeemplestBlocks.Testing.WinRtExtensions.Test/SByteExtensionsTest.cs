using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace SeemplestBlocks.Testing.WinRtExtensions.Test
{
    [TestClass]
    public class SByteExtensionsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void SByte_ShouldBeGreaterThanPassTest()
        {
            ((sbyte)0x01).ShouldBeGreaterThan(0x00);
        }

        [TestMethod]
        public void SByte_ShouldBeGreaterThanFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((sbyte)0x44).ShouldBeGreaterThan(0x45);
            });
        }

        [TestMethod]
        public void SByte_ShouldBeGreaterThanFailWhereEqualTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((sbyte)0x44).ShouldBeGreaterThan(0x44);
            });
        }

        [TestMethod]
        public void SByte_ShouldBeGreaterThanOrEqualToPassTest()
        {
            ((sbyte)0x01).ShouldBeGreaterThanOrEqualTo(0x00);
            ((sbyte)0x01).ShouldBeGreaterThanOrEqualTo(0x01);
        }

        [TestMethod]
        public void SByte_ShouldBeGreaterThanOrEqualToFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((sbyte)0x44).ShouldBeGreaterThanOrEqualTo(0x45);
            });
        }

        [TestMethod]
        public void SByte_ShouldBeLessThanPassTest()
        {
            ((sbyte)0x01).ShouldBeLessThan(0x02);
        }

        [TestMethod]
        public void SByte_ShouldBeLessThanFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((sbyte)0x44).ShouldBeLessThan(0x43);
            });
        }

        [TestMethod]
        public void SByte_ShouldBeLessThanFailWhereEqualTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((sbyte)0x44).ShouldBeLessThan(0x44);
            });
        }

        [TestMethod]
        public void SByte_ShouldBeLessThanOrEqualToPassTest()
        {
            ((sbyte)0x01).ShouldBeLessThanOrEqualTo(0x02);
            ((sbyte)0x01).ShouldBeLessThanOrEqualTo(0x01);
        }

        [TestMethod]
        public void SByte_ShouldBeLessThanOrEqualToFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((sbyte)0x44).ShouldBeLessThanOrEqualTo(0x43);
            });
        }

        [TestMethod]
        public void SByte_ShouldBeBetweenPassTest()
        {
            ((sbyte)0x01).ShouldBeBetween(0x00, 0x05);
            ((sbyte)0x01).ShouldBeBetween(0x01, 0x05);
            ((sbyte)0x01).ShouldBeBetween(0x01, 0x05);
            ((sbyte)0x05).ShouldBeBetween(0x01, 0x05);
        }

        [TestMethod]
        public void SByte_ShouldBeBetweenFailWhereLowerGreaterThanUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((sbyte)0x05).ShouldBeBetween(0x02, 0x01);
            });
        }

        [TestMethod]
        public void SByte_ShouldBeBetweenFailWhereLowerEqualToUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((sbyte)0x05).ShouldBeBetween(0x02, 0x02);
            });
        }

        [TestMethod]
        public void SByte_ShouldBeBetweenFailWhereLessThanLowerTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((sbyte)0x01).ShouldBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void SByte_ShouldBeBetweenFailWhereGreaterThanUpperTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((sbyte)0x06).ShouldBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void SByte_ShouldNotBeBetweenPassTest()
        {
            ((sbyte)0x06).ShouldNotBeBetween(0x00, 0x05);
            ((sbyte)0x00).ShouldNotBeBetween(0x01, 0x05);
        }

        [TestMethod]
        public void SByte_ShouldNotBeBetweenFailWhereLowerGreaterThanUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((sbyte)0x05).ShouldNotBeBetween(0x02, 0x01);
            });
        }

        [TestMethod]
        public void SByte_ShouldNotBeBetweenFailWhereLowerEqualToUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((sbyte)0x05).ShouldNotBeBetween(0x02, 0x02);
            });
        }

        [TestMethod]
        public void SByte_ShouldNotBeBetweenFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((sbyte)0x04).ShouldNotBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void SByte_ShouldBePositivePassTest()
        {
            ((sbyte)3).ShouldBePositive();
            ((sbyte)0).ShouldBePositive();
        }

        [TestMethod]
        public void SByte_ShouldBePositiveFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((sbyte)-2).ShouldBePositive();
            });
        }

        [TestMethod]
        public void SByte_ShouldBeNegativePassTest()
        {
            ((sbyte)-3).ShouldBeNegative();
        }

        [TestMethod]
        public void SByte_ShouldBeNegativeFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((sbyte)2).ShouldBeNegative();
            });
        }

        [TestMethod]
        public void SByte_ShouldBeNegativeFailWhereZeroTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((sbyte)0).ShouldBeNegative();
            });
        }
    }
}
