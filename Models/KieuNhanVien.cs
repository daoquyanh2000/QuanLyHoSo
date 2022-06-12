namespace QuanLyHoSo.Models
{
    public class KieuNhanVien
    {
        public long ID { get; set; }
        public string TenKieu { get; set; }
        public string ChuThich { get; set; }
        public byte TrangThai { get; set; }
        public string NguoiTao { get; set; }
        public string NgayTao { get; set; }
        public string NgaySua { get; set; }
        public string NguoiSua { get; set; }
    }
}