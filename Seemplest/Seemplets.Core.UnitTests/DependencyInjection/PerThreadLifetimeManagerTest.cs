using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration;
using Seemplest.Core.DependencyInjection;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.DependencyInjection
{
    [TestClass]
    public class PerThreadLifetimeManagerTest
    {
        [TestMethod]
        public void WorksWithTypeOnly()
        {
            // --- Arrange
            var ltManager = new PerThreadLifetimeManager
                {
                    ServiceObjectType = typeof(SampleObject)
                };

            // --- Act
            SampleObject instance1 = null, 
                instance2 = null,
                instance3 = null,
                instance4 = null;

            object threadInstance1 = null;
            object threadInstance2 = null;
            
            // ReSharper disable ImplicitlyCapturedClosure
            var thread1 = new Thread(() =>
                {
                    instance1 = ltManager.GetObject() as SampleObject;
                    instance2 = ltManager.GetObject() as SampleObject;
                    threadInstance1 = ltManager.Instance;
                });
            var thread2 = new Thread(() =>
                {
                    instance3 = ltManager.GetObject() as SampleObject;
                    instance4 = ltManager.GetObject() as SampleObject;
                    threadInstance2 = ltManager.Instance;
                });
            // ReSharper restore ImplicitlyCapturedClosure
            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();

            // --- Assert
            instance1.ShouldNotBeNull();
            instance1.ShouldBeSameAs(instance2);
            instance3.ShouldNotBeNull();
            instance3.ShouldBeSameAs(instance4);
            instance1.ShouldNotBeSameAs(instance3);
            threadInstance1.ShouldNotBeSameAs(threadInstance2);
            instance1.Property1.ShouldEqual(SampleObject.DEFAULT_INT);
            instance1.Property2.ShouldEqual(SampleObject.DEFAULT_STRING);
            instance3.Property1.ShouldEqual(SampleObject.DEFAULT_INT);
            instance3.Property2.ShouldEqual(SampleObject.DEFAULT_STRING);
        }

        [TestMethod]
        public void WorksWithResetOnly()
        {
            // --- Arrange
            var ltManager = new PerThreadLifetimeManager
            {
                ServiceObjectType = typeof(SampleObject)
            };

            // --- Act
            SampleObject instance1 = null,
                instance2 = null,
                instance3 = null,
                instance4 = null;

            object threadInstance1 = null;
            object threadInstance2 = null;

            // ReSharper disable ImplicitlyCapturedClosure
            var thread1 = new Thread(() =>
            {
                ltManager.ResetState();
                instance1 = ltManager.GetObject() as SampleObject;
                instance2 = ltManager.GetObject() as SampleObject;
                threadInstance1 = ltManager.Instance;
            });
            var thread2 = new Thread(() =>
            {
                ltManager.ResetState();
                instance3 = ltManager.GetObject() as SampleObject;
                instance4 = ltManager.GetObject() as SampleObject;
                threadInstance2 = ltManager.Instance;
            });
            // ReSharper restore ImplicitlyCapturedClosure
            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();

            // --- Assert
            instance1.ShouldNotBeNull();
            instance1.ShouldBeSameAs(instance2);
            instance3.ShouldNotBeNull();
            instance3.ShouldBeSameAs(instance4);
            instance1.ShouldNotBeSameAs(instance3);
            threadInstance1.ShouldNotBeSameAs(threadInstance2);
            instance1.Property1.ShouldEqual(SampleObject.DEFAULT_INT);
            instance1.Property2.ShouldEqual(SampleObject.DEFAULT_STRING);
            instance3.Property1.ShouldEqual(SampleObject.DEFAULT_INT);
            instance3.Property2.ShouldEqual(SampleObject.DEFAULT_STRING);
        }


        [TestMethod]
        public void WorksWithConstructorParams1()
        {
            // --- Arrange
            var ltManager = new PerThreadLifetimeManager
            {
                ServiceObjectType = typeof(SampleObject),
                ConstructionParameters = new object[] { 23, "hey" }
            };

            // --- Act
            SampleObject instance1 = null,
                instance2 = null,
                instance3 = null,
                instance4 = null;
            // ReSharper disable ImplicitlyCapturedClosure
            var thread1 = new Thread(() =>
            {
                instance1 = ltManager.GetObject() as SampleObject;
                instance2 = ltManager.GetObject() as SampleObject;
            });
            var thread2 = new Thread(() =>
            {
                instance3 = ltManager.GetObject() as SampleObject;
                instance4 = ltManager.GetObject() as SampleObject;
            });
            // ReSharper restore ImplicitlyCapturedClosure
            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();

            // --- Assert
            instance1.ShouldNotBeNull();
            instance1.ShouldBeSameAs(instance2);
            instance3.ShouldNotBeNull();
            instance3.ShouldBeSameAs(instance4);
            instance1.ShouldNotBeSameAs(instance3);
            instance1.Property1.ShouldEqual(23);
            instance1.Property2.ShouldEqual("hey");
            instance3.Property1.ShouldEqual(23);
            instance3.Property2.ShouldEqual("hey");
        }

        [TestMethod]
        public void WorksWithConstructorParams2()
        {
            // --- Arrange
            var ltManager = new PerThreadLifetimeManager
            {
                ServiceObjectType = typeof(SampleObject),
                ConstructionParameters = new object[] { 23 }
            };

            // --- Act
            // --- Act
            SampleObject instance1 = null,
                instance2 = null,
                instance3 = null,
                instance4 = null;
            // ReSharper disable ImplicitlyCapturedClosure
            var thread1 = new Thread(() =>
            {
                instance1 = ltManager.GetObject() as SampleObject;
                instance2 = ltManager.GetObject() as SampleObject;
            });
            var thread2 = new Thread(() =>
            {
                instance3 = ltManager.GetObject() as SampleObject;
                instance4 = ltManager.GetObject() as SampleObject;
            });
            // ReSharper restore ImplicitlyCapturedClosure
            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();

            // --- Assert
            instance1.ShouldNotBeNull();
            instance1.ShouldBeSameAs(instance2);
            instance3.ShouldNotBeNull();
            instance3.ShouldBeSameAs(instance4);
            instance1.ShouldNotBeSameAs(instance3);
            instance1.Property1.ShouldEqual(23);
            instance1.Property2.ShouldEqual(SampleObject.DEFAULT_STRING);
            instance3.Property1.ShouldEqual(23);
            instance3.Property2.ShouldEqual(SampleObject.DEFAULT_STRING);
        }

        [TestMethod]
        public void WorksWithPropertyInjection()
        {
            // --- Arrange
            var ltManager = new PerThreadLifetimeManager
                {
                    ServiceObjectType = typeof (SampleObject),
                    Properties = new PropertySettingsCollection(new List<PropertySettings>
                        {
                            new PropertySettings("Property1", "23"),
                            new PropertySettings("Property2", "hey")
                        })
                };

            // --- Act
            SampleObject instance1 = null,
                instance2 = null,
                instance3 = null,
                instance4 = null;
            // ReSharper disable ImplicitlyCapturedClosure
            var thread1 = new Thread(() =>
            {
                instance1 = ltManager.GetObject() as SampleObject;
                instance2 = ltManager.GetObject() as SampleObject;
            });
            var thread2 = new Thread(() =>
            {
                instance3 = ltManager.GetObject() as SampleObject;
                instance4 = ltManager.GetObject() as SampleObject;
            });
            // ReSharper restore ImplicitlyCapturedClosure
            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();

            // --- Assert
            instance1.ShouldNotBeNull();
            instance1.ShouldBeSameAs(instance2);
            instance3.ShouldNotBeNull();
            instance3.ShouldBeSameAs(instance4);
            instance1.ShouldNotBeSameAs(instance3);
            instance1.Property1.ShouldEqual(23);
            instance1.Property2.ShouldEqual("hey");
            instance3.Property1.ShouldEqual(23);
            instance3.Property2.ShouldEqual("hey");
        }

        [TestMethod]
        public void WorksWithConstructorAndPropertyInjection()
        {
            // --- Arrange
            var ltManager = new PerThreadLifetimeManager
                {
                    ServiceObjectType = typeof (SampleObject),
                    ConstructionParameters = new object[] {12},
                    Properties = new PropertySettingsCollection(new List<PropertySettings>
                        {
                            new PropertySettings("Property1", "45"),
                            new PropertySettings("Property2", "hello")
                        })
                };

            // --- Act
            SampleObject instance1 = null,
                instance2 = null,
                instance3 = null,
                instance4 = null;
            // ReSharper disable ImplicitlyCapturedClosure
            var thread1 = new Thread(() =>
            {
                instance1 = ltManager.GetObject() as SampleObject;
                instance2 = ltManager.GetObject() as SampleObject;
            });
            var thread2 = new Thread(() =>
            {
                instance3 = ltManager.GetObject() as SampleObject;
                instance4 = ltManager.GetObject() as SampleObject;
            });
            // ReSharper restore ImplicitlyCapturedClosure
            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();

            // --- Assert
            instance1.ShouldNotBeNull();
            instance1.ShouldBeSameAs(instance2);
            instance3.ShouldNotBeNull();
            instance3.ShouldBeSameAs(instance4);
            instance1.ShouldNotBeSameAs(instance3);
            instance1.Property1.ShouldEqual(45);
            instance1.Property2.ShouldEqual("hello");
            instance3.Property1.ShouldEqual(45);
            instance3.Property2.ShouldEqual("hello");
        }
    }
}
