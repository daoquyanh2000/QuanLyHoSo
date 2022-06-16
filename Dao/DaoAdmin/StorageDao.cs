using Dapper.Contrib.Extensions;
using QuanLyHoSo.Models;
using System;
using System.Data.SqlClient;

namespace QuanLyHoSo.Dao.DaoAdmin
{
    public class StorageDao
    {
        public static void UpdateStorage(Kho kho, long IDNV, string UserNameNV)
        {
            string query = "Update Kho Set TenKho=@TenKho,MaKho=@MaKho,IDKhoCha=@IDKhoCha,TrangThai=@TrangThai,KichThuoc=@KichThuoc,MoTa=@MoTa,NgaySua=GETDATE(),NguoiSua =@NguoiSua WHERE ID =@ID";
            object param = new {
                TenKho = kho.TenKho,
                MaKho = kho.MaKho,
                IDKhoCha = kho.IDKhoCha,
                TrangThai = kho.TrangThai,
                KichThuoc = kho.KichThuoc,
                MoTa = kho.MoTa,
                NguoiSua = UserNameNV,
                ID = IDNV };
            Stuff.ExecuteSql(query, param);
        }
        public static void CreateStorage(Kho kho,string UserNameNV)
        {
            //create new kho
            using (var con = new SqlConnection(ConnectString.Setup()))
            {
                kho.NgayTao = DateTime.Now.ToString();
                kho.NguoiTao = UserNameNV;
                con.Insert(kho);
            }
        }


    }
}