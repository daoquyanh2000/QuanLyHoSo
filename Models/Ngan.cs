using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyHoSo.Models
{
    [Table("Ngan")]
    public class Ngan
    {
        [Key]
        public long ID { get; set; }

        public string TenNgan { get; set; }
        public string MaNgan { get; set; }
        public long IDKho { get; set; }
        public byte TrangThai { get; set; }
        public  string DuongDan { get; set; }
        public int KichThuoc { get; set; }
        public string MoTa { get; set; }
        public string NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public string NgaySua { get; set; }
        public string NguoiSua { get; set; }
    }
}