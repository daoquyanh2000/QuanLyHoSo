using Dapper.Contrib.Extensions;

namespace QuanLyHoSo.Dao.DaoAdmin
{
    [Table("LoaiHoSo")]
    public class LoaiHoSo
    {
        [Key]
        public long ID { get; set; }

        public string TenLoaiHoSo { get; set; }
        public string MaLoaiHoSo { get; set; }
        public byte TrangThai { get; set; }
        public string MoTa { get; set; }
        public string NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public string NgaySua { get; set; }
        public string NguoiSua { get; set; }
    }
}