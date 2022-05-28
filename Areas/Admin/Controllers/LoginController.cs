using QuanLyLogin.Dao.DaoAdmin;
using QuanLyLogin.Models;
using System.Data.SqlClient;
using System.Web.Mvc;
namespace QuanLyLogin.Areas.Admin.Controllers
{

    public class LoginController : Controller
    {

        // GET: Admin/Login
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["UserNameNV"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(NhanVien model)
        {
            //kiem tra nguoi dung bam submit chua
            if (ModelState.IsValid)
            {
                LoginDao dao = new LoginDao();
                var result = dao.ValidateLogin(model);

                if (result > 0)
                {
                    Session["UserNameNV"] = model.Username;
                    Session["IDNV"] = model.Username;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty,"Sai thông tin tài khoản");
                }
            }

            return View();
        }
        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }

    }
}