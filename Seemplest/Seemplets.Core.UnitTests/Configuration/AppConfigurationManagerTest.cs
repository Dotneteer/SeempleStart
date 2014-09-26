using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Common;
using Seemplest.Core.Configuration;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Configuration
{
    [TestClass]
    public class AppConfigurationManagerTest
    {
        private bool _configurationChangedVisited;
        private IConfigurationProvider _before;
        private IConfigurationProvider _after;


        [TestInitialize]
        public void Intialize()
        {
            _configurationChangedVisited = false;
            _before = null;
            _after = null;
            AppConfigurationManager.Reset();
            AppConfigurationManager.ConfigurationProviderChanged += AppConfigurationManagerOnConfigurationProviderChanged;
            SetDefaultSectionName("AppConfiguration");
        }

        [TestCleanup]
        public void Cleanup()
        {
            AppConfigurationManager.ConfigurationProviderChanged -= AppConfigurationManagerOnConfigurationProviderChanged;
            AppConfigurationManager.Reset();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConfigurationFailsWithNullProvider()
        {
            // --- Act
            AppConfigurationManager.Configure((IConfigurationProvider)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConfigurationFailsWithNullSettings()
        {
            // --- Act
            AppConfigurationManager.Configure((AppConfigurationSettings)null);
        }

        [TestMethod]
        public void ConfigurationWorksWithSettings()
        {
            // --- Arrange
            var settings = new AppConfigurationSettings(typeof (TestConfigProvider),
                new ConstructorParameterSettingsCollection(),
                new PropertySettingsCollection(),
                "Prefix", "Name");

            // --- Act
            var before = AppConfigurationManager.CurrentProvider;
            AppConfigurationManager.Configure(settings);
            var after = AppConfigurationManager.CurrentProvider;

            // --- Assert
            _configurationChangedVisited.ShouldBeTrue();
            before.ShouldBeOfType(typeof(AppConfigProvider));
            after.ShouldBeOfType(typeof(TestConfigProvider));
            before.ShouldEqual(_before);
            after.ShouldEqual(_after);
        }

        [TestMethod]
        public void ConfigurationWorksWithSectionName()
        {
            // --- Act
            var before = AppConfigurationManager.CurrentProvider;
            AppConfigurationManager.Configure("AppConfiguration1");
            var after = AppConfigurationManager.CurrentProvider;

            // --- Assert
            _configurationChangedVisited.ShouldBeTrue();
            before.ShouldBeOfType(typeof(AppConfigProvider));
            after.ShouldBeOfType(typeof(AppConfigProvider));
            before.ShouldEqual(_before);
            after.ShouldEqual(_after);
        }

        [TestMethod]
        public void ConfigurationWorksWithNullSectionName()
        {
            // --- Act
            var before = AppConfigurationManager.CurrentProvider;
            AppConfigurationManager.Configure();
            var after = AppConfigurationManager.CurrentProvider;

            // --- Assert
            _configurationChangedVisited.ShouldBeTrue();
            before.ShouldBeOfType(typeof(AppConfigProvider));
            after.ShouldBeOfType(typeof(AppConfigProvider));
            before.ShouldEqual(_before);
            after.ShouldEqual(_after);
        }

        [TestMethod]
        public void ConfigurationWorksWithMissingAppConfigSection()
        {
            // --- Arrange
            var settings = new AppConfigurationSettings(typeof(TestConfigProvider),
                new ConstructorParameterSettingsCollection(),
                new PropertySettingsCollection(),
                "Prefix", "Name");
            SetDefaultSectionName("Missing");

            // --- Act
            var before = AppConfigurationManager.CurrentProvider;
            AppConfigurationManager.Configure(settings);
            var after = AppConfigurationManager.CurrentProvider;

            // --- Assert
            _configurationChangedVisited.ShouldBeTrue();
            before.ShouldBeOfType(typeof(AppConfigProvider));
            after.ShouldBeOfType(typeof(TestConfigProvider));
            before.ShouldEqual(_before);
            after.ShouldEqual(_after);
        }

        [TestMethod]
        public void GetSettingValueWorksWithDefaultProvider()
        {
            // --- Act
            var value1 = AppConfigurationManager.GetSettingValue("setting1");
            var value2 = AppConfigurationManager.GetSettingValue("setting2");

            // --- Assert
            value1.ShouldEqual("value1");
            value2.ShouldBeNull();
        }

        [TestMethod]
        public void GetSettingValueWorksWithCustomProvider()
        {
            // --- Arrange
            var newSettings = new AppConfigurationSettings(
               typeof(TestConfigProvider), null, null, "instance1", "name");
            AppConfigurationManager.Configure(newSettings);

            // --- Act
            var value1 = AppConfigurationManager.GetSettingValue("key1");
            var value2 = AppConfigurationManager.GetSettingValue("key2");
            var value3 = AppConfigurationManager.GetSettingValue("key3");

            // --- Assert
            value1.ShouldEqual("value1");
            value2.ShouldEqual("value2");
            value3.ShouldBeNull();
        }

        [TestMethod]
        public void GetSettingValueWorks()
        {
            // --- Act
            var value = AppConfigurationManager.GetSettingValue<string>("setting1");

            // --- Assert
            value.ShouldEqual("value1");
        }

        [TestMethod]
        public void IsSettingValueDefinedWorksAsExpected()
        {
            // --- Act
            var test1 = AppConfigurationManager.IsSettingValueDefined("setting1");
            var test2 = AppConfigurationManager.IsSettingValueDefined("nonExisting");

            // --- Assert
            test1.ShouldBeTrue();
            test2.ShouldBeFalse();
        }

        [TestMethod]
        public void IsSeectionDefinedWorksAsExpected()
        {
            // --- Act
            var test1 = AppConfigurationManager.IsSectionDefined("TypeResolver");
            var test2 = AppConfigurationManager.IsSectionDefined("nonExisting");

            // --- Assert
            test1.ShouldBeTrue();
            test2.ShouldBeFalse();
        }

        private void AppConfigurationManagerOnConfigurationProviderChanged(object sender,
        ConfigurationChangedEventArgs<IConfigurationProvider> configurationChangedEventArgs)
        {
            _configurationChangedVisited = true;
            _before = configurationChangedEventArgs.OldValue;
            _after = configurationChangedEventArgs.NewValue;
        }

        private void SetDefaultSectionName(string value)
        {
            var fieldInfo = typeof (AppConfigurationManager)
                .GetField("s_DefaultSectionName", BindingFlags.Static | BindingFlags.NonPublic);
            // ReSharper disable PossibleNullReferenceException
            fieldInfo.SetValue(null, value);
            // ReSharper restore PossibleNullReferenceException
        }
    }

    internal interface IService1 { }

    internal class Service1 : IService1 { }

    internal class TestConfigProvider: IConfigurationProvider
    {
        private readonly Dictionary<string, string> _values = new Dictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"},
            };

        public string GetValue(string settingKey)
        {
            string value;
            return _values.TryGetValue(settingKey, out value) ? value : null;
        }

        public TSetting GetSetting<TSetting>(string settingKey) where TSetting : IXElementRepresentable
        {
            throw new NotImplementedException();
        }

        public void SetValue(string settingKey, string value)
        {
            throw new NotImplementedException();
        }

        public void SetSetting<TSetting>(string settingKey, TSetting value) where TSetting : IXElementRepresentable
        {
            throw new NotImplementedException();
        }

        public bool IsValueDefined(string settingKey)
        {
            throw new NotImplementedException();
        }

        public bool IsSettingDefined(string settingKey)
        {
            throw new NotImplementedException();
        }
    }  
}
