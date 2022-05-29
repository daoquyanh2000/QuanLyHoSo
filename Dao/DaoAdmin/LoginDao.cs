using QuanLyHoSo.Dao;
using QuanLyLogin.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace QuanLyLogin.Dao.DaoAdmin
{
    public class LoginDao
    {
        public int ValidateLogin(NhanVien model)
        {
            using (SqlConnection con = new SqlConnection(ConnectString.Setup()))
            {
                con.Open();
                string stringQuery = "SELECT count(*) FROM [User] where Username =@Username and Password = @Password";
                SqlCommand cmd = new SqlCommand(stringQuery, con);
                cmd.Parameters.AddWithValue("@Username", model.Username);
                cmd.Parameters.AddWithValue("@Password", model.Password);
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }
    }
}