using QuanLyHoSo.Models;
using System.Web.Mvc;

namespace QuanLyHoSo.Areas.Admin.Controllers
{
    public class StorageController : Controller
    {
        // GET: Admin/Storage
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult Modal()
        {
            return PartialView("Modal");
        }
        public PartialViewResult Search(string keyword, int? page)
        {

            return PartialView("StorageTable");
        }
        [HttpPost]
        public JsonResult Save(Kho kho)
        {
            if (kho.ID == 0)
            {
                //create new kho
                
            }
            else
            {
                //update kho
            }
            return Json(new
            {

            }, JsonRequestBehavior.AllowGet);
        }

    }
}