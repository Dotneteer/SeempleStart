using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DependencyInjection;
using System.Collections.Generic;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.DependencyInjection
{
    [TestClass]
    public class ServiceManagerTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetRegistryFailsWithNull()
        {
            // --- Act
            ServiceManager.SetRegistry(null);
        }

        [TestMethod]
        public void ConfigureFromWorksAsExpected()
        {
            // --- Arrange
            const string SM_KEY = "ServiceManager1";

            // --- Act 
            ServiceManager.ConfigureFrom(SM_KEY);

            // --- Assert
            var srv = ServiceManager.GetService<IDummy>();
            srv.ShouldNotBeNull();
        }

        [TestMethod]
        public void RegisterWorksAsExpected()
        {
            // --- Arrange
            ServiceManager.Reset();

            // --- Act
            ServiceManager.Register<IDummy, DummyService>(new InjectedParameterSettingsCollection(
                new List<InjectedParameterSettings>()));

            // --- Assert
            var srv = ServiceManager.GetService(typeof(IDummy));
            srv.ShouldNotBeNull();
        }

        [TestMethod]
        public void GetRegisteredServiceWorksAsExpected()
        {
            // --- Arrange
            ServiceManager.Reset();
            ServiceManager.Register<IDummy, DummyService>(new InjectedParameterSettingsCollection(
                new List<InjectedParameterSettings>()));

            // --- Act
            var srvs = ServiceManager.GetRegisteredServices();

            // --- Assert
            srvs.ShouldHaveCountOf(1);
        }

        [TestMethod]
        public void RemoveServiceWorksAsExpected()
        {
            // --- Arrange
            ServiceManager.Reset();
            ServiceManager.Register<IDummy, DummyService>(new InjectedParameterSettingsCollection(
                new List<InjectedParameterSettings>()));

            // --- Act
            var count1 = ServiceManager.GetRegisteredServices().Count;
            ServiceManager.RemoveService(typeof(IDummy));
            var count2 = ServiceManager.GetRegisteredServices().Count;

            // --- Assert
            count1.ShouldEqual(1);
            count2.ShouldEqual(0);
        }

        public interface IDummy { }

        public class DummyService: IDummy { }
    }
}
