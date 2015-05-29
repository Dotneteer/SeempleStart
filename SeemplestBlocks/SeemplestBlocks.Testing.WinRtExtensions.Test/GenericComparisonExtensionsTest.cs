using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace SeemplestBlocks.Testing.WinRtExtensions.Test
{
    [TestClass]
    public class GenericComparisonExtensionsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Object_ShouldEqualPassTest()
        {
            true.ShouldEqual(true);
            "test".ShouldEqual("test");
            ((object)null).ShouldEqual(null);
        }

        [TestMethod]
        public void Object_ShouldEqualFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                "test".ShouldEqual("tester");
            });
        }

        [TestMethod]
        public void Object_ShouldNotEqualPassTest()
        {
            true.ShouldNotEqual(false);
            "test".ShouldNotEqual("tester");
            ((object)null).ShouldNotEqual(1);
            2.ShouldNotEqual(1);
        }

        [TestMethod]
        public void Object_ShouldNotEqualFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                0.ShouldNotEqual(0);
            });
        }

        [TestMethod]
        public void Object_ShouldBeSameAsPassTest()
        {
            const int I = 4;
            object firstObject = I;
            var secondObject = firstObject;

            firstObject.ShouldBeSameAs(secondObject);
            "test".ShouldBeSameAs("test");
            ((object)null).ShouldBeSameAs(null);
        }

        [TestMethod]
        public void Object_ShouldBeSameAsFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                const int I = 4;
                object firstObject = I;
                object secondObject = I;

                firstObject.ShouldBeSameAs(secondObject);
            });
        }

        [TestMethod]
        public void Object_ShouldNotBeSameAsPassTest()
        {
            const int I = 4;
            object firstObject = I;
            object secondObject = I;

            firstObject.ShouldNotBeSameAs(secondObject);
        }

        [TestMethod]
        public void Object_ShouldNotBeSameAsFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                const int I = 10;
                object firstObject = I;
                var secondObject = firstObject;

                firstObject.ShouldNotBeSameAs(secondObject);
            });
        }
    }
}
