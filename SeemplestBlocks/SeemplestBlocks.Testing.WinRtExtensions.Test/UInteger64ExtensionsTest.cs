using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace SeemplestBlocks.Testing.WinRtExtensions.Test
{
    [TestClass]
    public class UInteger64ExtensionsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void ULong_ShouldBeGreaterThanPassTest()
        {
            ((ulong)0x01).ShouldBeGreaterThan(0x00);
        }

        [TestMethod]
        public void ULong_ShouldBeGreaterThanFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((ulong)0x44).ShouldBeGreaterThan(0x45);
            });
        }

        [TestMethod]
        public void ULong_ShouldBeGreaterThanFailWhereEqualTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((ulong)0x44).ShouldBeGreaterThan(0x44);
            });
        }

        [TestMethod]
        public void ULong_ShouldBeGreaterThanOrEqualToPassTest()
        {
            ((ulong)0x01).ShouldBeGreaterThanOrEqualTo(0x00);
            ((ulong)0x01).ShouldBeGreaterThanOrEqualTo(0x01);
        }

        [TestMethod]
        public void ULong_ShouldBeGreaterThanOrEqualToFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((ulong)0x44).ShouldBeGreaterThanOrEqualTo(0x45);
            });
        }

        [TestMethod]
        public void ULong_ShouldBeLessThanPassTest()
        {
            ((ulong)0x01).ShouldBeLessThan(0x02);
        }

        [TestMethod]
        public void ULong_ShouldBeLessThanFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((ulong)0x44).ShouldBeLessThan(0x43);
            });
        }

        [TestMethod]
        public void ULong_ShouldBeLessThanFailWhereEqualTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((ulong)0x44).ShouldBeLessThan(0x44);
            });
        }

        [TestMethod]
        public void ULong_ShouldBeLessThanOrEqualToPassTest()
        {
            ((ulong)0x01).ShouldBeLessThanOrEqualTo(0x02);
            ((ulong)0x01).ShouldBeLessThanOrEqualTo(0x01);
        }

        [TestMethod]
        public void ULong_ShouldBeLessThanOrEqualToFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((ulong)0x44).ShouldBeLessThanOrEqualTo(0x43);
            });
        }

        [TestMethod]
        public void ULong_ShouldBeBetweenPassTest()
        {
            ((ulong)0x01).ShouldBeBetween(0x00, 0x05);
            ((ulong)0x01).ShouldBeBetween(0x01, 0x05);
            ((ulong)0x01).ShouldBeBetween(0x01, 0x05);
            ((ulong)0x05).ShouldBeBetween(0x01, 0x05);
        }

        [TestMethod]
        public void ULong_ShouldBeBetweenFailWhereLowerGreaterThanUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((ulong)0x05).ShouldBeBetween(0x02, 0x01);
            });
        }

        [TestMethod]
        public void ULong_ShouldBeBetweenFailWhereLowerEqualToUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((ulong)0x05).ShouldBeBetween(0x02, 0x02);
            });
        }

        [TestMethod]
        public void ULong_ShouldBeBetweenFailWhereLessThanLowerTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((ulong)0x01).ShouldBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void ULong_ShouldBeBetweenFailWhereGreaterThanUpperTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((ulong)0x06).ShouldBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void ULong_ShouldNotBeBetweenPassTest()
        {
            ((ulong)0x06).ShouldNotBeBetween(0x00, 0x05);
            ((ulong)0x00).ShouldNotBeBetween(0x01, 0x05);
        }

        [TestMethod]
        public void ULong_ShouldNotBeBetweenFailWhereLowerGreaterThanUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((ulong)0x05).ShouldNotBeBetween(0x02, 0x01);
            });
        }

        [TestMethod]
        public void ULong_ShouldNotBeBetweenFailWhereLowerEqualToUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((ulong)0x05).ShouldNotBeBetween(0x02, 0x02);
            });
        }

        [TestMethod]
        public void ULong_ShouldNotBeBetweenFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((ulong)0x04).ShouldNotBeBetween(0x02, 0x05);
            });
        }
    }
}
