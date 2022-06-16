using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyHoSo.ViewModel
{
    public class ViewExcelNhanVien
    {
        public string HoTen { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string MaKieu { get; set; }

        public int TrangThai { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }

        //8 truong option + 4 truong ngay tao
        public string NgaySinh { get; set; }

        public string AnhDaiDien { get; set; }
        public byte GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string QueQuan { get; set; }
        public string CongTy { get; set; }
        public string ChucVu { get; set; }
        public string TieuSu { get; set; }
    }
}