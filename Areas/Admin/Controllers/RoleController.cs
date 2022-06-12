using PagedList;
using QuanLyHoSo.Dao.DaoAdmin;
using QuanLyHoSo.Models;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace QuanLyHoSo.Areas.Admin.Controllers
{
    public class RoleController : Controller
    {
        // GET: Admin/Role
        public ActionResult Index()
        {
            ViewBag.listQuyen = RoleDao.GetQuyen();

            return View();
        }

        public PartialViewResult Search(string keyword, int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSizeNumber = 3;
            if (keyword == null) keyword = "";
            var results = from nv in RoleDao.GetKieuNhanViens()
                          where nv.TenKieu.Contains(keyword) && nv.TrangThai != 10
                          select nv;
            ViewBag.search = keyword;
            var model = results.ToPagedList(pageNumber, pageSizeNumber);
            return PartialView("_RoleTable", model);
        }

        [HttpPost]
        public JsonResult Save(FormCollection fc)
        {
            string UserNameNV = Session["UserNameNV"].ToString();
            var role = new KieuNhanVien();
            role.ID = Convert.ToInt32(fc["ID"]);
            role.TenKieu = fc["TenKieu"].ToString();
            role.ChuThich = fc["ChuThich"].ToString();
            role.TrangThai = Convert.ToByte(fc["TrangThai"]);
            var checkedID = (fc["checkedID"]).Split(',').ToList();

            if (role.ID == 0)
            {
                RoleDao.CreateNewRole(role, UserNameNV);
                var IDKnv = RoleDao.GetKieuNhanViens().First().ID;
                foreach (var item in checkedID)
                {
                    RoleDao.ThemQuyen(IDKnv, item);
                }

                return Json(new
                {
                    heading = "Thành công",
                    status = "success",
                    message = "Tạo tài khoản mới thành công!"
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //xoa het quyen cu va them quyen moi vao
                RoleDao.DeleteKieuNhanVien_Quyen(role.ID);
                foreach (var item in checkedID)
                {
                    RoleDao.ThemQuyen(role.ID, item);
                }
                RoleDao.UpdateRole(role, role.ID, UserNameNV);
                return Json(new
                {
                    heading = "Thành công",
                    status = "success",
                    message = "Sửa tài khoản thành công!"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Delete(long ID)
        {
            RoleDao.DeleteUserByID(ID, Session["UserNameNV"].ToString());
            return Json(new
            {
                heading = "Thành công",
                status = "success",
                message = "Xóa tài khoản thành công!"
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult View(long ID)
        {
            var role = RoleDao.GetRoleByID(ID);
            var result = from k in RoleDao.GetKieuNhanVien_Quyen()
                         join q in RoleDao.GetQuyen()
                         on k.IDQuyen equals q.ID
                         where k.IDKieuNhanVien == ID
                         select q.ID;

            var IDKnv = RoleDao.GetKieuNhanViens().First().ID;
            return Json(new
            {
                data = role.FirstOrDefault(),
                checkbox = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Change(long ID, int state)
        {
            RoleDao.ChangeStateByID(ID, state, Session["UserNameNV"].ToString());
            return Json(new
            {
                heading = "Thành công",
                status = "success",
                message = "Thay đổi trạng thái thành công!"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}