using OfficeOpenXml;
using QuanLyHoSo.Models;
using QuanLyHoSo.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace QuanLyHoSo.Dao.DaoAdmin
{
    public class UserDao
    {
        public static void ExecuteSql(string queryString)
        {
            using (SqlConnection con = new SqlConnection(ConnectString.Setup()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(queryString, con);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<NhanVien> GetUserByID(long ID)
        {
            string stringQuery = $"select * FROM [NhanVien] WHERE ID =@ID and TrangThai != 10";
            var param = new
            {
                ID = ID
            };
            return Stuff.GetList<NhanVien>(stringQuery, param);
        }

        /*        public static List<NhanVien> GetAllUser()
                {
                    List<NhanVien> listUser = new List<NhanVien>();
                    using (SqlConnection con = new SqlConnection(ConnectString.Setup()))
                    {
                        con.Open();
                        string stringQuery = "select * FROM [NhanVien] WHERE TrangTHai !=10 ORDER BY [ID] DESC";
                        SqlCommand cmd = new SqlCommand(stringQuery, con);
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            NhanVien data = new NhanVien();

                            data.ID = Convert.ToInt64(dr["ID"]);
                            data.UserName = dr["UserName"].ToString();
                            data.HoTen = dr["HoTen"].ToString();
                            data.Quyen = Convert.ToByte(dr["Quyen"]);
                            data.TrangThai = Convert.ToByte(dr["TrangThai"]);
                            data.NgayTao = Convert.ToString(dr["NgayTao"]);
                            data.NguoiTao = Convert.ToString(dr["NguoiTao"]);
                            data.NgaySua = Convert.ToString(dr["NgaySua"]);
                            data.NguoiSua = Convert.ToString(dr["NguoiSua"]);
                            listUser.Add(data);
                        }
                    }
                    return listUser;
                }*/

        public static List<DataNhanVien> GetAllUser()
        {
            string query = "select n.*, k.TenKieu as TenQuyen from NhanVien n inner join KieuNhanVien as k on n.MaKieu = k.MaKieu";
            return Stuff.GetList<DataNhanVien>(query);
        }

        public static void CreateNewUser(NhanVien newUser, string UserNameNV)
        {
            string query = $"INSERT INTO [NhanVien]([UserName],[Password],[TrangThai],[MaKieu],[HoTen],[SDT],[Email],[NgaySinh],[AnhDaiDien],[GioiTinh],[DiaChi],[QueQuan],[CongTy],[ChucVu],[TieuSu],[NguoiTao],[NgayTao])" +
$"VALUES(@UserName,@Password,@TrangThai,@MaKieu,@HoTen,@SDT,@Email,@NgaySinh,@AnhDaiDien,@GioiTinh,@DiaChi,@QueQuan,@CongTy,@ChucVu,@TieuSu,@NguoiTao,GETDATE())";
            object param = new
            {
                UserName = newUser.UserName,
                Password = newUser.Password,
                TrangThai = newUser.TrangThai,
                MaKieu = newUser.MaKieu,
                HoTen = newUser.HoTen,
                SDT = newUser.SDT,
                Email = newUser.Email,
                NgaySinh = newUser.NgaySinh,
                AnhDaiDien = newUser.AnhDaiDien,
                GioiTinh = newUser.GioiTinh,
                DiaChi = newUser.DiaChi,
                QueQuan = newUser.QueQuan,
                CongTy = newUser.CongTy,
                ChucVu = newUser.ChucVu,
                TieuSu = newUser.TieuSu,
                NguoiTao = UserNameNV,
            };
            Stuff.ExecuteSql(query, param);
        }

        public static void DeleteUserByID(long ID, string UserNameNV)
        {
            using (SqlConnection con = new SqlConnection(ConnectString.Setup()))
            {
                con.Open();
                string stringQuery = $"Update [NhanVien] SET [TrangThai]=10,[NguoiSua]='{UserNameNV}',[NgaySua]=GETDATE() WHERE ID={ID}";
                SqlCommand cmd = new SqlCommand(stringQuery, con);
                cmd.ExecuteNonQuery();
            }
        }

        public static void ChangeStateByID(long ID, int state, string UserNameNV)
        {
            using (SqlConnection con = new SqlConnection(ConnectString.Setup()))
            {
                con.Open();
                string stringQuery = $"Update [NhanVien] SET [TrangThai]={state},[NguoiSua]='{UserNameNV}',[NgaySua]=GETDATE() WHERE ID={ID}";
                SqlCommand cmd = new SqlCommand(stringQuery, con);
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateUser(NhanVien newUser, long userID, string UserNameNV)
        {
            string query = "UPDATE [dbo].[NhanVien] SET [UserName] = @UserName," +
                "[Password] = @Password," +
                "[TrangThai] = @TrangThai," +
                "[MaKieu] = @MaKieu," +
                "[HoTen] = @HoTen," +
                "[SDT] = @SDT," +
                "[Email] =@Email ," +
                "[NgaySinh] = @NgaySinh ," +
                "[AnhDaiDien] = @AnhDaiDien," +
                "[GioiTinh] = @GioiTinh," +
                "[DiaChi] = @DiaChi," +
                "[QueQuan] = @QueQuan," +
                "[CongTy] = @CongTy," +
                "[ChucVu] = @ChucVu," +
                "[TieuSu] = @TieuSu," +
                "[NgaySua] = GETDATE()," +
                "[NguoiSua] = @NguoiSua WHERE ID=@ID";
            object param = new
            {
                UserName = newUser.UserName,
                Password = newUser.Password,
                TrangThai = newUser.TrangThai,
                MaKieu = newUser.MaKieu,
                HoTen = newUser.HoTen,
                SDT = newUser.SDT,
                Email = newUser.Email,
                NgaySinh = newUser.NgaySinh,
                AnhDaiDien = newUser.AnhDaiDien,
                GioiTinh = newUser.GioiTinh,
                DiaChi = newUser.DiaChi,
                QueQuan = newUser.QueQuan,
                CongTy = newUser.CongTy,
                ChucVu = newUser.ChucVu,
                TieuSu = newUser.TieuSu,
                NguoiSua = UserNameNV,
                ID = userID
            };
            Stuff.ExecuteSql(query, param);
        }
        public static List<T> GetListExcel<T>(string PathExcel)
        {
            List<T> account = new List<T>(); 
            using (ExcelPackage package = new ExcelPackage(new FileInfo(PathExcel)))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var sheet = package.Workbook.Worksheets["data"];
                //first row is for knowing the properties of object
                var columnInfo = Enumerable.Range(1, sheet.Dimension.Columns).ToList().Select(n =>

                    new { Index = n, ColumnName = sheet.Cells[1, n].Value.ToString() }
                );

                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    T obj = (T)Activator.CreateInstance(typeof(T));//generic object
                    foreach (var prop in typeof(T).GetProperties())
                    {
                        int col = columnInfo.SingleOrDefault(c => c.ColumnName == prop.Name).Index;
                        var val = sheet.Cells[row, col].Value;
                        if (val != null)
                        {
                            var propType = prop.PropertyType;
                            prop.SetValue(obj, Convert.ChangeType(val, propType));
                        }
                        else
                        {
                        }

                    }
                    account.Add(obj);
                }
            };
                return account;
        }
    }
}