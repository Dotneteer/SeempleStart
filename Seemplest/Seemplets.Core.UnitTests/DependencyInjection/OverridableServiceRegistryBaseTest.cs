using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DependencyInjection;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.DependencyInjection
{
    [TestClass]
    public class OverridableServiceRegistryBaseTest
    {
        [TestMethod]
        public void DefaultServicesAreRegistered()
        {
            // --- Arrange
            var registry = new OverridableSericeRegistry();

            // --- Act
            var service1 = registry.GetService<ISampleService1>();
            var service3 = registry.GetService<ISampleService3>();

            // --- Assert
            registry.GetPresetContainer().GetRegisteredServices().ShouldHaveCountOf(2);
            service1.ShouldBeOfType(typeof (SampleService1));
            service3.ShouldBeOfType(typeof(SampleService3));
        }

        [TestMethod]
        public void ConfigurableContainerIsUsed()
        {
            // --- Arrange
            var registry = new OverridableSericeRegistry();
            var settings = new ServiceContainerSettings(
                "config",
                null,
                new List<MappingSettings>
                    {
                        new MappingSettings(typeof (ISampleService2), typeof (SampleService2)),
                        new MappingSettings(typeof (ISampleService3), typeof (SampleService3A))
                    });
            registry.GetConfigurableContainer().ConfigureFrom(settings);

            // --- Act
            var service1 = registry.GetService<ISampleService1>();
            var service2 = registry.GetService<ISampleService2>();
            var service3 = registry.GetService<ISampleService3>();

            // --- Assert
            registry.GetPresetContainer().GetRegisteredServices().ShouldHaveCountOf(2);
            registry.GetConfigurableContainer().GetRegisteredServices().ShouldHaveCountOf(2);
            service1.ShouldBeOfType(typeof(SampleService1));
            service2.ShouldBeOfType(typeof(SampleService2));
            service3.ShouldBeOfType(typeof(SampleService3A));
        }

        interface ISampleService1
        {
        }

        interface ISampleService2
        {
        }

        interface ISampleService3
        {
        }

        class SampleService1: ISampleService1
        {
        }

        class SampleService2 : ISampleService2
        {
        }

        class SampleService3 : ISampleService3
        {
        }

        class SampleService3A : ISampleService3
        {
        }

        class OverridableSericeRegistry: OverridableServiceRegistryBase
        {
            protected override void SetDefaultServices(ServiceContainer container)
            {
                container.Register<ISampleService1, SampleService1>();
                container.Register<ISampleService3, SampleService3>();
            }
        }
    }
}
