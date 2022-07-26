using AutoMapper;
using Dapper.Contrib.Extensions;
using OfficeOpenXml;
using OfficeOpenXml.Table;
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
using Ionic.Zip;
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
            var listExcel = new List<ViewExcelHoSo>();
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
            using (var za = ZipFile.Read(files[0].InputStream))
            {
                za.ExtractAll(Server.MapPath(pathFolder), ExtractExistingFileAction.DoNotOverwrite);
                var pathExcelHoSo =za.Entries.FirstOrDefault(x => x.FileName.Contains("HoSoTemplate.xlsx")).FileName;
                pathExcelHoSo = Path.Combine(Server.MapPath(pathFolder), pathExcelHoSo);
                //đọc file excel hs và lấy thông tin tất cả hs -> đẩy ra view
                listExcel = Stuff.GetListExcel<ViewExcelHoSo>(pathExcelHoSo);
            }

            var listDm = Stuff.GetAll<DanhMuc>().Where(x => x.TrangThai == 1);
            var lisNgan = Stuff.GetAll<Ngan>().Where(x => x.TrangThai == 1);
            var listKho = Stuff.GetAll<Kho>().Where(x => x.TrangThai == 1);
            var listType = Stuff.GetAll<LoaiHoSo>().Where(x => x.TrangThai == 1);

            var model = from hs in listExcel
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
            var listExcel = new List<ViewThanhPhanHoSoExcel>();
            try
            {

                var files = Request.Files;
                var folderName = Request["folderName"];

                Session["duongDanExcel"] = folderName;
                string pathFolder = "/Assets/Admin/zip";
                var pathZip = Path.Combine(Server.MapPath(pathFolder), files[0].FileName);
                //đọc tên file excel thành phần hồ sơ
                using (var za = ZipFile.Read(files[0].InputStream))
                {
                    var pathExcelHoSo = (from item in za.Entries
                                         where item.FileName.Contains(folderName+ "/ThanhPhanTemplate.xlsx")
                                         select item)
                                         .FirstOrDefault().FileName;

                    pathExcelHoSo = Path.Combine(Server.MapPath(pathFolder), pathExcelHoSo);
                    //đọc file excel hs và lấy thông tin tất cả hs -> đẩy ra view
                    listExcel = Stuff.GetListExcel<ViewThanhPhanHoSoExcel>(pathExcelHoSo);
                }
                //đọc file excel từ pathExcel
                var model = from tp in listExcel
                                //where tp.TenThuMuc != null &&
                            where
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

            var pathFile = Session["duongDanExcel"].ToString() + "/" + folderName;
            string pathFolder = "/Assets/Admin/zip";

            //lấy ra tên file từ trong file zip
            using (var za = ZipFile.Read(files[0].InputStream))
            {

                var result = from item in za.Entries
                             where item.FileName.Contains(pathFile)
                             where item.FileName.EndsWith(".pdf")
                             select new PDFThanhPhanHoSo
                             {
                                 PathPDF = pathFolder + "/" + item.FileName,
                                 TenPDF = item.FileName.Split('/').Last(),
                             };
                return PartialView("PDFHoSoTable", result);
            }
        }
/*        public ActionResult Save()
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
                    using (IDbConnection db = new SqlConnection(ConnectString.Setup()))
                    {
                        //kiểm tra xem hồ sơ đấy có thành phần không
                        if (excelPathFile.Any())
                        {
                            var pathFile = Path.Combine(Server.MapPath(pathFolder), excelPathFile.FirstOrDefault());
                            listExcelTPHS = Stuff.GetListExcel<ViewThanhPhanHoSoExcel>(pathFile);

                            //đọc file excel từ pathExcel
                            //đọc thành phần của hồ sơ
                            var model = from tp in listExcelTPHS
                                            //where tp.TenThuMuc != null &&
                                        where
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
                        else
                        {
                            db.Insert(hs);
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
        }*/

        public ActionResult DownloadZip()

        {

            //tạo folder
            string pathFolder = "Assets/Admin/zip/download/HoSoTemplate";
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var stream = new MemoryStream())
            {
                using (var zip = new ZipFile())
                {
                    //tạo file excel HoSoTemplate.xlsx
                    using (var package = new ExcelPackage())
                    {
                        //A workbook must have at least on cell, so lets add one... 
                        var wsData = package.Workbook.Worksheets.Add("Data");
                        var wsLoaiHoSo = package.Workbook.Worksheets.Add("LoaiHoSo");
                        var wsDanhMuc = package.Workbook.Worksheets.Add("DanhMuc");
                        var wsKho = package.Workbook.Worksheets.Add("Kho");
                        var wsNgan = package.Workbook.Worksheets.Add("Ngan");
                        var wsThoiHanBaoQuan = package.Workbook.Worksheets.Add("ThoiHan");
                        //tạo các bảng thông tin cần điền
                        var listLoaiHoSo = from item in Stuff.GetAll<LoaiHoSo>()
                                           where item.TrangThai == 1
                                           select new
                                           {
                                               TenLoaiHoSo = item.TenLoaiHoSo,
                                               MaLoaiHoSo = item.MaLoaiHoSo,
                                           };
                        var listDanhMuc = from item in Stuff.GetAll<DanhMuc>()
                                          where item.TrangThai == 1
                                          select new
                                          {
                                              TenDanhMuc = item.TenDanhMuc,
                                              MaDanhMuc = item.MaDanhMuc,
                                          };
                        var listKho = from item in Stuff.GetAll<Kho>()
                                      where item.TrangThai == 1
                                      select new
                                      {
                                          TenKho = item.TenKho,
                                          MaKho = item.MaKho,
                                      };
                        var listNgan = from item in Stuff.GetAll<Ngan>()
                                       where item.TrangThai == 1
                                       select new
                                       {
                                           TenNgan = item.TenNgan,
                                           MaNgan = item.MaNgan,
                                       };
                        var ThoiHanBaoQuan = new ThoiHan
                        {
                            TrangThai = "Vĩnh Viễn",
                            SoNam = 0,

                        };
                        var listThoiHan = new List<ThoiHan>();
                        listThoiHan.Add(ThoiHanBaoQuan);
                        //tạo ra mấy cái hồ sơ dummy
                        var listHoSoTemplate = new List<ViewExcelHoSo>();
                        var hs = new ViewExcelHoSo();
                        hs.TenThuMuc = $"HoSo1";
                        listHoSoTemplate.Add(hs);
                        //load list vào excel
                        wsData.Cells["A1"].LoadFromCollection(listHoSoTemplate, true, TableStyles.Medium1);
                        wsLoaiHoSo.Cells["A1"].LoadFromCollection(listLoaiHoSo, true, TableStyles.Medium1);
                        wsDanhMuc.Cells["A1"].LoadFromCollection(listDanhMuc, true, TableStyles.Medium1);
                        wsKho.Cells["A1"].LoadFromCollection(listKho, true, TableStyles.Medium1);
                        wsNgan.Cells["A1"].LoadFromCollection(listNgan, true, TableStyles.Medium1);
                        wsThoiHanBaoQuan.Cells["A1"].LoadFromCollection(listThoiHan, true, TableStyles.Medium1);

                        //add data validation
                        var dvLoaiHoSo = wsData.DataValidations.AddListValidation("C2");
                        var dvDanhMuc = wsData.DataValidations.AddListValidation("D2");
                        var dvKho = wsData.DataValidations.AddListValidation("E2");
                        var dvNgan = wsData.DataValidations.AddListValidation("F2");
                        var dvThoiHan = wsData.DataValidations.AddListValidation("K2");

                        dvLoaiHoSo.Formula.ExcelFormula = $"LoaiHoSo!$B$2:$B${listLoaiHoSo.Count() + 1}";
                        dvDanhMuc.Formula.ExcelFormula = $"DanhMuc!$B$2:$B${listDanhMuc.Count() + 1}";
                        dvKho.Formula.ExcelFormula = $"Kho!$B$2:$B${listKho.Count() + 1}";
                        dvNgan.Formula.ExcelFormula = $"Ngan!$B$2:$B${listNgan.Count() + 1}";
                        dvThoiHan.Formula.ExcelFormula = $"ThoiHan!$B$2:$B${listThoiHan.Count() + 1}";

                        //add date validation
                        wsData.Cells["J2:J2"].Style.Numberformat.Format = "m/d/yyyy";
                        wsData.Cells["H2:H2"].Formula = "=CONCATENATE(D2" + @","".""," + "E2" + @","".""," + "F2" + @","".""," + "G2" + ")";
                        wsData.Cells["H2:H2"].Calculate();
                        foreach (var ws in package.Workbook.Worksheets)
                        {
                            ws.Cells[1, 1, ws.Dimension.End.Row, ws.Dimension.End.Column].AutoFitColumns();
                        }
                        //save file
                        var fileName = "HoSoTemplate/HoSoTemplate.xlsx";

                        zip.AddEntry(fileName, package.GetAsByteArray());

                    }

                    //tạo file excel HoSo1/ThanhPhanTemplate.xlsx
                    using (var package = new ExcelPackage())
                    {
                        //A workbook must have at least on cell, so lets add one... 
                        var wsData = package.Workbook.Worksheets.Add("Data");
                        var wsLoaiThanhPhan = package.Workbook.Worksheets.Add("LoaiThanhPhan");
                        var wsTrangThai = package.Workbook.Worksheets.Add("TrangThai");

                        //tạo các bảng thông tin cần điền
                        var listLoaiTP = from item in Stuff.GetAll<LoaiThanhPhan>()
                                         select new
                                         {
                                             TenLoaiThanhPhan = item.TenLoaiThanhPhan,
                                             IDLoaiThanhPhan = item.ID,
                                         };
                        var listTrangThai = new List<TrangThaiThanhPhan>();
                        for (int i = 0; i <= 1; i++)
                        {
                            var TrangThai = new TrangThaiThanhPhan();
                            if (i == 0)
                            {
                                TrangThai = new TrangThaiThanhPhan
                                {
                                    TenTrangThai = "Công khai",
                                    IDTrangThai = i,
                                };
                            }
                            else
                            {
                                TrangThai = new TrangThaiThanhPhan
                                {
                                    TenTrangThai = "Công khai",
                                    IDTrangThai = i,
                                };
                            }
                            listTrangThai.Add(TrangThai);

                        }

                        //tạo ra mấy cái thành phần hồ sơ dummy
                        var listTPHS = new List<ViewThanhPhanHoSoExcel>();
                        var tphs = new ViewThanhPhanHoSoExcel();
                        tphs.TenThuMuc = $"ThanhPhan1";
                        listTPHS.Add(tphs);
                        //load list vào excel
                        wsData.Cells["A1"].LoadFromCollection(listTPHS, true, TableStyles.Medium1);
                        wsLoaiThanhPhan.Cells["A1"].LoadFromCollection(listLoaiTP, true, TableStyles.Medium1);
                        wsTrangThai.Cells["A1"].LoadFromCollection(listTrangThai, true, TableStyles.Medium1);

                        //add data validation
                        var dvLoaiThanhPhan = wsData.DataValidations.AddListValidation("D2");
                        var dvTrangThai = wsData.DataValidations.AddListValidation("G2");

                        dvLoaiThanhPhan.Formula.ExcelFormula = $"LoaiThanhPhan!$B$2:$B${listLoaiTP.Count() + 1}";
                        dvTrangThai.Formula.ExcelFormula = $"TrangThai!$B$2:$B${listTrangThai.Count() + 1}";

                        foreach (var ws in package.Workbook.Worksheets)
                        {
                            ws.Cells[1, 1, ws.Dimension.End.Row, ws.Dimension.End.Column].AutoFitColumns();
                        }
                        //save file
                        var fileName = "HoSoTemplate/HoSo1/ThanhPhanTemplate.xlsx";
                        zip.AddEntry(fileName, package.GetAsByteArray());
                    }

                    //thêm file pdf mẫu vào 
                    var filePDF = Server.MapPath("/Assets/Admin/zip/TaiLieuMau.pdf");
                    zip.AddFile(filePDF, "HoSoTemplate/HoSo1/ThanhPhan1");
                    zip.Save(stream);
                    return File(
                        stream.ToArray(),
                        System.Net.Mime.MediaTypeNames.Application.Zip,
                        "HoSoTemplate.zip"
                    );
                }
            }

        }/*
*/    }
}