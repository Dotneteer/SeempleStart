using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace SeemplestBlocks.Testing.WinRtExtensions.Test
{
    [TestClass]
    public class Integer64ExtensionsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Long_ShouldBeGreaterThanPassTest()
        {
            ((long)0x01).ShouldBeGreaterThan(0x00);
        }

        [TestMethod]
        public void Long_ShouldBeGreaterThanFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((long)0x44).ShouldBeGreaterThan(0x45);
            });
        }

        [TestMethod]
        public void Long_ShouldBeGreaterThanFailWhereEqualTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((long)0x44).ShouldBeGreaterThan(0x44);
            });
        }

        [TestMethod]
        public void Long_ShouldBeGreaterThanOrEqualToPassTest()
        {
            ((long)0x01).ShouldBeGreaterThanOrEqualTo(0x00);
            ((long)0x01).ShouldBeGreaterThanOrEqualTo(0x01);
        }

        [TestMethod]
        public void Long_ShouldBeGreaterThanOrEqualToFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((long)0x44).ShouldBeGreaterThanOrEqualTo(0x45);
            });
        }

        [TestMethod]
        public void Long_ShouldBeLessThanPassTest()
        {
            ((long)0x01).ShouldBeLessThan(0x02);
        }

        [TestMethod]
        public void Long_ShouldBeLessThanFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((long)0x44).ShouldBeLessThan(0x43);
            });
        }

        [TestMethod]
        public void Long_ShouldBeLessThanFailWhereEqualTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((long)0x44).ShouldBeLessThan(0x44);
            });
        }

        [TestMethod]
        public void Long_ShouldBeLessThanOrEqualToPassTest()
        {
            ((long)0x01).ShouldBeLessThanOrEqualTo(0x02);
            ((long)0x01).ShouldBeLessThanOrEqualTo(0x01);
        }

        [TestMethod]
        public void Long_ShouldBeLessThanOrEqualToFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((long)0x44).ShouldBeLessThanOrEqualTo(0x43);
            });
        }

        [TestMethod]
        public void Long_ShouldBeBetweenPassTest()
        {
            ((long)0x01).ShouldBeBetween(0x00, 0x05);
            ((long)0x01).ShouldBeBetween(0x01, 0x05);
            ((long)0x01).ShouldBeBetween(0x01, 0x05);
            ((long)0x05).ShouldBeBetween(0x01, 0x05);
        }

        [TestMethod]
        public void Long_ShouldBeBetweenFailWhereLowerGreaterThanUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((long)0x05).ShouldBeBetween(0x02, 0x01);
            });
        }

        [TestMethod]
        public void Long_ShouldBeBetweenFailWhereLowerEqualToUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((long)0x05).ShouldBeBetween(0x02, 0x02);
            });
        }

        [TestMethod]
        public void Long_ShouldBeBetweenFailWhereLessThanLowerTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((long)0x01).ShouldBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void Long_ShouldBeBetweenFailWhereGreaterThanUpperTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((long)0x06).ShouldBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void Long_ShouldNotBeBetweenPassTest()
        {
            ((long)0x06).ShouldNotBeBetween(0x00, 0x05);
            ((long)0x00).ShouldNotBeBetween(0x01, 0x05);
        }

        [TestMethod]
        public void Long_ShouldNotBeBetweenFailWhereLowerGreaterThanUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((long)0x05).ShouldNotBeBetween(0x02, 0x01);
            });
        }

        [TestMethod]
        public void Long_ShouldNotBeBetweenFailWhereLowerEqualToUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((long)0x05).ShouldNotBeBetween(0x02, 0x02);
            });
        }

        [TestMethod]
        public void Long_ShouldNotBeBetweenFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((long)0x04).ShouldNotBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void Long_ShouldBePositivePassTest()
        {
            ((long)3).ShouldBePositive();
            ((long)0).ShouldBePositive();
        }

        [TestMethod]
        public void Long_ShouldBePositiveFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((long)-2).ShouldBePositive();
            });
        }

        [TestMethod]
        public void Long_ShouldBeNegativePassTest()
        {
            ((long)-3).ShouldBeNegative();
        }

        [TestMethod]
        public void Long_ShouldBeNegativeFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((long)2).ShouldBeNegative();
            });
        }

        [TestMethod]
        public void Long_ShouldBeNegativeFailWhereZeroTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((long)0).ShouldBeNegative();
            });
        }
    }
}
