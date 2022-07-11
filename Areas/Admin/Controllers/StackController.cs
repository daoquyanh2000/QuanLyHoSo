using Dapper.Contrib.Extensions;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using PagedList;
using QuanLyHoSo.Dao;
using QuanLyHoSo.Dao.DaoAdmin;
using QuanLyHoSo.Models;
using QuanLyHoSo.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyHoSo.Areas.Admin.Controllers
{
    [Authorize(Roles = "Ngan")]
    public class StackController : System.Web.Mvc.Controller
    {
        // GET: Admin/Stack
        public ActionResult Index()
        {
            ViewBag.listKho = from k in Stuff.GetAll<Kho>()
                              where k.TrangThai == 1
                              select new SelectListItem
                              {
                                  Text = k.TenKho,
                                  Value = k.ID.ToString(),
                              };
            return View();
        }

        public PartialViewResult Search(string keyword, int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSizeNumber = 3;
            if (keyword == null) keyword = "";

            var results = from n in StackDao.GetAllStack()
                          orderby n.ID descending
                          where n.TenNgan.Contains(keyword) && n.TrangThai != 10
                          select n;
            ViewBag.search = keyword;
            var model = results.ToPagedList(pageNumber, pageSizeNumber);
            return PartialView("StackTable", model);
        }

        public JsonResult View(long ID)
        {
            Ngan ngan = Stuff.GetByID<Ngan>(ID);
            return Json(new
            {
                data = ngan,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Change(long ID, int state)
        {
            StackDao.ChangeStateByID(ID, state, Session["UserNameNV"].ToString());
            return Json(new
            {
                heading = "Thành công",
                status = "success",
                message = "Thay đổi trạng thái thành công!"
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Save(Ngan Ngan)
        {
            string UserNameNV = Session["UserNameNV"].ToString();
            if (Ngan.ID == 0)
            {
                //create new kho
                StackDao.CreateStorage(Ngan, UserNameNV);
                var nextStk = Stuff.GetAll<Ngan>().LastOrDefault();
                Ngan.DuongDan = Stuff.GetByID<Kho>(Ngan.IDKho).DuongDan+"-"+ nextStk.ID.ToString();
                StackDao.UpdateStackNoChange(Ngan, Ngan.ID);
            }
            else
            {
                //update kho
                var dmCha = Stuff.GetByID<Kho>(Ngan.IDKho);
                string newPath = dmCha.DuongDan + "-" + Ngan.ID.ToString();
                Ngan.DuongDan = newPath;
                StackDao.UpdateStackNoChange(Ngan, Ngan.ID);
            }
            return Json(new
            {
                heading = "Thành công",
                status = "success",
                message = "Lưu thành công!"
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Delete(long ID)
        {
            StackDao.DeleteUserByID(ID, Session["UserNameNV"].ToString());
            return Json(new
            {
                error = false,
                heading = "Thành công",
                status = "success",
                message = "Xóa bản ghi thành công!"
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAll(List<int> checkboxs)
        {
            foreach (var id in checkboxs)
            {
                StackDao.DeleteUserByID(id, Session["UserNameNV"].ToString());
            }
            return Json(new
            {
                heading = "Thành công",
                status = "success",
                message = $"Xóa {checkboxs.Count} bản ghi thành công!"
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExcelModal()
        {
            try
            {
                string pathFile = string.Empty;
                string fileName = "Stack.xlsx";
                string pathFolder = "/Assets/Excel/Stack";
                if (Request.Files.Count > 0)
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        //tao folder
                        Directory.CreateDirectory(Server.MapPath(pathFolder));
                        // tao duong dan path
                        pathFile = Path.Combine(Server.MapPath(pathFolder), fileName);
                        file.SaveAs(pathFile);
                    }
                }
                Session["pathFile"] = pathFile;
                var listNganExcel = Stuff.GetListExcel<ViewExcelNgan>(pathFile);
                var listKho = Stuff.GetAll<Kho>();
                ViewBag.listNganExcel = listNganExcel;
                ViewBag.listKho = listKho;
                return PartialView();
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    error = true,
                    heading = "Thất bại",
                    icon = "error",
                    message = $"{ex}"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Excel(FormCollection fc)
        {
            var checkbox = (fc["checkbox"]).Split(',');
            var account =
                   from ke in Stuff.GetListExcel<ViewExcelNgan>(Session["pathFile"].ToString())
                   where ke.TenNgan != null &&
                         ke.MaNgan != null &&
                         ke.MaKhoChua != null &&
                         ke.KichThuoc != 0
                   join k in Stuff.GetAll<Kho>()
                   on ke.MaKhoChua equals k.MaKho
                   select new Ngan
                   {
                       TenNgan = ke.TenNgan,
                       MaNgan = ke.MaNgan,
                       IDKho = k.ID,
                       DuongDan =k.DuongDan,
                       TrangThai = ke.TrangThai,
                       KichThuoc = ke.KichThuoc,
                       MoTa = ke.MoTa,
                       NgayTao = DateTime.Now.ToString(),
                       NguoiTao = Session["UserNameNV"].ToString(),
                   };
            account = account.ToList();
            long tk;
            int i = 0;
            foreach (var nv in account)
            {
                nv.TrangThai = Convert.ToByte(checkbox[i]);
                i++;
            }

            using (var db = new SqlConnection(ConnectString.Setup()))
            {
                tk = db.Insert(account);
            }
            var listFixPath = Stuff.GetList<Ngan>($"select top {tk} * from Ngan  order by id desc");
            foreach(var n in listFixPath)
            {
                    var newPath = n.DuongDan + "-" + n.ID.ToString();
                    Stuff.ExecuteSql("Update Ngan Set DuongDan =@newPath where ID =@ID", new { newPath = newPath, ID = n.ID });
            }
            return Json(new
            {
                heading = "Thành công",
                status = "success",
                message = $"Lưu {tk} bản ghi thành công!"
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            using (var package = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                var wsData = package.Workbook.Worksheets.Add("Data");
                var wsThongTinBang = package.Workbook.Worksheets.Add("ThongTinBang");

                //To set values in the spreadsheet use the Cells indexer.
                var dm = from k in Stuff.GetAll<Kho>()
                         where k.TrangThai !=10 && k.TrangThai!=100
                         select new
                         {
                             MaKHoChua = k.TenKho,
                             MaKhoCha = k.MaKho,
                         };

                var TrangThai = new List<State>() {
                    new State { TrangThai = "Đóng", MaTrangThai = 0  },
                    new State { TrangThai = "Mở", MaTrangThai = 1  },
                };
                var nd = new List<ViewExcelNgan>();
                wsData.Cells["A1"].LoadFromCollection(nd, true, TableStyles.Medium1);
                wsThongTinBang.Cells["A1"].LoadFromCollection(dm, true, TableStyles.Medium1);
                wsThongTinBang.Cells["D1"].LoadFromCollection(TrangThai, true, TableStyles.Medium1);


                var listDm = wsData.DataValidations.AddListValidation("C2");
                var listTrangThai = wsData.DataValidations.AddListValidation("D2");

                listDm.Formula.ExcelFormula = $"ThongTinBang!$B$2:$B${dm.Count() + 1}";
                listTrangThai.Formula.ExcelFormula = "ThongTinBang!$E$2:$E$3";


                wsData.Cells[1, 1, wsData.Dimension.End.Row, wsData.Dimension.End.Column].AutoFitColumns();
                wsThongTinBang.Cells[1, 1, wsThongTinBang.Dimension.End.Row, wsThongTinBang.Dimension.End.Column].AutoFitColumns();
                //Save the new workbook. We haven't specified the filename so use the Save as method.
                var excelData = package.GetAsByteArray();
                var fileName = "StackTemplate.xlsx";
                return File(excelData, ContentType, fileName);
            }
        }

    }
}