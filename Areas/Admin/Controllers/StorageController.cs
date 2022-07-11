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
                    var khoCha = Stuff.GetByID<DanhMuc>(kho.IDKhoCha);
                    string newPath = khoCha.DuongDan + "-" + kho.ID.ToString();
                    kho.DuongDan = newPath;
                    StorageDao.UpdateStorage(kho, kho.ID, UserNameNV);

                }

                //xu ly truong hop path bi ngat quang
                var listNgatQuang = Stuff.GetList<DanhMuc>($"select * from Kho Where DuongDan  like '%{kho.ID}-%'");
                foreach (var nq in listNgatQuang)
                {
                    var khoCha = Stuff.GetByID<DanhMuc>(nq.IDDanhMucCha);
                    string newPath = khoCha.DuongDan + "-" + nq.ID.ToString();
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