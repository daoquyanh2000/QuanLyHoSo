using Dapper.Contrib.Extensions;
using PagedList;
using QuanLyHoSo.Dao;
using QuanLyHoSo.Dao.DaoAdmin;
using QuanLyHoSo.Models;
using System;
using System.Data.SqlClient;
using System.Linq;
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
            return PartialView();
        }
        public PartialViewResult Search(string keyword, int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSizeNumber = 3;
            if (keyword == null) keyword = "";
            var results = from k in Stuff.GetAll<Kho>()
                          where k.TenKho.Contains(keyword) && k.TrangThai != 10
                          select k;
            ViewBag.search = keyword;
            var model = results.ToPagedList(pageNumber, pageSizeNumber);
            return PartialView("StorageTable",model);
        }
        [HttpPost]
        public JsonResult Save(Kho kho)
        {
            string UserNameNV = Session["UserNameNV"].ToString();
            if (kho.ID == 0)
            {

                //create new kho
                StorageDao.CreateStorage(kho, UserNameNV);
            }
            else
            {
                //update kho
                StorageDao.UpdateStorage(kho, kho.ID, UserNameNV);
            }
            return Json(new
            {

            }, JsonRequestBehavior.AllowGet);
        }

    }
}