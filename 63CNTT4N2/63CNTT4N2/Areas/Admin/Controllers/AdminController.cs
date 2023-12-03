using _63CNTT4N2.App_Start;
using MyClass.DAO;
using MyClass.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _63CNTT4N2.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        UsersDAO usersDAO = new UsersDAO();
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection field)
        {
            string username = field["Username"];
            string password = field["Password"];

            Users user = usersDAO.GetUserByUsername(username);

            if (user != null && user.Password == password)
            {
                SessionConfig.SetUser(user);
                Session["UserID"] = user.Id;
                Session["Username"] = user.Username;
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View("DangNhap");
            }
        }
        public ActionResult Dangxuat()
        {
            SessionConfig.SetUser(null);
            return RedirectToAction("Dangnhap", "Admin");
        }
    }
}