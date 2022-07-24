using AutoMapper;
using Dapper.Contrib.Extensions;
using QuanLyHoSo.App_Start;
using QuanLyHoSo.Dao;
using QuanLyHoSo.Dao.DaoAdmin;
using QuanLyHoSo.Models;
using QuanLyHoSo.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
            //giải nén ra và lấy file excel hồ sơ
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
            if (files.Count > 0)
            {
                Session["danhSachHoSo"] = model;
            }
            return PartialView("HoSoExcelTable",model);
        }
        public ActionResult GetThanhPhanHoSoExcel()
        {
            IEnumerable<string> result = new string[] { };

            try
            {
                var listExcelTPHS = new List<ViewThanhPhanHoSoExcel>();

                var files = Request.Files;
                var folderName = Request["folderName"];

                Session["duongDanExcel"] = folderName;
                string pathFolder = "/Assets/Admin/zip";
                var pathZip = Path.Combine(Server.MapPath(pathFolder), files[0].FileName);
                //lấy ra tên file từ trong file zip
                using (ZipArchive za = ZipFile.OpenRead(pathZip))
                {
                     result = from item in za.Entries
                                 where item.FullName.Contains(Session["duongDanExcel"].ToString())
                                 where item.FullName.EndsWith(".xlsx")
                                 select item.FullName;
                    if (!result.Any())
                    {
                        Exception myexception = new Exception("Không tìm thấy thư mục hoặc file này!");
                        throw myexception;
                    }
                    var pathFile = Path.Combine(Server.MapPath(pathFolder), result.FirstOrDefault());

                    listExcelTPHS = Stuff.GetListExcel<ViewThanhPhanHoSoExcel>(pathFile);

                }
                //đọc file excel từ pathExcel
                var model = from tp in listExcelTPHS
                            where tp.TenThuMuc != null &&
                                    tp.TieuDe != null &&
                                    tp.MaThanhPhan != null &&
                                    tp.IDLoaiThanhPhan != null &&
                                    tp.TrangThai != null
                            join ltp in Stuff.GetAll<LoaiThanhPhan>()
                            on tp.IDLoaiThanhPhan equals ltp.ID.ToString()
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
            // bắt ngoại lệ
            catch (Exception e)
            {
                return Json(new
                {
                    state = false,
                    icon = "error",
                    heading = "Có lỗi",
                    message = e.Message,
                }, JsonRequestBehavior.AllowGet); 
            }

        }
        public ActionResult GetPDFExcel()
        {
            var files = Request.Files;
            var folderName = Request["folderName"];
             
            var pathFile = Session["duongDanExcel"].ToString()+"/"+  folderName;
            string pathFolder = "/Assets/Admin/zip";
            var pathZip = Path.Combine(Server.MapPath(pathFolder), files[0].FileName);
            //lấy ra tên file từ trong file zip
            using (ZipArchive za = ZipFile.OpenRead(pathZip))
            {

                var result = from item in za.Entries
                             where item.FullName.Contains(pathFile)
                             where item.FullName.EndsWith(".pdf")
                             select new PDFThanhPhanHoSo
                             {
                                 PathPDF = pathFolder+ "/"+ item.FullName,
                                 TenPDF = item.Name,
                             };
            return PartialView("PDFHoSoTable", result);
            }

        }
        public ActionResult Save()
        {
            var files = Request.Files;
            string pathFolder = "/Assets/Admin/zip";
            string pathFolderPDF = "/Assets/Admin/pdf";

            var pathZip = Path.Combine(Server.MapPath(pathFolder), files[0].FileName);



            //lưu từng hồ sơ 1 
            //lưu tiếp thành phần 1 của hồ sơ 1 
            //lưu tiếp nội dung thành phần 1
            //lặp lại đến khi hết nội dung thành phần
            //lặp lại đến khi hết tp
            //lặp lại đến khi hết hồ sơ 

            //lưu hồ sơ 
            //đọc từ file zip ra filepath danhsachhoso
            var danhSachHoSo = (List<ViewHoSo>)Session["danhSachHoSo"];

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = config.CreateMapper();

            //duyệt qua từng hồ sơ 
            foreach (var itemHS in danhSachHoSo)
            {
                //map lại kiểu từ viewhoso -> hoso
                HoSo hs = mapper.Map<HoSo>(itemHS);
                hs.NgayTao = DateTime.Now.ToString();
                hs.NguoiTao = Session["UserNameNV"].ToString();

                var listExcelTPHS = new List<ViewThanhPhanHoSoExcel>();
                //mở file zip ra để đọc tên file
                using (ZipArchive za = ZipFile.OpenRead(pathZip))
                {
                    var excelPathFile = from item in za.Entries
                             where item.FullName.Contains(itemHS.TenThuMuc)
                             where item.FullName.EndsWith(".xlsx")
                             select item.FullName;
                    //kiểm tra xem hồ sơ đấy có thành phần không
                    if (excelPathFile.Any())
                    {
                        var pathFile = Path.Combine(Server.MapPath(pathFolder), excelPathFile.FirstOrDefault());
                        listExcelTPHS = Stuff.GetListExcel<ViewThanhPhanHoSoExcel>(pathFile);

                        //đọc file excel từ pathExcel
                        //đọc thành phần của hồ sơ
                        var model = from tp in listExcelTPHS
                                    where tp.TenThuMuc != null &&
                                            tp.TieuDe != null &&
                                            tp.MaThanhPhan != null &&
                                            tp.IDLoaiThanhPhan != null &&
                                            tp.TrangThai != null
                                    join ltp in Stuff.GetAll<LoaiThanhPhan>()
                                    on tp.IDLoaiThanhPhan equals ltp.ID.ToString()
                                    select new ViewThanhPhanHoSo
                                    {
                                        TenThuMuc = tp.TenThuMuc,
                                        TieuDe = tp.TieuDe,
                                        MaThanhPhan = tp.MaThanhPhan,
                                        IDLoaiThanhPhan = ltp.ID,
                                        KiHieu = tp.KiHieu,
                                        ChuThich = tp.ChuThich,
                                        NgayTao = DateTime.Now.ToString(),
                                        NguoiTao = Session["UserNameNV"].ToString(),
                                    };
                        //duyệt qua từng thành phần hồ sơ trong excel và map lại thành tphs có trong db
                        using (IDbConnection db = new SqlConnection(ConnectString.Setup()))
                        {
                            //lưu hồ sơ vào để tý lấy ra ID  gán cho thành phần
                            db.Insert(hs);

                            foreach (var itemTPHS in model)
                            {
                                ThanhPhanHoSo tphs = mapper.Map<ThanhPhanHoSo>(itemTPHS);
                                tphs.IDHoSo = Stuff.GetList<HoSo>("select  top 1 ID from HoSo order by ID desc").FirstOrDefault().ID;
                                //lưu vào db
                                db.Insert(tphs);
                                var listPDF = new List<PDFThanhPhanHoSo>();
                                //lại mở file zip ra để đọc lấy cái file pdf của thành phần

                                foreach (var item in za.Entries)
                                {
                                    if (item.FullName.Contains(itemHS.TenThuMuc + "/" + itemTPHS.TenThuMuc) && item.FullName.EndsWith(".pdf"))
                                    {
                                        //lưu file
                                        string fixedName = item.Name.Split('.')[0] + "_" + DateTime.Now.Ticks.ToString() + "." + item.Name.Split('.')[1];
                                        string pathSave = Path.Combine(Server.MapPath(pathFolderPDF), fixedName);
                                        item.ExtractToFile(pathSave);

                                        //lưu từng pdf
                                        var itemPDF = new PDFThanhPhanHoSo();
                                        itemPDF.PathPDF = pathFolderPDF + "/" + fixedName;
                                        itemPDF.TenPDF = item.Name;
                                        itemPDF.TrangThai = 1;
                                        itemPDF.IDThanhPhan = Stuff.GetList<ThanhPhanHoSo>("select top 1 ID from ThanhPhanHoSo order by ID desc").FirstOrDefault().ID;
                                        listPDF.Add(itemPDF);
                                    }
                                }
                                //lưu path pdf vào db
                                db.Insert(listPDF);
                            }
                        }
                    }

                }
            }
            return Json(new
            {
                icon = "sucess",
                heading = "Thành công",
                message = "Lưu thành công"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}