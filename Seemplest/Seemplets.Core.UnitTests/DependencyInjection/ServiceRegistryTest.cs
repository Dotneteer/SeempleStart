using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DependencyInjection;
using Seemplest.Core.Exceptions;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.DependencyInjection
{
    [TestClass]
    public class ServiceRegistryTest
    {
        [TestMethod]
        public void ConstructionWorksWithSimpleContainer()
        {
            // --- Arrange
            var settings = new ServiceRegistrySettings("default", new List<ServiceContainerSettings>
                {
                    new ServiceContainerSettings("default", null, new List<MappingSettings>
                        {
                            new MappingSettings(typeof (ISampleRepository),
                                typeof (SampleRepository)),
                            new MappingSettings(typeof (ISampleService), typeof (ISampleService))
                        })
                });

            // --- Act
            var registry = new ServiceRegistry(settings);

            // --- Assert
            registry.ContainerCount.ShouldEqual(1);
            registry.HasContainer("default").ShouldBeTrue();
            registry["default"].ShouldNotBeNull();
            registry["default"].ShouldBeSameAs(registry.DefaultContainer);
            registry[null].ShouldBeSameAs(registry.DefaultContainer);
            registry[""].ShouldBeSameAs(registry.DefaultContainer);
        }

        [TestMethod]
        public void ConstructionWorksWithNoExplicitDefaultContainer()
        {
            // --- Arrange
            var settings = new ServiceRegistrySettings(null, new List<ServiceContainerSettings>
                {
                    new ServiceContainerSettings("default", null, new List<MappingSettings>
                        {
                            new MappingSettings(typeof (ISampleRepository),
                                typeof (SampleRepository)),
                            new MappingSettings(typeof (ISampleService), typeof (ISampleService))
                        })
                });

            // --- Act
            var registry = new ServiceRegistry(settings);

            // --- Assert
            registry.ContainerCount.ShouldEqual(1);
            registry["default"].ShouldNotBeNull();
            registry["default"].ShouldBeSameAs(registry.DefaultContainer);
        }

        [TestMethod]
        public void ConstructionWorskWithMultipleContainer()
        {
            // --- Arrange
            var settings = new ServiceRegistrySettings("other", new List<ServiceContainerSettings>
                {
                    new ServiceContainerSettings("default", null, new List<MappingSettings>
                        {
                            new MappingSettings(typeof (ISampleService), typeof (ISampleService))
                        }),
                    new ServiceContainerSettings("other", null, new List<MappingSettings>
                        {
                            new MappingSettings(typeof (ISampleRepository), typeof (SampleRepository)),
                            new MappingSettings(typeof (ISampleService), typeof (ISampleService))
                        })
                });

            // --- Act
            var registry = new ServiceRegistry(settings);

            // --- Assert
            registry.ContainerCount.ShouldEqual(2);
            registry["default"].ShouldNotBeNull();
            registry["default"].GetRegisteredServices().ShouldHaveCountOf(1);
            registry["other"].ShouldNotBeNull();
            registry["other"].GetRegisteredServices().ShouldHaveCountOf(2);
            registry["other"].ShouldBeSameAs(registry.DefaultContainer);
        }

        [TestMethod]
        public void ConstructionWorskWithMultipleContainerAndParents1()
        {
            // --- Arrange
            var settings = new ServiceRegistrySettings("other", new List<ServiceContainerSettings>
                {
                    new ServiceContainerSettings("default", null, new List<MappingSettings>
                        {
                            new MappingSettings(typeof (ISampleService), typeof (ISampleService))
                        }),
                    new ServiceContainerSettings("other", "default", new List<MappingSettings>
                        {
                            new MappingSettings(typeof (ISampleRepository), typeof (SampleRepository)),
                            new MappingSettings(typeof (ISampleService), typeof (ISampleService))
                        })
                });

            // --- Act
            var registry = new ServiceRegistry(settings);

            // --- Assert
            registry.ContainerCount.ShouldEqual(2);
            registry["default"].ShouldNotBeNull();
            registry["default"].GetRegisteredServices().ShouldHaveCountOf(1);
            registry["other"].ShouldNotBeNull();
            registry["other"].Parent.ShouldBeSameAs(registry["default"]);
            registry["other"].GetRegisteredServices().ShouldHaveCountOf(2);
            registry["other"].ShouldBeSameAs(registry.DefaultContainer);
        }

        [TestMethod]
        public void ConstructionWorskWithMultipleContainerAndParents2()
        {
            // --- Arrange
            var settings = new ServiceRegistrySettings("other", new List<ServiceContainerSettings>
                {
                    new ServiceContainerSettings("default", "other", new List<MappingSettings>
                        {
                            new MappingSettings(typeof (ISampleService), typeof (ISampleService))
                        }),
                    new ServiceContainerSettings("other", null, new List<MappingSettings>
                        {
                            new MappingSettings(typeof (ISampleRepository), typeof (SampleRepository)),
                            new MappingSettings(typeof (ISampleService), typeof (ISampleService))
                        })
                });

            // --- Act
            var registry = new ServiceRegistry(settings);

            // --- Assert
            registry.ContainerCount.ShouldEqual(2);
            registry["default"].ShouldNotBeNull();
            registry["default"].GetRegisteredServices().ShouldHaveCountOf(1);
            registry["other"].ShouldNotBeNull();
            registry["default"].Parent.ShouldBeSameAs(registry["other"]);
            registry["other"].GetRegisteredServices().ShouldHaveCountOf(2);
            registry["other"].ShouldBeSameAs(registry.DefaultContainer);
        }

        [TestMethod]
        [ExpectedException(typeof(CircularContainerReferenceException))]
        public void CircularContainerReferencesAreRecognized1()
        {
            // --- Arrange
            var settings = new ServiceRegistrySettings("other", new List<ServiceContainerSettings>
                {
                    new ServiceContainerSettings("default", "other", new List<MappingSettings>()),
                    new ServiceContainerSettings("other", "default", new List<MappingSettings>())
                });

            // --- Act
            // ReSharper disable ObjectCreationAsStatement
            new ServiceRegistry(settings);
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestMethod]
        [ExpectedException(typeof(CircularContainerReferenceException))]
        public void CircularContainerReferencesAreRecognized2()
        {
            // --- Arrange
            var settings = new ServiceRegistrySettings(null, new List<ServiceContainerSettings>
                {
                    new ServiceContainerSettings("container1", "container3", new List<MappingSettings>()),
                    new ServiceContainerSettings("container2", "container1", new List<MappingSettings>()),
                    new ServiceContainerSettings("container3", "container2", new List<MappingSettings>()),
                });

            // --- Act
            // ReSharper disable ObjectCreationAsStatement
            new ServiceRegistry(settings);
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestMethod]
        public void ConstructionWorksWithNoContainer()
        {
            // --- Arrange
            var settings = new ServiceRegistrySettings(null, new List<ServiceContainerSettings>());

            // --- Act
            var registry = new ServiceRegistry(settings);

            // --- Assert
            registry.ContainerCount.ShouldEqual(1);
            registry.DefaultContainer.GetRegisteredServices().ShouldHaveCountOf(0);
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicatedContainerNameException))]
        public void DuplicateContainerNamesFail()
        {
            // --- Arrange
            var settings = new ServiceRegistrySettings("other", new List<ServiceContainerSettings>
                {
                    new ServiceContainerSettings("other", null, new List<MappingSettings>()),
                    new ServiceContainerSettings("other", null, new List<MappingSettings>()),
                });

            // --- Act
            // ReSharper disable ObjectCreationAsStatement
            new ServiceRegistry(settings);
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestMethod]
        public void DuplicateContainerNamesAreInTheExceptionMessage1()
        {
            // --- Arrange
            var settings = new ServiceRegistrySettings(null, new List<ServiceContainerSettings>
                {
                    new ServiceContainerSettings("doublecontainer", null, new List<MappingSettings>()),
                    new ServiceContainerSettings("doublecontainer", null, new List<MappingSettings>()),
                    new ServiceContainerSettings("other1", null, new List<MappingSettings>()),
                    new ServiceContainerSettings("other2", null, new List<MappingSettings>()),
                    new ServiceContainerSettings("other2", null, new List<MappingSettings>())
                });

            // --- Act
            try
            {
                // ReSharper disable ObjectCreationAsStatement
                new ServiceRegistry(settings);
                // ReSharper restore ObjectCreationAsStatement
                Assert.Fail("Exception was expected");
            }
            catch (DuplicatedContainerNameException ex)
            {
                ex.Message.ShouldContain("doublecontainer");
                ex.Message.ShouldContain("other2");
                ex.Message.ShouldNotContain("other1");
            }
        }

        [TestMethod]
        public void DuplicateContainerNamesAreInTheExceptionMessage2()
        {
            // --- Arrange
            var settings = new ServiceRegistrySettings(null, new List<ServiceContainerSettings>
                {
                    new ServiceContainerSettings("doublecontainer", null, new List<MappingSettings>()),
                    new ServiceContainerSettings("doublecontainer", null, new List<MappingSettings>()),
                });

            // --- Act
            try
            {
                // ReSharper disable ObjectCreationAsStatement
                new ServiceRegistry(settings);
                // ReSharper restore ObjectCreationAsStatement
                Assert.Fail("Exception was expected");
            }
            catch (DuplicatedContainerNameException ex)
            {
                ex.Message.ShouldContain("doublecontainer");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ContainerNotFoundException))]
        public void UnknownDefaultContainerFails1()
        {
            // --- Arrange
            var settings = new ServiceRegistrySettings("default", new List<ServiceContainerSettings>
                {
                    new ServiceContainerSettings("other", null, new List<MappingSettings>()),
                    new ServiceContainerSettings("services", null, new List<MappingSettings>()),
                });

            // --- Act
            // ReSharper disable ObjectCreationAsStatement
            new ServiceRegistry(settings);
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestMethod]
        [ExpectedException(typeof(ContainerNotFoundException))]
        public void UnknownDefaultContainerFails2()
        {
            // --- Arrange
            var settings = new ServiceRegistrySettings(null, new List<ServiceContainerSettings>
                {
                    new ServiceContainerSettings("other", "default", new List<MappingSettings>()),
                    new ServiceContainerSettings("services", null, new List<MappingSettings>()),
                });

            // --- Act
            // ReSharper disable ObjectCreationAsStatement
            new ServiceRegistry(settings);
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestMethod]
        [ExpectedException(typeof(ContainerNotFoundException))]
        public void UnknownDefaultContainerFails3()
        {
            // --- Arrange
            var settings = new ServiceRegistrySettings(null, new List<ServiceContainerSettings>
                {
                    new ServiceContainerSettings("other", null, new List<MappingSettings>()),
                    new ServiceContainerSettings("services", "default", new List<MappingSettings>()),
                });

            // --- Act
            // ReSharper disable ObjectCreationAsStatement
            new ServiceRegistry(settings);
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestMethod]
        public void ConstructionWorksWithExplicitContainer1()
        {
            // --- Act
            var registry = new ServiceRegistry(new ServiceContainer());

            // --- Assert
            registry.ContainerCount.ShouldEqual(1);
            registry.DefaultContainer.ShouldNotBeNull();
        }

        [TestMethod]
        public void ConstructionWorksWithExplicitContainer2()
        {
            // --- Arrange
            var defaultParent = new ServiceContainer();
            // --- Act
            var registry = new ServiceRegistry(
                defaultParent, 
                new ServiceContainer("container1", defaultParent),
                new ServiceContainer("container2", defaultParent)
                );

            // --- Assert
            registry.ContainerCount.ShouldEqual(3);
            registry["container1"].Parent.ShouldBeSameAs(registry.DefaultContainer);
            registry["container2"].Parent.ShouldBeSameAs(registry.DefaultContainer);
            registry.DefaultContainer.ShouldBeSameAs(registry["default"]);
        }

        [TestMethod]
        public void ConstructionWorksWithExplicitContainer3()
        {
            // --- Arrange
            var defaultParent = new ServiceContainer();
            // --- Act
            var registry = new ServiceRegistry(
                defaultParent,
                new List<ServiceContainer>
                {
                    new ServiceContainer("container1", defaultParent),
                    new ServiceContainer("container2", defaultParent)
                });

            // --- Assert
            registry.ContainerCount.ShouldEqual(3);
            registry["container1"].Parent.ShouldBeSameAs(registry.DefaultContainer);
            registry["container2"].Parent.ShouldBeSameAs(registry.DefaultContainer);
            registry.DefaultContainer.ShouldBeSameAs(registry["default"]);
            registry.ContainerNames.ShouldContain("container1");
            registry.ContainerNames.ShouldContain("container2");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructionWithExplicitContainerFailsWithNull1()
        {
            // --- Act
            // ReSharper disable ObjectCreationAsStatement
            new ServiceRegistry(null, new ServiceContainer());
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructionWithExplicitContainerFailsWithNull2()
        {
            // --- Act
            // ReSharper disable ObjectCreationAsStatement
            new ServiceRegistry(new ServiceContainer(), new ServiceContainer("container1"), null);
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructionWithExplicitContainerFailsWithNull3()
        {
            // --- Act
            // ReSharper disable ObjectCreationAsStatement
            new ServiceRegistry(new ServiceContainer(), 
                new List<ServiceContainer>
                {
                    new ServiceContainer("container1"), 
                    null
                });
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidContainerNameException))]
        public void ConstructionWithInvalidNameFails1()
        {
            // --- Act
            // ReSharper disable ObjectCreationAsStatement
            new ServiceRegistry(new ServiceContainer(""));
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidContainerNameException))]
        public void ConstructionWithInvalidNameFails2()
        {
            // --- Act
            // ReSharper disable ObjectCreationAsStatement
            new ServiceRegistry(new ServiceContainer("container1"), new ServiceContainer(""));
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidContainerNameException))]
        public void ConstructionWithInvalidNameFails3()
        {
            // --- Act
            // ReSharper disable ObjectCreationAsStatement
            new ServiceRegistry(new ServiceContainer("container1"), 
                new List<ServiceContainer>
                    {
                        new ServiceContainer("container2"),
                        new ServiceContainer("")
                    });
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestMethod]
        public void RegisterWorks()
        {
            // --- Arrange
            var settings = new ServiceRegistrySettings("default", new List<ServiceContainerSettings>
                {
                    new ServiceContainerSettings("default", null, new List<MappingSettings>
                        {
                            new MappingSettings(typeof (ISampleRepository), typeof (SampleRepository)),
                        })
                });
            var registry = new ServiceRegistry(settings);

            // --- Act
            var before = registry.GetRegisteredServices();
            registry.Register(typeof (ISampleService), typeof (SampleService));
            var after = registry.GetRegisteredServices();

            // --- Assert
            before.ShouldHaveCountOf(1);
            after.ShouldHaveCountOf(2);
        }


        [TestMethod]
        public void GetServiceWorks1()
        {
            // --- Arrange
            var settings = new ServiceRegistrySettings("default", new List<ServiceContainerSettings>
                {
                    new ServiceContainerSettings("default", null, new List<MappingSettings>
                        {
                            new MappingSettings(typeof (ISampleRepository),
                                typeof (SampleRepository)),
                            new MappingSettings(typeof (ISampleService), typeof (ISampleService))
                        })
                });
            var registry = new ServiceRegistry(settings);

            // --- Act
            var service = registry.GetService(typeof (ISampleRepository));

            // --- Assert
            service.ShouldBeOfType(typeof (SampleRepository));
        }

        [TestMethod]
        public void GetServiceWorks2()
        {
            // --- Arrange
            var settings = new ServiceRegistrySettings("default", new List<ServiceContainerSettings>
                {
                    new ServiceContainerSettings("default", null, new List<MappingSettings>
                        {
                            new MappingSettings(typeof (ISampleRepository),
                                typeof (SampleRepository)),
                            new MappingSettings(typeof (ISampleService), typeof (ISampleService))
                        })
                });
            var registry = new ServiceRegistry(settings);

            // --- Act
            var service = registry.GetService<ISampleRepository>();

            // --- Assert
            service.ShouldBeOfType(typeof (SampleRepository));
        }

        [TestMethod]
        public void RemoveServiceWorks()
        {
            // --- Arrange
            var settings = new ServiceRegistrySettings("default", new List<ServiceContainerSettings>
                {
                    new ServiceContainerSettings("default", null, new List<MappingSettings>
                        {
                            new MappingSettings(typeof (ISampleRepository),
                                typeof (SampleRepository)),
                            new MappingSettings(typeof (ISampleService), typeof (ISampleService))
                        })
                });
            var registry = new ServiceRegistry(settings);

            // --- Act
            var before = registry.DefaultContainer.GetRegisteredServices();
            registry.RemoveService(typeof(ISampleRepository));
            var after = registry.DefaultContainer.GetRegisteredServices();

            // --- Assert
            before.ShouldHaveCountOf(2);
            after.ShouldHaveCountOf(1);
        }


    }
}
