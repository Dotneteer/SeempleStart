using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DependencyInjection;
using Seemplest.Core.ServiceObjects;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.ServiceObjects
{
    [TestClass]
    public class EmptyContextServiceFactoryTest
    {
        [TestInitialize]
        public void Initialize()
        {
            ServiceManager.Reset();
            ServiceManager.Register<IDummy2, Dummy2Service>();
        }

        [TestMethod]
        public void CreateServiceWorksAsExpected()
        {
            // --- Arrange
            var factory = new EmptyContextServiceFactory(ServiceManager.ServiceRegistry);

            // --- Act
            var service = factory.CreateService(typeof(IDummy2));

            // --- Assert
            service.ShouldNotBeNull();
        }

        [TestMethod]
        public void CreateServiceRetrievesNullForUnknownService()
        {
            // --- Arrange
            var factory = new EmptyContextServiceFactory(ServiceManager.ServiceRegistry);

            // --- Act
            var service = factory.CreateService(typeof(IComparable));

            // --- Assert
            service.ShouldBeNull();
        }

        [TestMethod]
        public void TypedCreateServiceWorksAsExpected()
        {
            // --- Arrange
            var factory = new EmptyContextServiceFactory(ServiceManager.ServiceRegistry);

            // --- Act
            var service = factory.CreateService<IDummy2>();

            // --- Assert
            service.ShouldNotBeNull();
        }

        [TestMethod]
        public void ServiceIsSetupProperly()
        {
            // --- Arrange
            var factory = new EmptyContextServiceFactory(ServiceManager.ServiceRegistry);

            // --- Act
            var service = factory.CreateService<IDummy2>();
            var context = service.CallContext;
            var locator = service.ServiceLocator;
            var realService = service.GetObject(typeof(IDummy2));

            // --- Assert
            context.ShouldNotBeNull();
            locator.ShouldNotBeNull();
            realService.ShouldNotBeNull();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceContextCannotBeSetToNull()
        {
            // --- Arrange
            var service = new Dummy2Service();

            // --- Act
            ((IServiceObject)service).SetCallContext(null);
        }

        [TestMethod]
        public void GetServiceIsAvailable()
        {
            // --- Arrange
            var factory = new EmptyContextServiceFactory(new SimpleServiceLocator());

            // --- Act
            var service = factory.CreateService<IDummy2>() as Dummy2Service;

            // --- Assert
            service.ShouldNotBeNull();
            // ReSharper disable once PossibleNullReferenceException
            service.GetService<IDummy2>().ShouldBeOfType(typeof (Dummy2Service));
        }



        public interface IDummy2 : IServiceObject
        {
            object GetObject(Type type);
        }

        public class Dummy2Service : ServiceObjectBase, IDummy2 {
            public object GetObject(Type type)
            {
                return GetService(type);
            }
        }

        public class SimpleServiceLocator : IServiceLocator
        {
            public object GetService(Type service)
            {
                return service == typeof (IDummy2) ? new Dummy2Service() : null;
            }

            public T GetService<T>()
            {
                return typeof(T) == typeof(IDummy2) ? (T)GetService(typeof(T)) : default(T);
            }
        }
    }
}
