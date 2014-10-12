using System.Web.Mvc;
using SeemplestBlocks.Core.Internationalization;

namespace SeemplestCloud.WebClient.Controllers
{
    [Authorize]
    public class SubscriptionController : LanguageAwareControllerBase
    {
        // GET: Subscription
        public ActionResult InviteUsers()
        {
            return View();
        }

        public ActionResult ManageUserRights()
        {
            return View();
        }

    }
}