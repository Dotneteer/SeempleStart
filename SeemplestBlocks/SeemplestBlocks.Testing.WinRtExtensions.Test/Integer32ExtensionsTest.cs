using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace SeemplestBlocks.Testing.WinRtExtensions.Test
{
    [TestClass]
    public class Integer32ExtensionsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Int_ShouldBeGreaterThanPassTest()
        {
            0x01.ShouldBeGreaterThan(0x00);
        }

        [TestMethod]
        public void Int_ShouldBeGreaterThanFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                0x44.ShouldBeGreaterThan(0x45);
            });
        }

        [TestMethod]
        public void Int_ShouldBeGreaterThanFailWhereEqualTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                0x44.ShouldBeGreaterThan(0x44);
            });
        }

        [TestMethod]
        public void Int_ShouldBeGreaterThanOrEqualToPassTest()
        {
            0x01.ShouldBeGreaterThanOrEqualTo(0x00);
            0x01.ShouldBeGreaterThanOrEqualTo(0x01);
        }

        [TestMethod]
        public void Int_ShouldBeGreaterThanOrEqualToFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                0x44.ShouldBeGreaterThanOrEqualTo(0x45);
            });
        }

        [TestMethod]
        public void Int_ShouldBeLessThanPassTest()
        {
            0x01.ShouldBeLessThan(0x02);
        }

        [TestMethod]
        public void Int_ShouldBeLessThanFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                0x44.ShouldBeLessThan(0x43);
            });
        }

        [TestMethod]
        public void Int_ShouldBeLessThanFailWhereEqualTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                0x44.ShouldBeLessThan(0x44);
            });
        }

        [TestMethod]
        public void Int_ShouldBeLessThanOrEqualToPassTest()
        {
            0x01.ShouldBeLessThanOrEqualTo(0x02);
            0x01.ShouldBeLessThanOrEqualTo(0x01);
        }

        [TestMethod]
        public void Int_ShouldBeLessThanOrEqualToFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                0x44.ShouldBeLessThanOrEqualTo(0x43);
            });
        }

        [TestMethod]
        public void Int_ShouldBeBetweenPassTest()
        {
            0x01.ShouldBeBetween(0x00, 0x05);
            0x01.ShouldBeBetween(0x01, 0x05);
            0x01.ShouldBeBetween(0x01, 0x05);
            0x05.ShouldBeBetween(0x01, 0x05);
        }

        [TestMethod]
        public void Int_ShouldBeBetweenFailWhereLowerGreaterThanUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                0x05.ShouldBeBetween(0x02, 0x01);
            });
        }

        [TestMethod]
        public void Int_ShouldBeBetweenFailWhereLowerEqualToUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                0x05.ShouldBeBetween(0x02, 0x02);
            });
        }

        [TestMethod]
        public void Int_ShouldBeBetweenFailWhereLessThanLowerTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                0x01.ShouldBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void Int_ShouldBeBetweenFailWhereGreaterThanUpperTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                0x06.ShouldBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void Int_ShouldNotBeBetweenPassTest()
        {
            0x06.ShouldNotBeBetween(0x00, 0x05);
            0x00.ShouldNotBeBetween(0x01, 0x05);
        }

        [TestMethod]
        public void Int_ShouldNotBeBetweenFailWhereLowerGreaterThanUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                0x05.ShouldNotBeBetween(0x02, 0x01);
            });
        }

        [TestMethod]
        public void Int_ShouldNotBeBetweenFailWhereLowerEqualToUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                0x05.ShouldNotBeBetween(0x02, 0x02);
            });
        }

        [TestMethod]
        public void Int_ShouldNotBeBetweenFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                0x04.ShouldNotBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void Int_ShouldBePositivePassTest()
        {
            3.ShouldBePositive();
            0.ShouldBePositive();
        }

        [TestMethod]
        public void Int_ShouldBePositiveFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                (-2).ShouldBePositive();
            });
        }

        [TestMethod]
        public void Int_ShouldBeNegativePassTest()
        {
            (-3).ShouldBeNegative();
        }

        [TestMethod]
        public void Int_ShouldBeNegativeFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                2.ShouldBeNegative();
            });
        }

        [TestMethod]
        public void Int_ShouldBeNegativeFailWhereZeroTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                0.ShouldBeNegative();
            });
        }
    }
}
