using Seemplest.Core.DependencyInjection;

namespace Seemplest.Core.ServiceObjects
{
    /// <summary>
    /// This interface describes the behavior of a service object
    /// </summary>
    public interface IServiceObject
    {
        /// <summary>
        /// Gets the call context associated with the service object
        /// </summary>
        IServiceCallContext CallContext { get; }

        /// <summary>
        /// Gets the service locator that can be used to obtain and manage services
        /// </summary>
        IServiceLocator ServiceLocator { get; }

        /// <summary>
        /// Sets up the call context of a service.
        /// </summary>
        /// <param name="context">Context instance</param>
        void SetCallContext(IServiceCallContext context);

        /// <summary>
        /// Sets up the locator used by the service object
        /// </summary>
        /// <param name="locator">Service locator object</param>
        void SetServiceLocator(IServiceLocator locator);
    }
}