using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace SeemplestBlocks.Testing.WinRtExtensions.Test
{
    [TestClass]
    public class UInteger16ExtensionsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void UShort_ShouldBeGreaterThanPassTest()
        {
            ((ushort)0x01).ShouldBeGreaterThan(0x00);
        }

        [TestMethod]
        public void UShort_ShouldBeGreaterThanFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((ushort)0x44).ShouldBeGreaterThan(0x45);
            });
        }

        [TestMethod]
        public void UShort_ShouldBeGreaterThanFailWhereEqualTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((ushort)0x44).ShouldBeGreaterThan(0x44);
            });
        }

        [TestMethod]
        public void UShort_ShouldBeGreaterThanOrEqualToPassTest()
        {
            ((ushort)0x01).ShouldBeGreaterThanOrEqualTo(0x00);
            ((ushort)0x01).ShouldBeGreaterThanOrEqualTo(0x01);
        }

        [TestMethod]
        public void UShort_ShouldBeGreaterThanOrEqualToFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((ushort)0x44).ShouldBeGreaterThanOrEqualTo(0x45);
            });
        }

        [TestMethod]
        public void UShort_ShouldBeLessThanPassTest()
        {
            ((ushort)0x01).ShouldBeLessThan(0x02);
        }

        [TestMethod]
        public void UShort_ShouldBeLessThanFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((ushort)0x44).ShouldBeLessThan(0x43);
            });
        }

        [TestMethod]
        public void UShort_ShouldBeLessThanFailWhereEqualTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((ushort)0x44).ShouldBeLessThan(0x44);
            });
        }

        [TestMethod]
        public void UShort_ShouldBeLessThanOrEqualToPassTest()
        {
            ((ushort)0x01).ShouldBeLessThanOrEqualTo(0x02);
            ((ushort)0x01).ShouldBeLessThanOrEqualTo(0x01);
        }

        [TestMethod]
        public void UShort_ShouldBeLessThanOrEqualToFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((ushort)0x44).ShouldBeLessThanOrEqualTo(0x43);
            });
        }

        [TestMethod]
        public void UShort_ShouldBeBetweenPassTest()
        {
            ((ushort)0x01).ShouldBeBetween(0x00, 0x05);
            ((ushort)0x01).ShouldBeBetween(0x01, 0x05);
            ((ushort)0x01).ShouldBeBetween(0x01, 0x05);
            ((ushort)0x05).ShouldBeBetween(0x01, 0x05);
        }

        [TestMethod]
        public void UShort_ShouldBeBetweenFailWhereLowerGreaterThanUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((ushort)0x05).ShouldBeBetween(0x02, 0x01);
            });
        }

        [TestMethod]
        public void UShort_ShouldBeBetweenFailWhereLowerEqualToUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((ushort)0x05).ShouldBeBetween(0x02, 0x02);
            });
        }

        [TestMethod]
        public void UShort_ShouldBeBetweenFailWhereLessThanLowerTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((ushort)0x01).ShouldBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void UShort_ShouldBeBetweenFailWhereGreaterThanUpperTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((ushort)0x06).ShouldBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void UShort_ShouldNotBeBetweenPassTest()
        {
            ((ushort)0x06).ShouldNotBeBetween(0x00, 0x05);
            ((ushort)0x00).ShouldNotBeBetween(0x01, 0x05);
        }

        [TestMethod]
        public void UShort_ShouldNotBeBetweenFailWhereLowerGreaterThanUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((ushort)0x05).ShouldNotBeBetween(0x02, 0x01);
            });
        }

        [TestMethod]
        public void UShort_ShouldNotBeBetweenFailWhereLowerEqualToUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((ushort)0x05).ShouldNotBeBetween(0x02, 0x02);
            });
        }

        [TestMethod]
        public void UShort_ShouldNotBeBetweenFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((ushort)0x04).ShouldNotBeBetween(0x02, 0x05);
            });
        }
    }
}
