using QuanLyHoSo.Models;
using System;
using System.Data.SqlClient;

namespace QuanLyHoSo.Dao.DaoAdmin
{
    public class LoginDao
    {
        public static NhanVien GetUserByUserNamePassword(NhanVien model)
        {
            NhanVien User = new NhanVien();
            using (SqlConnection con = new SqlConnection(ConnectString.Setup()))
            {
                con.Open();
                string stringQuery = $"SELECT * FROM [NhanVien] WHERE UserName ='{model.UserName}' and Password ='{model.Password}'";
                SqlCommand cmd = new SqlCommand(stringQuery, con);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    User.ID = Convert.ToInt64(dr["ID"]);
                    User.HoTen = dr["HoTen"].ToString();
                    User.UserName = dr["UserName"].ToString();
                    User.TrangThai = Convert.ToByte(dr["TrangThai"]);
                    User.Quyen = Convert.ToByte(dr["Quyen"]);
                }
            }
            return User;
        }
    }
}