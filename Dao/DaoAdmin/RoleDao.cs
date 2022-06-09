using Dapper;
using QuanLyHoSo.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace QuanLyHoSo.Dao.DaoAdmin
{
    public class RoleDao
    {
        public static List<KieuNhanVien> GetKieuNhanViens()
        {
            using(SqlConnection con =new SqlConnection(ConnectString.Setup()))
            {
                con.Open();
                string query = "Select * from KieuNhanVien";
                var kieuNhanViens = con.Query<KieuNhanVien>(query).ToList();
                return kieuNhanViens;
            }
        }
    }
}