using Dapper.Contrib.Extensions;
using QuanLyHoSo.Models;
using QuanLyHoSo.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace QuanLyHoSo.Dao.DaoAdmin
{
    public class StorageDao
    {
        public static List<ViewKho> GetAllKho()
        {
            string query = "select k1.*,k2.TenKho as TenKhoCha  from Kho k1 left join Kho k2 on k2.ID=k1.IDKhoCha ";

            return Stuff.GetList<ViewKho>(query);
        }
        public static void UpdateStorage(Kho kho, long IDNV, string UserNameNV)
        {
            string query = "Update Kho Set TenKho=@TenKho,MaKho=@MaKho,IDKhoCha=@IDKhoCha,TrangThai=@TrangThai,KichThuoc=@KichThuoc,MoTa=@MoTa,NgaySua=GETDATE(),NguoiSua =@NguoiSua,DuongDan=@DuongDan WHERE ID =@ID";
            object param = new
            {
                TenKho = kho.TenKho,
                MaKho = kho.MaKho,
                IDKhoCha = kho.IDKhoCha,
                TrangThai = kho.TrangThai,
                KichThuoc = kho.KichThuoc,
                DuongDan= kho.DuongDan,
                MoTa = kho.MoTa,
                NguoiSua = UserNameNV,
                ID = IDNV
            };
            Stuff.ExecuteSql(query, param);
        }
        public static void UpdateStorageNoChange(Kho kho, long IDNV)
        {
            string query = "Update Kho Set TenKho=@TenKho,MaKho=@MaKho,IDKhoCha=@IDKhoCha,TrangThai=@TrangThai,KichThuoc=@KichThuoc,MoTa=@MoTa,DuongDan=@DuongDan WHERE ID =@ID";
            object param = new
            {
                TenKho = kho.TenKho,
                MaKho = kho.MaKho,
                IDKhoCha = kho.IDKhoCha,
                TrangThai = kho.TrangThai,
                KichThuoc = kho.KichThuoc,
                DuongDan = kho.DuongDan,
                MoTa = kho.MoTa,
                ID = IDNV
            };
            Stuff.ExecuteSql(query, param);
        }
        public static void CreateStorage(Kho kho, string UserNameNV)
        {
            //create new kho
            using (var con = new SqlConnection(ConnectString.Setup()))
            {
                kho.NgayTao = DateTime.Now.ToString();
                kho.NguoiTao = UserNameNV;
                con.Insert(kho);
            }
        }

        public static void ChangeStateByID(long ID, int state, string UserNameNV)
        {
            string query = $"Update [Kho] SET [TrangThai]={state},[NguoiSua]='{UserNameNV}',[NgaySua]=GETDATE() WHERE ID={ID}";
            Stuff.ExecuteSql(query);
        }

        public static void DeleteUserByID(long ID, string UserNameNV)
        {
            string query = $"Update [Kho] SET [TrangThai]=10,[NguoiSua]='{UserNameNV}',[NgaySua]=GETDATE() WHERE ID={ID}";
            Stuff.ExecuteSql(query);
        }
    }
}