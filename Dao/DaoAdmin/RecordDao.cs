using Dapper.Contrib.Extensions;
using QuanLyHoSo.Models;
using QuanLyHoSo.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace QuanLyHoSo.Dao.DaoAdmin
{
    public class RecordDao
    {
        public static List<ViewHoSo> GetAllRecord()
        {
            string query = @"select hs.*,k.TenKho,n.TenNgan,dm.TenDanhMuc from HoSo hs 
	                        left join Kho k on hs.MaKho = k.MaKho 
	                        left join Ngan n on hs.MaNgan = n.MaNgan 
	                        left join DanhMuc dm on hs.MaDanhMuc =dm.MaDanhMuc";
            return Stuff.GetList<ViewHoSo>(query);

        }
        public static void ChangeStateByID(long ID, int state, string UserNameNV)
        {
            string query = $"Update [HoSo] SET [TrangThai]={state},[NguoiSua]='{UserNameNV}',[NgaySua]=GETDATE() WHERE ID={ID}";
            Stuff.ExecuteSql(query);
        }
        public static void DeleteUserByID(long ID, string UserNameNV)
        {
            string query = $"Update [HoSo] SET [TrangThai]=10,[NguoiSua]='{UserNameNV}',[NgaySua]=GETDATE() WHERE ID={ID}";
            Stuff.ExecuteSql(query);
        }
        public static void Create(HoSo hs, string UserNameNV)
        {
            //create new kho
            using (var con = new SqlConnection(ConnectString.Setup()))
            {
                hs.NgayTao = DateTime.Now.ToString();
                hs.NguoiTao = UserNameNV;
                con.Insert(hs);
            }
        }
        public static void Update(HoSo hs, long IDNV, string UserNameNV)
        {
            string query = @"Update HoSo Set 
                                    TieuDe=@TieuDe,
                                    MaHoSo=@MaHoSo,
                                    KyHieu=@KyHieu,
                                    MaDanhMuc=@MaDanhMuc,
                                    MaKho=@MaKho,
                                    MaNgan=@MaNgan,
                                    MaLoaiHoSo=@MaLoaiHoSo,
                                    AnhBia=@AnhBia,
                                    TrangThai=@TrangThai,
                                    MoTa=@MoTa,
                                    ThoiGianLuuTru=@ThoiGianLuuTru,
                                    ThoiHanBaoQuan=@ThoiHanBaoQuan,
                                    NgaySua=GETDATE(),NguoiSua =@NguoiSua WHERE ID =@ID";
            object param = new
            {
                TieuDe = hs.TieuDe,
                MaHoSo = hs.MaHoSo,
                KyHieu = hs.KyHieu,
                MaDanhMuc = hs.MaDanhMuc,
                MaKho = hs.MaKho,
                MaNgan = hs.MaNgan,
                MaLoaiHoSo = hs.MaLoaiHoSo,
                AnhBia = hs.AnhBia,
                TrangThai = hs.TrangThai,
                MoTa = hs.MoTa,
                ThoiGianLuuTru = hs.ThoiGianLuuTru,
                ThoiHanBaoQuan = hs.ThoiHanBaoQuan,
                NguoiSua = UserNameNV,
                ID = IDNV
            };
            Stuff.ExecuteSql(query, param);
        }
        /*        public static void Update(HoSo hs, long IDNV, string UserNameNV)
                {
                    //create new kho
                    using (var con = new SqlConnection(ConnectString.Setup()))
                    {
                        hs.NgaySua = DateTime.Now.ToString();
                        hs.NguoiSua = UserNameNV;
                        con.Update(hs);
                    }
                }*/
    }
}