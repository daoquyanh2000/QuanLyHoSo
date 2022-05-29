using QuanLyLogin.Dao.DaoAdmin;
using QuanLyLogin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        public JsonResult Create()
        {
            return Json(new
            {
                status = true,
                msg="hello"
            });
        }

    }
}