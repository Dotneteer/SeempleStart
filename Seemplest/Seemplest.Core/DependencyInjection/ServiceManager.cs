using System;
using System.Linq;
using System.Collections.Generic;
using Seemplest.Core.Configuration;

namespace Seemplest.Core.DependencyInjection
{
    /// <summary>
    /// This static class is intended to be a singleton service manager class for most
    /// application.
    /// </summary>
    public static class ServiceManager
    {
        private const string SERVICE_MANAGER_SECTION = "ServiceManager";

        /// <summary>
        /// Initializes the static members of this class
        /// </summary>
        static ServiceManager()
        {
            ConfigureFrom(SERVICE_MANAGER_SECTION);
        }

        /// <summary>
        /// Gets the service registry to be used with this class
        /// </summary>
        public static IServiceRegistry ServiceRegistry { get; private set; }

        /// <summary>
        /// Set the service registry to be used with this class
        /// </summary>
        /// <param name="registry">The registry to be used with this class</param>
        public static void SetRegistry(IServiceRegistry registry)
        {
            if (registry == null) throw new ArgumentNullException("registry");
            ServiceRegistry = registry;
        }

        /// <summary>
        /// Resets the service registry
        /// </summary>
        public static void Reset()
        {
            ServiceRegistry = new DefaultServiceRegistry();
        }

        /// <summary>
        /// Configures the configurable part of the service manager from the 
        /// specified section
        /// </summary>
        /// <param name="sectionName">Name of the configuration section</param>
        public static void ConfigureFrom(string sectionName)
        {
            var registry = new DefaultServiceRegistry();
            if (AppConfigurationManager.IsSectionDefined(sectionName))
            {
                registry.GetConfigurableContainer().ConfigureFrom(AppConfigurationManager
                    .GetSettings<ServiceContainerSettings>(sectionName));
            }
            ServiceRegistry = registry;
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
        public static void Register(Type serviceType, Type serviceObjectType,
            InjectedParameterSettingsCollection parameters = null,
            PropertySettingsCollection properties = null, ILifetimeManager ltManager = null, object customContext = null)
        {
            ServiceRegistry.DefaultContainer.Register(serviceType, serviceObjectType, parameters,
                properties, ltManager, customContext);
        }

        /// <summary>
        /// Registers the specified object with its related lifetime manager, and the
        /// construction parameters used by the lifetime manager.
        /// </summary>
        /// <typeparam name="TService">Type of service to register</typeparam>
        /// <typeparam name="TObject">Type of object implementing the service</typeparam>
        /// <param name="parameters">Object construction parameters</param>
        /// <param name="properties">Object properties to inject</param>
        /// <param name="ltManager">Lifetime manager object</param>
        /// <param name="customContext"></param>
        public static void Register<TService, TObject>(InjectedParameterSettingsCollection parameters = null,
            PropertySettingsCollection properties = null, ILifetimeManager ltManager = null,
            object customContext = null)
        {
            ServiceRegistry.DefaultContainer.Register(typeof(TService), typeof(TObject), 
                parameters, properties, ltManager, customContext);
        }

        /// <summary>
        /// Registers the specified object with its related lifetime manager, and the
        /// construction parameters used by the lifetime manager.
        /// </summary>
        /// <typeparam name="TService">Type of service to register</typeparam>
        /// <typeparam name="TObject">Type of object implementing the servi</typeparam>
        /// <param name="constructorParams">Constructor parameters</param>
        public static void Register<TService, TObject>(params object[] constructorParams)
        {
            var injectedPars = constructorParams
                .Select(p => new InjectedParameterSettings(p.GetType(), p));
            Register(typeof(TService), typeof(TObject),
                new InjectedParameterSettingsCollection(injectedPars));
        }
        /// <summary>
        /// Removes the specified service from the registry
        /// </summary>
        /// <param name="serviceType"></param>
        public static void RemoveService(Type serviceType)
        {
            ServiceRegistry.DefaultContainer.RemoveService(serviceType);
        }

        /// <summary>
        /// Gets the collection of registered services.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyList<Type> GetRegisteredServices()
        {
            return ServiceRegistry.DefaultContainer.GetRegisteredServices();
        }

        /// <summary>
        /// Gest the service object specified with the input parameter
        /// </summary>
        /// <param name="service">Type of the service</param>
        /// <returns>Service object, or null, if the specified service not found</returns>
        public static object GetService(Type service)
        {
            return ServiceRegistry.DefaultContainer.GetService(service);
        }

        /// <summary>
        /// Gest the service object specified with the specified type
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns>The requested service object</returns>
        public static T GetService<T>()
        {
            return ServiceRegistry.DefaultContainer.GetService<T>();
        }
    }
}