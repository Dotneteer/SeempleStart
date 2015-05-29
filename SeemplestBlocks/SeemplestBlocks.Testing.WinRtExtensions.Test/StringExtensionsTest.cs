using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace SeemplestBlocks.Testing.WinRtExtensions.Test
{
    [TestClass]
    public class StringExtensionsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void String_ShouldEqualIgnoringCasePassTest()
        {
            "test".ShouldEqualIgnoringCase("TeSt");
            "test".ShouldEqualIgnoringCase("test");
        }

        [TestMethod]
        public void String_ShouldEqualIgnoringCaseFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                "test".ShouldEqualIgnoringCase("test1");
            });
        }

        [TestMethod]
        public void String_ShouldNotEqualIgnoringCasePassTest()
        {
            "test".ShouldNotEqualIgnoringCase("Test1");
            "".ShouldNotEqualIgnoringCase("test");
        }

        [TestMethod]
        public void String_ShouldNotEqualIgnoringCaseFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                "test".ShouldNotEqualIgnoringCase("Test");
            });
        }

        [TestMethod]
        public void String_ShouldBeEmptyPassTest()
        {
            "".ShouldBeEmpty();
            String.Empty.ShouldBeEmpty();
        }

        [TestMethod]
        public void String_ShouldBeEmptyFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                "test".ShouldBeEmpty();
            });
        }


        [TestMethod]
        public void String_ShouldNotBeEmptyPassTest()
        {
            "test".ShouldNotBeEmpty();
            ((string)null).ShouldNotBeEmpty();
        }

        [TestMethod]
        public void String_ShouldNotBeEmptyFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                "".ShouldNotBeEmpty();
            });
        }

        [TestMethod]
        public void String_ShouldContainPassTest()
        {
            "test".ShouldContain("es");
            "test".ShouldContain("te");
        }

        [TestMethod]
        public void String_ShouldContainFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                "test".ShouldContain("foo");
            });
        }

        [TestMethod]
        public void String_ShouldContainIgnoringCasePassTest()
        {
            "test".ShouldContainIgnoringCase("es");
            "test".ShouldContainIgnoringCase("TE");
            "test".ShouldContainIgnoringCase("eSt");
        }

        [TestMethod]
        public void String_ShouldContainIgnoringCaseFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                "test".ShouldContainIgnoringCase("foo");
            });
        }

        [TestMethod]
        public void String_ShouldNotContainPassTest()
        {
            "test".ShouldNotContain("foo");
            "test".ShouldNotContain("TE");
        }

        [TestMethod]
        public void String_ShouldNotContainFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                "test".ShouldNotContain("te");
            });
        }

        [TestMethod]
        public void String_ShouldJNotContainIgnoringCasePassTest()
        {
            "test".ShouldNotContainIgnoringCase("foo");
        }

        [TestMethod]
        public void String_ShouldNotContainIgnoringCaseFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                "test".ShouldNotContainIgnoringCase("TE");
            });
        }

        [TestMethod]
        public void String_ShouldHaveLengthOfPassTest()
        {
            "test".ShouldHaveLengthOf(4);
            String.Empty.ShouldHaveLengthOf(0);
        }

        [TestMethod]
        public void String_ShouldHaveLengthOfFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                "test".ShouldHaveLengthOf(5);
            });
        }

        [TestMethod]
        public void String_ShouldNotHaveLengthOfPassTest()
        {
            "test".ShouldNotHaveLengthOf(3);
            String.Empty.ShouldNotHaveLengthOf(1);
        }

        [TestMethod]
        public void String_ShouldNotHaveLengthOfFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                "test".ShouldNotHaveLengthOf(4);
            });
        }

        [TestMethod]
        public void String_ShouldHaveLengthOfAtLeastPassTest()
        {
            "test".ShouldHaveLengthOfAtLeast(3);
            String.Empty.ShouldHaveLengthOfAtLeast(0);
        }

        [TestMethod]
        public void String_ShouldHaveLengthOfAtLeastFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                "test".ShouldHaveLengthOfAtLeast(5);
            });
        }

        [TestMethod]
        public void String_ShouldHaveLengthOfAtMostPassTest()
        {
            "test".ShouldHaveLengthOfAtMost(5);
            String.Empty.ShouldHaveLengthOfAtMost(2);
        }

        [TestMethod]
        public void String_ShouldHaveLengthOfAtMostFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                "test".ShouldHaveLengthOfAtMost(3);
            });
        }
    }
}
