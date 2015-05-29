using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace SeemplestBlocks.Testing.WinRtExtensions.Test
{
    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class ICollectionExtensionsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void ICollection_ShouldBeEmptyPassTest()
        {
            var source = new List<int>();
            source.ShouldBeEmpty();
        }

        [TestMethod]
        public void ICollection_ShouldBeEmptyFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                var source = new List<int>() { 1 };
                source.ShouldBeEmpty();
            });
        }

        [TestMethod]
        public void ICollection_ShouldNotBeEmptyPassTest()
        {
            var source = new List<int> { 1 };
            source.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void ICollection_ShouldNotBeEmptyFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                var source = new List<int>();
                source.ShouldNotBeEmpty();
            });
        }

        [TestMethod]
        public void ICollection_ShouldContainPassTest()
        {
            var source = new List<int>() { 1, 2, 3, 4, 5 };
            source.ShouldContain(2);
        }

        [TestMethod]
        public void ICollection_ShouldContainFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                var source = new List<int>() { 1, 2, 3, 4, 5 };
                source.ShouldContain(6);
            });
        }

        [TestMethod]
        public void ICollection_ShouldContainWhereEmptyCollectionFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                var source = new List<int>();
                source.ShouldContain(6);
            });
        }

        [TestMethod]
        public void ICollection_ShouldNotContainPassTest()
        {
            var source = new List<int>() { 1, 2, 3, 4, 5 };
            source.ShouldNotContain(6);

            source = new List<int>();
            source.ShouldNotContain(1);
        }

        [TestMethod]
        public void ICollection_ShouldNotContainFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                var source = new List<int> { 1, 2, 3, 4, 5 };
                source.ShouldNotContain(2);
            });
        }

        [TestMethod]
        public void ICollection_ShouldHaveCountOfPassTest()
        {
            var source = new List<int> { 1, 2, 3, 4, 5 };
            source.ShouldHaveCountOf(5);

            source = new List<int>();
            source.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public void ICollection_ShouldHaveCountOfFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                var source = new List<int> { 1, 2, 3, 4, 5 };
                source.ShouldHaveCountOf(3);
            });
        }
    }
}
