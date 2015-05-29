using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace SeemplestBlocks.Testing.WinRtExtensions.Test
{
    [TestClass]
    public class DateTimeExtensionsTests
    {
        [TestMethod]
        public void DateTime_ShouldBeBeforePassTest()
        {
            DateTime.MinValue.ShouldBeBefore(DateTime.MaxValue);
            DateTime.Now.AddSeconds(-1).ShouldBeBefore(DateTime.Now);
        }

        [TestMethod]
        public void DateTime_ShouldBeBeforeFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                DateTime.MaxValue.ShouldBeBefore(DateTime.MinValue);
            });
        }

        [TestMethod]
        public void DateTime_ShouldBeBeforeFailWhereEqualTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                DateTime.MinValue.ShouldBeBefore(DateTime.MinValue);
            });
        }

        [TestMethod]
        public void DateTime_ShouldBeBeforeOrSameAsPassTest()
        {
            DateTime.MinValue.ShouldBeBeforeOrSameAs(DateTime.MaxValue);
            DateTime.MinValue.ShouldBeBeforeOrSameAs(DateTime.MinValue);
        }

        [TestMethod]
        public void DateTime_ShouldBeBeforeOrSameAsFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                DateTime.MaxValue.ShouldBeBeforeOrSameAs(DateTime.MinValue);
            });
        }

        [TestMethod]
        public void DateTime_ShouldBeAfterPassTest()
        {
            DateTime.MaxValue.ShouldBeAfter(DateTime.MinValue);
            DateTime.Now.AddHours(1).ShouldBeAfter(DateTime.Now);
        }

        [TestMethod]
        public void DateTime_ShouldBeAfterFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                DateTime.MinValue.ShouldBeAfter(DateTime.MaxValue);
            });
        }

        [TestMethod]
        public void DateTime_ShouldBeAfterFailWhereEqualTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                DateTime.MinValue.ShouldBeAfter(DateTime.MinValue);
            });
        }

        [TestMethod]
        public void DateTime_ShouldBeAfterOrSameAsPassTest()
        {
            DateTime.MaxValue.ShouldBeAfterOrSameAs(DateTime.MinValue);
            DateTime.MinValue.ShouldBeAfterOrSameAs(DateTime.MinValue);
        }

        [TestMethod]
        public void DateTime_ShouldBeAfterOrSameAsFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                DateTime.MinValue.ShouldBeAfterOrSameAs(DateTime.MaxValue);
            });
        }
    }
}
