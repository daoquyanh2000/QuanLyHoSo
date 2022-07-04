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
            }
            else
            {
                //update kho
                StackDao.UpdateStack(Ngan, Ngan.ID, UserNameNV);
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
                message = $"Lưu {tk} bản ghi thành công!"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}