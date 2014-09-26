using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration;
using Seemplest.Core.DependencyInjection;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.DependencyInjection
{
    [TestClass]
    public class PerCallLifetimeManagerTest
    {
        [TestMethod]
        public void WorksWithTypeOnly()
        {
            // --- Arrange
            var ltManager = new PerCallLifetimeManager { ServiceObjectType = typeof (SampleObject) };

            // --- Act
            var instance1 = ltManager.GetObject() as SampleObject;
            var instance2 = ltManager.GetObject() as SampleObject;

            // --- Assert
            instance1.ShouldNotBeNull();
            instance2.ShouldNotBeNull();
            // ReSharper disable PossibleNullReferenceException
            instance1.Property1.ShouldEqual(SampleObject.DEFAULT_INT);
            instance1.Property2.ShouldEqual(SampleObject.DEFAULT_STRING);
            instance2.Property1.ShouldEqual(SampleObject.DEFAULT_INT);
            instance2.Property2.ShouldEqual(SampleObject.DEFAULT_STRING);
            // ReSharper restore PossibleNullReferenceException
            instance1.ShouldNotBeSameAs(instance2);
        }

        [TestMethod]
        public void WorksWithConstructorParams1()
        {
            // --- Arrange
            var ltManager = new PerCallLifetimeManager
                {
                    ServiceObjectType = typeof(SampleObject),
                    ConstructionParameters = new object[] { 23, "hey" }
                };

            // --- Act
            var instance1 = ltManager.GetObject() as SampleObject;
            var instance2 = ltManager.GetObject() as SampleObject;

            // --- Assert
            instance1.ShouldNotBeNull();
            instance2.ShouldNotBeNull();
            // ReSharper disable PossibleNullReferenceException
            instance1.Property1.ShouldEqual(23);
            instance1.Property2.ShouldEqual("hey");
            instance2.Property1.ShouldEqual(23);
            instance2.Property2.ShouldEqual("hey");
            // ReSharper restore PossibleNullReferenceException
            instance1.ShouldNotBeSameAs(instance2);
        }

        [TestMethod]
        public void WorksWithConstructorParams2()
        {
            // --- Arrange
            var ltManager = new PerCallLifetimeManager
            {
                ServiceObjectType = typeof(SampleObject),
                ConstructionParameters = new object[] { 23 }
            };

            // --- Act
            var instance1 = ltManager.GetObject() as SampleObject;
            var instance2 = ltManager.GetObject() as SampleObject;

            // --- Assert
            instance1.ShouldNotBeNull();
            instance2.ShouldNotBeNull();
            // ReSharper disable PossibleNullReferenceException
            instance1.Property1.ShouldEqual(23);
            instance1.Property2.ShouldEqual(SampleObject.DEFAULT_STRING);
            instance2.Property1.ShouldEqual(23);
            instance2.Property2.ShouldEqual(SampleObject.DEFAULT_STRING);
            // ReSharper restore PossibleNullReferenceException
            instance1.ShouldNotBeSameAs(instance2);
        }

        [TestMethod]
        public void WorksWithPropertyInjection()
        {
            // --- Arrange
            var ltManager = new PerCallLifetimeManager
                {
                    ServiceObjectType = typeof (SampleObject),
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
            instance2.ShouldNotBeNull();
            // ReSharper disable PossibleNullReferenceException
            instance1.Property1.ShouldEqual(23);
            instance1.Property2.ShouldEqual("hey");
            instance2.Property1.ShouldEqual(23);
            instance2.Property2.ShouldEqual("hey");
            // ReSharper restore PossibleNullReferenceException
            instance1.ShouldNotBeSameAs(instance2);
        }

        [TestMethod]
        public void WorksWithConstructorAndPropertyInjection()
        {
            // --- Arrange
            var ltManager = new PerCallLifetimeManager
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
            instance2.ShouldNotBeNull();
            // ReSharper disable PossibleNullReferenceException
            instance1.Property1.ShouldEqual(45);
            instance1.Property2.ShouldEqual("hello");
            instance2.Property1.ShouldEqual(45);
            instance2.Property2.ShouldEqual("hello");
            // ReSharper restore PossibleNullReferenceException
            instance1.ShouldNotBeSameAs(instance2);
        }
    }
}
