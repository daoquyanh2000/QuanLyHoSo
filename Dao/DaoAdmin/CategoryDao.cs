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
    public class CategoryDao
    {
        public static List<ViewDanhMuc> GetAllCategory()
        {
            string query = "select dm1.*,dm2.TenDanhMuc as TenDanhMucCha from DanhMuc dm1 left join DanhMuc dm2 on dm1.IDDanhMucCha=dm2.ID";

            return Stuff.GetList<ViewDanhMuc>(query);
        }
        public static void UpdateStorage(DanhMuc dm, long IDNV, string UserNameNV)
        {
            string query = "Update DanhMuc Set TenDanhMuc=@TenDanhMuc,MaDanhMuc=@MaDanhMuc,IDDanhMucCha=@IDDanhMucCha,TrangThai=@TrangThai,MoTa=@MoTa,NgaySua=GETDATE(),NguoiSua =@NguoiSua,DuongDan=@DuongDan WHERE ID =@ID";
            object param = new
            {
                TenDanhMuc = dm.TenDanhMuc,
                MaDanhMuc = dm.MaDanhMuc,
                IDDanhMucCha = dm.IDDanhMucCha,
                TrangThai = dm.TrangThai,
                MoTa = dm.MoTa,
                NguoiSua = UserNameNV,
                DuongDan=dm.DuongDan,
                ID = IDNV
            };
            Stuff.ExecuteSql(query, param);
        }

        public static void CreateStorage(DanhMuc dm, string UserNameNV)
        {
            //create new kho
            using (var con = new SqlConnection(ConnectString.Setup()))
            {
                dm.NgayTao = DateTime.Now.ToString();
                dm.NguoiTao = UserNameNV;
                con.Insert(dm);
            }
        }

        public static void ChangeStateByID(long ID, int state, string UserNameNV)
        {
            string query = $"Update [DanhMuc] SET [TrangThai]={state},[NguoiSua]='{UserNameNV}',[NgaySua]=GETDATE() WHERE ID={ID}";
            Stuff.ExecuteSql(query);
        }

        public static void DeleteUserByID(long ID, string UserNameNV)
        {
            string query = $"Update [DanhMuc] SET [TrangThai]=10,[NguoiSua]='{UserNameNV}',[NgaySua]=GETDATE() WHERE ID={ID}";
            Stuff.ExecuteSql(query);
        }
        public static List<DanhMuc> GetByIDCha(long IDCha)
        {
            string query = "select * from DanhMuc Where DuongDan like '@IDCha-%'";
            object param = new
            {
                IDCHa = IDCha
            };
            return Stuff.GetList<DanhMuc>(query,param);
        }
                
    }
}