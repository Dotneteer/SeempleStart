using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace SeemplestBlocks.Testing.WinRtExtensions.Test
{
    [TestClass]
    public class SingleExtensionsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Single_ShouldBeGreaterThanPassTest()
        {
            ((float)0x01).ShouldBeGreaterThan(0x00);
        }

        [TestMethod]
        public void Single_ShouldBeGreaterThanFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((float)0x44).ShouldBeGreaterThan(0x45);
            });
        }

        [TestMethod]
        public void Single_ShouldBeGreaterThanFailWhereEqualTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((float)0x44).ShouldBeGreaterThan(0x44);
            });
        }

        [TestMethod]
        public void Single_ShouldBeGreaterThanOrEqualToPassTest()
        {
            ((float)0x01).ShouldBeGreaterThanOrEqualTo(0x00);
            ((float)0x01).ShouldBeGreaterThanOrEqualTo(0x01);
        }

        [TestMethod]
        public void Single_ShouldBeGreaterThanOrEqualToFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((float)0x44).ShouldBeGreaterThanOrEqualTo(0x45);
            });
        }

        [TestMethod]
        public void Single_ShouldBeLessThanPassTest()
        {
            ((float)0x01).ShouldBeLessThan(0x02);
        }

        [TestMethod]
        public void Single_ShouldBeLessThanFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((float)0x44).ShouldBeLessThan(0x43);
            });
        }

        [TestMethod]
        public void Single_ShouldBeLessThanFailWhereEqualTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((float)0x44).ShouldBeLessThan(0x44);
            });
        }

        [TestMethod]
        public void Single_ShouldBeLessThanOrEqualToPassTest()
        {
            ((float)0x01).ShouldBeLessThanOrEqualTo(0x02);
            ((float)0x01).ShouldBeLessThanOrEqualTo(0x01);
        }

        [TestMethod]
        public void Single_ShouldBeLessThanOrEqualToFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((float)0x44).ShouldBeLessThanOrEqualTo(0x43);
            });
        }

        [TestMethod]
        public void Single_ShouldBeBetweenPassTest()
        {
            ((float)0x01).ShouldBeBetween(0x00, 0x05);
            ((float)0x01).ShouldBeBetween(0x01, 0x05);
            ((float)0x01).ShouldBeBetween(0x01, 0x05);
            ((float)0x05).ShouldBeBetween(0x01, 0x05);
        }

        [TestMethod]
        public void Single_ShouldBeBetweenFailWhereLowerGreaterThanUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((float)0x05).ShouldBeBetween(0x02, 0x01);
            });
        }

        [TestMethod]
        public void Single_ShouldBeBetweenFailWhereLowerEqualToUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((float)0x05).ShouldBeBetween(0x02, 0x02);
            });
        }

        [TestMethod]
        public void Single_ShouldBeBetweenFailWhereLessThanLowerTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((float)0x01).ShouldBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void Single_ShouldBeBetweenFailWhereGreaterThanUpperTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((float)0x06).ShouldBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void Single_ShouldNotBeBetweenPassTest()
        {
            ((float)0x06).ShouldNotBeBetween(0x00, 0x05);
            ((float)0x00).ShouldNotBeBetween(0x01, 0x05);
        }

        [TestMethod]
        public void Single_ShouldNotBeBetweenFailWhereLowerGreaterThanUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((float)0x05).ShouldNotBeBetween(0x02, 0x01);
            });
        }

        [TestMethod]
        public void Single_ShouldNotBeBetweenFailWhereLowerEqualToUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((float)0x05).ShouldNotBeBetween(0x02, 0x02);
            });
        }

        [TestMethod]
        public void Single_ShouldNotBeBetweenFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((float)0x04).ShouldNotBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void Single_ShouldBePositivePassTest()
        {
            ((float)3).ShouldBePositive();
            ((float)0).ShouldBePositive();
        }

        [TestMethod]
        public void Single_ShouldBePositiveFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((float)-2).ShouldBePositive();
            });
        }

        [TestMethod]
        public void Single_ShouldBeNegativePassTest()
        {
            ((float)-3).ShouldBeNegative();
        }

        [TestMethod]
        public void Single_ShouldBeNegativeFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((float)2).ShouldBeNegative();
            });
        }

        [TestMethod]
        public void Single_ShouldBeNegativeFailWhereZeroTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((float)0).ShouldBeNegative();
            });
        }

        [TestMethod]
        public void Single_ShouldBeIntegralPassTest()
        {
            ((float)3.0).ShouldBeIntegral();
            ((float)-3.0).ShouldBeIntegral();
            ((float)0.0).ShouldBeIntegral();
        }

        [TestMethod]
        public void Single_ShouldBeIntegralFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((float)3.345).ShouldBeIntegral();
            });
        }

        [TestMethod]
        public void Single_ShouldBeFractionalPassTest()
        {
            ((float)3.5).ShouldBeFractional();
            ((float)-3.3).ShouldBeFractional();
            ((float)0.1).ShouldBeFractional();
        }

        [TestMethod]
        public void Single_ShouldBeFractionalFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((float)3.0).ShouldBeFractional();
            });
        }

        [TestMethod]
        public void Single_ShouldRoundToPassTest()
        {
            ((float)3.5).ShouldRoundTo(4);
            ((float)-3.3).ShouldRoundTo(-3);
            ((float)0.1).ShouldRoundTo(0);
            ((float)4).ShouldRoundTo(4);
        }

        [TestMethod]
        public void Single_ShouldRoundToFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((float)3.1).ShouldRoundTo(4);
            });
        }
    }
}
