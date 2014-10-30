using Seemplest.Core.DependencyInjection;
using SeemplestBlocks.Core.Internationalization;
using SeemplestCloud.Services.Infrastructure;

namespace SeemplestCloud.WebClient.Infrastructure
{
    public class ControllerWithAppPrincipalBase: LanguageAwareControllerBase
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