using QuanLyLogin.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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

    }
}