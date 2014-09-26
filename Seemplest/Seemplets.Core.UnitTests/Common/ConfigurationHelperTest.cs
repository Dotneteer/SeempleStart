using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Common;
using Seemplest.Core.Configuration;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Common
{
    [TestClass]
    public class ConfigurationHelperTest
    {
        [TestMethod]
        public void CreateInstanceWithPropertiesWorksAsExpected()
        {
            // --- Arrange
            var settings = new PropertySettingsCollection(
                new List<PropertySettings>
                    {
                        new PropertySettings("X", "12"),
                        new PropertySettings("Y", "23")
                    }
                );

            // --- Act
            var myPoint = ConfigurationHelper.CreateInstance(typeof (MyPoint), settings) as MyPoint;

            // --- Assert
            myPoint.ShouldNotBeNull();
            // ReSharper disable PossibleNullReferenceException
            myPoint.X.ShouldEqual(12);
            // ReSharper restore PossibleNullReferenceException
            myPoint.Y.ShouldEqual(23);
        }

        [TestMethod]
        public void CreateInstanceWithConstructorParamsWorksAsExpected()
        {
            // --- Arrange
            var settings = new ConstructorParameterSettingsCollection(
                new List<ConstructorParameterSettings>
                    {
                        new ConstructorParameterSettings(typeof(int), "12"),
                        new ConstructorParameterSettings(typeof(int), "23"),
                    }
                );

            // --- Act
            var myPoint = ConfigurationHelper.CreateInstance(typeof(MyPoint), settings,
                new PropertySettingsCollection()) as MyPoint;

            // --- Assert
            myPoint.ShouldNotBeNull();
            // ReSharper disable PossibleNullReferenceException
            myPoint.X.ShouldEqual(12);
            // ReSharper restore PossibleNullReferenceException
            myPoint.Y.ShouldEqual(23);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void InjectPropertiesWithInvalidNameFails()
        {
            // --- Arrange
            var settings = new PropertySettingsCollection(
                new List<PropertySettings>
                    {
                        new PropertySettings("X", "12"),
                        new PropertySettings("Dummy", "23")
                    }
                );

            // --- Act
            object myPoint = new MyPoint();
            ConfigurationHelper.InjectProperties(ref myPoint, settings);
        }

        [TestMethod]
        public void InjectPropertieswithEnumWorksAsExpected()
        {
            // --- Arrange
            var settings = new PropertySettingsCollection(
                new List<PropertySettings>
                    {
                        new PropertySettings("Entry", EventLogEntryType.Error.ToString()),
                    }
                );

            // --- Act
            object myEnum = new MyEnum();
            ConfigurationHelper.InjectProperties(ref myEnum, settings);

            // --- Assert
            var asEnum = (MyEnum)myEnum;
            asEnum.Entry.ShouldEqual(EventLogEntryType.Error);
        }

        class MyPoint
        {
            public MyPoint()
            {
            }

            // ReSharper disable UnusedMember.Local
            public MyPoint(int x, int y)
            // ReSharper restore UnusedMember.Local
            {
                X = x;
                Y = y;
            }

            public int X { get; private set; }
            public int Y { get; private set; }
        }

        class MyEnum
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public EventLogEntryType Entry { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }
    }
}
