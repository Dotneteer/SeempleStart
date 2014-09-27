using System.Web.Mvc;

namespace Younderwater.Webclient.Controllers
{
    [Authorize]
    public class DiveController : Controller
    {
        // GET: Dive
        public ActionResult Index()
        {
            return View();
        }
    }
}