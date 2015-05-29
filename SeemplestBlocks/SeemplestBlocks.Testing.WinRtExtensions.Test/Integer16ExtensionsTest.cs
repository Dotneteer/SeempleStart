using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace SeemplestBlocks.Testing.WinRtExtensions.Test
{
    [TestClass]
    public class Integer16ExtensionsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Short_ShouldBeGreaterThanPassTest()
        {
            ((short)0x01).ShouldBeGreaterThan(0x00);
        }

        [TestMethod]
        public void Short_ShouldBeGreaterThanFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((short)0x44).ShouldBeGreaterThan(0x45);
            });
        }

        [TestMethod]
        public void Short_ShouldBeGreaterThanFailWhereEqualTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((short)0x44).ShouldBeGreaterThan(0x44);
            });
        }

        [TestMethod]
        public void Short_ShouldBeGreaterThanOrEqualToPassTest()
        {
            ((short)0x01).ShouldBeGreaterThanOrEqualTo(0x00);
            ((short)0x01).ShouldBeGreaterThanOrEqualTo(0x01);
        }

        [TestMethod]
        public void Short_ShouldBeGreaterThanOrEqualToFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((short)0x44).ShouldBeGreaterThanOrEqualTo(0x45);
            });
        }

        [TestMethod]
        public void Short_ShouldBeLessThanPassTest()
        {
            ((short)0x01).ShouldBeLessThan(0x02);
        }

        [TestMethod]
        public void Short_ShouldBeLessThanFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((short)0x44).ShouldBeLessThan(0x43);
            });
        }

        [TestMethod]
        public void Short_ShouldBeLessThanFailWhereEqualTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((short)0x44).ShouldBeLessThan(0x44);
            });
        }

        [TestMethod]
        public void Short_ShouldBeLessThanOrEqualToPassTest()
        {
            ((short)0x01).ShouldBeLessThanOrEqualTo(0x02);
            ((short)0x01).ShouldBeLessThanOrEqualTo(0x01);
        }

        [TestMethod]
        public void Short_ShouldBeLessThanOrEqualToFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((short)0x44).ShouldBeLessThanOrEqualTo(0x43);
            });
        }

        [TestMethod]
        public void Short_ShouldBeBetweenPassTest()
        {
            ((short)0x01).ShouldBeBetween(0x00, 0x05);
            ((short)0x01).ShouldBeBetween(0x01, 0x05);
            ((short)0x01).ShouldBeBetween(0x01, 0x05);
            ((short)0x05).ShouldBeBetween(0x01, 0x05);
        }

        [TestMethod]
        public void Short_ShouldBeBetweenFailWhereLowerGreaterThanUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((short)0x05).ShouldBeBetween(0x02, 0x01);
            });
        }

        [TestMethod]
        public void Short_ShouldBeBetweenFailWhereLowerEqualToUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((short)0x05).ShouldBeBetween(0x02, 0x02);
            });
        }

        [TestMethod]
        public void Short_ShouldBeBetweenFailWhereLessThanLowerTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((short)0x01).ShouldBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void Short_ShouldBeBetweenFailWhereGreaterThanUpperTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((short)0x06).ShouldBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void Short_ShouldNotBeBetweenPassTest()
        {
            ((short)0x06).ShouldNotBeBetween(0x00, 0x05);
            ((short)0x00).ShouldNotBeBetween(0x01, 0x05);
        }

        [TestMethod]
        public void Short_ShouldNotBeBetweenFailWhereLowerGreaterThanUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((short)0x05).ShouldNotBeBetween(0x02, 0x01);
            });
        }

        [TestMethod]
        public void Short_ShouldNotBeBetweenFailWhereLowerEqualToUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((short)0x05).ShouldNotBeBetween(0x02, 0x02);
            });
        }

        [TestMethod]
        public void Short_ShouldNotBeBetweenFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((short)0x04).ShouldNotBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void Short_ShouldBePositivePassTest()
        {
            ((short)3).ShouldBePositive();
            ((short)0).ShouldBePositive();
        }

        [TestMethod]
        public void Short_ShouldBePositiveFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((short)-2).ShouldBePositive();
            });
        }

        [TestMethod]
        public void Short_ShouldBeNegativePassTest()
        {
            ((short)-3).ShouldBeNegative();
        }

        [TestMethod]
        public void Short_ShouldBeNegativeFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((short)2).ShouldBeNegative();
            });
        }

        [TestMethod]
        public void Short_ShouldBeNegativeFailWhereZeroTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((short)0).ShouldBeNegative();
            });
        }
    }
}
