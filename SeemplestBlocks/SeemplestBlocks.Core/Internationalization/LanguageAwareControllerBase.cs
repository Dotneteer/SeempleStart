using System;
using System.Threading;
using System.Web.Mvc;

namespace SeemplestBlocks.Core.Internationalization
{
    /// <summary>
    /// This controller can be used to implement language-agnostic controllers
    /// </summary>
    public abstract class LanguageAwareControllerBase : Controller
    {
        /// <summary>
        /// Begins to invoke the action in the current controller context.
        /// </summary>
        /// <returns>
        /// Returns an IAsyncController instance.
        /// </returns>
        /// <param name="callback">The callback.</param><param name="state">The state.</param>
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName;

            // --- Attempt to read the culture cookie from Request
            var cultureCookie = Request.Cookies["_culture"];
            if (cultureCookie != null)
            {
                cultureName = cultureCookie.Value;
            }
            else
            {
                cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0
                    ? Request.UserLanguages[0]
                    : null;
            }
            // --- Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName);

            // --- Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            return base.BeginExecuteCore(callback, state);
        }
    }
}