using QuanLyHoSo.Models;

namespace QuanLyHoSo.ViewModel
{
    public class ViewHoSo : HoSo
    {
        public string TenThuMuc { get; set; }
        public string TenKho { get; set; }
        public string TenDanhMuc { get; set; }
        public string TenNgan { get; set; }
        public string TenLoaiHoSo { get; set; }
        
    }
    public class ThoiHan
    {
        public string TrangThai { get; set; }
        public int SoNam { get; set; }
    }
}