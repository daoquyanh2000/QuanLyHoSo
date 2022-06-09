using PagedList;
using QuanLyHoSo.Dao.DaoAdmin;
using System.Linq;
using System.Web.Mvc;

namespace QuanLyHoSo.Areas.Admin.Controllers
{
    public class RoleController : Controller
    {
        // GET: Admin/Role
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult Search(string keyword, int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSizeNumber = 8;
            if (keyword == null) keyword = "";
            var results = from nv in RoleDao.GetKieuNhanViens()
                          where nv.TenQuyen.Contains(keyword)
                          select nv;
            ViewBag.search = keyword;
            var model = results.ToPagedList(pageNumber, pageSizeNumber);
            return PartialView("_RoleTable", model);
        }
    }
}