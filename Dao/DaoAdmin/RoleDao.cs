using QuanLyHoSo.Models;
using System.Collections.Generic;

namespace QuanLyHoSo.Dao.DaoAdmin
{
    public class RoleDao
    {
        public static List<KieuNhanVien> GetKieuNhanViens()
        {
            string query = "Select * from KieuNhanVien ORDER BY ID DESC ";
            return Stuff.GetList<KieuNhanVien>(query);
        }

        public static List<KieuNhanVien> GetRoleByID(long ID)
        {
            string query = "Select * from KieuNhanVien WHERE ID =@ID and TrangThai !=10";
            return Stuff.GetList<KieuNhanVien>(query, new { ID = ID });
        }

        public static List<Quyen> GetQuyen()
        {
            string query = "Select * from Quyen ";
            return Stuff.GetList<Quyen>(query);
        }

        public static void CreateNewRole(KieuNhanVien role, string HoTenNV)
        {
            string query = $"Insert into KieuNhanVien(TenKieu,ChuThich,TrangThai,NgayTao,NguoiTao)  VALUES(@TenKieu,@ChuThich,@TrangThai,GETDATE(),@NguoiTao)";
            object param = new { TenKieu = role.TenKieu, ChuThich = role.ChuThich, TrangThai = role.TrangThai, NguoiTao = HoTenNV };
            Stuff.ExecuteSql(query, param);
        }

        public static void UpdateRole(KieuNhanVien role, long IDNV, string UserNameNV)
        {
            string query = "Update KieuNhanVien Set TenKieu=@TenKieu,ChuThich=@ChuThich,TrangThai=@TrangThai,NgaySua=GETDATE(),NguoiSua =@NguoiSua WHERE ID =@ID";
            object param = new { TenKieu = role.TenKieu, ChuThich = role.ChuThich, TrangThai = role.TrangThai, NguoiSua = UserNameNV, ID = IDNV };
            Stuff.ExecuteSql(query, param);
        }

        public static void ChangeStateByID(long ID, int state, string UserNameNV)
        {
            string query = $"Update [KieuNhanVien] SET [TrangThai]={state},[NguoiSua]='{UserNameNV}',[NgaySua]=GETDATE() WHERE ID={ID}";
            Stuff.ExecuteSql(query);
        }

        public static void DeleteUserByID(long ID, string UserNameNV)
        {
            string query = $"Update [KieuNhanVien] SET [TrangThai]=10,[NguoiSua]='{UserNameNV}',[NgaySua]=GETDATE() WHERE ID={ID}";
            Stuff.ExecuteSql(query);
        }

        public static void ThemQuyen(long IDKnv, string IDQuyen)
        {
            string query = $"Insert into KieuNhanVien_Quyen(IDKieuNhanVien,IDQuyen)  VALUES('{IDKnv}','{IDQuyen}')";
            Stuff.ExecuteSql(query);
        }

        public static List<KieuNhanVien_Quyen> GetKieuNhanVien_Quyen()
        {
            string query = "Select * from KieuNhanVien_Quyen ";
            return Stuff.GetList<KieuNhanVien_Quyen>(query);
        }

        public static void DeleteKieuNhanVien_Quyen(long RoleID)
        {
            string query = $"Delete  from KieuNhanVien_Quyen where KieuNhanVien_Quyen.IDKieuNhanVien ={RoleID}";
            Stuff.ExecuteSql(query);
        }
    }
}