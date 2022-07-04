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
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyHoSo.Areas.Admin.Controllers
{
    [Authorize(Roles = "HSLT")]
    public class HoSoLTController : Controller
    {
        // GET: Admin/HoSoLT
        public ActionResult Index()
        {
            TempData["listLoaiHoSo"] = from k in Stuff.GetAll<LoaiHoSo>()
                                       orderby k.ID descending
                                       where k.TrangThai == 1
                                       select new SelectListItem
                                       {
                                           Text = k.TenLoaiHoSo,
                                           Value = k.MaLoaiHoSo,
                                       };
            TempData["listDanhMuc"] = from k in Stuff.GetAll<DanhMuc>()
                                      orderby k.ID descending
                                      where k.TrangThai == 1
                                      select new SelectListItem
                                      {
                                          Text = k.TenDanhMuc,
                                          Value = k.MaDanhMuc,
                                      };
            TempData["listKho"] = from k in Stuff.GetAll<Kho>()
                                  orderby k.ID descending
                                  where k.TrangThai == 1
                                  select new SelectListItem
                                  {
                                      Text = k.TenKho,
                                      Value = k.MaKho,
                                  };
            TempData["listNgan"] = from k in Stuff.GetAll<Ngan>()
                                   orderby k.ID descending
                                   where k.TrangThai == 1
                                   select new SelectListItem
                                   {
                                       Text = k.TenNgan,
                                       Value = k.MaNgan,
                                   };
            return View();
        }

        public PartialViewResult Search(string keyword, int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSizeNumber = 3;
            if (keyword == null) keyword = "";

            var results = from hs in RecordDao.GetAllRecord()
                          where (hs.TrangThai == 0 || hs.TrangThai == 100)
                          && (hs.TieuDe.Contains(keyword)
                          || hs.MaHoSo.Contains(keyword)
                          || hs.TenDanhMuc.Contains(keyword)
                          || hs.TenKho.Contains(keyword)
                          || hs.TenNgan.Contains(keyword))
                          orderby hs.ID descending
                          select hs;
            ViewBag.search = keyword;
            var model = results.ToPagedList(pageNumber, pageSizeNumber);
            return PartialView("HoSoLTTable", model);
        }

        public JsonResult View(long ID)
        {
            HoSo hs = Stuff.GetByID<HoSo>(ID);
            return Json(new
            {
                data = hs,
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(long ID)
        {
            RecordDao.DeleteUserByID(ID, Session["UserNameNV"].ToString());
            return Json(new
            {
                error = false,
                heading = "Thành công",
                status = "success",
                message = "Xóa bản ghi thành công!"
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAll(List<int> checkboxs, int state)
        {
            /*state =0 -> lưu trữ
            state = 1 -> hình thành
            state = 2 -> gửi duyệt
            state =10 -> xóa
            state =100  -> đóng băng
            */
            if (state == 10)
            {
                foreach (var id in checkboxs)
                {
                    RecordDao.DeleteUserByID(id, Session["UserNameNV"].ToString());
                }
            }
            else
            {
                foreach (var id in checkboxs)
                {
                    RecordDao.ChangeStateByID(id, state, Session["UserNameNV"].ToString());
                }
            }

            return Json(new
            {
                heading = "Thành công",
                status = "success",
                message = $"Thay đổi trạng thái của {checkboxs.Count} bản ghi thành công!"
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Save(HoSo hs)
        {
            string UserNameNV = Session["UserNameNV"].ToString();
            HttpFileCollectionBase file = Request.Files;
            if (file.Count > 0)
            {
                if (file[0].ContentLength > 0)
                {
                    var newName = file[0].FileName.Split('.');
                    string fName = newName[0] + "_" + DateTime.Now.Ticks.ToString() + "." + newName[1];
                    string pathFolder = "/Assets/Images/Record";
                    //tao folder
                    Directory.CreateDirectory(Server.MapPath(pathFolder));
                    // tao duong dan path
                    string pathFile = Path.Combine(Server.MapPath(pathFolder), fName);
                    file[0].SaveAs(pathFile);
                    hs.AnhBia = pathFolder + "/" + fName;
                }
            }
            hs.ThoiHanBaoQuan = hs.ThoiHanBaoQuan ?? "0";
            if (hs.ID == 0)
            {
                //create new kho
                RecordDao.Create(hs, UserNameNV);
            }
            else
            {
                //update kho
                RecordDao.Update(hs, hs.ID, UserNameNV);
            }
            return Json(new
            {
                heading = "Thành công",
                status = "success",
                message = "Lưu thành công!"
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNgan(string MaKho)
        {
            return Json(new
            {
                data = from n in Stuff.GetAll<Ngan>()
                       join k in Stuff.GetAll<Kho>()
                       on n.IDKho equals k.ID
                       where n.TrangThai == 1 &&
                       k.TrangThai == 1 &&
                       k.MaKho == MaKho
                       select n,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExcelModal()
        {
            try
            {
                string pathFile = string.Empty;
                string fileName = "Record.xlsx";
                string pathFolder = "/Assets/Excel/Record";
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
                var listeExcelKho = Stuff.GetListExcel<ViewExcelHoSo>(pathFile);
                var listDm = Stuff.GetAll<DanhMuc>().Where(x => x.TrangThai == 1);
                var lisNgan = Stuff.GetAll<Ngan>().Where(x => x.TrangThai == 1);
                var listKho = Stuff.GetAll<Kho>().Where(x => x.TrangThai == 1);
                var listType = Stuff.GetAll<LoaiHoSo>().Where(x => x.TrangThai == 1);

                var model = from hs in listeExcelKho
                            where hs.TieuDe != null &&
                                    hs.MaHoSo != null &&
                                    hs.KyHieu != null &&
                                    hs.MaDanhMuc != null &&
                                    hs.MaKho != null &&
                                    hs.MaNgan != null &&
                                    hs.MaLoaiHoSo != null &&
                                    hs.ThoiGianLuuTru != null &&
                                    hs.ThoiHanBaoQuan != null
                            join k in listKho
                            on hs.MaKho equals k.MaKho
                            join n in lisNgan
                            on hs.MaNgan equals n.MaNgan
                            join dm in listDm
                            on hs.MaDanhMuc equals dm.MaDanhMuc
                            join t in listType
                            on hs.MaLoaiHoSo equals t.MaLoaiHoSo
                            select new ViewHoSo
                            {
                                TieuDe = hs.TieuDe,
                                MaHoSo = hs.MaHoSo,
                                KyHieu = hs.KyHieu,
                                MaDanhMuc = hs.MaDanhMuc,
                                MaNgan = hs.MaNgan,
                                MaKho = hs.MaKho,
                                MaLoaiHoSo = hs.MaLoaiHoSo,
                                MoTa = hs.MoTa,
                                ThoiGianLuuTru = hs.ThoiGianLuuTru,
                                ThoiHanBaoQuan = hs.ThoiHanBaoQuan,
                                TenDanhMuc = dm.TenDanhMuc,
                                TenKho = k.TenKho,
                                TenNgan = n.TenNgan,
                                TenLoaiHoSo = t.TenLoaiHoSo
                            };
                model = model.ToList();
                TempData["listExcelHoSo"] = model;
                return PartialView(model);
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

        public JsonResult Excel()
        {
            var model = (List<ViewHoSo>)TempData["listExcelHoSo"];
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = config.CreateMapper();
            // Chuyển đổi danh sách ViewExcelNhanVIen qua danh sách NhanVien.
            var listHoSo = mapper.Map<List<HoSo>>(model);
            foreach (var hs in listHoSo)
            {
                hs.NgayTao = DateTime.UtcNow.ToString();
                hs.NguoiTao = Session["UserNameNV"].ToString();
            }
            long tk;
            using (var db = new SqlConnection(ConnectString.Setup()))
            {
                tk = db.Insert(listHoSo);
            }
            return Json(new
            {
                heading = "Thành công",
                status = "success",
                message = $"Lưu {tk} bản ghi thành công!"
            }, JsonRequestBehavior.AllowGet);
        }

        //đóng băng hồ sơ
        public JsonResult Change(int ID, int state)
        {
            /*              0 : hồ sơ lưu trữ
                            1 : hồ sơ hình thành
                            10 : hồ sơ xóa
                            100 hồ sơ đóng băng*/
            RecordDao.ChangeStateByID(ID, state, Session["UserNameNV"].ToString());
            return Json(new
            {
                heading = "Thành công",
                status = "success",
                message = "Lưu thành công!"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}