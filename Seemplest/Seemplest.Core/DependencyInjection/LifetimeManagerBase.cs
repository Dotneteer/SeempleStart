using System;
using Seemplest.Core.Common;
using Seemplest.Core.Configuration;
using Seemplest.Core.Interception;

namespace Seemplest.Core.DependencyInjection
{
    public abstract class LifetimeManagerBase: ILifetimeManager
    {
        /// <summary>
        /// Gets or sets the type of the service supported by this lifetime manager
        /// </summary>
        public Type ServiceType { get; set; }

        /// <summary>
        /// Gets the type of the service object provided by this lifetime manager
        /// </summary>
        public Type ServiceObjectType { get; set; }

        /// <summary>
        /// Gets or sets the construction parameters of this lifetime manager
        /// </summary>
        public object[] ConstructionParameters { get; set; }

        /// <summary>
        /// Gets or sets the property values to set after construction
        /// </summary>
        public PropertySettingsCollection Properties { get; set; }

        /// <summary>
        /// Gets or sets the custom context object of the lifetime manager
        /// </summary>
        public object CustomContext { get; set; }

        /// <summary>
        /// Retrieve an object from the backing store associated with this Lifetime manager.
        /// </summary>
        /// <returns>
        /// The object retrieved by the lifetime manager.
        /// </returns>
        public abstract object GetObject();

        /// <summary>
        /// Resets the state of the lifetime manager
        /// </summary>
        public abstract void ResetState();

        /// <summary>
        /// Creates a new object instance according to the lifetime manager's properties.
        /// If the target type is decorated with the <see cref="InterceptedAttribute"/>,
        /// it is automatically intercepted.
        /// </summary>
        /// <returns></returns>
        protected virtual object CreateObjectInstance()
        {
            var instance = ConfigurationHelper.CreateInstance(
                ServiceObjectType,
                ConstructionParameters,
                Properties);
            if (ServiceObjectType.GetCustomAttributes(typeof (InterceptedAttribute), true).Length > 0 &&
                ServiceObjectType.GetCustomAttributes(typeof(DisableInterceptionAttribute), false).Length == 0)
            {
                // --- This is an intercepted object
                instance = Interceptor.GetInterceptedObject(ServiceType, instance, null);
            }
            return instance;
        }
    }
}