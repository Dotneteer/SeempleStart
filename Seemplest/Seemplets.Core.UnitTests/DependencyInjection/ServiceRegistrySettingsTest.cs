using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DependencyInjection;
using Seemplest.Core.TypeResolution;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.DependencyInjection
{
    [TestClass]
    public class ServiceRegistrySettingsTest
    {
        [TestMethod]
        public void ReadAndWriteWorksAsExpected1()
        {
            const string CONTAINER_NAME = "containerName";
            const string ROOT = "Root";

            // --- Arrange
            var settings = new ServiceRegistrySettings(CONTAINER_NAME);

            // --- Act
            var element = settings.WriteToXml(ROOT);
            var newSetting = new ServiceRegistrySettings(element);

            // --- Assert
            newSetting.Resolver.ShouldBeNull();
            newSetting.DefaultContainer.ShouldEqual(CONTAINER_NAME);
            newSetting.Containers.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public void ReadAndWriteWorksAsExpected2()
        {
            const string ROOT = "Root";

            // --- Arrange
            var settings = new ServiceRegistrySettings(null, null);

            // --- Act
            var element = settings.WriteToXml(ROOT);
            var newSetting = new ServiceRegistrySettings(element);

            // --- Assert
            newSetting.Resolver.ShouldBeNull();
            newSetting.DefaultContainer.ShouldBeNull();
            newSetting.Containers.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public void ReadAndWriteWorksAsExpected3()
        {
            const string CONTAINER_NAME = "containerName";
            const string ROOT = "Root";

            // --- Arrange
            var settings = new ServiceRegistrySettings(CONTAINER_NAME, null, new DefaultTypeResolver());

            // --- Act
            var element = settings.WriteToXml(ROOT);
            var newSetting = new ServiceRegistrySettings(element);

            // --- Assert
            newSetting.Resolver.ShouldNotBeNull();
            newSetting.DefaultContainer.ShouldEqual(CONTAINER_NAME);
            newSetting.Containers.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public void ReadAndWriteWorksAsExpected4()
        {
            const string ROOT = "Root";

            // --- Arrange
            var containers = new List<ServiceContainerSettings>
                {
                    new ServiceContainerSettings("Container1")
                };
            var settings = new ServiceRegistrySettings(null, containers);

            // --- Act
            var element = settings.WriteToXml(ROOT);
            var newSetting = new ServiceRegistrySettings(element);

            // --- Assert
            newSetting.Resolver.ShouldBeNull();
            newSetting.DefaultContainer.ShouldBeNull();
            newSetting.Containers.ShouldHaveCountOf(1);
        }
    }
}
