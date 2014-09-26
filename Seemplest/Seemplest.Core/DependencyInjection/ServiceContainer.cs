using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Seemplest.Core.Configuration;
using Seemplest.Core.Exceptions;

namespace Seemplest.Core.DependencyInjection
{
    /// <summary>
    /// This class implements a service container that behaves as a service registry.
    /// </summary>
    public class ServiceContainer: IServiceContainerEx
    {
        // --- Stores the registered services
        private readonly Dictionary<Type, ServiceMapping> _services =
            new Dictionary<Type, ServiceMapping>();

        /// <summary>
        /// Gets the name of the container.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the parent service container.
        /// </summary>
        public IServiceContainer Parent { get; private set; }

        /// <summary>
        /// Creates a new instance with the specified parent container.
        /// </summary>
        /// <param name="name">Container name</param>
        /// <param name="parent">Parent service container</param>
        public ServiceContainer(string name = "default", IServiceContainer parent = null)
        {
            if (name == null) throw new ArgumentNullException("name");
            Name = name;
            Parent = parent;
        }

        /// <summary>
        /// Gest the service object specified with the input parameter
        /// </summary>
        /// <param name="service">Type of the service</param>
        /// <returns>Service object, or null, if the specified service not found</returns>
        public object GetService(Type service)
        {
            return ((IServiceContainerEx)(this)).GetService(service, new List<Type>());
        }

        /// <summary>
        /// Gest the service object specified with the specified type
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns>The requested service object</returns>
        public T GetService<T>()
        {
            return (T)GetService(typeof (T));
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
            if (serviceType == null) throw new ArgumentNullException("serviceType");
            if (serviceObjectType == null) throw new ArgumentNullException("serviceObjectType");
            if (_services.ContainsKey(serviceType))
            {
                throw new ServiceAlreadyRegisteredException(serviceType);
            }

            // --- Register the new mapping
            _services[serviceType] = new ServiceMapping(
                serviceType,
                serviceObjectType, 
                ltManager ?? new PerCallLifetimeManager(),
                parameters,
                properties,
                customContext);

            // --- Reset lifetime managers
            foreach (var mapping in _services.Values)
            {
                mapping.LifetimeManager.ResetState();
                mapping.Resolved = false;
            }
        }

        /// <summary>
        /// Registers the specified object with its related lifetime manager, and the
        /// construction parameters used by the lifetime manager.
        /// </summary>
        /// <typeparam name="TService">Type of service to register</typeparam>
        /// <typeparam name="TObject">Type of object implementing the servi</typeparam>
        /// <param name="parameters">Object construction parameters</param>
        /// <param name="properties">Object properties to inject</param>
        /// <param name="ltManager">Lifetime manager object</param>
        /// <param name="customContext"></param>
        public void Register<TService, TObject>(InjectedParameterSettingsCollection parameters = null,
            PropertySettingsCollection properties = null, ILifetimeManager ltManager = null,
            object customContext = null)
        {
            Register(typeof(TService), typeof(TObject), parameters, properties, ltManager, customContext);
        }

        /// <summary>
        /// Registers the specified object with its related lifetime manager, and the
        /// construction parameters used by the lifetime manager.
        /// </summary>
        /// <typeparam name="TService">Type of service to register</typeparam>
        /// <typeparam name="TObject">Type of object implementing the servi</typeparam>
        /// <param name="constructorParams">Constructor parameters</param>
        public void Register<TService, TObject>(params object[] constructorParams)
        {
            var injectedPars = constructorParams
                .Select(p => new InjectedParameterSettings(p.GetType(), p));
            Register(typeof (TService), typeof (TObject),
                new InjectedParameterSettingsCollection(injectedPars));
        }

        /// <summary>
        /// Removes the specified service from the registry
        /// </summary>
        /// <param name="serviceType"></param>
        public void RemoveService(Type serviceType)
        {
            _services.Remove(serviceType);
        }

        /// <summary>
        /// Gets the collection of registered services.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<Type> GetRegisteredServices()
        {
            return new ReadOnlyCollection<Type>(_services.Keys.ToList());
        }

        /// <summary>
        /// Configures the container from the specified settings
        /// </summary>
        /// <param name="settings">Service container settings</param>
        public void ConfigureFrom(ServiceContainerSettings settings)
        {
            _services.Clear();
            foreach (var mapping in settings.Mappings)
            {
                var ltType = mapping.Lifetime ?? typeof(PerCallLifetimeManager);
                var ltManager = (ILifetimeManager)Activator.CreateInstance(ltType);
                Register(mapping.From, mapping.To, mapping.Parameters, mapping.Properties,
                    ltManager);
            }
        }

        /// <summary>
        /// Resolves the lifetime manager of the specified mapping.
        /// </summary>
        /// <param name="mapping">Type mapping information</param>
        /// <param name="visitedTypes">Types visited during resolution</param>
        private void ResolveLifetimeManager(ServiceMapping mapping, List<Type> visitedTypes)
        {
            // --- Do not resolve again
            if (mapping.Resolved) return;

            // --- Obtain the appropriate constructor parameters
            var ctorPars = new object[0];
            if (mapping.ConstructionParameters != null)
            {
                var ctors = FindMatchingConstructors(mapping);
                if (ctors.Length != 1)
                {
                    // --- Check for ambiguity
                    var signature = GetConstructorSignature(mapping);
                    if (ctors.Length == 0)
                    {
                        throw new NoMatchingConstructorException(mapping.ServiceObjectType, signature);
                    }
                    throw new AmbigousConstructorException(mapping.ServiceObjectType, signature);
                }

                // --- At this point we have the only matching constructor, let's inject its parameters
                var ctor = ctors[0];
                ctorPars = PrepareConstructorParameters(mapping, visitedTypes, ctor);
            }

            // --- Prepare the lifetime manager
            var ltManager = mapping.LifetimeManager;
            ltManager.ServiceType = mapping.ServiceType;
            ltManager.ServiceObjectType = mapping.ServiceObjectType;
            ltManager.ConstructionParameters = ctorPars;
            ltManager.Properties = mapping.Properties;
            ltManager.CustomContext = mapping.CustomContext;
            mapping.Resolved = true;
        }

        /// <summary>
        /// Prepares constructor parameters.
        /// </summary>
        /// <param name="mapping">Service mapping information</param>
        /// <param name="visitedTypes">Types visited during resolution</param>
        /// <param name="ctor">Constructor to preapre</param>
        /// <returns>Array of constructor parameters</returns>
        private object[] PrepareConstructorParameters(ServiceMapping mapping, List<Type> visitedTypes, ConstructorInfo ctor)
        {
            var parTypes = ctor.GetParameters().Select(p => p.ParameterType).ToArray();
            var parCount = parTypes.Length;
            var ctorPars = new object[parCount];

            // --- Let's match the specified parameter values
            for (var i = 0; i < parCount; i++)
            {
                InjectedParameterSettings param;
                if (i < mapping.ConstructionParameters.Count &&
                    !(param = mapping.ConstructionParameters[i]).Resolve)
                {
                    // --- Use the specified value
                    if (param.Type == param.Value.GetType())
                    {
                        ctorPars[i] = param.Value;
                    }
                    else
                    {
                        var converter = TypeDescriptor.GetConverter(param.Type);
                        ctorPars[i] = converter.ConvertFromString(param.Value.ToString());
                    }
                }
                else
                {
                    // --- Resolve the instance from the container
                    var lastVisitedTypes = new List<Type>(visitedTypes);
                    var paramObject = ((IServiceContainerEx)(this))
                        .GetService(parTypes[i], lastVisitedTypes);
                    if (paramObject == null)
                    {
                        throw new NoMatchingConstructorException(mapping.ServiceObjectType, 
                            GetConstructorSignature(mapping), i);
                    }
                    ctorPars[i] = paramObject;
                }
            }
            return ctorPars;
        }

        /// <summary>
        /// Finds all constructors matching with the specified mapping
        /// </summary>
        private static ConstructorInfo[] FindMatchingConstructors(ServiceMapping mapping)
        {
            // --- Find matching constructors
            var ctorParamIndex = 0;
            var ctors = mapping.ServiceObjectType.GetConstructors();
            foreach (var param in mapping.ConstructionParameters)
            {
                // --- This parameter should be matched directly
                ctors = (from ctor in ctors
                         let pars = ctor.GetParameters()
                         where pars.Length > ctorParamIndex &&
                               pars[ctorParamIndex].ParameterType == param.Type
                         select ctor).ToArray();
                if (ctors.Length == 0) break;
                ctorParamIndex++;
            }

            // --- If there are multiple candidates, try to obtains the one with exact number of arguments
            if (ctors.Length <= 1) return ctors;
            var exactOne = ctors.Where(c => c.GetParameters().Length 
                                            == mapping.ConstructionParameters.Count).ToArray();
            return exactOne.Length == 1 ? exactOne : ctors;
        }

        /// <summary>
        /// Obtains the signature of the constructor specified by the mapping
        /// </summary>
        private static string GetConstructorSignature(ServiceMapping mapping)
        {
            // --- Create signature
            var needComma = false;
            var signature = new StringBuilder("(");
            foreach (var type in mapping.ConstructionParameters.Select(p => p.Type.FullName))
            {
                if (needComma) signature.Append(", ");
                needComma = true;
                signature.Append(type);
            }
            signature.Append(")");
            return signature.ToString();
        }

        /// <summary>
        /// Gest the service object specified with the input parameter
        /// </summary>
        /// <param name="service">Type of the service</param>
        /// <param name="visitedTypes">Collection of types already visited</param>
        /// <returns>Service object, or null, if the specified service not found</returns>
        object IServiceContainerEx.GetService(Type service, List<Type> visitedTypes)
        {
            // --- Check for circular reference
            if (visitedTypes.Contains(service))
            {
                throw new CircularServiceReferenceException(service);
            }

            // --- Obtain the service object
            ServiceMapping mapping;
            if (!_services.TryGetValue(service, out mapping))
            {
                // --- Service not found, check the parent container
                return Parent == null 
                    ? null 
                    : ((IServiceContainerEx)Parent).GetService(service, visitedTypes);
            }

            // --- Service descriptor found, retrieve the service object
            visitedTypes.Add(service);
            ResolveLifetimeManager(mapping, visitedTypes);
            return mapping.LifetimeManager.GetObject();
        }

        /// <summary>
        /// This class represents a service mapping
        /// </summary>
        private class ServiceMapping
        {
            /// <summary>
            /// Gets the type of service
            /// </summary>
            public Type ServiceType { get; private set; }

            /// <summary>
            /// Gets the type of service object
            /// </summary>
            public Type ServiceObjectType { get; private set; }

            /// <summary>
            /// Gets the lifetime manager associated with the service type
            /// </summary>
            public ILifetimeManager LifetimeManager { get; private set; }

            /// <summary>
            /// Gets the constructor parameters
            /// </summary>
            public InjectedParameterSettingsCollection ConstructionParameters { get; private set; }

            /// <summary>
            /// Gets the collection of properties to inject
            /// </summary>
            public PropertySettingsCollection Properties { get; private set; }

            /// <summary>
            /// Gest the custom context object of the lifetime manager
            /// </summary>
            public object CustomContext { get; private set; }

            /// <summary>
            /// Indicates if the lifetime manager has resolved
            /// </summary>
            public bool Resolved { get; set; }

            /// <summary>
            /// Creates a new instance with the specified parameters.
            /// </summary>
            public ServiceMapping( 
                Type serviceType,
                Type serviceObjectType, 
                ILifetimeManager lifetimeManager, 
                InjectedParameterSettingsCollection constructionParameters, 
                PropertySettingsCollection properties, 
                object customContext)
            {
                ServiceType = serviceType;
                ServiceObjectType = serviceObjectType;
                LifetimeManager = lifetimeManager;
                ConstructionParameters = constructionParameters;
                Properties = properties;
                CustomContext = customContext;
            }
        }
    }
}