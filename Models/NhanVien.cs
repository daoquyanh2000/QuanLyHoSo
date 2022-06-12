using System.ComponentModel.DataAnnotations;

namespace QuanLyHoSo.Models
{
    public class NhanVien

    {
        //7 truong required
        public long ID { get; set; }

        [Display(Name = "Tên tài khoản")]
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản")]
        public string UserName { get; set; }

        public byte Quyen { get; set; }
        public string HoTen { get; set; }

        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        public int TrangThai { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }

        //8 truong option + 4 truong ngay tao
        public string NgaySinh { get; set; }

        public string AnhDaiDien { get; set; }
        public byte? GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string QueQuan { get; set; }
        public string CongTy { get; set; }
        public string ChucVu { get; set; }
        public string TieuSu { get; set; }
        public string NguoiTao { get; set; }
        public string NgayTao { get; set; }
        public string NgaySua { get; set; }
        public string NguoiSua { get; set; }
    }

    public class DataNhanVien : NhanVien
    {
        public string TenQuyen { get; set; }
    }
}