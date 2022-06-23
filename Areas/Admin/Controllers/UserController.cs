using AutoMapper;
using Dapper.Contrib.Extensions;
using PagedList;
using QuanLyHoSo.App_Start;
using QuanLyHoSo.Dao;
using QuanLyHoSo.Dao.DaoAdmin;
using QuanLyHoSo.Models;
using QuanLyHoSo.ViewModel;
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
    public class UserController : System.Web.Mvc.Controller
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
                          where nv.UserName.Contains(keyword) && nv.TrangThai != 10
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
                                  Value = k.MaKieu,
                              };
            return PartialView("Modal");
        }

        [HttpPost]
        public JsonResult Save(NhanVien User)
        {
            string UserNameNV = Session["UserNameNV"].ToString();
            User.Password = Stuff.MD5Hash(User.Password);
            /*            NhanVien User = new NhanVien();
                        User.ID = Convert.ToInt32(fc["ID"]);
                        User.HoTen = fc["HoTen"].ToString();
                        User.UserName = fc["UserName"].ToString();
                        User.Password = Stuff.MD5Hash(fc["Password"].ToString());
                        User.TrangThai = Convert.ToByte(fc["TrangThai"]);
                        User.MaKieu = fc["MaKieu"].ToString();
                        User.SDT = fc["SDT"].ToString();
                        User.Email = fc["Email"].ToString();
                        User.NgaySinh = fc["NgaySinh"].ToString();
                        User.GioiTinh = fc["GioiTinh"] ?? "0";
                        User.DiaChi = fc["DiaChi"].ToString();
                        User.QueQuan = fc["QueQuan"].ToString();
                        User.ChucVu = fc["ChucVu"].ToString();
                        User.TieuSu = fc["TieuSu"].ToString();
                        User.CongTy = fc["CongTy"].ToString();*/
            var a = User.AnhDaiDien;
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
                    message = "Sửa bản ghi thành công!"
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
                message = "Xóa bản ghi thành công!"
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
        public JsonResult Excel(FormCollection fc)
        {
            //path file
            string fileName = "User.xlsx";
            string pathFolder = "/Assets/Excel/User";
            //tao folder
            Directory.CreateDirectory(Server.MapPath(pathFolder));
            // tao duong dan path
            string pathFile = Path.Combine(Server.MapPath(pathFolder), fileName);

            var checkbox = (fc["checkbox"]).Split(',');
            var listNhanVien = new List<NhanVien>();
            var account =
                   from nv in Stuff.GetListExcel<ViewExcelNhanVien>(pathFile)
                   where nv.HoTen != null &&
                         nv.UserName != null &&
                         nv.Password != null &&
                         nv.MaKieu != null &&
                         nv.SDT != null &&
                         nv.Email != null
                   join k in RoleDao.GetKieuNhanViens()
                   on nv.MaKieu equals k.MaKieu
                   select nv;
            long tk;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = config.CreateMapper();
            // Chuyển đổi danh sách ViewExcelNhanVIen qua danh sách NhanVien.
            listNhanVien = mapper.Map<List<NhanVien>>(account);
            int i = 0;
            foreach (var nv in listNhanVien)
            {
                nv.TrangThai = Convert.ToInt32(checkbox[i]);
                nv.NgayTao = DateTime.UtcNow.ToString();
                nv.NguoiTao = Session["UserNameNV"].ToString();
                i++;
            }
            using (IDbConnection db = new SqlConnection(ConnectString.Setup()))
            {
                tk = db.Insert(listNhanVien);
            }
            return Json(new
            {
                heading = "Thành công",
                status = "success",
                message = $"Tạo {tk} bản ghi thành công!"
            }, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult ExcelModal()
        {
            string pathFile = string.Empty;
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
                    pathFile = Path.Combine(Server.MapPath(pathFolder), fileName);
                    file.SaveAs(pathFile);
                }
            }
            ViewBag.listAccount = Stuff.GetListExcel<ViewExcelNhanVien>(pathFile);
            ViewBag.listKnd = RoleDao.GetKieuNhanViens();
            return PartialView();
        }

        public JsonResult DeleteAll(List<int> checkboxs)
        {
            foreach (var id in checkboxs)
            {
                UserDao.DeleteUserByID(id, Session["UserNameNV"].ToString());
            }
            return Json(new
            {
                heading = "Thành công",
                status = "success",
                message = $"Xóa {checkboxs.Count} bản ghi thành công!"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}