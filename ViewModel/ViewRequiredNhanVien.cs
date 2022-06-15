using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyHoSo.ViewModel
{
    public class ViewRequiredNhanVien
    {
        public string HoTen { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public byte Quyen { get; set; }

        public int TrangThai { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
    }
}