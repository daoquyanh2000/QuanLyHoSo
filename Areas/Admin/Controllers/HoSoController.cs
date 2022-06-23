using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyHoSo.Areas.Admin.Controllers
{
    public class HoSoController : Controller
    {
        // GET: Admin/HoSo
        public ActionResult Index()
        {
            return View();
        }
    }
}