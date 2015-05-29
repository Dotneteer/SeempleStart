using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace SeemplestBlocks.Testing.WinRtExtensions.Test
{
    [TestClass]
    public class DoubleExtensionsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Double_ShouldBeGreaterThanPassTest()
        {
            ((double)0x01).ShouldBeGreaterThan(0x00);
        }

        [TestMethod]
        public void Double_ShouldBeGreaterThanFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((double)0x44).ShouldBeGreaterThan(0x45);
            });
        }

        [TestMethod]
        public void Double_ShouldBeGreaterThanFailWhereEqualTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((double)0x44).ShouldBeGreaterThan(0x44);
            });
        }

        [TestMethod]
        public void Double_ShouldBeGreaterThanOrEqualToPassTest()
        {
            ((double)0x01).ShouldBeGreaterThanOrEqualTo(0x00);
            ((double)0x01).ShouldBeGreaterThanOrEqualTo(0x01);
        }

        [TestMethod]
        public void Double_ShouldBeGreaterThanOrEqualToFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((double)0x44).ShouldBeGreaterThanOrEqualTo(0x45);
            });
        }

        [TestMethod]
        public void Double_ShouldBeLessThanPassTest()
        {
            ((double)0x01).ShouldBeLessThan(0x02);
        }

        [TestMethod]
        public void Double_ShouldBeLessThanFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((double)0x44).ShouldBeLessThan(0x43);
            });
        }

        [TestMethod]
        public void Double_ShouldBeLessThanFailWhereEqualTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((double)0x44).ShouldBeLessThan(0x44);
            });
        }

        [TestMethod]
        public void Double_ShouldBeLessThanOrEqualToPassTest()
        {
            ((double)0x01).ShouldBeLessThanOrEqualTo(0x02);
            ((double)0x01).ShouldBeLessThanOrEqualTo(0x01);
        }

        [TestMethod]
        public void Double_ShouldBeLessThanOrEqualToFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((double)0x44).ShouldBeLessThanOrEqualTo(0x43);
            });
        }

        [TestMethod]
        public void Double_ShouldBeBetweenPassTest()
        {
            ((double)0x01).ShouldBeBetween(0x00, 0x05);
            ((double)0x01).ShouldBeBetween(0x01, 0x05);
            ((double)0x01).ShouldBeBetween(0x01, 0x05);
            ((double)0x05).ShouldBeBetween(0x01, 0x05);
        }

        [TestMethod]
        public void Double_ShouldBeBetweenFailWhereLowerGreaterThanUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((double)0x05).ShouldBeBetween(0x02, 0x01);
            });
        }

        [TestMethod]
        public void Double_ShouldBeBetweenFailWhereLowerEqualToUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((double)0x05).ShouldBeBetween(0x02, 0x02);
            });
        }

        [TestMethod]
        public void Double_ShouldBeBetweenFailWhereLessThanLowerTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((double)0x01).ShouldBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void Double_ShouldBeBetweenFailWhereGreaterThanUpperTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((double)0x06).ShouldBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void Double_ShouldNotBeBetweenPassTest()
        {
            ((double)0x06).ShouldNotBeBetween(0x00, 0x05);
            ((double)0x00).ShouldNotBeBetween(0x01, 0x05);
        }

        [TestMethod]
        public void Double_ShouldNotBeBetweenFailWhereLowerGreaterThanUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((double)0x05).ShouldNotBeBetween(0x02, 0x01);
            });
        }

        [TestMethod]
        public void Double_ShouldNotBeBetweenFailWhereLowerEqualToUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ((double)0x05).ShouldNotBeBetween(0x02, 0x02);
            });
        }

        [TestMethod]
        public void Double_ShouldNotBeBetweenFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((double)0x04).ShouldNotBeBetween(0x02, 0x05);
            });
        }

        [TestMethod]
        public void Double_ShouldBePositivePassTest()
        {
            ((double)3).ShouldBePositive();
            ((double)0).ShouldBePositive();
        }

        [TestMethod]
        public void Double_ShouldBePositiveFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((double)-2).ShouldBePositive();
            });
        }

        [TestMethod]
        public void Double_ShouldBeNegativePassTest()
        {
            ((double)-3).ShouldBeNegative();
        }

        [TestMethod]
        public void Double_ShouldBeNegativeFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((double)2).ShouldBeNegative();
            });
        }

        [TestMethod]
        public void Double_ShouldBeNegativeFailWhereZeroTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((double)0).ShouldBeNegative();
            });
        }

        [TestMethod]
        public void Double_ShouldBeIntegralPassTest()
        {
            3.0.ShouldBeIntegral();
            (-3.0).ShouldBeIntegral();
            0.0.ShouldBeIntegral();
        }

        [TestMethod]
        public void Double_ShouldBeIntegralFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                3.345.ShouldBeIntegral();
            });
        }

        [TestMethod]
        public void Double_ShouldBeFractionalPassTest()
        {
            3.5.ShouldBeFractional();
            (-3.3).ShouldBeFractional();
            0.1.ShouldBeFractional();
        }

        [TestMethod]
        public void Double_ShouldBeFractionalFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                3.0.ShouldBeFractional();
            });
        }

        [TestMethod]
        public void Double_ShouldRoundToPassTest()
        {
            3.5.ShouldRoundTo(4);
            (-3.3).ShouldRoundTo(-3);
            0.1.ShouldRoundTo(0);
            ((double)4).ShouldRoundTo(4);
        }

        [TestMethod]
        public void Double_ShouldRoundToFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                3.1.ShouldRoundTo(4);
            });
        }
    }
}
