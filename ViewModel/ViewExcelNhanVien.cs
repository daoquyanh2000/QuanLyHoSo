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

        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string QueQuan { get; set; }
        public string CongTy { get; set; }
        public string ChucVu { get; set; }
        public string TieuSu { get; set; }
        public string AnhDaiDien { get; set; }

    }

    public class Sex
    {
        public string GioiTinh { get; set; }
        public int MaGioiTinh { get; set; }

    }
    public class State
    {
        public string TrangThai { get; set; }
        public int MaTrangThai { get; set; }

    }
    public class Image
    {
        public string TenAnhDaiDien { get; set; }
        public string DuongDan { get; set; }

    }

    public class ViewExcelNhanVienNoImg
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

        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string QueQuan { get; set; }
        public string CongTy { get; set; }
        public string ChucVu { get; set; }
        public string TieuSu { get; set; }
    }
}