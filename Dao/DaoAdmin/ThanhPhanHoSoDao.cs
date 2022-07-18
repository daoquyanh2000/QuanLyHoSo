using QuanLyHoSo.Models;
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
        public static void Update(ThanhPhanHoSo TPHS, long ID,string UserNameNV)
        {
            string query = @"update ThanhPhanHoSo set 
	            TieuDe =@TieuDe,
	            MaThanhPhan=@MaThanhPhan,
	            IDLoaiThanhPhan=@IDLoaiThanhPhan,
	            IDHoSo=@IDHoSo,
	            KiHieu=@KiHieu,
	            ChuThich=@ChuThich,
	            TrangThai=@TrangThai,
	            NgaySua=GETDATE(),
	            NguoiSua=@NguoiSua
	            where ID=@ID";
            object param = new
            {
                IDHoSo = TPHS.IDHoSo,
                TieuDe = TPHS.TieuDe,
                IDLoaiThanhPhan = TPHS.IDLoaiThanhPhan,
                KiHieu = TPHS.KiHieu,
                MaThanhPhan = TPHS.MaThanhPhan,
                TrangThai = TPHS.TrangThai,
                ChuThich = TPHS.ChuThich,
                NguoiSua = UserNameNV,
                ID = ID,
            };
            Stuff.ExecuteSql(query, param);
        }
    }
}