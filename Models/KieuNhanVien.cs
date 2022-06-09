using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyHoSo.Models
{
    public class KieuNhanVien
    {
        public long ID { get; set; }
        public string TenQuyen { get; set; }
        public string KieuQuyen { get; set; }
        public string Mota { get; set; }
        public byte TrangThai { get; set; }
        public string NguoiTao { get; set; }
        public string NgayTao { get; set; }
        public string NgaySua { get; set; }
        public string NguoiSua { get; set; }
    }
}