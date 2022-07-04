using Dapper.Contrib.Extensions;

namespace QuanLyHoSo.Models
{
    [Table("DanhMuc")]
    public class DanhMuc
    {
        [Key]
        public long ID { get; set; }

        public string TenDanhMuc { get; set; }
        public string MaDanhMuc { get; set; }
        public long IDDanhMucCha { get; set; }
        public byte TrangThai { get; set; }
        public string MoTa { get; set; }
        public string NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public string NgaySua { get; set; }
        public string NguoiSua { get; set; }
    }
}