using Dapper.Contrib.Extensions;
using OfficeOpenXml;
using PagedList;
using QuanLyHoSo.Dao;
using QuanLyHoSo.Dao.DaoAdmin;
using QuanLyHoSo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyHoSo.Areas.Admin.Controllers
{
    [Authorize(Roles = "NhanVien")]
    public class UserController : Controller
    {
        // GET: Admin/User
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult Search(string keyword, int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSizeNumber = 4;
            if (keyword == null) keyword = "";
            var results = from nv in UserDao.GetAllUser()
                          where nv.UserName.Contains(keyword)
                          orderby nv.ID descending
                          select nv;
            ViewBag.search = keyword;
            var model = results.ToPagedList(pageNumber, pageSizeNumber);
            return PartialView("UserTable", model);
        }

        public PartialViewResult Modal()
        {
            ViewBag.listKnv = from k in RoleDao.GetKieuNhanViens()
                              orderby k.ID descending
                              where k.TrangThai == 1
                              select new SelectListItem
                              {
                                  Text = k.TenKieu,
                                  Value = k.ID.ToString()
                              };
            return PartialView("Modal");
        }

        [HttpPost]
        public JsonResult Save(FormCollection fc)
        {
            string UserNameNV = Session["UserNameNV"].ToString();
            NhanVien User = new NhanVien();
            User.ID = Convert.ToInt32(fc["ID"]);
            User.HoTen = fc["HoTen"].ToString();
            User.UserName = fc["UserName"].ToString();
            User.Password = Stuff.MD5Hash(fc["Password"].ToString());
            User.TrangThai = Convert.ToByte(fc["TrangThai"]);
            User.Quyen = Convert.ToByte(fc["Quyen"]);
            User.SDT = fc["SDT"].ToString();
            User.Email = fc["Email"].ToString();
            User.NgaySinh = fc["NgaySinh"].ToString();
            User.GioiTinh = Convert.ToByte(fc["GioiTinh"]);
            User.DiaChi = fc["DiaChi"].ToString();
            User.QueQuan = fc["QueQuan"].ToString();
            User.ChucVu = fc["ChucVu"].ToString();
            User.TieuSu = fc["TieuSu"].ToString();
            User.CongTy = fc["CongTy"].ToString();

            HttpFileCollectionBase file = Request.Files;
            if (file.Count > 0)
            {
                if (file[0].ContentLength > 0)
                {
                    var newName = file[0].FileName.Split('.');
                    string fName = newName[0] + "_" + DateTime.Now.Ticks.ToString() + "." + newName[1];
                    string pathFolder = "/Assets/Images/Avatars/User";
                    //tao folder
                    Directory.CreateDirectory(Server.MapPath(pathFolder));
                    // tao duong dan path
                    string pathFile = Path.Combine(Server.MapPath(pathFolder), fName);
                    file[0].SaveAs(pathFile);
                    User.AnhDaiDien = pathFolder + "/" + fName;
                }
                else
                {
                    User.AnhDaiDien = "";
                    /*User.AnhDaiDien = "";*/
                }
            }
            if (User.ID == 0)
            {
                UserDao.CreateNewUser(User, UserNameNV);
                return Json(new
                {
                    heading = "Thành công",
                    status = "success",
                    message = "Tạo tài khoản mới thành công!"
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                UserDao.UpdateUser(User, User.ID, UserNameNV);
                return Json(new
                {
                    heading = "Thành công",
                    status = "success",
                    message = "Sửa tài khoản thành công!"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Delete(long ID)
        {
            UserDao.DeleteUserByID(ID, Session["UserNameNV"].ToString());
            return Json(new
            {
                heading = "Thành công",
                status = "success",
                message = "Xóa tài khoản thành công!"
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult View(long ID)
        {
            return Json(new
            {
                data = UserDao.GetUserByID(ID).First(),
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Change(long ID, int state)
        {
            UserDao.ChangeStateByID(ID, state, Session["UserNameNV"].ToString());
            return Json(new
            {
                heading = "Thành công",
                status = "success",
                message = "Thay đổi trạng thái thành công!"
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Excel()
        {
            /*string PathExcel = "C:\\Users\\teu-pc\\source\\repos\\daoquyanh2000\\QuanLyHoSo\\Assets\\Excel\\User\\excelQuanLyHoSo.xlsx";*/
            string PathExcel = "C:\\Users\\teu-laptop\\source\\repos\\QuanLyHoSo\\Assets\\Excel\\User\\User.xlsx";
            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    string fileName = "User.xlsx";
                    string pathFolder = "/Assets/Excel/User";
                    //tao folder
                    Directory.CreateDirectory(Server.MapPath(pathFolder));
                    // tao duong dan path
                    string pathFile = Path.Combine(Server.MapPath(pathFolder), fileName);
                    file.SaveAs(pathFile);
                }
            }
            DataTable dt = Stuff.ExcelToDataTable(PathExcel, Session["UserNameNV"].ToString());
            int tk = Stuff.DataTableToDb(dt, "NhanVien");
            return Json(new
            {
                heading = "Thành công",
                status = "success",
                message = $"Tạo {tk} tài khoản thành công!"
            }, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult ExcelModal()
        {

            string PathExcel = "C:\\Users\\teu-pc\\source\\repos\\daoquyanh2000\\QuanLyHoSo\\Assets\\Excel\\User\\User.xlsx";


            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    string fileName = "User.xlsx";
                    string pathFolder = "/Assets/Excel/User";
                    //tao folder
                    Directory.CreateDirectory(Server.MapPath(pathFolder));
                    // tao duong dan path
                    string pathFile = Path.Combine(Server.MapPath(pathFolder), fileName);
                    file.SaveAs(pathFile);
                }
            }
            using (ExcelPackage package = new ExcelPackage(new FileInfo(PathExcel)))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var sheet = package.Workbook.Worksheets["data"];


                ViewBag.listAccount = Stuff.GetListExcel<ExcelNhanVien>(sheet);
                ViewBag.listKnd = RoleDao.GetKieuNhanViens();
                return PartialView();
            };
            
        }
    }
}