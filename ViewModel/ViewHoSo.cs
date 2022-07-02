using QuanLyHoSo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyHoSo.ViewModel
{
    public class ViewHoSo :HoSo
    {
        public string TenKho { get; set; }
        public string TenDanhMuc { get; set; }
        public string TenNgan { get; set; }
        public string TenLoaiHoSo { get; set; }

    }
}