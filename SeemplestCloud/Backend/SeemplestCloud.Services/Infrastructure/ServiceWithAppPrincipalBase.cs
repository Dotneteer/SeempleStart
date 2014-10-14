using Seemplest.Core.ServiceObjects;
using System.Web;

namespace SeemplestCloud.Services.Infrastructure
{
    /// <summary>
    /// This class implements a service that can access the AppPrincipal object
    /// </summary>
    public abstract class ServiceWithAppPrincipalBase: ServiceObjectBase
    {
        /// <summary>
        /// Gets the current application principal
        /// </summary>
        public AppPrincipal Principal
        {
            get { return (AppPrincipal)HttpContext.Current.User; }
        }
    }
}