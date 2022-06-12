﻿using QuanLyHoSo.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

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

        public static NhanVien GetUserByID(long ID)
        {
            NhanVien User = new NhanVien();
            using (SqlConnection con = new SqlConnection(ConnectString.Setup()))
            {
                con.Open();
                string stringQuery = $"select * FROM [NhanVien] WHERE ID ={ID} and TrangThai != 10";
                SqlCommand cmd = new SqlCommand(stringQuery, con);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    User.ID = Convert.ToInt64(dr["ID"]);
                    User.HoTen = dr["HoTen"].ToString();
                    User.UserName = dr["UserName"].ToString();
                    User.Password = dr["Password"].ToString();
                    User.Quyen = Convert.ToByte(dr["Quyen"]);
                    User.TrangThai = Convert.ToByte(dr["TrangThai"]);
                    User.NgaySinh = dr["NgaySinh"].ToString();
                    User.AnhDaiDien = dr["AnhDaiDien"].ToString();
                    User.SDT = dr["SDT"].ToString();
                    User.Email = dr["Email"].ToString();
                    User.GioiTinh = Convert.ToByte(dr["GioiTinh"] ?? "0");
                    User.DiaChi = dr["DiaChi"].ToString();
                    User.QueQuan = dr["QueQuan"].ToString();
                    User.CongTy = dr["CongTy"].ToString();
                    User.ChucVu = dr["ChucVu"].ToString();
                    User.TieuSu = dr["TieuSu"].ToString();
                }
            }
            return User;
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
            string query = "select n.*, k.TenKieu as TenQuyen from NhanVien n inner join KieuNhanVien as k on n.Quyen = k.ID";
            return Stuff.GetList<DataNhanVien>(query);
        }

        public static void CreateNewUser(NhanVien newUser, string UserNameNV)
        {
            string query = $"INSERT INTO [NhanVien]([UserName],[Password],[TrangThai],[Quyen],[HoTen],[SDT],[Email],[NgaySinh],[AnhDaiDien],[GioiTinh],[DiaChi],[QueQuan],[CongTy],[ChucVu],[TieuSu],[NguoiTao],[NgayTao])" +
$"VALUES(@UserName,@Password,@TrangThai,@Quyen,@HoTen,@SDT,@Email,@NgaySinh,@AnhDaiDien,@GioiTinh,@DiaChi,@QueQuan,@CongTy,@ChucVu,@TieuSu,@NguoiTao,GETDATE())";
            object param = new
            {
                UserName = newUser.UserName,
                Password = newUser.Password,
                TrangThai = newUser.TrangThai,
                Quyen = newUser.Quyen,
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
                "[Quyen] = @Quyen," +
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
                Quyen = newUser.Quyen,
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
    }
}