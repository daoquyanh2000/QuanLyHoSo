using Dapper.Contrib.Extensions;
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
                          where k.TrangThai != 10 && (k.TenDanhMuc.Contains(keyword)
                          || k.MaDanhMuc.Contains(keyword)
                          || (k.TenDanhMucCha ?? "trống").Contains(keyword))
                          select k;
            ViewBag.search = keyword;
            var model = results.ToPagedList(pageNumber, pageSizeNumber);
            return PartialView("CategoryTable", model);
        } 
        public PartialViewResult Modal(long ID)
        {
            TempData["listDanhMuc"] = from k in Stuff.GetAll<DanhMuc>()
                                  orderby k.ID descending
                                  where k.TrangThai == 1 && k.ID != ID
                                  select new SelectListItem
                                  {
                                      Text = k.TenDanhMuc,
                                      Value = k.ID.ToString(),
                                  };
            return PartialView("DropList");
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
            CategoryDao.DeleteUserByID(ID, Session["UserNameNV"].ToString());
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
            }
            else
            {
                //update kho
                CategoryDao.UpdateStorage(dm, dm.ID, UserNameNV);
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
                CategoryDao.DeleteUserByID(id, Session["UserNameNV"].ToString());
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
                var account =
                from ke in Stuff.GetListExcel<ViewExcelDanhMuc>(pathFile)
                where ke.TenDanhMuc != null &&
                      ke.MaDanhMuc != null &&
                      ke.MaDanhMucCha != null 
                join k in Stuff.GetAll<DanhMuc>()
                on ke.MaDanhMucCha equals k.MaDanhMuc
                select new ViewDanhMuc
                {
                    TenDanhMuc = ke.TenDanhMuc,
                    MaDanhMuc = ke.MaDanhMuc,
                    TenDanhMucCha = k.TenDanhMuc,
                    TrangThai = ke.TrangThai,
                    MoTa = ke.MoTa

                };
                
                account = account.ToList();
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
            join k in Stuff.GetAll<DanhMuc>()
            on ke.MaDanhMucCha equals k.MaDanhMuc
            select new DanhMuc
            {
                TenDanhMuc = ke.TenDanhMuc,
                MaDanhMuc = ke.MaDanhMuc,
                IDDanhMucCha = k.ID,
                TrangThai = ke.TrangThai,
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
    }
}