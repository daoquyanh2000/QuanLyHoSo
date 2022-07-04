using Dapper.Contrib.Extensions;

namespace QuanLyHoSo.Models
{
    [Table("HoSo")]
    public class HoSo
    {
        [Key]
        public long ID { get; set; }

        public string TieuDe { get; set; }
        public string MaHoSo { get; set; }
        public string KyHieu { get; set; }
        public string MaDanhMuc { get; set; }
        public string MaKho { get; set; }
        public string MaNgan { get; set; }
        public string MaLoaiHoSo { get; set; }
        public string AnhBia { get; set; }
        public byte TrangThai { get; set; }
        public string MoTa { get; set; }
        public string ThoiGianLuuTru { get; set; }
        public string ThoiHanBaoQuan { get; set; }
        public string NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public string NgaySua { get; set; }
        public string NguoiSua { get; set; }
    }
}

/*                            ;*/