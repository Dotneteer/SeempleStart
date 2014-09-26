using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Tasks.Converters;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Tasks.Converters
{
    [TestClass]
    public class SelfArgumentConverterTest
    {
        [TestMethod]
        public void ConvertToArgumentWorksAsExpected()
        {
            // --- Arrange
            const string VALUE = "Message";
            var converter = new SelfArgumentConverter();

            // --- Act
            var converted = converter.ConvertToArgument(VALUE);

            // --- Assert
            converted.ShouldEqual(VALUE);
        }
    }
}
