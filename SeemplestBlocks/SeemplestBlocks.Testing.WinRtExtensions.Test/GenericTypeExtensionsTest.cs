using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace SeemplestBlocks.Testing.WinRtExtensions.Test
{
    [TestClass]
    public class GenericTypeExtensionsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Object_ShouldBeOfTypePassTest()
        {
            false.ShouldBeOfType(typeof(bool));
            "test".ShouldBeOfType(typeof(string));
            4.ShouldBeOfType(typeof(int));
        }

        [TestMethod]
        public void Object_ShouldBeOfTypeFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                4.ShouldBeOfType(typeof(bool));
            });
        }

        [TestMethod]
        public void Object_ShouldNotBeOfTypePassTest()
        {
            false.ShouldNotBeOfType(typeof(int));
            "test".ShouldNotBeOfType(typeof(bool));
            4.ShouldNotBeOfType(typeof(short));
        }

        [TestMethod]
        public void Object_ShouldNotBeOfTypeFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                4.ShouldNotBeOfType(typeof(int));
            });
        }
    }
}
