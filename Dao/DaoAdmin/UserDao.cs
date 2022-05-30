using QuanLyHoSo.Dao;
using QuanLyLogin.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;

namespace QuanLyLogin.Dao.DaoAdmin
{

    public class UserDao
    {
        string ConnectionString = "server=TEU-LAPTOP\\TEU; database=QuanLyHoSo;integrated security=true";
        public List<DataNhanVien> GetAllUser()
        {
            List<DataNhanVien> listUser = new List<DataNhanVien>();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {

                con.Open();
                string stringQuery = "select Id,HoTen,UserName,Quyen,NgayTao FROM [User] ";
                SqlCommand cmd = new SqlCommand(stringQuery, con);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    DataNhanVien data = new DataNhanVien();

                    data.ID = Convert.ToInt64(dr["ID"]);
                    data.HoTen = dr["HoTen"].ToString();
                    data.UserName = dr["UserName"].ToString();
                    data.Quyen = Convert.ToByte(dr["Quyen"]);
                    listUser.Add(data);
                }
            }
            return listUser;
        }
        public static void CreateNewUser(NhanVien newUser)
        {
            using(SqlConnection con= new SqlConnection(ConnectString.Setup()))
            {
                con.Open();
                string stringQuery = "INSERT INTO [User] VALUES( " +
                    "@UserName," +
                    "@Password," +
                    "@TrangThai," +
                    "@Quyen," +
                    "@HoTen," +
                    "@SDT," +
                    "@Email," +
                    "@NgaySinh," +
                    "@AnhDaiDien," +
                    "@GioiTinh," +
                    "@DiaChi," +
                    "@QueQuan," +
                    "@CongTy," +
                    "@ChucVu," +
                    "@TieuSu," +
                    "@NgayTao," +
                    "@NguoiTao," +
                    "@NgaySua," +
                    "@NguoiSua)";
                var NullOjbect = (object)DBNull.Value;
                SqlCommand cmd = new SqlCommand(stringQuery, con);
                cmd.Parameters.AddWithValue("@UserName", newUser.UserName);
                cmd.Parameters.AddWithValue("@Password", newUser.Password);
                cmd.Parameters.AddWithValue("@TrangThai", newUser.TrangThai);
                cmd.Parameters.AddWithValue("@Quyen", newUser.Quyen);
                cmd.Parameters.AddWithValue("@HoTen", newUser.HoTen);
                cmd.Parameters.AddWithValue("@SDT", newUser.SDT);
                cmd.Parameters.AddWithValue("@Email", newUser.Email  );
                cmd.Parameters.AddWithValue("@NgaySinh", NullOjbect);
                cmd.Parameters.AddWithValue("@AnhDaiDien", newUser.AnhDaiDien );
                cmd.Parameters.AddWithValue("@GioiTinh", newUser.GioiTinh );
                cmd.Parameters.AddWithValue("@DiaChi", newUser.DiaChi );
                cmd.Parameters.AddWithValue("@QueQuan", newUser.QueQuan );
                cmd.Parameters.AddWithValue("@CongTy", newUser.CongTy );
                cmd.Parameters.AddWithValue("@ChucVu", newUser.ChucVu );
                cmd.Parameters.AddWithValue("@TieuSu", newUser.TieuSu );
                cmd.Parameters.AddWithValue("@NgayTao", NullOjbect);
                cmd.Parameters.AddWithValue("@NguoiTao",Convert.ToInt64 (HttpContext.Current.Session["IDNV"]));
                cmd.Parameters.AddWithValue("@NgaySua", NullOjbect);
                cmd.Parameters.AddWithValue("@NguoiSua", NullOjbect);
                cmd.ExecuteNonQuery();
            }
        }

    }
}