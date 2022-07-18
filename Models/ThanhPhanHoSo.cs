using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyHoSo.Models
{
    [Table("ThanhPhanHoSo")]
    public class ThanhPhanHoSo
    {
        [Key]
        public long ID { get; set; }
        public string TieuDe { get; set; }
        public string MaThanhPhan { get; set; }
        public long IDLoaiThanhPhan { get; set; }
        public long IDHoSo { get; set; }
        public string KiHieu { get; set; }
        public string ChuThich { get; set; }
        public byte TrangThai { get; set; }
        public string NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public string NgaySua { get; set; }
        public string NguoiSua { get; set; }


    }
}