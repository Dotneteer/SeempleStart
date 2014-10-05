using System;
using System.Web;
using System.Web.Mvc;
using SeemplestBlocks.Core.Internationalization;

namespace SeemplestCloud.WebClient.Controllers
{
    public class LanguageController : Controller
    {
        public ActionResult SetLanguage(string code, string returnLink)
        {
            // --- Validate input
            code = CultureHelper.GetImplementedCulture(code);
            
            // --- Save culture in a cookie
            var cookie = Request.Cookies["_culture"];
            if (cookie != null)
            {
                cookie.Value = code;
            }
            else
            {
                cookie = new HttpCookie("_culture")
                {
                    Value = code, 
                    Expires = DateTime.Now.AddYears(1)
                };
            }
            Response.Cookies.Add(cookie);
            return Redirect(returnLink);
        }
    }
}