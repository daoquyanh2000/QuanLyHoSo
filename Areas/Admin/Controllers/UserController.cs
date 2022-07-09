using AutoMapper;
using Dapper.Contrib.Extensions;
using OfficeOpenXml;
using OfficeOpenXml.LoadFunctions.Params;
using OfficeOpenXml.Table;
using PagedList;
using QuanLyHoSo.App_Start;
using QuanLyHoSo.Dao;
using QuanLyHoSo.Dao.DaoAdmin;
using QuanLyHoSo.Models;
using QuanLyHoSo.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.SqlClient;
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
                          where (nv.HoTen.Contains(keyword) ||
                          nv.UserName.Contains(keyword) ||
                          nv.TenQuyen.Contains(keyword))
                          && nv.TrangThai != 10
                          orderby nv.ID descending
                          select nv;
            ViewBag.search = keyword;
            var model = results.ToPagedList(pageNumber, pageSizeNumber);
            return PartialView("UserTable", model);
        }



        public JsonResult GetKieuNhanVien()
        {
            return Json(new
            {
                data = from n in Stuff.GetAll<KieuNhanVien>()
                       orderby n.ID descending
                       where n.TrangThai==1
                       select n,
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Save(NhanVien User)
        {
            string UserNameNV = Session["UserNameNV"].ToString();
            User.Password = Stuff.MD5Hash(User.Password);
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


        public PartialViewResult ExcelModal(FormCollection fc)
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
            var model = Stuff.GetListExcel<ViewExcelNhanVien>(pathFile);
            var account =
            from nv in model
                where nv.HoTen != null &&
                      nv.UserName != null &&
                      nv.Password != null &&
                      nv.MaKieu != null &&
                      nv.SDT != null &&
                      nv.Email != null
                join k in Stuff.GetAll<KieuNhanVien>()
                on nv.MaKieu equals k.MaKieu
                select new DataNhanVien
                {
                    HoTen= nv.HoTen,
                    UserName = nv.UserName,
                    Password = nv.Password,
                    TrangThai = nv.TrangThai,
                    SDT = nv.SDT,
                    Email = nv.Email,
                    NgaySinh = nv.NgaySinh,
                    AnhDaiDien = nv.AnhDaiDien,
                    GioiTinh = nv.GioiTinh,
                    DiaChi = nv.DiaChi,
                    QueQuan = nv.QueQuan,
                    CongTy = nv.CongTy,
                    ChucVu = nv.ChucVu,
                    TieuSu = nv.TieuSu,
                    TenQuyen = k.TenKieu
                };

            return PartialView(account);
        }

        [HttpGet]
        public ActionResult DownloadExcel(int state)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            using (var package = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                var wsData = package.Workbook.Worksheets.Add("Data");
                var wsThongTinBang = package.Workbook.Worksheets.Add("ThongTinBang");

                //To set values in the spreadsheet use the Cells indexer.
                var knd = from k in Stuff.GetAll<KieuNhanVien>()
                          where k.TrangThai == 1
                          select new
                          {
                              TenKieu = k.TenKieu,
                              MaKieu = k.MaKieu,
                          };
                knd = knd.ToList();
                var GioiTinh = new List<Sex>() {
                    new Sex { GioiTinh = "Nam", MaGioiTinh = 1  },
                    new Sex { GioiTinh = "Nữ", MaGioiTinh = 0  },
                };
                var TrangThai = new List<State>() {
                    new State { TrangThai = "Đóng", MaTrangThai = 0  },
                    new State { TrangThai = "Mở", MaTrangThai = 1  },
                };
                var AnhDaiDien = (List<Image>)Session["AnhDaiDien"];
                var nd = new List<ViewExcelNhanVien>();
                wsData.Cells["A1"].LoadFromCollection(nd, true, TableStyles.Medium1);
                wsThongTinBang.Cells["A1"].LoadFromCollection(knd, true, TableStyles.Medium1);
                wsThongTinBang.Cells["D1"].LoadFromCollection(GioiTinh, true, TableStyles.Medium1);
                wsThongTinBang.Cells["G1"].LoadFromCollection(TrangThai, true, TableStyles.Medium1);

                //khi co anh vao
                if (state == 1)
                {
                    wsThongTinBang.Cells["J1"].LoadFromCollection(AnhDaiDien, true, TableStyles.Medium1);
                    var listAnhDaiDien = wsData.DataValidations.AddListValidation("O2");
                    listAnhDaiDien.Formula.ExcelFormula = $"ThongTinBang!$K$2:$K${AnhDaiDien.Count() + 1}";

                }

                var listKnd = wsData.DataValidations.AddListValidation("D2");
                var listTrangThai = wsData.DataValidations.AddListValidation("E2");
                var listGioiTinh = wsData.DataValidations.AddListValidation("I2");

                listKnd.Formula.ExcelFormula = $"ThongTinBang!$B$2:$B${knd.Count()+1}";
                listTrangThai.Formula.ExcelFormula = "ThongTinBang!$E$2:$E$3";
                listGioiTinh.Formula.ExcelFormula = "ThongTinBang!$H$2:$H$3";


                wsData.Cells["H:H"].Style.Numberformat.Format = "m/d/yyyy";
                wsData.Cells[1, 1, wsData.Dimension.End.Row, wsData.Dimension.End.Column].AutoFitColumns();
                wsThongTinBang.Cells[1, 1, wsThongTinBang.Dimension.End.Row, wsThongTinBang.Dimension.End.Column].AutoFitColumns();
                //Save the new workbook. We haven't specified the filename so use the Save as method.
                var excelData = package.GetAsByteArray();
                var fileName = "UserTemplate.xlsx";
                return File(excelData, ContentType, fileName);
            }
        }
        public JsonResult GetAnhDaiDien(FormCollection fc)
        {
            HttpFileCollectionBase file = Request.Files;
            var AnhDaiDien = new List<Image>();
            for(var i = 0; i < file.Count; i++)
            {
                var newName = file[i].FileName.Split('.');
                string fName = newName[0] + "_" + DateTime.Now.Ticks.ToString() + "." + newName[1];
                string pathFolder = "/Assets/Images/Avatars/User";
                //tao folder
                Directory.CreateDirectory(Server.MapPath(pathFolder));
                // tao duong dan path
                string pathFile = Path.Combine(Server.MapPath(pathFolder), fName);
                var img = new Image { TenAnhDaiDien = file[i].FileName, DuongDan = pathFolder + "/" + fName };
                AnhDaiDien.Add(img);
                file[i].SaveAs(pathFile);
            };
            Session["AnhDaiDien"] = AnhDaiDien;
            return Json(new
            {
                data = AnhDaiDien,
                heading = "Thành công",
                status = "success",
                message = $"tải lên {AnhDaiDien.Count()} ảnh thành công!"
            }, JsonRequestBehavior.AllowGet) ;
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