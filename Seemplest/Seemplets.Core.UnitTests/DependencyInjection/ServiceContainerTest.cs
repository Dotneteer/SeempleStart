using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DependencyInjection;
using Seemplest.Core.Exceptions;
using Seemplest.Core.Interception;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.DependencyInjection
{
    [TestClass]
    public class ServiceContainerTest
    {
        [TestMethod]
        public void RegisterWorksWithSimpleParameters()
        {
            // --- Arrange
            var container = new ServiceContainer();
            
            // --- Act
            container.Register(typeof(ISampleService), typeof(SampleService));

            // --- Assert
            container.GetRegisteredServices().ShouldHaveCountOf(1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterFailsWithNullContainerName()
        {
            // --- Arrange
            var container = new ServiceContainer(null);

            // --- Act
            container.Register(null, typeof(SampleService));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterFailsWithNullServiceType()
        {
            // --- Arrange
            var container = new ServiceContainer();

            // --- Act
            container.Register(null, typeof(SampleService));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterFailsWithNullServiceObjectType()
        {
            // --- Arrange
            var container = new ServiceContainer();

            // --- Act
            container.Register(typeof(ISampleService), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceAlreadyRegisteredException))]
        public void RegisterFailsWithDoubleRegistration()
        {
            // --- Arrange
            var container = new ServiceContainer();

            // --- Act
            container.Register(typeof(ISampleService), typeof(SampleService));
            container.Register(typeof(ISampleService), typeof(int));
        }

        [TestMethod]
        public void RemoveServiceWorksAsExpected()
        {
            // --- Arrange
            var container = new ServiceContainer();

            // --- Act
            container.Register(typeof(ISampleService), typeof(SampleService));
            var countBefore = container.GetRegisteredServices().Count;
            container.RemoveService(typeof(ISampleService));
            var countAfter = container.GetRegisteredServices().Count;
            container.Register(typeof(ISampleService), typeof(SampleService));
            var reRegisterCount = container.GetRegisteredServices().Count;

            // --- Assert
            countBefore.ShouldEqual(1);
            countAfter.ShouldEqual(0);
            reRegisterCount.ShouldEqual(1);
        }

        [TestMethod]
        public void GetServiceWorksWithDefaultConstructor()
        {
            // --- Arrange
            var container = new ServiceContainer();

            // --- Act
            container.Register(typeof(ISampleService), typeof(SampleService));
            var serviceObject = container.GetService(typeof (ISampleService));

            // --- Assert
            serviceObject.ShouldNotBeNull();
            serviceObject.ShouldBeOfType(typeof (SampleService));
        }

        [TestMethod]
        public void MultipleResolutionProducesTheSameResult()
        {
            // --- Arrange
            var container = new ServiceContainer();

            // --- Act
            container.Register(typeof(ISampleService), typeof(SampleService));
            var serviceObject1 = container.GetService(typeof(ISampleService));
            var serviceObject2 = container.GetService(typeof(ISampleService));

            // --- Assert
            serviceObject1.ShouldNotBeNull();
            serviceObject1.ShouldBeOfType(typeof(SampleService));
            serviceObject2.ShouldNotBeNull();
            serviceObject2.ShouldBeOfType(typeof(SampleService));
        }

        [TestMethod]
        public void GetServiceWorksWithOneConstructorArgument()
        {
            // --- Arrange
            var container = new ServiceContainer();

            // --- Act
            container.Register(
                typeof(ISampleService), 
                typeof(SampleService),
                new InjectedParameterSettingsCollection(new List<InjectedParameterSettings>
                    {
                        new InjectedParameterSettings(typeof(int), "12")
                    }));
            var serviceObject = container.GetService<ISampleService>();
            var result = serviceObject.Operation();

            // --- Assert
            serviceObject.ShouldNotBeNull();
            serviceObject.ShouldBeOfType(typeof(SampleService));
            result.ShouldEqual("12");
        }

        [TestMethod]
        public void GetServiceWorksWithTwoConstructorArgument()
        {
            // --- Arrange
            var container = new ServiceContainer();

            // --- Act
            container.Register(
                typeof(ISampleService),
                typeof(SampleService),
                new InjectedParameterSettingsCollection(new List<InjectedParameterSettings>
                    {
                        new InjectedParameterSettings(typeof(int), "12"),
                        new InjectedParameterSettings(typeof(string), "Monkies")
                    }));
            var serviceObject = container.GetService<ISampleService>();
            var result = serviceObject.Operation();

            // --- Assert
            serviceObject.ShouldNotBeNull();
            serviceObject.ShouldBeOfType(typeof(SampleService));
            result.ShouldEqual("12Monkies");
        }

        [TestMethod]
        public void RegisterAndGetServiceWorksWithTwoConstructorArgument()
        {
            // --- Arrange
            var container = new ServiceContainer();

            // --- Act
            container.Register<ISampleService, SampleService>(12, "Monkies");
            var serviceObject = container.GetService<ISampleService>();
            var result = serviceObject.Operation();

            // --- Assert
            serviceObject.ShouldNotBeNull();
            serviceObject.ShouldBeOfType(typeof(SampleService));
            result.ShouldEqual("12Monkies");
        }

        [TestMethod]
        [ExpectedException(typeof(NoMatchingConstructorException))]
        public void GetServiceFailsWithNoMatchingConstructor()
        {
            // --- Arrange
            var container = new ServiceContainer();

            // --- Act
            container.Register(
                typeof(ISampleService),
                typeof(SampleService),
                new InjectedParameterSettingsCollection(new List<InjectedParameterSettings>
                    {
                        new InjectedParameterSettings(typeof(bool), "True")
                    }));
            container.GetService<ISampleService>();
        }

        [TestMethod]
        [ExpectedException(typeof(AmbigousConstructorException))]
        public void GetServiceFailsWithMultipleMatchingConstructor()
        {
            // --- Arrange
            var container = new ServiceContainer();

            // --- Act
            container.Register(
                typeof(ISampleService),
                typeof(SampleService),
                new InjectedParameterSettingsCollection(new List<InjectedParameterSettings>
                    {
                        new InjectedParameterSettings(typeof(string), "Monkies"),
                        new InjectedParameterSettings(typeof(int), "42"),
                    }));
            container.GetService<ISampleService>();
        }

        [TestMethod]
        public void GetServiceWorksWithOneResolvedArgument()
        {
            // --- Arrange
            var container = new ServiceContainer();

            // --- Act
            container.Register(typeof(ISampleRepository), typeof(SampleRepository));
            container.Register(
                typeof(ISampleService),
                typeof(SampleService),
                new InjectedParameterSettingsCollection(new List<InjectedParameterSettings>
                    {
                        new InjectedParameterSettings(typeof(ISampleRepository), null, true)
                    }));
            var serviceObject = container.GetService<ISampleService>();

            // --- Assert
            serviceObject.ShouldNotBeNull();
            serviceObject.ShouldBeOfType(typeof(SampleService));
            // ReSharper disable PossibleNullReferenceException
            (serviceObject as SampleService).Repository.ShouldNotBeNull();
            // ReSharper restore PossibleNullReferenceException
        }

        [TestMethod]
        public void GetServiceWorksWithTrailResolution()
        {
            // --- Arrange
            var container = new ServiceContainer();

            // --- Act
            container.Register(typeof(ISampleRepository), typeof(SampleRepository));
            container.Register(
                typeof (ISampleService),
                typeof (SampleService),
                new InjectedParameterSettingsCollection(new List<InjectedParameterSettings>
                    {
                        new InjectedParameterSettings(typeof (ISampleRepository), null, true),
                        new InjectedParameterSettings(typeof (string), "Hello")
                    }));
            var serviceObject = container.GetService<ISampleService>();

            // --- Assert
            serviceObject.ShouldNotBeNull();
            serviceObject.ShouldBeOfType(typeof(SampleService));
            // ReSharper disable PossibleNullReferenceException
            (serviceObject as SampleService).B.ShouldEqual("Hello");
            // ReSharper restore PossibleNullReferenceException
        }

        [TestMethod]
        [ExpectedException(typeof(NoMatchingConstructorException))]
        public void GetServiceFailsWithUnresolvedTrail()
        {
            // --- Arrange
            var container = new ServiceContainer();

            // --- Act
            container.Register(typeof(ISampleRepository), typeof(SampleRepository));
            container.Register(
                typeof(ISampleService),
                typeof(SampleService),
                new InjectedParameterSettingsCollection(new List<InjectedParameterSettings>
                    {
                        new InjectedParameterSettings(typeof (string), "Hello"),
                        new InjectedParameterSettings(typeof (ISampleRepository), null, true)
                    }));
            var serviceObject = container.GetService<ISampleService>();

            // --- Assert
            serviceObject.ShouldNotBeNull();
            serviceObject.ShouldBeOfType(typeof(SampleService));
            // ReSharper disable PossibleNullReferenceException
            (serviceObject as SampleService).B.ShouldEqual("Hello");
            // ReSharper restore PossibleNullReferenceException
        }

        [TestMethod]
        [ExpectedException(typeof(CircularServiceReferenceException))]
        public void GetServiceFailsWithCircularReference()
        {
            // --- Arrange
            var container = new ServiceContainer();

            // --- Act
            container.Register(typeof(ISampleRepository), 
                typeof(SampleRepository),
                new InjectedParameterSettingsCollection(new List<InjectedParameterSettings>
                    {
                        new InjectedParameterSettings(typeof (ISampleService), null, true)
                    }));
            container.Register(
                typeof(ISampleService),
                typeof(SampleService),
                new InjectedParameterSettingsCollection(new List<InjectedParameterSettings>
                    {
                        new InjectedParameterSettings(typeof (ISampleRepository), null, true),
                        new InjectedParameterSettings(typeof (string), "Hello")
                    }));
            container.GetService<ISampleService>();
        }

        [TestMethod]
        public void GetServiceWorksWithTrailResolutionInMultipleContainers()
        {
            // --- Arrange
            var container1 = new ServiceContainer("parent");
            var container2 = new ServiceContainer("child", container1);

            // --- Act
            container1.Register(typeof(ISampleRepository), typeof(SampleRepository));
            container2.Register(
                typeof(ISampleService),
                typeof(SampleService),
                new InjectedParameterSettingsCollection(new List<InjectedParameterSettings>
                    {
                        new InjectedParameterSettings(typeof (ISampleRepository), null, true),
                        new InjectedParameterSettings(typeof (string), "Hello")
                    }));
            var serviceObject = container2.GetService<ISampleService>();

            // --- Assert
            serviceObject.ShouldNotBeNull();
            serviceObject.ShouldBeOfType(typeof(SampleService));
            // ReSharper disable PossibleNullReferenceException
            (serviceObject as SampleService).B.ShouldEqual("Hello");
            // ReSharper restore PossibleNullReferenceException
        }

        [TestMethod]
        [ExpectedException(typeof(CircularServiceReferenceException))]
        public void GetServiceFailsWithCircularReferenceInMultipleContainers()
        {
            // --- Arrange
            var container1 = new ServiceContainer("parent");
            var container2 = new ServiceContainer("child", container1);

            // --- Act
            container1.Register(typeof(ISampleRepository),
                typeof(SampleRepository),
                new InjectedParameterSettingsCollection(new List<InjectedParameterSettings>
                    {
                        new InjectedParameterSettings(typeof (ISampleService), null, true)
                    }));
            container2.Register(
                typeof(ISampleService),
                typeof(SampleService),
                new InjectedParameterSettingsCollection(new List<InjectedParameterSettings>
                    {
                        new InjectedParameterSettings(typeof (ISampleRepository), null, true),
                        new InjectedParameterSettings(typeof (string), "Hello")
                    }));
            container2.GetService<ISampleService>();
        }

        [TestMethod]
        public void InterceptedAttributeWorks()
        {
            // --- Arrange
            var container = new ServiceContainer();

            // --- Act
            container.Register(typeof(ISampleRepository), typeof(InterceptedSampleRepository));
            var serviceObject = container.GetService<ISampleRepository>();

            // --- Assert
            serviceObject.ShouldNotBeNull();
            serviceObject.GetType().ShouldNotBeOfType(typeof (InterceptedSampleRepository));
        }

        [TestMethod]
        public void DisableInterceptionAttributeWorks()
        {
            // --- Arrange
            var container = new ServiceContainer();

            // --- Act
            container.Register(typeof(ISampleRepository), typeof(NotInterceptedSampleRepository));
            var serviceObject = container.GetService<ISampleRepository>();

            // --- Assert
            serviceObject.ShouldNotBeNull();
            serviceObject.GetType().ShouldNotBeOfType(typeof(NotInterceptedSampleRepository));
        }
    }

    public interface ISampleService
    {
        string Operation();
        string Operation(ref int a);
    }

    public class SampleService : ISampleService
    {
        public int A { get; set; }
        public string B { get; set; }
        public ISampleRepository Repository { get; set; }

        public SampleService() { }

        public SampleService(int a)
        {
            A = a;
            B = "";
        }

        public SampleService(int a, string b)
        {
            A = a;
            B = b;
        }

        // ReSharper disable UnusedParameter.Local
        public SampleService(string a, int b, bool c) {}

        public SampleService(string a, int b, string c) {}
        // ReSharper restore UnusedParameter.Local

        public SampleService(ISampleRepository repo)
        {
            Repository = repo;
        }

        // ReSharper disable UnusedParameter.Local
        public SampleService(ISampleRepository repo1, string b, ISampleRepository repo2)
        {
            B = b;
        }

        public SampleService(string b, ISampleRepository repo, string c)
        {
        }
        // ReSharper restore UnusedParameter.Local

        public string Operation()
        {
            return A + B;
        }

        public string Operation(ref int a)
        {
            a++;
            return "OK";
        }
    }

    public interface ISampleRepository
    {
        void Operation();
    }

    public class SampleRepository: ISampleRepository
    {
        public SampleRepository() {}

        // ReSharper disable UnusedParameter.Local
        public SampleRepository(ISampleService srv) {}
        // ReSharper restore UnusedParameter.Local

        public void Operation() 
        {

        }
    }

    [Intercepted]
    public class InterceptedSampleRepository : ISampleRepository
    {
        public InterceptedSampleRepository() { }

        // ReSharper disable UnusedParameter.Local
        public InterceptedSampleRepository(ISampleService srv) { }
        // ReSharper restore UnusedParameter.Local

        public void Operation()
        {

        }
    }

    [Intercepted]
    [DisableInterception]
    public class NotInterceptedSampleRepository : ISampleRepository
    {
        public NotInterceptedSampleRepository() { }

        // ReSharper disable UnusedParameter.Local
        public NotInterceptedSampleRepository(ISampleService srv) { }
        // ReSharper restore UnusedParameter.Local

        public void Operation()
        {

        }
    }
}
