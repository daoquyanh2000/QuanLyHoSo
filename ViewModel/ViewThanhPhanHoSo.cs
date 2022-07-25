﻿using QuanLyHoSo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyHoSo.ViewModel
{
    public class ViewThanhPhanHoSo : ThanhPhanHoSo
    {
        public string TenThuMuc { get; set; }
        public string TenLoaiThanhPhan { get; set; }
        public string TenPDF { get; set; }
        public byte[] DataPDF { get; set; }
    }
    public class TrangThaiThanhPhan
    {
        public string TenTrangThai { get; set; }
        public int IDTrangThai { get; set; }

    }
}