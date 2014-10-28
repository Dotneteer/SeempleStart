using System.Web;

namespace SeemplestCloud.Services.Infrastructure
{
    /// <summary>
    /// Default implementation of IAppPrincipalProvider
    /// </summary>
    public class AppPrincipalProvider : IAppPrincipalProvider
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