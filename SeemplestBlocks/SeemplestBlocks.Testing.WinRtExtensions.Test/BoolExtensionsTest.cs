using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace SeemplestBlocks.Testing.WinRtExtensions.Test
{
    [TestClass]
    public class BoolExtensionsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Bool_ShouldBeFalsePassTest()
        {
            false.ShouldBeFalse();
        }

        [TestMethod]
        public void Bool_ShouldBeFalseFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                true.ShouldBeFalse();
            });
        }

        [TestMethod]
        public void Bool_ShouldBeFalseNullablePassTest()
        {
            ((bool?)false).ShouldBeFalse();
        }

        [TestMethod]
        public void Bool_ShouldBeFalseNullableFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((bool?)true).ShouldBeFalse();
            });
        }

        [TestMethod]
        public void Bool_ShouldBeFalseNullableFailWhereNullTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((bool?)null).ShouldBeFalse();
            });
        }

        [TestMethod]
        public void Bool_ShouldBeTruePassTest()
        {
            true.ShouldBeTrue();
        }

        [TestMethod]
        public void Bool_ShouldBeTrueFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                false.ShouldBeTrue();
            });
        }


        [TestMethod]
        public void Bool_ShouldBeTrueNullablePassTest()
        {
            ((bool?)true).ShouldBeTrue();
        }

        [TestMethod]
        public void Bool_ShouldBeTrueNullableFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((bool?)false).ShouldBeTrue();
            });
        }

        [TestMethod]
        public void Bool_ShouldBeTrueNullableFailWhereNullTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((bool?)null).ShouldBeTrue();
            });
        }
    }
}
