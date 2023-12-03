using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyClass.Model;
using MyClass.DAO;
using UDW.Library;
using _63CNTT4N2.Library;
using _63CNTT4N2.App_Start;

namespace _63CNTT4N2.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        UsersDAO usersDAO = new UsersDAO();

        public ActionResult Index()
        {
            return View(usersDAO.getList("Index"));
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Không tìm thấy thấy tài khoản");
                return RedirectToAction("Index");
            }
            Users users = usersDAO.getRow(id);
            if (users == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Không tìm thấy thấy tài khoản");
            }
            return View(users);
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Users users)
        {
            if (ModelState.IsValid)
            {
                //Xử lý một sô trường tự động
                //CreateAt
                users.CreateAt = DateTime.Now;
                //UpdateAt
                users.UpdateAt = DateTime.Now;
                //CreateBy
                users.CreateBy = Convert.ToInt32(Session["UserID"]);
                //UpdateBy
                users.UpdateBy = Convert.ToInt32(Session["UserID"]);

                //Thêm mới dòng dữ liệu
                usersDAO.Insert(users);
                //thong bao
                TempData["message"] = new XMessage("success", "Đăng kí tài khoản thành công");

                return RedirectToAction("Index");

            }
            return View(users);
        }
        public ActionResult Status(int? id)
        {
            if (id == null)
            {
                //thong bao
                TempData["message"] = new XMessage("danger", "Thay đổi status thất bại");
                return RedirectToAction("Index");
            }
            Users users = usersDAO.getRow(id);
            if (users == null)
            {
                //thong bao
                TempData["message"] = new XMessage("danger", "Thay đổi status thất bại");
                return RedirectToAction("Index");
            }
            //cap nhat
            //UpdateAt
            users.UpdateAt = DateTime.Now;
            //UpdateBy
            users.UpdateBy = Convert.ToInt32(Session["UserID"]);
            //Status
            users.Status = (users.Status == 1) ? 2 : 1;
            //Update DB
            usersDAO.Update(users);
            //thong bao
            TempData["message"] = new XMessage("success", "Thay đổi status thành công");
            return RedirectToAction("Index");
        }
        // GET: Admin/Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            Users users = usersDAO.getRow(id);
            if (users == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
                //return HttpNotFound();
            }
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Users users)
        {
            if (ModelState.IsValid)
            {
                //UpdateAt
                users.UpdateAt = DateTime.Now;
                //UpdateBy
                users.UpdateBy = Convert.ToInt32(Session["UserID"]);
                //cap nhat DB
                usersDAO.Update(users);
                //hien thi thong bao thanh cong
                TempData["message"] = new XMessage("success", "Cập nhật tài khoản thành công");
                return RedirectToAction("Index");
            }
            return View(users);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                //thong bao
                TempData["message"] = new XMessage("danger", "Không tìm thấy tài khoản để xóa");
                return RedirectToAction("Index");
            }
            Users users = usersDAO.getRow(id);
            if (users == null)
            {
                //thong bao
                TempData["message"] = new XMessage("danger", "Không tìm thấy tài khoản để xóa");
                return RedirectToAction("Index");
            }
            return View(users);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Users users = usersDAO.getRow(id);
            //xóa 1 dòng
            usersDAO.Delete(users);
            //hien thi thong bao
            TempData["message"] = new XMessage("success", "Xóa tài khoản thành công");
            return RedirectToAction("Trash");
        }
        public ActionResult DelTrash(int? id)
        {
            if (id == null)
            {
                //thong bao
                TempData["message"] = new XMessage("danger", "Không tìm thấy tài khoản");
                return RedirectToAction("Index");
            }
            Users users = usersDAO.getRow(id);
            if (users == null)
            {
                //thong bao
                TempData["message"] = new XMessage("danger", "Không tìm thấy tài khoản");
                return RedirectToAction("Index");
            }
            //cap nhat
            //UpdateAt
            users.UpdateAt = DateTime.Now;
            //UpdateBy
            users.UpdateBy = Convert.ToInt32(Session["UserID"]);
            //Status
            users.Status = 0;
            //Update DB
            usersDAO.Update(users);
            //thong bao
            TempData["message"] = new XMessage("success", "Xóa tài khoản thành công");
            return RedirectToAction("Index");
        }
        // TRASH
        public ActionResult Trash()
        {
            return View(usersDAO.getList("Trash"));//status = 0
        }
        public ActionResult Undo(int? id)
        {
            if (id == null)
            {
                //thong bao
                TempData["message"] = new XMessage("danger", "Phục hồi tài khoản thất bại");
                return RedirectToAction("Index");
            }
            Users users = usersDAO.getRow(id);
            if (users == null)
            {
                //thong bao
                TempData["message"] = new XMessage("danger", "Phục hồi tài khoản thất bại");
                return RedirectToAction("Index");
            }
            //cap nhat trang thai status = 2
            users.Status = 2;
            //cap nhạt Update At
            users.UpdateAt = DateTime.Now;
            //cap nhat Update By
            users.UpdateBy = Convert.ToInt32(Session["UserID"]);
            //Update DB
            usersDAO.Update(users);
            //hien thi thong bao
            TempData["message"] = new XMessage("success", "Phục hồi tài khoản thành công");
            //tro ve trang Index
            return RedirectToAction("Index");
        }
    }
}

