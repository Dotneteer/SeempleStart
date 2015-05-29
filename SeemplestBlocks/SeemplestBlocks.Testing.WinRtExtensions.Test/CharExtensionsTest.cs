using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace SeemplestBlocks.Testing.WinRtExtensions.Test
{
    [TestClass]
    public class CharExtensionsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Char_ShouldBeGreaterThanPassTest()
        {
            ('a').ShouldBeGreaterThan('Z');
            ('z').ShouldBeGreaterThan('a');
        }

        [TestMethod]
        public void Char_ShouldBeGreaterThanFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ('A').ShouldBeGreaterThan('B');
            });
        }

        [TestMethod]
        public void Char_ShouldBeGreaterThanFailWhereEqualTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ('A').ShouldBeGreaterThan('A');
            });
        }

        [TestMethod]
        public void Char_ShouldBeGreaterThanOrEqualToPassTest()
        {
            ('B').ShouldBeGreaterThanOrEqualTo('A');
            ('B').ShouldBeGreaterThanOrEqualTo('B');
        }

        [TestMethod]
        public void Char_ShouldBeGreaterThanOrEqualToFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ('A').ShouldBeGreaterThanOrEqualTo('B');
            });
        }

        [TestMethod]
        public void Char_ShouldBeLessThanPassTest()
        {
            ('Z').ShouldBeLessThan('a');
            ('a').ShouldBeLessThan('z');
        }

        [TestMethod]
        public void Char_ShouldBeLessThanFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ('B').ShouldBeLessThan('A');
            });
        }

        [TestMethod]
        public void Char_ShouldBeLessThanFailWhereEqualTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ('A').ShouldBeLessThan('A');
            });
        }

        [TestMethod]
        public void Char_ShouldBeLessThanOrEqualToPassTest()
        {
            ('B').ShouldBeLessThanOrEqualTo('C');
            ('B').ShouldBeLessThanOrEqualTo('B');
        }

        [TestMethod]
        public void Char_ShouldBeLessThanOrEqualToFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ('B').ShouldBeLessThanOrEqualTo('A');
            });
        }

        [TestMethod]
        public void Char_ShouldBeBetweenPassTest()
        {
            ('B').ShouldBeBetween('A', 'E');
            ('A').ShouldBeBetween('A', 'E');
            ('D').ShouldBeBetween('A', 'E');
            ('E').ShouldBeBetween('A', 'E');
        }

        [TestMethod]
        public void Char_ShouldBeBetweenFailWhereLowerGreaterThanUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ('B').ShouldBeBetween('B', 'A');
            });
        }

        [TestMethod]
        public void Char_ShouldBeBetweenFailWhereLowerEqualToUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ('B').ShouldBeBetween('B', 'B');
            });
        }

        [TestMethod]
        public void Char_ShouldBeBetweenFailWhereLessThanLowerTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ('A').ShouldBeBetween('B', 'E');
            });
        }

        [TestMethod]
        public void Char_ShouldBeBetweenFailWhereGreaterThanUpperTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ('F').ShouldBeBetween('B', 'E');
            });
        }

        [TestMethod]
        public void Char_ShouldNotBeBetweenPassTest()
        {
            ('B').ShouldNotBeBetween('C', 'F');
            ('G').ShouldNotBeBetween('C', 'F');
        }

        [TestMethod]
        public void Char_ShouldNotBeBetweenFailWhereLowerGreaterThanUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ('G').ShouldNotBeBetween('C', 'B');
            });
        }

        [TestMethod]
        public void Char_ShouldNotBeBetweenFailWhereLowerEqualToUpperTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                ('G').ShouldNotBeBetween('C', 'C');
            });
        }

        [TestMethod]
        public void Char_ShouldNotBeBetweenFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ('E').ShouldNotBeBetween('C', 'F');
            });
        }

        [TestMethod]
        public void Char_ShouldEqualIgnoringCasePassTest()
        {
            ('a').ShouldEqualIgnoringCase('A');
            ('A').ShouldEqualIgnoringCase('a');
        }

        [TestMethod]
        public void Char_ShouldEqualIgnoringCaseFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ('a').ShouldEqualIgnoringCase('B');
            });
        }

        [TestMethod]
        public void Char_ShouldNotEqualIgnoringCasePassTest()
        {
            ('a').ShouldNotEqualIgnoringCase('B');
            ('A').ShouldNotEqualIgnoringCase('b');
        }

        [TestMethod]
        public void Char_ShouldNotEqualIgnoringCaseFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ('a').ShouldNotEqualIgnoringCase('A');
            });
        }

        [TestMethod]
        public void Char_ShouldBePrintablePassTest()
        {
            ('a').ShouldBePrintable();
            (' ').ShouldBePrintable();
            ('/').ShouldBePrintable();
        }

        [TestMethod]
        public void Char_ShouldBePrintableFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ('\t').ShouldBePrintable();
            });
        }

        [TestMethod]
        public void Char_ShouldNotBePrintablePassTest()
        {
            ((char)2).ShouldNotBePrintable();
            ((char)140).ShouldNotBePrintable();
            ('\t').ShouldNotBePrintable();
        }

        [TestMethod]
        public void Char_ShouldNotBePrintableFailTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                ('a').ShouldNotBePrintable();
            });
        }
    }
}
