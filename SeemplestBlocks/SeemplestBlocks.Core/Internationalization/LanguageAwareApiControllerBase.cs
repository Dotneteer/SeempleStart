using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace SeemplestBlocks.Core.Internationalization
{
    /// <summary>
    /// This controller can be used to implement language-agnostic controllers
    /// </summary>
    public abstract class LanguageAwareApiControllerBase : ApiController
    {
        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            string cultureName;

            // --- Attempt to read the culture cookie from Request
            var cultureCookie = controllerContext.Request.Headers.GetCookies("_culture").FirstOrDefault();
            if (cultureCookie != null)
            {
                cultureName = cultureCookie["_culture"].Value;
            }
            else
            {
                var headers = controllerContext.Request.Headers;
                cultureName = headers.AcceptLanguage != null && headers.AcceptLanguage.Count > 0
                    ? headers.AcceptLanguage.First().Value
                    : null;
            }
            // --- Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName);

            // --- Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            return base.ExecuteAsync(controllerContext, cancellationToken);
        }
    }
}