using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Seemplest.Core.Configuration;
using Seemplest.Core.Exceptions;

namespace Seemplest.Core.DependencyInjection
{
    /// <summary>
    /// This class provides a default implementation of s service registry.
    /// </summary>
    public class ServiceRegistry: IServiceRegistry
    {
        // --- Stores the containers
        protected readonly Dictionary<string, IServiceContainer> Containers =
            new Dictionary<string, IServiceContainer>();

        /// <summary>
        /// Gets the default container of the service registry
        /// </summary>
        public IServiceContainer DefaultContainer { get; protected set; }

        /// <summary>
        /// Initializes the service registry
        /// </summary>
        /// <remarks>Use this constructor in derived classes to set up the registry</remarks>
        protected ServiceRegistry()
        {
        }

        /// <summary>
        /// Configures the service registry from the specified settings.
        /// </summary>
        /// <param name="settings">Service registry settings</param>
        public ServiceRegistry(ServiceRegistrySettings settings)
        {
            // --- Check for duplicate container names
            var dupNames = (from container in settings.Containers
                           group container by container.Name
                           into g
                           where g.Count() > 1
                           select g.Key).ToList();
            if (dupNames.Count > 0) throw new DuplicatedContainerNameException(dupNames);

            // --- Register containers
            foreach (var container in settings.Containers)
            {
                RegisterContainer(settings.Containers.ToList(), container, new List<string>());
            }

            // --- Set the default container
            var defaultName = settings.DefaultContainer;
            if (string.IsNullOrWhiteSpace(defaultName))
            {
                // --- The first container is the default, or an empty container, if the is not one defined
                if (Containers.Count == 0)
                {
                    DefaultContainer = new ServiceContainer();
                    Containers.Add(DefaultContainer.Name, DefaultContainer);
                }
                else
                {
                    DefaultContainer = Containers[settings.Containers[0].Name];
                }
            }
            else
            {
                // --- Find the container by the provided name
                DefaultContainer = this[defaultName];
                if (DefaultContainer == null) throw new ContainerNotFoundException(defaultName);
            }
        }

        /// <summary>
        /// Creates a new instance with the specified service container as the default one.
        /// </summary>
        /// <param name="defaultContainer">Default service container</param>
        /// <param name="otherContainers">Other service containers</param>
        public ServiceRegistry(IServiceContainer defaultContainer, params IServiceContainer[] otherContainers)
            : this(defaultContainer, new List<IServiceContainer>(otherContainers))
        {
        }

        /// <summary>
        /// Creates a new instance with the specified service container as the default one.
        /// </summary>
        /// <param name="defaultContainer">Default service container</param>
        /// <param name="otherContainers">Other service containers</param>
        public ServiceRegistry(IServiceContainer defaultContainer, IEnumerable<IServiceContainer> otherContainers)
        {
            // --- Check the default container
            if (defaultContainer == null) throw new ArgumentNullException("defaultContainer");
            if (string.IsNullOrWhiteSpace(defaultContainer.Name)) throw new InvalidContainerNameException();

            // --- Add the default container
            DefaultContainer = defaultContainer;
            Containers.Add(DefaultContainer.Name, DefaultContainer);

            // --- Add all the other containers
            foreach (var container in otherContainers)
            {
                if (container == null) throw new ArgumentException("Container must not be null.");
                if (string.IsNullOrWhiteSpace(container.Name)) throw new InvalidContainerNameException();
                Containers.Add(container.Name, container);
            }
        }

        /// <summary>
        /// Checks if the registry has a container with the specified name.
        /// </summary>
        /// <param name="name">Container name</param>
        /// <returns>True, if the registry has the specified container; otherwise, false.</returns>
        public bool HasContainer(string name)
        {
            return Containers.ContainsKey(name);
        }

        /// <summary>
        /// Gets the service container with the specified name.
        /// </summary>
        /// <param name="name">Service container name</param>
        /// <returns>Service container instance if found; otherwise, null</returns>
        /// <remarks>Use null or empty string as a name to obtain the default container.</remarks>
        public IServiceContainer this[string name]
        {
            get 
            { 
                if (string.IsNullOrWhiteSpace(name)) return DefaultContainer;
                IServiceContainer container;
                return Containers.TryGetValue(name, out container) ? container : null;
            }
        }

        /// <summary>
        /// Gets the container names used within this registry
        /// </summary>
        public IReadOnlyList<string> ContainerNames
        {
            get { return new ReadOnlyCollection<string>(Containers.Keys.ToList()); }
        }

        /// <summary>
        /// Gets the number of containers within this registry
        /// </summary>
        public int ContainerCount
        {
            get { return Containers.Count; }
        }

        /// <summary>
        /// Gest the service object specified with the input parameter
        /// </summary>
        /// <param name="service">Type of the service</param>
        /// <returns>Service object, or null, if the specified service not found</returns>
        public object GetService(Type service)
        {
            return DefaultContainer.GetService(service);
        }

        /// <summary>
        /// Gest the service object specified with the specified type
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns>The requested service object</returns>
        public T GetService<T>()
        {
            return DefaultContainer.GetService<T>();
        }

        /// <summary>
        /// Registers the specified object with its related lifetime manager, and the
        /// construction parameters used by the lifetime manager.
        /// </summary>
        /// <param name="serviceType">Type of service to register</param>
        /// <param name="serviceObjectType">Type of object implementing the service</param>
        /// <param name="parameters">Object construction parameters</param>
        /// <param name="properties">Object properties to inject</param>
        /// <param name="ltManager">Lifetime manager object</param>
        /// <param name="customContext"></param>
        public void Register(Type serviceType, Type serviceObjectType,
            InjectedParameterSettingsCollection parameters = null,
            PropertySettingsCollection properties = null, ILifetimeManager ltManager = null,
            object customContext = null)
        {
            DefaultContainer.Register(serviceType, serviceObjectType, parameters, properties,
                ltManager, customContext);
        }

        /// <summary>
        /// Removes the specified service from the registry
        /// </summary>
        /// <param name="serviceType"></param>
        public void RemoveService(Type serviceType)
        {
            DefaultContainer.RemoveService(serviceType);
        }

        /// <summary>
        /// Gets the collection of registered services.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<Type> GetRegisteredServices()
        {
            return DefaultContainer.GetRegisteredServices();
        }

        /// <summary>
        /// Registers the specified container chain including parents
        /// </summary>
        /// <param name="containers">Collection of all containers to be registered</param>
        /// <param name="container"></param>
        /// <param name="visitedContainers"></param>
        // ReSharper disable ParameterTypeCanBeEnumerable.Local
        private void RegisterContainer(List<ServiceContainerSettings> containers, ServiceContainerSettings container, 
        // ReSharper restore ParameterTypeCanBeEnumerable.Local
            ICollection<string> visitedContainers)
        {
            // --- Go back, if the container has already been registered.
            var name = container.Name;
            if (Containers.ContainsKey(name)) return;

            // --- Observe circular container reference
            if (visitedContainers.Contains(name)) throw new CircularContainerReferenceException(name);

            // --- Obtain parent container, if there is any
            var parentName = container.ParentName;
            IServiceContainer parentContainer = null;
            if (parentName != null)
            {
                // --- First register the parent container
                visitedContainers.Add(name);
                var parentSettings = containers.FirstOrDefault(c => c.Name == parentName);
                if (parentSettings == null) throw new ContainerNotFoundException(parentName);
                RegisterContainer(containers, parentSettings, visitedContainers);
                parentContainer = Containers[parentName];
            }

            // --- Prepare the container and initialize its mappings
            var newContainer = new ServiceContainer(name, parentContainer);
            newContainer.ConfigureFrom(container);
            Containers.Add(name, newContainer);
        }
    }
}