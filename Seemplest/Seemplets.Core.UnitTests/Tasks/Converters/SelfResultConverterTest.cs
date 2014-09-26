using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Tasks.Converters;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Tasks.Converters
{
    [TestClass]
    public class SelfResultConverterTest
    {
        [TestMethod]
        public void ConvertToResultWorksAsExpected()
        {
            // --- Arrange
            const string VALUE = "Message";
            var converter = new SelfResultConverter();

            // --- Act
            var converted = converter.ConvertToResult(VALUE);

            // --- Assert
            converted.ShouldEqual(VALUE);
        }
    }
}
