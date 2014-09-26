using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration;
using Seemplest.Core.DependencyInjection;
using SoftwareApproach.TestingExtensions;
using Seemplest.Core.TypeResolution;

namespace Seemplest.Core.UnitTests.DependencyInjection
{
    [TestClass]
    public class ServiceContainerSettingsTest
    {
        [TestMethod]
        public void ReadAndWriteWorksAsExpected()
        {
            // --- Arrange
            var settings = new ServiceContainerSettings("container", "parent",
                new List<MappingSettings>
                    {
                        new MappingSettings(typeof(ISampleService), typeof(SampleService)),
                        new MappingSettings(typeof(ISampleRepository), typeof(SampleRepository),
                            typeof(SingletonLifetimeManager),
                            new InjectedParameterSettingsCollection(new List<InjectedParameterSettings>
                                {
                                    new InjectedParameterSettings(typeof(int), "12"),
                                    new InjectedParameterSettings(typeof(string), "Hello")
                                }),
                             new PropertySettingsCollection(new List<PropertySettings>
                                 {
                                     new PropertySettings("Prop1", "value1"),
                                     new PropertySettings("Prop2", "value2")
                                 }))
                    });

            // --- Act
            var element = settings.WriteToXml("Temp");
            var newSettings = new ServiceContainerSettings(element);

            // --- Assert
            newSettings.Mappings.ShouldHaveCountOf(2);
            newSettings.Mappings[0].From.ShouldEqual(typeof (ISampleService));
            newSettings.Mappings[0].To.ShouldEqual(typeof(SampleService));
            newSettings.Mappings[0].Lifetime.ShouldBeNull();
            newSettings.Mappings[0].Parameters.ShouldBeNull();
            newSettings.Mappings[0].Properties.ShouldBeNull();
            newSettings.Mappings[1].From.ShouldEqual(typeof(ISampleRepository));
            newSettings.Mappings[1].To.ShouldEqual(typeof(SampleRepository));
            newSettings.Mappings[1].Lifetime.ShouldEqual(typeof(SingletonLifetimeManager));
            newSettings.Mappings[1].Parameters.ShouldHaveCountOf(2);
            newSettings.Mappings[1].Properties.ShouldHaveCountOf(2);
        }

        [TestMethod]
        public void LifetimeManagerResolutionWorksAsExpected()
        {
            // --- Arrange
            const string SOURCE = @"<Temp name='container' parent='parent'>
                             <Map from='Seemplest.Core.UnitTests.DependencyInjection.ISampleRepository, Seemplest.Core.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' 
                                to='Seemplest.Core.UnitTests.DependencyInjection.SampleRepository, Seemplest.Core.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' 
                                lifetime='PerCall'>
                             </Map>
                             <Map from='Seemplest.Core.UnitTests.DependencyInjection.ISampleRepository, Seemplest.Core.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' 
                                to='Seemplest.Core.UnitTests.DependencyInjection.SampleRepository, Seemplest.Core.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' 
                                lifetime='PerThread'>
                             </Map>
                             <Map from='Seemplest.Core.UnitTests.DependencyInjection.ISampleRepository, Seemplest.Core.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' 
                                to='Seemplest.Core.UnitTests.DependencyInjection.SampleRepository, Seemplest.Core.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' 
                                lifetime='Singleton'>
                             </Map>
                           </Temp>";
            var element = XElement.Parse(SOURCE);

            // --- Act
            var container1 = new ServiceContainerSettings(element);
            var container2 = new ServiceContainerSettings(element, TypeResolver.Current);

            // --- Assert
            container1.Mappings.ShouldHaveCountOf(3);
            container1.Mappings[0].Lifetime.ShouldEqual(typeof (PerCallLifetimeManager));
            container1.Mappings[1].Lifetime.ShouldEqual(typeof (PerThreadLifetimeManager));
            container1.Mappings[2].Lifetime.ShouldEqual(typeof (SingletonLifetimeManager));

            container2.Mappings.ShouldHaveCountOf(3);
            container2.Mappings[0].Lifetime.ShouldEqual(typeof(PerCallLifetimeManager));
            container2.Mappings[1].Lifetime.ShouldEqual(typeof(PerThreadLifetimeManager));
            container2.Mappings[2].Lifetime.ShouldEqual(typeof(SingletonLifetimeManager));
        }
    }
}
