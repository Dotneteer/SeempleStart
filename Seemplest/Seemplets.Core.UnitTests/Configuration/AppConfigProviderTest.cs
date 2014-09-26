using System.Collections.Generic;
using System.Configuration;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration;
using Seemplest.Core.TypeResolution;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Configuration
{
    [TestClass]
    public class AppConfigProviderTest
    {
        [TestCleanup]
        public void Cleanup()
        {
            new AppConfigProvider().SetValue("setting1", "value1");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void GetSettingValueRaisesExceptionWithMissingSection()
        {
            // --- Arrange
            const string KEY = "missingSection";
            var provider = new AppConfigProvider();

            // --- Act
            provider.GetSetting<AppConfigurationSettings>(KEY);
        }


        [TestMethod]
        public void SetSettingValueWorksAsExpected()
        {
            // --- Arrange
            const string KEY = "setting1";
            const string VALUE = "testValue";
            var provider = new AppConfigProvider();

            // --- Act
            provider.SetValue(KEY, VALUE);

            // --- Assert
            ConfigurationManager.RefreshSection("appSettings");
            var value = ConfigurationManager.AppSettings[KEY];
            value.ShouldEqual(VALUE);
        }

        [TestMethod]
        public void SetSettingValueWorksWithNewSettingName()
        {
            // --- Arrange
            const string KEY = "newTestSetting";
            const string VALUE = "testValue";
            var provider = new AppConfigProvider();

            // --- Act
            provider.SetValue(KEY, VALUE);

            // --- Assert
            ConfigurationManager.RefreshSection("appSettings");
            var value = ConfigurationManager.AppSettings[KEY];
            value.ShouldEqual(VALUE);
        }

        [TestMethod]
        public void SetSettingsWorksAsExpected()
        {
            // --- Arrange
            const string KEY = "TypeResolverBackup";
            var settings = new TypeResolverConfigurationSettings(new List<string> { "System" },
                                                                 new List<string> { "TypeResolution" });
            var provider = new AppConfigProvider();

            // --- Act
            provider.SetSetting(KEY, settings);

            // --- Assert
            ConfigurationManager.RefreshSection(KEY);
            var config = ConfigurationManager.GetSection(KEY) as XElement;
            config.ShouldNotBeNull();
            settings = new TypeResolverConfigurationSettings(config);
            settings.AssemblyNames.ShouldHaveCountOf(1);
            settings.AssemblyNames[0].ShouldEqual("System");
            settings.Namespaces.ShouldHaveCountOf(1);
            settings.Namespaces[0].ShouldEqual("TypeResolution");
        }

        [TestMethod]
        public void IsSettingValueDefinedWorksAsExpected()
        {
            // --- Arrange
            var provider = new AppConfigProvider();

            // --- Act
            var test1 = provider.IsValueDefined("setting1");
            var test2 = provider.IsValueDefined("nonExisting");

            // --- Assert
            test1.ShouldBeTrue();
            test2.ShouldBeFalse();
        }

        [TestMethod]
        public void IsSeectionDefinedWorksAsExpected()
        {
            // --- Arrange
            var provider = new AppConfigProvider();

            // --- Act
            var test1 = provider.IsSettingDefined("TypeResolver");
            var test2 = provider.IsSettingDefined("nonExisting");

            // --- Assert
            test1.ShouldBeTrue();
            test2.ShouldBeFalse();
        }
    }
}
