using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace SeemplestBlocks.Testing.WinRtExtensions.Test
{
    [TestClass]
    public class TestingExceptionsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Testing_ExceptionsPassTest()
        {
            // ReSharper disable once ConvertToConstant.Local
            var zero = 0;
            // ReSharper disable once UnusedVariable
            Testing.ShouldThrowException<DivideByZeroException>(() => { var y = 15 / zero; });
        }

        [TestMethod]
        public void Testing_ExceptionNotActualExceptionFailedTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                // ReSharper disable once ConvertToConstant.Local
                var zero = 0;
                // ReSharper disable once UnusedVariable
                Testing.ShouldThrowException<Exception>(() => { var y = 15/zero; });
            });
        }

        [TestMethod]
        public void Testing_ExceptionDifferentExceptionFailedTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                // ReSharper disable once ConvertToConstant.Local
                var zero = 0;
                // ReSharper disable once UnusedVariable
                Testing.ShouldThrowException<InvalidCastException>(() => { var y = 15 / zero; });
            });
        }

        [TestMethod]
        public void Testing_ExceptionNoExceptionThrownFailedTest()
        {
            Assert.ThrowsException<AssertFailedException>(() =>
            {
                // ReSharper disable once ConvertToConstant.Local
                var divisor = 3;
                // ReSharper disable once UnusedVariable
                Testing.ShouldThrowException<DivideByZeroException>(() => { var y = 15 / divisor; });
            });
        }

        [TestMethod]
        public void Testing_ExceptionWhereActionNullFailedTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                Testing.ShouldThrowException<DivideByZeroException>(null);
            });
        }
    }
}
