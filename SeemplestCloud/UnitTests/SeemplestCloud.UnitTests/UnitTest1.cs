using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeemplestBlocks.Core.Internationalization;
using SoftwareApproach.TestingExtensions;

namespace SeemplestCloud.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SetAndGetImplementedCulturesWorksAsExpected()
        {
            // --- Act
            CultureHelper.SetImplementedCultures(new List<string>
            {
                "hu-HU", "de-DE", "en-US"
            });

            // --- Assert
            CultureHelper.GetImplementedCultures().ShouldHaveCountOf(3);
        }

        [TestMethod]
        public void SetAndGetImplementedCulturesWorksAsExpectedWithInvalidCulture()
        {
            // --- Act
            CultureHelper.SetImplementedCultures(new List<string>
            {
                "hu-HU", "dummy", "en-US"
            });

            // --- Assert
            CultureHelper.GetImplementedCultures().ShouldHaveCountOf(2);
        }

        [TestMethod]
        public void GetEnglishDisplayNameWorksAsExpected()
        {
            // --- Arrange
            CultureHelper.SetImplementedCultures(new List<string>
            {
                "hu-HU", "de-DE", "en-US"
            });

            // --- Act
            var huName = CultureHelper.GetImplementedCultureDisplayName("hu");
            var deName = CultureHelper.GetImplementedCultureDisplayName("de");
            var enName = CultureHelper.GetImplementedCultureDisplayName("en");

            // --- Assert
            huName.ShouldEqual("Hungarian");
            deName.ShouldEqual("German");
            enName.ShouldEqual("English");
        }
    }
}
