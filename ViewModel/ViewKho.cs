using QuanLyHoSo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyHoSo.ViewModel
{
    public class ViewKho : Kho
    {
        public string TenKhoCha { get; set; }
        public List<ViewKho> KhoCon { get; set; }
        public List<ViewNgan> NganCon { get; set; }

    }
}