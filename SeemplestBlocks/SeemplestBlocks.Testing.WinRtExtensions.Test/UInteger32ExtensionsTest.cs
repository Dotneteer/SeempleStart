using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace SeemplestBlocks.Testing.WinRtExtensions.Test
{
    [TestClass]
    public class UInteger32ExtensionsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void UInt_ShouldBeGreaterThanPassTest()
        {
            ((uint)0x01).ShouldBeGreaterThan(0x00);
        }

        [TestMethod]
        public void UInt_ShouldBeGreaterThanFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((uint)0x44).ShouldBeGreaterThan(0x45);
            });
        }

        [TestMethod]
        public void UInt_ShouldBeGreaterThanFailWhereEqualTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((uint)0x44).ShouldBeGreaterThan(0x44);
            });
        }

        [TestMethod]
        public void UInt_ShouldBeGreaterThanOrEqualToPassTest()
        {
            ((uint)0x01).ShouldBeGreaterThanOrEqualTo(0x00);
            ((uint)0x01).ShouldBeGreaterThanOrEqualTo(0x01);
        }

        [TestMethod]
        public void UInt_ShouldBeGreaterThanOrEqualToFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((uint)0x44).ShouldBeGreaterThanOrEqualTo(0x45);
            });
        }

        [TestMethod]
        public void UInt_ShouldBeLessThanPassTest()
        {
            ((uint)0x01).ShouldBeLessThan(0x02);
        }

        [TestMethod]
        public void UInt_ShouldBeLessThanFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((uint)0x44).ShouldBeLessThan(0x43);
            });
        }

        [TestMethod]
        public void UInt_ShouldBeLessThanFailWhereEqualTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((uint)0x44).ShouldBeLessThan(0x44);
            });
        }

        [TestMethod]
        public void UInt_ShouldBeLessThanOrEqualToPassTest()
        {
            ((uint)0x01).ShouldBeLessThanOrEqualTo(0x02);
            ((uint)0x01).ShouldBeLessThanOrEqualTo(0x01);
        }

        [TestMethod]
        public void UInt_ShouldBeLessThanOrEqualToFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((uint)0x44).ShouldBeLessThanOrEqualTo(0x43);
            });
        }

        [TestMethod]
        public void UInt_ShouldBeBetweenPassTest()
        {
            ((uint)0x01).ShouldBeBetween(0x00, 0x05);
            ((uint)0x01).ShouldBeBetween(0x01, 0x05);
            ((uint)0x01).ShouldBeBetween(0x01, 0x05);
            ((uint)0x05).ShouldBeBetween(0x01, 0x05);
        }

        [TestMethod]
        public void UInt_ShouldBeBetweenFailWhereLowerGreaterThanUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((uint)0x05).ShouldBeBetween(0x02, 0x01);
            });
        }

        [TestMethod]
        public void UInt_ShouldBeBetweenFailWhereLowerEqualToUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((uint)0x05).ShouldBeBetween(0x02, 0x02);
            });
        }

        [TestMethod]
        public void UInt_ShouldBeBetweenFailWhereLessThanLowerTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((uint)0x01).ShouldBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void UInt_ShouldBeBetweenFailWhereGreaterThanUpperTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((uint)0x06).ShouldBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void UInt_ShouldNotBeBetweenPassTest()
        {
            ((uint)0x06).ShouldNotBeBetween(0x00, 0x05);
            ((uint)0x00).ShouldNotBeBetween(0x01, 0x05);
        }

        [TestMethod]
        public void UInt_ShouldNotBeBetweenFailWhereLowerGreaterThanUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((uint)0x05).ShouldNotBeBetween(0x02, 0x01);
            });
        }

        [TestMethod]
        public void UInt_ShouldNotBeBetweenFailWhereLowerEqualToUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((uint)0x05).ShouldNotBeBetween(0x02, 0x02);
            });
        }

        [TestMethod]
        public void UInt_ShouldNotBeBetweenFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((uint)0x04).ShouldNotBeBetween(0x02, 0x05);
            });
        }
    }
}
