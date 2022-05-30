using QuanLyLogin.Dao.DaoAdmin;
using QuanLyLogin.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace QuanLyLogin.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        // GET: Admin/User
        public ActionResult Index()
        {
            UserDao dao = new UserDao();
            List<DataNhanVien> listUser = dao.GetAllUser();
            ViewBag.listUser = listUser;
            return View();
        }
        [HttpPost]
        public JsonResult Index(string jsonObj)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            NhanVien nv = js.Deserialize<NhanVien>(jsonObj);
            UserDao.CreateNewUser(nv);
            return Json(nv);
        }

    }
}