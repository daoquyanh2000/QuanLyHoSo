﻿using Dapper.Contrib.Extensions;

namespace QuanLyHoSo.Models
{
    [Table("Kho")]
    public class Kho
    {
        [Key]
        public long ID { get; set; }

        public string TenKho { get; set; }
        public string MaKho { get; set; }
        public long IDKhoCha { get; set; }
        public byte TrangThai { get; set; }
        public string DuongDan { get; set; }
        public int KichThuoc { get; set; }
        public string MoTa { get; set; }
        public string NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public string NgaySua { get; set; }
        public string NguoiSua { get; set; }
    }
}