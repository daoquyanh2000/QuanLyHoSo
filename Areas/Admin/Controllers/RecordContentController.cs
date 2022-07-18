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
using System.Text;
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
        public ActionResult View(long ID)
        {
            var TPHS = Stuff.GetList<ThanhPhanHoSo>($@"select  * from ThanhPhanHoSo tphs  where tphs.ID ='{ID}'");
            var PDFs = Stuff.GetList<PDFThanhPhanHoSo>($@"
            select  ID,TenPDF from PDFThanhPhanHoso tphs  
            where tphs.IDThanhPhan ='{ID}' and TrangThai =1");
            JsonResult jsonResult = Json(new
            {
                TPHS = TPHS.FirstOrDefault(),
                PDFs = PDFs,
                heading = "Thành công",
                status = "success",
                message = "Lưu thành công!"
            }, JsonRequestBehavior.AllowGet); ;
            return jsonResult;
        }
        public ActionResult Save(List<HttpPostedFileBase> FilePDF)
        {
            var IdTPHS = Convert.ToInt64(Request["ID"]);
            //lưu dữ liệu thành phần hồ sơ
            var TPHS = new ThanhPhanHoSo();
            TPHS.IDHoSo = Convert.ToInt64(Request["IDHoSo"]);
            TPHS.TieuDe = (Request["TieuDe"]);
            TPHS.IDLoaiThanhPhan = Convert.ToInt64(Request["IDLoaiThanhPhan"]);
            TPHS.KiHieu = (Request["KiHieu"]);
            TPHS.MaThanhPhan = (Request["MaThanhPhan"]);
            TPHS.TrangThai = Convert.ToByte(Request["TrangThai"]);
            TPHS.ChuThich = (Request["ChuThich"]);
            TPHS.NgayTao = DateTime.Now.ToString();
            TPHS.NguoiTao = Session["UserNameNV"].ToString();
            //nếu tạo mới thành phần hồ sơ
            var listPDF = new List<PDFThanhPhanHoSo>();

            if (IdTPHS == 0)
            {
                using (var con = new SqlConnection(ConnectString.Setup()))
                {
                    con.Insert(TPHS);
                }
                //lưu file tài liệu
                var newTHPS = Stuff.GetList<ThanhPhanHoSo>("select top 1 ID from ThanhPhanHoSo order by ID desc").FirstOrDefault();
                for (int i = 0; i < FilePDF.Count; i++)
                {
                    var PDFTPHS = new PDFThanhPhanHoSo();
                    var newName = FilePDF[i].FileName.Split('.');
                    string fName = newName[0] + "_" + DateTime.Now.Ticks.ToString() + "." + newName[1];
                    string pathFolder = "/Assets/Admin/pdf";
                    //tao folder
                    Directory.CreateDirectory(Server.MapPath(pathFolder));
                    // tao duong dan path
                    string pathFile = Path.Combine(Server.MapPath(pathFolder), fName);
                    FilePDF[i].SaveAs(pathFile);
                    //lưu dữ liệu vào db
                    PDFTPHS.TenPDF = FilePDF[i].FileName;
                    PDFTPHS.PathPDF = pathFolder + "/" + fName;
                    PDFTPHS.TrangThai = 1;
                    PDFTPHS.IDThanhPhan = newTHPS.ID;
                    listPDF.Add(PDFTPHS);
                }
            }
            //nếu sửa hồ sơ
            else
            {
                //update lại thông tin thành phần hồ sơ
                ThanhPhanHoSoDao.Update(TPHS, IdTPHS, Session["UserNameNV"].ToString());
                //update lại file pDF
                for (int i = 0; i < FilePDF.Count; i++)
                {
                    var PDFTPHS = new PDFThanhPhanHoSo();
                    var newName = FilePDF[i].FileName.Split('.');
                    string fName = newName[0] + "_" + DateTime.Now.Ticks.ToString() + "." + newName[1];
                    string pathFolder = "/Assets/Admin/pdf";
                    //tao folder
                    Directory.CreateDirectory(Server.MapPath(pathFolder));
                    // tao duong dan path
                    string pathFile = Path.Combine(Server.MapPath(pathFolder), fName);
                    FilePDF[i].SaveAs(pathFile);
                    //lưu dữ liệu vào db
                    PDFTPHS.TenPDF = FilePDF[i].FileName;
                    PDFTPHS.PathPDF = pathFolder + "/" + fName;
                    PDFTPHS.TrangThai = 1;
                    PDFTPHS.IDThanhPhan = IdTPHS;
                    listPDF.Add(PDFTPHS);
                }

            }
            using (var con = new SqlConnection(ConnectString.Setup()))
            {
                con.Insert(listPDF);
            }
            JsonResult jsonResult = Json(new
            {
                heading = "Thành công",
                status = "success",
                message = "Lưu thành công!"
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ViewPDF(long ID)
        {
           
            var url = Stuff.GetList<PDFThanhPhanHoSo>($@"
            select  PathPDF from PDFThanhPhanHoso tphs  
            where tphs.ID ='{ID}' and TrangThai =1");
            JsonResult jsonResult = Json(new
            {
                data = url.FirstOrDefault().PathPDF,
                heading = "Thành công",
                status = "success",
                message = "Lưu thành công!"
            }, JsonRequestBehavior.AllowGet); ;
            return jsonResult;
        }

        /*public ActionResult SavePDF(List<HttpPostedFileBase> postedFiles)
        {
            var TPHS = new ThanhPhanHoSo();
            //lưu dữ liệu thành phần hồ sơ
            TPHS.NgayTao = DateTime.Now.ToString();
            TPHS.NguoiTao = Session["UserNameNV"].ToString();
            using (var con = new SqlConnection(ConnectString.Setup()))
            {
                con.Insert(TPHS);
            }
            //lưu files tài liệu vào bảng PDFThanhPhanHoSo
            var listPDF = new List<PDFThanhPhanHoSo>();
            HttpFileCollectionBase files = Request.Files;

            for (int i = 0; i < files.Count; i++)
            {
                var PDFTHPS = new PDFThanhPhanHoSo();
                byte[] bytes;
                using (BinaryReader br = new BinaryReader(files[i].InputStream))
                {
                    bytes = br.ReadBytes(files[i].ContentLength);
                }
                PDFTHPS.TenPDF = files[0].FileName;
                PDFTHPS.DataPDF = bytes;
                PDFTHPS.IDThanhPhan = Stuff.GetList<ThanhPhanHoSo>("select top 1 ID from ThanhPhanHoSo order by ID desc").FirstOrDefault().ID;
                listPDF.Add(PDFTHPS);

            }
            using (var con = new SqlConnection(ConnectString.Setup()))
            {
                con.Insert(listPDF);
            }
            JsonResult jsonResult = Json(new
            {
                TPHS = TPHS,
                heading = "Thành công",
                status = "success",
                message = "Lưu thành công!"
            }, JsonRequestBehavior.AllowGet); ;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }*/
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
        public ActionResult DeletePDF(long ID)
        {
            Stuff.ExecuteSql($"Update PDFThanhPhanHoSo Set TrangThai='0' where ID='{ID}'");

            JsonResult jsonResult = Json(new
            {
                heading = "Thành công",
                status = "success",
                message = "Lưu thành công!"
            }, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }
    }
}