using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _63CNTT4N2.Library;
using MyClass.DAO;
using MyClass.Model;
using UDW.Library;

namespace _63CNTT4N2.Areas.Admin.Controllers
{
    public class SupplierController : Controller
    {
        SuppliersDAO suppliersDAO = new SuppliersDAO();

        /// /////////////////////////////////////////////////////////////////////
        // GET: Admin/Supplier/Index
        public ActionResult Index()
        {
            return View(suppliersDAO.getList("Index"));
        }
        /// /////////////////////////////////////////////////////////////////////
        // GET: Admin/Supplier/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                //hien thong bao loi
                TempData["message"] = new XMessage("danger", "Không tìm thấy mẫu tin");
                return RedirectToAction("Index");
            }
            //tim kiem mau tin bang id
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                TempData["message"] = new XMessage("danger", "Không tìm thấy mẫu tin");
                return RedirectToAction("Index");
            }
            return View(suppliers);
        }
        /// /////////////////////////////////////////////////////////////////////
        // GET: Admin/Supplier/Create
        public ActionResult Create(Suppliers suppliers)
        {
            if (ModelState.IsValid)
            {
                //Xử lý 
                //CreateAt
                suppliers.CreateAt = DateTime.Now;
                //UpdateAt
                suppliers.UpdateAt = DateTime.Now;
                //CreateBy
                suppliers.CreateBy = Convert.ToInt32(Session["UserID"]);
                //UpdateBy
                suppliers.UpdateBy = Convert.ToInt32(Session["UserID"]);
                //Slug
                suppliers.Slug = XString.Str_Slug(suppliers.Name);

                //Order
                if (suppliers.Order == null)
                {
                    suppliers.Order = 1;
                }
                else
                {
                    suppliers.Order += 1;
                }
                //db.Suppliers.Add(suppliers);
                //db.SaveChanges();
                suppliersDAO.Insert(suppliers);
                return RedirectToAction("Index");
            }
            ViewBag.OrderList = new SelectList(suppliersDAO.getList("Index"), "Order", "Name");
            return View(suppliers);
        }

        // GET: Admin/Supplier/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Không tìm thấy mẩu tin");
                return RedirectToAction("Index");
            }
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                TempData["message"] = new XMessage("danger", "Không tìm thấy mẩu tin");
                return RedirectToAction("Index");
            }
            ViewBag.OrderList = new SelectList(suppliersDAO.getList("Index"), "Order", "Name");
            return View(suppliers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Suppliers suppliers)
        {
            if (ModelState.IsValid)
            {
                //Xử lý 
                //không có ngày tạo
                //CreateBy
                suppliers.UpdateBy = Convert.ToInt32(Session["UserID"]);
                //UpdateBy
                suppliers.UpdateAt = DateTime.Now; ;
                //Slug
                suppliers.Slug = XString.Str_Slug(suppliers.Name);

                //Order
                if (suppliers.Order == null)
                {
                    suppliers.Order = 1;
                }
                else
                {
                    suppliers.Order += 1;
                }
                //db.Suppliers.Add(suppliers);
                //db.SaveChanges();
                suppliersDAO.Insert(suppliers);
                TempData["message"] = new XMessage("danger", "Thêm mới nhà cung cấp thành công");
                return RedirectToAction("Index");

            }
            // ViewBag.OrderList = new SelectList(suppliersDAO.getList("Index"), "Order", "Name");
            return View(suppliers);
        }

        // GET: Admin/Supplier/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Không tìm thấy mẩu tin");
                return RedirectToAction("Index");
            }
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                TempData["message"] = new XMessage("danger", "Không tìm thấy mẩu tin");
                return RedirectToAction("Index");
            }
            return View(suppliers);
        }

        // POST: Admin/Supplier/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Suppliers suppliers = suppliersDAO.getRow(id);
            suppliersDAO.Delete(suppliers);

            return RedirectToAction("Index");
        }
        public ActionResult DelTrash(int? id)
        {
            if (id == null)
            {
                //thong bao
                TempData["message"] = new XMessage("danger", "Không tìm thấy mẫu tin");
                return RedirectToAction("Index");
            }
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                //thong bao
                TempData["message"] = new XMessage("danger", "Không tìm thấy mẫu tin");
                return RedirectToAction("Index");
            }
            //cap nhat
            //UpdateAt
            suppliers.UpdateAt = DateTime.Now;
            //UpdateBy
            suppliers.UpdateBy = Convert.ToInt32(Session["UserID"]);
            //Status
            suppliers.Status = 0;
            //Update DB
            suppliersDAO.Update(suppliers);
            //thong bao
            TempData["message"] = new XMessage("success", "Xóa mẫu tin thành công");
            return RedirectToAction("Index");
        }
        // TRASH
        public ActionResult Trash()
        {
            return View(suppliersDAO.getList("Trash"));//status = 0
        }
        public ActionResult Undo(int? id)
        {
            if (id == null)
            {
                //thong bao
                TempData["message"] = new XMessage("danger", "Phục hồi mẩu tin thất bại");
                return RedirectToAction("Index");
            }
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                //thong bao
                TempData["message"] = new XMessage("danger", "Phục hồi mẩu tin thất bại");
                return RedirectToAction("Index");
            }
            //cap nhat trang thai status = 2
            suppliers.Status = 2;
            //cap nhạt Update At
            suppliers.UpdateAt = DateTime.Now;
            //cap nhat Update By
            suppliers.UpdateBy = Convert.ToInt32(Session["UserID"]);
            //Update DB
            suppliersDAO.Update(suppliers);
            //hien thi thong bao
            TempData["message"] = new XMessage("success", "Phục hồi mẩu tin thành công");
            //tro ve trang Index
            return RedirectToAction("Index");
        }
    }
}
