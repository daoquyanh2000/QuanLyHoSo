using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyHoSo.Models
{
    [Table("LoaiThanhPhan")]
    public class LoaiThanhPhan
    {
        [Key]
        public long ID { get; set; }
        public string TenLoaiThanhPhan { get; set; }
        public string MaLoaiThanhPhan { get; set; }
        public byte ChuThich { get; set; }
    }
}