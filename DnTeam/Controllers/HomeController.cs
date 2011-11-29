using System.Web.Mvc;

namespace DnTeam.Controllers
{
    public class HomeController : Controller
    {
        [OpenIdAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Xrds()
        {
            Response.ContentType = "application/xrds+xml";
            return View("Xrds");
        }
    }
}
