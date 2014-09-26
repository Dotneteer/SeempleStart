using Seemplest.Core.DependencyInjection;

namespace Seemplest.Core.ServiceObjects
{
    /// <summary>
    /// This class implements a service factory that creates service objects with empty context.
    /// </summary>
    public class EmptyContextServiceFactory : ContextedServiceFactoryBase
    {
        /// <summary>
        /// Initializes this factory with the specified service locator
        /// </summary>
        /// <param name="serviceLocator">Service locator used by this object</param>
        public EmptyContextServiceFactory(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
        }

        /// <summary>
        /// Creates a call context for the service object
        /// </summary>
        /// <returns>Service call context instance</returns>
        protected override IServiceCallContext CreateServiceCallContext()
        {
            return new ServiceCallContext();
        }
    }
}