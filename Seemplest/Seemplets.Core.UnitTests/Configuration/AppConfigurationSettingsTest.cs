using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Configuration
{
    [TestClass]
    public class AppConfigurationSettingsTest
    {
        [TestMethod]
        public void WriteXmlAndReadXmlWorksAsExpected()
        {
            // --- Act
            var settings = new AppConfigurationSettings(
                typeof (AppConfigProvider), null, null,
                "Prefix", "Name");

            var element = settings.WriteToXml("Temp");
            var newSetting = new AppConfigurationSettings(element);

            // --- Assert
            newSetting.Provider.ShouldEqual(typeof (AppConfigProvider));
            newSetting.InstancePrefix.ShouldEqual("Prefix");
            newSetting.InstanceName.ShouldEqual("Name");
            newSetting.ConstructorParameters.ShouldHaveCountOf(0);
            newSetting.Properties.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public void WriteXmlAndReadXmlWorksAsExpectedWithEmptyProperties()
        {
            // --- Act
            var settings = new AppConfigurationSettings(null, null);

            var element = settings.WriteToXml("Temp");
            var newSetting = new AppConfigurationSettings(element);

            // --- Assert
            newSetting.Provider.ShouldEqual(typeof(AppConfigProvider));
            newSetting.InstancePrefix.ShouldEqual("");
            newSetting.InstanceName.ShouldEqual("");
            newSetting.ConstructorParameters.ShouldHaveCountOf(0);
            newSetting.Properties.ShouldHaveCountOf(0);
        }
    }
}
