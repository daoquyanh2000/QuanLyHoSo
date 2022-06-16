using Dapper;
using Dapper.Contrib.Extensions;
using OfficeOpenXml;
using QuanLyHoSo.Models;
using QuanLyHoSo.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyHoSo.Dao
{
    public class Stuff
    {


        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits
                //for each byte
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }


        public static void ExecuteSql(string query, object param = null)
        {
            using (SqlConnection con = new SqlConnection(ConnectString.Setup()))
            {
                con.Open();

                con.Execute(query, param);
            }
        }

        public static List<T> GetList<T>(string query, object param = null)
        {
            using (SqlConnection con = new SqlConnection(ConnectString.Setup()))
            {
                con.Open();
                var list = con.Query<T>(query, param).ToList();
                return list;
            }
        }
        public static List<T> GetAll<T>() where T : class
        {
            List<T> list = new List<T>();
            using (var con = new SqlConnection(ConnectString.Setup()))
            {
                list = con.GetAll<T>().ToList();
            }
            return list;
        }

    }
}