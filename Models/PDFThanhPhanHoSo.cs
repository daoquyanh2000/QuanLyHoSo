using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyHoSo.Models
{
    [Table("PDFThanhPhanHoSo")]

    public class PDFThanhPhanHoSo
    {
        [Key]
        public long ID { get; set; }
        public long IDThanhPhan { get; set; }
        public string TenPDF { get; set; }
        public string PathPDF { get; set; }   
        public byte[] DataPDF { get; set; }
        public byte TrangThai { get; set; }

    }
}