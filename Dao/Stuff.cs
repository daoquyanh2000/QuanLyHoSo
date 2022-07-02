using Dapper;
using Dapper.Contrib.Extensions;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyHoSo.Dao
{
    public class Stuff
    {
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

        public static T GetByID<T>(long ID) where T : class
        {
            using (var con = new SqlConnection(ConnectString.Setup()))
            {
                return con.Get<T>(ID);
            }
        }
        
    }
}