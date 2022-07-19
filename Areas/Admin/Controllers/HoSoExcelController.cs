using QuanLyHoSo.Dao;
using QuanLyHoSo.Dao.DaoAdmin;
using QuanLyHoSo.Models;
using QuanLyHoSo.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyHoSo.Areas.Admin.Controllers
{
    public class HoSoExcelController : Controller
    {
        // GET: Admin/HoSoExcel
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UnZipFile()
        {
            var files = Request.Files;
            string pathZip = string.Empty;
            string pathFolder = "/Assets/Admin/zip";

            //save file zip 
            if (files != null)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    //tao folder
                    Directory.CreateDirectory(Server.MapPath(pathFolder));
                    // tao duong dan path
                    pathZip = Path.Combine(Server.MapPath(pathFolder), files[i].FileName);
                    if (System.IO.File.Exists(pathZip))
                    {
                        System.IO.File.Delete(pathZip);
                    }
                    files[i].SaveAs(pathZip);
                }
            }
            TempData["PathFile"] = pathFolder + "/" + files[0].FileName.Split('.')[0];
            //lấy ra file hồ sơ chính 
            using (ZipArchive za = ZipFile.OpenRead(pathZip))
            {
                foreach (ZipArchiveEntry item in za.Entries)
                {
                    //save tất cả các file thành các đường dẫn rồi làm gì thì làm 
                    pathZip = Path.Combine(Server.MapPath(pathFolder), item.FullName);
                    //tao folder
                    if (System.IO.File.Exists(pathZip))
                    {
                        System.IO.File.Delete(pathZip);
                    }
                    //tạo folder
                    if (item.Name== "")
                    {
                        Directory.CreateDirectory(pathZip);
                    }
                    //tạo file
                    else
                    {
                        item.ExtractToFile(pathZip);
                    }

                }
            }
            //đọc file excel hs và lấy thông tin tất cả hs -> đẩy ra view
            var result = Stuff.GetListExcel<ViewExcelHoSo>(pathZip);

            var listeExcelKho = Stuff.GetListExcel<ViewExcelHoSo>(pathZip);
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
                            TenThuMuc =hs.TenThuMuc,
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

            return PartialView("HoSoExcelTable",model);
        }
        public ActionResult GetThanhPhanHoSoExcel(string folderName)
        {
            var pathExcel = TempData["PathFile"] +"/"+ folderName;
            pathExcel = Path.Combine(Server.MapPath(pathExcel), "ThanhPhanTemplate.xlsx");
            //đọc file excel từ pathExcel
            var listExcelTPHS = Stuff.GetListExcel<ViewThanhPhanHoSoExcel>(pathExcel);
            var model = from tp in listExcelTPHS
                        where tp.TenThuMuc != null &&
                                tp.TieuDe != null &&
                                tp.MaThanhPhan != null &&
                                tp.IDLoaiThanhPhan != null &&
                                tp.TrangThai != null
                        join ltp in Stuff.GetAll<LoaiThanhPhan>()
                        on tp.IDLoaiThanhPhan equals ltp.ID
                        select new ViewThanhPhanHoSo
                        {
                            TenThuMuc = tp.TenThuMuc,
                            TieuDe = tp.TieuDe,
                            MaThanhPhan = tp.MaThanhPhan,
                            TenLoaiThanhPhan = ltp.TenLoaiThanhPhan,
                            KiHieu = tp.KiHieu,
                            ChuThich = tp.ChuThich,
                        };
            model = model.ToList();
            return PartialView("ThanhPhanHoSoExcelTable", model);
        }
    }
}