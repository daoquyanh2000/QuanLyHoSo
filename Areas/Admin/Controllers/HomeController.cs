using System.Web.Mvc;

namespace QuanLyHoSo.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            if(!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
    }
}