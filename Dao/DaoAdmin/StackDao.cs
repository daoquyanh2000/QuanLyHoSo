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
    public class StackDao
    {
        public static List<ViewNgan> GetAllStack()
        {
            string query = "select ng.*,TenKho from Ngan ng inner join Kho on ng.IDKho = Kho.ID";
            return Stuff.GetList<ViewNgan>(query);
        }
        public static void UpdateStack(Ngan ngan, long IDNV, string UserNameNV)
        {
            string query = "Update Ngan Set TenNgan=@TenNgan,MaNgan=@MaNgan,IDKho=@IDKho,TrangThai=@TrangThai,KichThuoc=@KichThuoc,MoTa=@MoTa,NgaySua=GETDATE(),NguoiSua =@NguoiSua WHERE ID =@ID";
            object param = new
            {
                TenNgan = ngan.TenNgan,
                MaNgan = ngan.MaNgan,
                IDKho = ngan.IDKho,
                TrangThai = ngan.TrangThai,
                KichThuoc = ngan.KichThuoc,
                MoTa = ngan.MoTa,
                NguoiSua = UserNameNV,
                ID = IDNV
            };
            Stuff.ExecuteSql(query, param);
        }

        public static void CreateStorage(Ngan ngan, string UserNameNV)
        {
            //create new kho
            using (var con = new SqlConnection(ConnectString.Setup()))
            {
                ngan.NgayTao = DateTime.Now.ToString();
                ngan.NguoiTao = UserNameNV;
                con.Insert(ngan);
            }
        }

        public static void ChangeStateByID(long ID, int state, string UserNameNV)
        {
            string query = $"Update [Ngan] SET [TrangThai]={state},[NguoiSua]='{UserNameNV}',[NgaySua]=GETDATE() WHERE ID={ID}";
            Stuff.ExecuteSql(query);
        }

        public static void DeleteUserByID(long ID, string UserNameNV)
        {
            string query = $"Update [Ngan] SET [TrangThai]=10,[NguoiSua]='{UserNameNV}',[NgaySua]=GETDATE() WHERE ID={ID}";
            Stuff.ExecuteSql(query);
        }
    }
}