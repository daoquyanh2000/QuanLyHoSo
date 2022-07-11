using QuanLyHoSo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyHoSo.ViewModel
{
    public class ViewNgan :Ngan
    {
        public string TenKho { get; set; }

        public List<ViewNgan> NganCon { get; set; }

    }
}