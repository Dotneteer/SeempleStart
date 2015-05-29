using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace SeemplestBlocks.Testing.WinRtExtensions.Test
{
    [TestClass]
    public class GenericNullExtensionsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Object_ShouldBeNullPassTest()
        {
            ((bool?)null).ShouldBeNull();
            ((string)null).ShouldBeNull();
            ((object)null).ShouldBeNull();
        }

        [TestMethod]
        public void Object_ShouldBeNullFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                "test".ShouldBeNull();
            });
        }

        [TestMethod]
        public void Object_ShouldNotBeNullPassTest()
        {
            0.ShouldNotBeNull();
            ((int?)1).ShouldNotBeNull();
            "test".ShouldNotBeNull();
        }

        [TestMethod]
        public void Object_ShouldNotBeNullFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ((object)null).ShouldNotBeNull();
            });
        }
    }
}
