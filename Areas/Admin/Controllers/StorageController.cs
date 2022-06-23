using Dapper.Contrib.Extensions;
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
    public class StorageController : System.Web.Mvc.Controller
    {
        // GET: Admin/Storage
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult Modal(long ID)
        {
            TempData["listKho"] = from k in Stuff.GetAll<Kho>()
                                  orderby k.ID descending
                                  where k.TrangThai == 1 && k.ID != ID
                                  select new SelectListItem
                                  {
                                      Text = k.TenKho,
                                      Value = k.ID.ToString(),
                                  };
            return PartialView("DropList");
        }

        public PartialViewResult Search(string keyword, int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSizeNumber = 3;
            if (keyword == null) keyword = "";
            var results = from k in StorageDao.GetAllKho()
                          orderby k.ID descending
                          where k.TrangThai != 10 && k.TenKho.Contains(keyword)
                          || k.MaKho.Contains(keyword)
                          || (k.TenKhoCha ?? "trống").Contains(keyword)
                          select k;
            ViewBag.search = keyword;
            var model = results.ToPagedList(pageNumber, pageSizeNumber);
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
            }
            else
            {
                //update kho
                StorageDao.UpdateStorage(kho, kho.ID, UserNameNV);
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
                StackDao.DeleteUserByID(id, Session["UserNameNV"].ToString());
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