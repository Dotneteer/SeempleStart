using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SeemplestBlocks.Core.Internationalization;
using SeemplestCloud.WebClient.Infrastructure;

namespace SeemplestCloud.WebClient
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // --- Use implemented languages
            CultureHelper.SetImplementedCultures(ImplementedCultures.GetCultureCodes());
        }
    }
}
