using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLyLogin.Models
{
    public class NhanVien 

    {
        public long ID { get; set; }
        [Display(Name = "Tên tài khoản")]
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản")]
        public string UserName { get; set; }
        public byte? Quyen { get; set; }
        public string HoTen { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string Password { get; set; }
        public int TrangThai { get; set; }

        public string NgaySinh { get; set; }

        public string AnhDaiDien { get; set; }

        public int SDT { get; set; }
        public string Email { get; set; }
        public bool GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string QueQuan { get; set; }

        public string CongTy { get; set; }

        public string ChucVu { get; set; }

        public string TieuSu { get; set; }
        public long? NguoiTao { get; set; }
        public Nullable<DateTime> NgayTao { get; set; }

        public DateTime? NgaySua { get; set; }

        public long? NguoiSua { get; set; }


    }
    public class DataNhanVien : NhanVien { }

}