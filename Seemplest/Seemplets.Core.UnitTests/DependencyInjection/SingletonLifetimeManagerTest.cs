using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration;
using Seemplest.Core.DependencyInjection;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.DependencyInjection
{
    [TestClass]
    public class SingletonLifetimeManagerTest
    {
        [TestMethod]
        public void WorksWithTypeOnly()
        {
            // --- Arrange
            var ltManager = new SingletonLifetimeManager
                {
                    ServiceObjectType = typeof(SampleObject)
                };

            // --- Act
            var instance1 = ltManager.GetObject() as SampleObject;
            var instance2 = ltManager.GetObject() as SampleObject;

            // --- Assert
            instance1.ShouldNotBeNull();
            instance1.ShouldBeSameAs(instance2);
            // ReSharper disable PossibleNullReferenceException
            instance1.Property1.ShouldEqual(SampleObject.DEFAULT_INT);
            instance1.Property2.ShouldEqual(SampleObject.DEFAULT_STRING);
            // ReSharper restore PossibleNullReferenceException
        }

        [TestMethod]
        public void WorksWithReset()
        {
            // --- Arrange
            var ltManager = new SingletonLifetimeManager
            {
                ServiceObjectType = typeof(SampleObject)
            };

            // --- Act
            var instance1 = ltManager.GetObject() as SampleObject;
            ltManager.ResetState();
            var instance2 = ltManager.GetObject() as SampleObject;
            var instance3 = ltManager.GetObject() as SampleObject;

            // --- Assert
            instance1.ShouldNotBeNull();
            instance2.ShouldNotBeNull();
            instance1.ShouldNotBeSameAs(instance2);
            instance2.ShouldBeSameAs(instance3);
        }

        [TestMethod]
        public void WorksWithInstance()
        {
            // --- Arrange
            var ltManager = new SingletonLifetimeManager(new SampleObject());

            // --- Act
            var instance1 = ltManager.GetObject() as SampleObject;
            var instance2 = ltManager.GetObject() as SampleObject;

            // --- Assert
            instance1.ShouldNotBeNull();
            instance1.ShouldBeSameAs(instance2);
            // ReSharper disable PossibleNullReferenceException
            instance1.Property1.ShouldEqual(SampleObject.DEFAULT_INT);
            instance1.Property2.ShouldEqual(SampleObject.DEFAULT_STRING);
            // ReSharper restore PossibleNullReferenceException
        }

        [TestMethod]
        public void WorksWithConstructorParams1()
        {
            // --- Arrange
            var ltManager = new SingletonLifetimeManager
                {
                    ServiceObjectType = typeof(SampleObject),
                    ConstructionParameters = new object[] {23, "hey"}
                };

            // --- Act
            var instance1 = ltManager.GetObject() as SampleObject;
            var instance2 = ltManager.GetObject() as SampleObject;

            // --- Assert
            instance1.ShouldNotBeNull();
            instance1.ShouldBeSameAs(instance2);
            // ReSharper disable PossibleNullReferenceException
            instance1.Property1.ShouldEqual(23);
            instance1.Property2.ShouldEqual("hey");
            // ReSharper restore PossibleNullReferenceException
        }

        [TestMethod]
        public void WorksWithConstructorParams2()
        {
            // --- Arrange
            var ltManager = new SingletonLifetimeManager
                {
                    ServiceObjectType = typeof(SampleObject),
                    ConstructionParameters = new object[] { 23 }
                };

            // --- Act
            var instance1 = ltManager.GetObject() as SampleObject;
            var instance2 = ltManager.GetObject() as SampleObject;

            // --- Assert
            instance1.ShouldNotBeNull();
            instance1.ShouldBeSameAs(instance2);
            // ReSharper disable PossibleNullReferenceException
            instance1.Property1.ShouldEqual(23);
            instance1.Property2.ShouldEqual(SampleObject.DEFAULT_STRING);
            // ReSharper restore PossibleNullReferenceException
        }

        [TestMethod]
        public void WorksWithPropertyInjection()
        {
            // --- Arrange
            var ltManager = new SingletonLifetimeManager
                {
                    ServiceObjectType = typeof(SampleObject),
                    Properties = new PropertySettingsCollection(new List<PropertySettings>
                        {
                            new PropertySettings("Property1", "23"),
                            new PropertySettings("Property2", "hey")
                        })
                };

            // --- Act
            var instance1 = ltManager.GetObject() as SampleObject;
            var instance2 = ltManager.GetObject() as SampleObject;

            // --- Assert
            instance1.ShouldNotBeNull();
            instance1.ShouldBeSameAs(instance2);
            // ReSharper disable PossibleNullReferenceException
            instance1.Property1.ShouldEqual(23);
            instance1.Property2.ShouldEqual("hey");
            // ReSharper restore PossibleNullReferenceException
        }

        [TestMethod]
        public void WorksWithConstructorAndPropertyInjection()
        {
            // --- Arrange
            var ltManager = new SingletonLifetimeManager
                {
                    ServiceObjectType = typeof(SampleObject),
                    ConstructionParameters = new object[] { 12 },
                    Properties = new PropertySettingsCollection(new List<PropertySettings>
                        {
                            new PropertySettings("Property1", "45"),
                            new PropertySettings("Property2", "hello")
                        })
                };

            // --- Act
            var instance1 = ltManager.GetObject() as SampleObject;
            var instance2 = ltManager.GetObject() as SampleObject;

            // --- Assert
            instance1.ShouldNotBeNull();
            instance1.ShouldBeSameAs(instance2);
            // ReSharper disable PossibleNullReferenceException
            instance1.Property1.ShouldEqual(45);
            instance1.Property2.ShouldEqual("hello");
            // ReSharper restore PossibleNullReferenceException
        }
    }
}