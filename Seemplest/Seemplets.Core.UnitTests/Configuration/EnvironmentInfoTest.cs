using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Configuration
{
    [TestClass]
    public class EnvironmentInfoTest
    {
        [TestMethod]
        public void EnvironmentInfoResetsToDefault()
        {
            // --- Act
            EnvironmentInfo.Reset();

            // --- Assert
            EnvironmentInfo.Provider.ShouldBeOfType(typeof (DefaultEnvironmentInfoProvider));
        }

        [TestMethod]
        public void GetCurrentDateTimeUtcWorksAsExpected()
        {
            // --- Arrange
            EnvironmentInfo.Reset();

            // --- Act
            var utcNow = EnvironmentInfo.GetCurrentDateTimeUtc();

            // --- Assert
            ((DateTime.UtcNow - utcNow) < TimeSpan.FromMilliseconds(100)).ShouldBeTrue();
        }

        [TestMethod]
        public void GetMachineNameWorksAsExpected()
        {
            // --- Arrange
            EnvironmentInfo.Reset();

            // --- Act
            var machineName = EnvironmentInfo.GetMachineName();

            // --- Assert
            machineName.ShouldEqual(Environment.MachineName);
        }

        [TestMethod]
        public void ConfigureWorksAsExpected()
        {
            // --- Arrange
            EnvironmentInfo.Reset();

            // --- Act
            EnvironmentInfo.Configure(new MockEnvironpentInfoProvider());
            var utcNow = EnvironmentInfo.GetCurrentDateTimeUtc();
            var machineName = EnvironmentInfo.GetMachineName();

            // --- Assert
            EnvironmentInfo.Provider.ShouldBeOfType(typeof (MockEnvironpentInfoProvider));
            utcNow.ShouldEqual(new DateTime(2012, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            machineName.ShouldEqual("DummyMachine");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConfigureWithNullRaisesException()
        {
            // --- Arrange
            EnvironmentInfo.Reset();

            // --- Act
            EnvironmentInfo.Configure(null);
        }


        class MockEnvironpentInfoProvider: IEnvironmentInfoProvider
        {
            public DateTime GetCurrentDateTimeUtc()
            {
                return new DateTime(2012, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            }

            public string GetMachineName()
            {
                return "DummyMachine";
            }
        }
    }
}
