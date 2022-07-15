using QuanLyHoSo.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyHoSo.Dao.DaoAdmin
{
    public class ThanhPhanHoSoDao
    {
        public static List<ViewThanhPhanHoSo> GetAllContentByID(long ID)
        {
            string query = $@"select tp.*,l.TenLoaiThanhPhan as TenLoaiThanhPhan from ThanhPhanHoSo tp
			left join LoaiThanhPhan l on tp.IDLoaiThanhPhan = l.ID where tp.IDHoSo ='{ID}'";
            return Stuff.GetList<ViewThanhPhanHoSo>(query);
        }
    }
}