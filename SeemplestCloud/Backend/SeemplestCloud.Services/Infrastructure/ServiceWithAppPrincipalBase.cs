using Seemplest.Core.DependencyInjection;
using Seemplest.Core.ServiceObjects;

namespace SeemplestCloud.Services.Infrastructure
{
    /// <summary>
    /// This class implements a service that can access the AppPrincipal object
    /// </summary>
    public abstract class ServiceWithAppPrincipalBase : ServiceObjectBase
    {
        /// <summary>
        /// Gets the current application principal
        /// </summary>
        public AppPrincipal Principal
        {
            get
            {
                var provider = ServiceManager.GetService<IAppPrincipalProvider>();
                return provider.Principal;
            }
        }
    }
}
