using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyHoSo.Dao
{
    public class Stuff
    {
        public static DataTable ExcelToDataTable(string path, string UserNameNV)
        {
            DataTable dt = new DataTable();
            using (ExcelPackage package = new ExcelPackage(new FileInfo(path)))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                worksheet.Cells[1, 17].Value = "NgayTao";
                worksheet.Cells[1, 18].Value = "NguoiTao";
                worksheet.Cells[1, 19].Value = "NgaySua";
                worksheet.Cells[1, 20].Value = "NguoiSua";
                // Đọc tất cả các header
                foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    dt.Columns.Add(new DataColumn(firstRowCell.Text, typeof(string)));
                }

                for (var rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                {
                    //ngay tao
                    worksheet.Cells[rowNumber, 17].Value = DateTime.Now;
                    //nguoi tao
                    worksheet.Cells[rowNumber, 18].Value = UserNameNV;
                    // Lấy 1 row trong excel để truy vấn
                    var row = worksheet.Cells[rowNumber, 1, rowNumber, worksheet.Dimension.End.Column];
                    // tạo 1 row trong data table
                    var newRow = dt.NewRow();
                    foreach (var cell in row)
                    {
                        newRow[cell.Start.Column - 1] = cell.GetValue<string>();
                    }
                    dt.Rows.Add(newRow);
                }
            }
            //delete all duplicate user name
            //delete all empty row
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                dt.Rows[i][2] = MD5Hash(dt.Rows[i][2].ToString());
                for (int j = 1; j <= 7; j++)
                {
                    if (dt.Rows[i][j] == DBNull.Value)
                    {
                        dt.Rows[i].Delete();
                        break;
                    }
                }
            }
            dt.AcceptChanges();
            return dt;
        }

        public static List<T> DataTableToList<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        public static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
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

        public static int DataTableToDb(DataTable dt,string TenBang)
        {
            using (SqlConnection con = new SqlConnection(ConnectString.Setup()))
            {
                con.Open();
                //create object of SqlBulkCopy which help to insert  
                SqlBulkCopy objbulk = new SqlBulkCopy(con);
                objbulk.DestinationTableName = TenBang;
                //assign Destination table name  
                //insert bulk Records into DataBase.  
                objbulk.WriteToServer(dt);
                return dt.Rows.Count;
            }
        }
        public static DataTable DbToDataTable(string TenBang)
        {
            DataTable dt = new DataTable();
            string connection = ConnectString.Setup();
            SqlConnection con = new SqlConnection(connection);
            SqlDataAdapter da = new SqlDataAdapter($"select * from {TenBang}", con);
            da.Fill(dt);
            return dt;
        }
    }
}