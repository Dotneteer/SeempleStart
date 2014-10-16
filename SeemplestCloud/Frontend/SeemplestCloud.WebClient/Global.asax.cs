using System;
using System.Security.Cryptography;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Newtonsoft.Json;
using Seemplest.Core.DependencyInjection;
using SeemplestBlocks.Core.Internationalization;
using SeemplestCloud.Services.Infrastructure;
using SeemplestCloud.Services.SubscriptionService;
using SeemplestCloud.WebClient.Controllers;
using SeemplestCloud.WebClient.Infrastructure;
using SeemplestCloud.WebClient.Models.UserManagement;

namespace SeemplestCloud.WebClient
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            ServiceConfig.RegisterServices();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // --- Use implemented languages
            CultureHelper.SetImplementedCultures(ImplementedCultures.GetCultureCodes());
        }

        /// <summary>
        /// This method sets the current principal value ofter the authentication.
        /// </summary>
        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            // --- Is the a forms authentication cookie?
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null) return;


            try
            {
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (authTicket == null || authTicket.UserData == AuthenticationTicketState.Terminated) return;
                var ticket = JsonConvert.DeserializeObject<AppPrincipal.SerializationModel>(authTicket.UserData);
                HttpContext.Current.User = new AppPrincipal(ticket, null);
            }
            catch (CryptographicException)
            {
                // --- This exception is intentionally caught
            }
        }
    }
}
