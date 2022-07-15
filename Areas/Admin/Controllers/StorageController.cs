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
    [Authorize(Roles = "Kho")]

    public class StorageController : Controller
    {
        // GET: Admin/Storage
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult GetKho(long ID)
        {
            var listKho = new List<ViewKho>();
            if (ID == 0)
            {
                listKho = StorageDao.GetAllKho();
            }
            else
            {
                listKho = Stuff.GetList<ViewKho>($"select * from Kho Where DuongDan not like '%{ID}%'");
            }
            //them list kho con
            foreach (var k in listKho)
            {
               
                k.KhoCon = (from dm in listKho
                                orderby dm.ID descending
                                where dm.IDKhoCha == k.ID && dm.TrangThai != 10
                                select dm).ToList();
                //them list ngan con
                k.NganCon = (from n in StackDao.GetAllStack()
                             where n.IDKho == k.ID
                             select n).ToList();
            }
            var model = from k in listKho
                        where k.IDKhoCha == 0 && k.TrangThai != 100 && k.TrangThai != 10
                        orderby k.ID descending
                        select k;
            return PartialView("DropListKho", model);
        }

        public PartialViewResult Search(string keyword, int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSizeNumber = 3;
            if (keyword == null) keyword = "";
            var results = from k in StorageDao.GetAllKho()
                          orderby k.ID descending
                          where k.TrangThai != 10 && k.TrangThai != 100 &&  (k.TenKho.Contains(keyword)
                          || k.MaKho.Contains(keyword)
                          || (k.TenKhoCha ?? "trống Trống TRỐNG").Contains(keyword))
                          select k;
            ViewBag.search = keyword;

            var listNgan = (from n in StackDao.GetAllStack()
                            where n.TrangThai != 10
                            orderby n.ID descending
                            select n).ToList();
            foreach (var k in results)
            {
                k.KhoCon = results.Where(x => x.IDKhoCha == k.ID).OrderByDescending(x => x.ID).ToList();
                k.NganCon = listNgan.Where(x => x.IDKho == k.ID).ToList();
            }
            var model = results.Where(x=>x.IDKhoCha==0).ToPagedList(pageNumber, pageSizeNumber);
            return PartialView("StorageTable", model);
        }

        [HttpPost]
        public JsonResult Save(Kho kho)
        {
            string UserNameNV = Session["UserNameNV"].ToString();
            if (kho.ID == 0)
            {
                //create new kho
                StorageDao.CreateStorage(kho, UserNameNV);
                var nextKho = Stuff.GetAll<Kho>().LastOrDefault();
                //update duong dan
                //neu la kho goc
                if (nextKho.IDKhoCha == 0)
                {
                    nextKho.DuongDan = nextKho.ID.ToString();
                    StorageDao.UpdateStorageNoChange(nextKho, nextKho.ID);

                }
                else
                {
                    var khoCha = Stuff.GetByID<Kho>(nextKho.IDKhoCha);
                    string newPath = khoCha.DuongDan + "-" + nextKho.ID.ToString();
                    nextKho.DuongDan = newPath;
                }
                StorageDao.UpdateStorageNoChange(nextKho, nextKho.ID);
            }
            else
            {
                //neu no la kho cha
                if (kho.IDKhoCha == 0)
                {
                    kho.DuongDan = kho.ID.ToString();
                    StorageDao.UpdateStorage(kho, kho.ID, UserNameNV);

                }
                else
                {
                    var khoCha = Stuff.GetByID<Kho>(kho.IDKhoCha);
                    string newPath = khoCha.DuongDan + "-" + kho.ID.ToString();
                    kho.DuongDan = newPath;
                    StorageDao.UpdateStorage(kho, kho.ID, UserNameNV);

                }

                //xu ly đường đẫn của kho theo kho cha
                var listNgatQuang = Stuff.GetList<Kho>($"select * from Kho Where DuongDan  like '%{kho.ID}-%'");
                foreach (var nq in listNgatQuang)
                {
                    var khoCha = Stuff.GetByID<Kho>(nq.IDKhoCha);
                    string newPath = khoCha.DuongDan + "-" + nq.ID.ToString();
                    nq.DuongDan = newPath;
                    StorageDao.UpdateStorage(nq, nq.ID, UserNameNV);
                }
                //xu ly đường dẫn của ngăn theo kho cha
                var listNganCon = Stuff.GetList<Ngan>($"select * from Ngan Where DuongDan  like'%{kho.ID}%'");
                foreach(var item in listNganCon)
                {
                    var khoCha = Stuff.GetByID<Kho>(item.IDKho);
                    item.DuongDan = khoCha.DuongDan + "-" + item.ID;
                    StackDao.UpdateStackNoChange(item, item.ID);
                }
                
            }
            return Json(new
            {
                heading = "Thành công",
                status = "success",
                message = "Lưu thành công!"
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Change(long ID, int state)
        {
            StorageDao.ChangeStateByID(ID, state, Session["UserNameNV"].ToString());
            return Json(new
            {
                heading = "Thành công",
                status = "success",
                message = "Thay đổi trạng thái thành công!"
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult View(long ID)
        {
            Kho kho = Stuff.GetByID<Kho>(ID);
            return Json(new
            {
                data = kho,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Delete(long ID)
        {
            StorageDao.DeleteUserByID(ID, Session["UserNameNV"].ToString());
            return Json(new
            {
                error = false,
                heading = "Thành công",
                status = "success",
                message = "Xóa bản ghi thành công!"
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExcelModal()
        {
            try
            {
                string pathFile = string.Empty;
                string fileName = "Storage.xlsx";
                string pathFolder = "/Assets/Excel/Storage";
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
                var listeExcelKho = Stuff.GetListExcel<ViewExcelKho>(pathFile);
                var listKho = Stuff.GetAll<Kho>();
                var account =
                    from ke in listeExcelKho
                    where ke.TenKho != null &&
                          ke.MaKho != null &&
                          ke.MaKhoCha != null
                    join k in listKho
                    on ke.MaKhoCha equals k.MaKho
                    select new ViewKho
                    {
                        TenKho = ke.TenKho,
                        MaKho = ke.MaKho,
                        TenKhoCha = k.TenKho,
                        TrangThai = ke.TrangThai,
                        KichThuoc = ke.KichThuoc,
                        MoTa = ke.MoTa

                    };
                account = account.ToList();
                return PartialView(account);
            }
            catch(Exception ex)
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
                   from ke in Stuff.GetListExcel<ViewExcelKho>(Session["pathFile"].ToString())
                   where ke.TenKho != null &&
                         ke.MaKho != null &&
                         ke.MaKhoCha != null 
                   join k in Stuff.GetAll<Kho>()
                   on ke.MaKhoCha equals k.MaKho
                   select new Kho
                   {
                       TenKho = ke.TenKho,
                       MaKho = ke.MaKho,
                       IDKhoCha = k.ID,
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
            //sau khi insert tien hanh them duong dan 
            var listFix = Stuff.GetList<Kho>($"select top {tk} * from Kho  order by id desc");

            foreach (var dm in listFix)
            {
                if (dm.IDKhoCha == 20010)
                {

                    Stuff.ExecuteSql("Update Kho Set DuongDan =@newPath,IDKhoCha =0 where ID =@ID", new { newPath = dm.ID.ToString(), ID = dm.ID });
                }
                else
                {
                    //lay duong dan cha
                    var newPath = Stuff.GetByID<Kho>(dm.IDKhoCha).DuongDan + "-" + dm.ID.ToString();

                    //luu duong dan
                    Stuff.ExecuteSql("Update Kho Set DuongDan =@newPath where ID =@ID", new { newPath = newPath, ID = dm.ID });
                }
            }
            return Json(new
            {
                heading = "Thành công",
                status = "success",
                message = $"Tạo {tk} bản ghi thành công!"
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAll(List<int> checkboxs)
        {
            foreach (var id in checkboxs)
            {
                var listKho = Stuff.GetList<Kho>($"select * from Kho Where DuongDan like '%{id}%'");
                foreach(var item in listKho)
                {
                StorageDao.DeleteUserByID(item.ID, Session["UserNameNV"].ToString());

                }
                var listNgan = Stuff.GetList<Ngan>($"select * from Ngan Where DuongDan like '%{id}%'");
                foreach (var item in listNgan)
                {
                    StackDao.DeleteUserByID(item.ID, Session["UserNameNV"].ToString());

                }
            }
            return Json(new
            {
                heading = "Thành công",
                status = "success",
                message = $"Xóa {checkboxs.Count} bản ghi thành công!"
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
                /*                var listDmAll = ();
                                var firstDm = new DanhMuc();
                                firstDm.TenDanhMuc = "Trống";
                                firstDm.MaDanhMuc = "100";
                                firstDm.TrangThai = 1;
                                listDmAll.Insert(0,firstDm);*/
                var dm = from k in Stuff.GetAll<Kho>()
                         where k.TrangThai != 10
                         select new
                         {
                             TenKhoCha = k.TenKho,
                             MaKho = k.MaKho,
                         };

                var TrangThai = new List<State>() {
                    new State { TrangThai = "Đóng", MaTrangThai = 0  },
                    new State { TrangThai = "Mở", MaTrangThai = 1  },
                };
                var nd = new List<ViewExcelKho>();
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
                var fileName = "StorageTemplate.xlsx";
                return File(excelData, ContentType, fileName);
            }
        }
    }
}