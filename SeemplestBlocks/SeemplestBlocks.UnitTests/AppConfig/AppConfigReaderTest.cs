using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeemplestBlocks.Core.AppConfig;
using SoftwareApproach.TestingExtensions;

namespace SeemplestBlocks.UnitTests.AppConfig
{
    [TestClass]
    public class AppConfigReaderTest
    {
        [TestMethod]
        public void GetConfigurationValueWorksAsExpected()
        {
            // --- Arrange
            var handler = new AppConfigReader();

            // --- Act
            string value1;
            var found1 = handler.GetConfigurationValue("NonExistingCategory", "key", out value1);
            string value2;
            var found2 = handler.GetConfigurationValue("Category1", "NonExistingKey", out value2);
            string value3;
            var found3 = handler.GetConfigurationValue("Category1", "Key1", out value3);

            // --- Assert
            found1.ShouldBeFalse();
            found2.ShouldBeFalse();
            found3.ShouldBeTrue();
            value3.ShouldEqual("123");
        }

    }
}
