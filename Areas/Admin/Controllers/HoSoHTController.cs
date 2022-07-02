using PagedList;
using QuanLyHoSo.Dao.DaoAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyHoSo.Areas.Admin.Controllers
{
    public class HoSoHTController : Controller
    {
        // GET: Admin/HoSoHT
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public PartialViewResult Search(string keyword, int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSizeNumber = 3;
            if (keyword == null) keyword = "";

            var results = from hs in RecordDao.GetAllRecord()
                          where (hs.TrangThai == 1)
                          && (hs.TieuDe.Contains(keyword)
                          || hs.MaHoSo.Contains(keyword)
                          || hs.TenDanhMuc.Contains(keyword)
                          || hs.TenKho.Contains(keyword)
                          || hs.TenNgan.Contains(keyword))
                          orderby hs.NgaySua descending
                          select hs;
            ViewBag.search = keyword;
            var model = results.ToPagedList(pageNumber, pageSizeNumber);
            return PartialView("HoSoHTTable",model);
        }
    }
}