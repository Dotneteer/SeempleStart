using System.Web.Mvc;
using SeemplestBlocks.Core.Internationalization;

namespace SeemplestCloud.WebClient.Controllers
{
    public class HomeController : LanguageAwareControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}