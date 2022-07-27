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
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyHoSo.Areas.Admin.Controllers
{
    [Authorize(Roles = "DanhMuc")]
    public class CategoryController : Controller
    {

        // GET: Admin/Category
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult Search(string keyword, int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSizeNumber = 3;
            if (keyword == null) keyword = "";
            var results = from k in CategoryDao.GetAllCategory()
                          orderby k.ID descending
                          where k.TrangThai != 10 && k.TrangThai!=100&&(k.TenDanhMuc.Contains(keyword)
                          || k.MaDanhMuc.Contains(keyword)
                          || (k.TenDanhMucCha ?? "trống").Contains(keyword))
                          select k; 
            //add danh muc con
            foreach(var k in results)
            {
                k.DanhMucCon = results.Where(x => x.IDDanhMucCha == k.ID).OrderByDescending(x => x.ID).ToList();
            }
            ViewBag.search = keyword;
            var model = results.Where(x=>x.IDDanhMucCha==0).ToPagedList(pageNumber, pageSizeNumber);
            return PartialView("CategoryTable", model);
        } 
        public PartialViewResult GetDanhMuc(long ID)
        {
            var listDm = new List<ViewDanhMuc>();
            if (ID == 0)
            {
                listDm = CategoryDao.GetAllCategory();
            }
            else
            {
                listDm = Stuff.GetList<ViewDanhMuc>($"select * from DanhMuc Where DuongDan not like '%{ID}%'");
            }
            foreach (var k in listDm)
            {
                /*k.DanhMucCon = listDm.Where(x => x.IDDanhMucCha == k.ID).OrderByDescending(x => x.ID).ToList();*/
                k.DanhMucCon = (from dm in listDm
                                orderby dm.ID descending
                                where dm.IDDanhMucCha == k.ID && dm.TrangThai !=10 
                                select dm).ToList();
            }
            var model = from k in listDm
                        where k.IDDanhMucCha == 0 && k.TrangThai != 100 && k.TrangThai != 10
                        orderby k.ID descending
                        select k;
            return PartialView("DropListDanhMuc", model);
        }
        [HttpGet]
        public JsonResult Change(long ID, int state)
        {
            CategoryDao.ChangeStateByID(ID, state, Session["UserNameNV"].ToString());
            return Json(new
            {
                heading = "Thành công",
                status = "success",
                message = "Thay đổi trạng thái thành công!"
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult View(long ID)
        {
            var dm = Stuff.GetByID<DanhMuc>(ID);
            return Json(new
            {
                data = dm,
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult Delete(long ID)
        {
            var listCon = Stuff.GetList<DanhMuc>($"select * from DanhMuc Where DuongDan like '%{ID}%'");
            foreach(var dm in listCon)
            {
                CategoryDao.DeleteUserByID(dm.ID, Session["UserNameNV"].ToString());

            }
            return Json(new
            {
                error = false,
                heading = "Thành công",
                status = "success",
                message = "Xóa bản ghi thành công!"
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Save(DanhMuc dm)
        {
            string UserNameNV = Session["UserNameNV"].ToString();

            if (dm.ID == 0)
            {
                    //create new kho
                    CategoryDao.CreateStorage(dm, UserNameNV);
                var nextDm = Stuff.GetAll<DanhMuc>().LastOrDefault();
                //update duong dan
                if (nextDm.IDDanhMucCha == 0)
                {
                    nextDm.DuongDan = nextDm.ID.ToString();
                    CategoryDao.UpdateStorageNoChange(nextDm, nextDm.ID);

                }
                else
                {
                    var dmCha = Stuff.GetByID<DanhMuc>(nextDm.IDDanhMucCha);
                    string newPath = dmCha.DuongDan + "-" + nextDm.ID.ToString();
                    nextDm.DuongDan = newPath;
                    CategoryDao.UpdateStorageNoChange(nextDm, nextDm.ID);

                }

            }
            else
            {
                //neu no la danh muc cha
                if (dm.IDDanhMucCha == 0)
                {
                    dm.DuongDan = dm.ID.ToString();
                    CategoryDao.UpdateStorage(dm, dm.ID, UserNameNV);

                }
                else
                {
                    var dmCha = Stuff.GetByID<DanhMuc>(dm.IDDanhMucCha);
                    string newPath = dmCha.DuongDan + "-" + dm.ID.ToString();
                    dm.DuongDan = newPath;
                    CategoryDao.UpdateStorage(dm, dm.ID, UserNameNV);

                }

                //xu ly truong hop path bi ngat quang
                var listNgatQuang = Stuff.GetList<DanhMuc>($"select * from DanhMuc Where DuongDan  like '%{dm.ID}-%'");
                foreach(var nq in listNgatQuang)
                {
                    var dmCha = Stuff.GetByID<DanhMuc>(nq.IDDanhMucCha);
                    string newPath = dmCha.DuongDan + "-" + nq.ID.ToString();
                    nq.DuongDan = newPath;
                    CategoryDao.UpdateStorage(nq, nq.ID, UserNameNV);
                }
            }
            return Json(new
            {
                heading = "Thành công",
                status = "success",
                message = "Lưu thành công!"
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteAll(List<int> checkboxs)
        {
            foreach (var id in checkboxs)
            {
                var listCon = Stuff.GetList<DanhMuc>($"select * from DanhMuc Where DuongDan like '%{id}%'");
                foreach (var dm in listCon)
                {
                    CategoryDao.DeleteUserByID(dm.ID, Session["UserNameNV"].ToString());

                }
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
                string fileName = "Category.xlsx";
                string pathFolder = "/Assets/Excel/Category";
                //tao folder
                Directory.CreateDirectory(Server.MapPath(pathFolder));
                // tao duong dan path
                pathFile = Path.Combine(Server.MapPath(pathFolder), fileName);
                if (Request.Files.Count > 0)
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        file.SaveAs(pathFile);
                    }
                }
                Session["pathFile"] = pathFile;
                var account =
                from ke in Stuff.GetListExcel<ViewExcelDanhMuc>(pathFile)
                where ke.TenDanhMuc != null &&
                      ke.MaDanhMuc != null &&
                      ke.MaDanhMucCha != null
                select new ViewDanhMuc
                {
                    TenDanhMuc = ke.TenDanhMuc,
                    MaDanhMuc = ke.MaDanhMuc,
                    MaDanhMucCha = ke.MaDanhMucCha,
                    TrangThai = ke.TrangThai,
                    MoTa = ke.MoTa
                };
                return PartialView(account);
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
        public JsonResult Excel(FormCollection fc)
        {
            var checkbox = (fc["checkbox"]).Split(',');
            var account =
            from ke in Stuff.GetListExcel<ViewExcelDanhMuc>(Session["pathFile"].ToString())
            where ke.TenDanhMuc != null &&
                  ke.MaDanhMuc != null &&
                  ke.MaDanhMucCha != null
                  let IDcha =( ke.MaDanhMucCha =="NONE")?1:2
            select new DanhMuc
            {
                TenDanhMuc = ke.TenDanhMuc,
                MaDanhMuc = ke.MaDanhMuc,
                IDDanhMucCha = ,
                TrangThai = ke.TrangThai,
                MoTa = ke.MoTa,
                NgayTao = DateTime.Now.ToString(),
                NguoiTao = Session["UserNameNV"].ToString(),
            };
            long tk;
            int i = 0;
            foreach (var k in account)
            {
                k.TrangThai = Convert.ToByte(checkbox[i]);
                i++;
            }
            using (var db = new SqlConnection(ConnectString.Setup()))
            {
                tk = db.Insert(account);
            }
            //sau khi insert tien hanh them duong dan 
            var listFix = Stuff.GetList<DanhMuc>($"select top {tk} * from DanhMuc  order by id desc");
            
            foreach(var dm in listFix)
            {
                if (dm.IDDanhMucCha == 10139)
                {

                    Stuff.ExecuteSql("Update DanhMuc Set DuongDan =@newPath,IDDanhMucCha =0 where ID =@ID", new { newPath = dm.ID.ToString(), ID = dm.ID });
                }
                else
                {
                    //lay duong dan cha
                    var newPath = Stuff.GetByID<DanhMuc>(dm.IDDanhMucCha).DuongDan + "-" + dm.ID.ToString();

                    //luu duong dan
                    Stuff.ExecuteSql("Update DanhMuc Set DuongDan =@newPath where ID =@ID", new { newPath = newPath, ID = dm.ID });
                }
            }

            return Json(new
            {
                heading = "Thành công",
                status = "success",
                message = $"Tạo {tk} bản ghi thành công!"
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
                List<SheetDanhMucCha> sheetDmCha = new List<SheetDanhMucCha>();
                sheetDmCha.Add(new SheetDanhMucCha { TenDanhMucCha = "Trống", MaDanhMucCha = "NONE" });
                var dm = from k in Stuff.GetAll<DanhMuc>()
                         where k.TrangThai !=10 
                          select new SheetDanhMucCha
                          {
                              TenDanhMucCha = k.TenDanhMuc,
                              MaDanhMucCha = k.MaDanhMuc,
                          };
                foreach (var item in dm)
                {
                    sheetDmCha.Add(item);
                }
                var TrangThai = new List<State>() {
                    new State { TrangThai = "Đóng", MaTrangThai = 0  },
                    new State { TrangThai = "Mở", MaTrangThai = 1  },
                };
                var nd = new List<ViewExcelDanhMuc>();
                wsData.Cells["A1"].LoadFromCollection(nd,true,TableStyles.Medium2);
                wsThongTinBang.Cells["A1"].LoadFromCollection(sheetDmCha, true, TableStyles.Medium2);
                wsThongTinBang.Cells["D1"].LoadFromCollection(TrangThai, true, TableStyles.Medium2);
                var listDm = wsData.DataValidations.AddListValidation("C2");
                var listTrangThai = wsData.DataValidations.AddListValidation("D2");
                listDm.Formula.ExcelFormula = $"ThongTinBang!$B$2:$B${sheetDmCha.Count() + 1}";
                listTrangThai.Formula.ExcelFormula = $"ThongTinBang!$E$2:$E${TrangThai.Count() + 1}";
                wsData.Cells[1, 1, wsData.Dimension.End.Row, wsData.Dimension.End.Column].AutoFitColumns();
                wsThongTinBang.Cells[1, 1, wsThongTinBang.Dimension.End.Row, wsThongTinBang.Dimension.End.Column].AutoFitColumns();
                //Save the new workbook. We haven't specified the filename so use the Save as method.
                var excelData = package.GetAsByteArray();
                var fileName = "CategoryTemplate.xlsx";
                return File(excelData, ContentType, fileName);
            }
        }
    }
}