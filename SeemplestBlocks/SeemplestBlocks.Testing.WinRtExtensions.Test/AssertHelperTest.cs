using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace SeemplestBlocks.Testing.WinRtExtensions.Test
{
    [TestClass]
    public class AssertHelperTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void AssertFailed_should_throw_argument_exception_if_assertionName_is_null()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                AssertHelper.AssertFailed(null, "test", "test");
            });
        }

        [TestMethod]
        public void AssertFailed_should_throw_argument_exception_if_assertionName_is_empty_string()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                AssertHelper.AssertFailed(String.Empty, "test", "test");
            });
        }

        [TestMethod]
        public void AssertFailed_should_create_assertion_failure()
        {
            const string ASSERTION_NAME = "TestAssertion";
            const string MESSAGE = "TestMessage: {0}";
            const string PARAM = "TestParam";

            var exceptionCaught = false;
            try
            {
                AssertHelper.AssertFailed(ASSERTION_NAME, MESSAGE, PARAM);
            }
            catch (AssertFailedException afe)
            {
                exceptionCaught = true;
                Assert.IsTrue(afe.Message.Contains(ASSERTION_NAME));
                Assert.IsTrue(afe.Message.Contains(String.Format(MESSAGE, PARAM)));
            }
            if (!exceptionCaught)
            {
                Assert.Fail("AssertFailed did not throw an exception as expected.");
            }
        }

        [TestMethod]
        public void AssertHelper_HandleFailTest()
        {
            var exceptionCaught = false;
            try
            {
                AssertHelper.HandleFail("TestAssertion", "Test Message: {0}", "Test Param");
            }
            catch (AssertFailedException afe)
            {
                exceptionCaught = true;
                Assert.IsTrue(afe.Message.Contains("TestAssertion"));
                Assert.IsTrue(afe.Message.Contains("Test Message"));
                Assert.IsTrue(afe.Message.Contains("Test Param"));
            }

            if (!exceptionCaught) Assert.Fail("HandleFail did not throw an exception as expected.");
        }

        [TestMethod]
        public void AssertHelper_AppendMessageTest()
        {
            var actual = "SourceString".AppendMessage("SomeMessage: {0}", "SomeParam");
            const string EXPECTED = "SomeMessage: SomeParam SourceString";
            Assert.AreEqual(EXPECTED, actual);
        }
    }
}
