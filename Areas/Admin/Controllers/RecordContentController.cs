using Dapper.Contrib.Extensions;
using PagedList;
using QuanLyHoSo.Dao;
using QuanLyHoSo.Dao.DaoAdmin;
using QuanLyHoSo.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyHoSo.Areas.Admin.Controllers
{
    public class RecordContentController : Controller
    {
        // GET: Admin/RecordContent
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult Search(long ID, int? page, string keyword= "")
        {


            int pageNumber = (page ?? 1);
            int pageSizeNumber = 3;
            if (keyword == null) keyword = "";

            var results = from hs in ThanhPhanHoSoDao.GetAllContentByID(ID)
                          where (hs.TrangThai !=10)
                          && (hs.TieuDe.Contains(keyword)
                          || hs.TenLoaiThanhPhan.Contains(keyword)
                          || hs.KiHieu.Contains(keyword)
                          || hs.ChuThich.Contains(keyword))
                          orderby hs.ID descending
                          select hs;
            ViewBag.search = keyword;
            var model = results.ToPagedList(pageNumber, pageSizeNumber);
            return PartialView("ThanhPhanTable", model);
        }
/*        public ActionResult Save(List<HttpPostedFileBase> postedFiles)
        {

            //Lưu dữ liệu thành phần
            var TPHS = new ThanhPhanHoSo();

            using (var con = new SqlConnection(ConnectString.Setup()))
            {
                TPHS.TieuDe = Request["TieuDe"];
                TPHS.MaThanhPhan = Request["MaThanhPhan"];
                TPHS.IDLoaiThanhPhan = Convert.ToInt64(Request["MaThanhPhan"]);
                TPHS.KiHieu = Request["KiHieu"];
                TPHS.ChuThich = Request["ChuThich"];
                TPHS.TrangThai = Convert.ToByte(Request["TrangThai"]);
                TPHS.NgayTao = DateTime.Now.ToString();
                TPHS.NguoiTao = Session["UserNameNV"].ToString();
                con.Insert(TPHS);
            }
            //thêm data vào bảng PDFThanhPhanHoSo

            var listPDF = new List<PDFThanhPhanHoSo>();
            if (postedFiles != null)
            {
                foreach (var postedFile in postedFiles)
                {
                    var PDFTHPS = new PDFThanhPhanHoSo();
                    byte[] bytes;
                    using (BinaryReader br = new BinaryReader(postedFile.InputStream))
                    {
                        bytes = br.ReadBytes(postedFile.ContentLength);
                    }
                    PDFTHPS.TenPDF = postedFile.FileName;
                    PDFTHPS.PathPDF = bytes;

                    listPDF.Add(PDFTHPS);

                    PDFTHPS.IDThanhPhan = Stuff.GetAll<ThanhPhanHoSo>().LastOrDefault().ID;
                    PDFTHPS.TrangThai = 1;
                    using (var con = new SqlConnection(ConnectString.Setup()))
                    {
                        con.Insert(PDFTHPS);
                    }
                }
            }
            JsonResult jsonResult = Json(new
            {
                data = listPDF,
                heading = "Thành công",
                status = "success",
                message = "Lưu thành công!"
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }*/
        public ActionResult SavePDF(List<HttpPostedFileBase> postedFiles)
        {
            var listPDF = new List<PDFThanhPhanHoSo>();
            if (postedFiles != null)
            {
                foreach (var postedFile in postedFiles)
                {
                    var PDFTHPS = new PDFThanhPhanHoSo();
                    var newName = postedFile.FileName.Split('.');
                    string fName = newName[0] + "_" + DateTime.Now.Ticks.ToString() + "." + newName[1];
                    string pathFolder = "/Assets/Admin/pdf/";
                    //tao folder
                    Directory.CreateDirectory(Server.MapPath(pathFolder));
                    // tao duong dan path
                    string pathFile = Path.Combine(Server.MapPath(pathFolder), fName);
                    PDFTHPS.TenPDF = postedFile.FileName;
                    PDFTHPS.PathPDF = pathFolder+ "/" + fName;
                    PDFTHPS.TrangThai = 1;
                    postedFile.SaveAs(pathFile);
                    listPDF.Add(PDFTHPS);
                }
                TempData["listPDF"] = listPDF;
            }
            long rc;
            using (var con = new SqlConnection(ConnectString.Setup()))
            {
                rc = con.Insert(listPDF);
            }
            var listID = Stuff.GetList<PDFThanhPhanHoSo>($"select top {rc} ID,TenPDF from PDFThanhPhanHoSo order by ID desc");
            return PartialView("PDFTable", listID);
        }
        public ActionResult GetPDF(long fileId)
        {
            var listPDF = Stuff.GetAll<PDFThanhPhanHoSo>().Where(x => x.ID == fileId).Where(x=>x.TrangThai==1);

            JsonResult jsonResult = Json(new
            {
                data = listPDF.FirstOrDefault(),
                heading = "Thành công",
                status = "success",
                message = "Lưu thành công!"
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult Delete(long fileId)
        {
            Stuff.ExecuteSql($"Update PDFThanhPhanHoSo Set TrangThai='0' where ID='{fileId}'");

            JsonResult jsonResult = Json(new
            {
                heading = "Thành công",
                status = "success",
                message = "Lưu thành công!"
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
    }
}