using QuanLyHoSo.Dao.DaoAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace QuanLyHoSo.App_Start
{
    public class CustomRoleProvider : RoleProvider
    {
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            var account = from nv in UserDao.GetAllUser()
                          join knv in RoleDao.GetKieuNhanViens()
                          on nv.Quyen equals knv.ID
                          join knv_q in RoleDao.GetKieuNhanVien_Quyen()
                          on knv.ID equals knv_q.IDKieuNhanVien
                          join q in RoleDao.GetQuyen()
                          on knv_q.IDQuyen equals q.ID
                          where nv.UserName == username && 
                          knv.TrangThai ==1
                          select q.MaQuyen;
            return account.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}