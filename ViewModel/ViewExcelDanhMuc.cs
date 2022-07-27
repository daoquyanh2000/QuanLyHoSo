using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using OfficeOpenXml.Attributes;
using OfficeOpenXml.Table;

namespace QuanLyHoSo.ViewModel
{
 

    public class ViewExcelDanhMuc
    {
        public string TenDanhMuc { get; set; }

        public string MaDanhMuc { get; set; }

        public string MaDanhMucCha { get; set; }

        public byte TrangThai { get; set; }

        public string MoTa { get; set; }
    }


    public class SheetDanhMucCha
    {
        public string TenDanhMucCha { get; set; }
        public string MaDanhMucCha { get; set; }
    }
}