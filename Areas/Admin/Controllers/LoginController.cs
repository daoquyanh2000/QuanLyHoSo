using QuanLyHoSo.Dao;
using QuanLyHoSo.Dao.DaoAdmin;
using QuanLyHoSo.Models;
using System.Web.Mvc;
using System.Web.Security;

namespace QuanLyHoSo.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        // GET: Admin/Login
        [HttpGet]
        public ActionResult Index()
        {
            if(User.Identity.IsAuthenticated && Session["UserNameNV"] !=null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public JsonResult Index(FormCollection fc)
        {
            //kiem tra nguoi dung bam submit chua
            NhanVien UserLogin = new NhanVien();
            UserLogin.UserName = fc["UserName"].ToString();
            UserLogin.Password = Stuff.MD5Hash(fc["Password"].ToString());
            NhanVien User = LoginDao.GetUserByUserNamePassword(UserLogin);
            if (User.ID > 0)
            {
                if (User.TrangThai == 1)
                {
                    Session["HoTenNV"] = User.HoTen;
                    Session["UserNameNV"] = User.UserName;
                    Session["IDNV"] = User.ID;
                    Session["QuyenNV"] = User.MaKieu;
                    FormsAuthentication.SetAuthCookie(User.UserName, false);
                    return Json(new
                    {
                        status = true,
                        icon = "success",
                        heading = "Thành công",
                        message = "Đăng nhập thành công!"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        heading = "Có lỗi",
                        icon = "warning",
                        message = "Tài khoản đăng nhập đã bị khóa!"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new
                {
                    heading = "Có lỗi",
                    icon = "error",
                    message = "Tài khoản không tồn tại hoặc sai thông tin đăng nhập!"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LogOut()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Login");
        }
    }
}