using System.Collections;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace SeemplestBlocks.Testing.WinRtExtensions.Test
{
    [TestClass]
    public class TypeExtensionsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Type_ShouldBeAssignableFromPassTest()
        {
            (typeof(ICollection)).ShouldBeAssignableFrom(typeof(IEnumerable));
        }

        [TestMethod]
        public void Type_ShouldBeAssignableFromFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                (typeof(IEnumerable)).ShouldBeAssignableFrom(typeof(ICollection));
            });
        }

        [TestMethod]
        public void Type_TypeParam_ShouldBeAssignableFromPassTest()
        {
            (typeof(IList)).ShouldBeAssignableFrom<IEnumerable>();
        }

        [TestMethod]
        public void Type_TypeParam_ShouldBeAssignableFromFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                (typeof(IEnumerable)).ShouldBeAssignableFrom<IList>();
            });
        }
    }
}
