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

namespace _63CNTT4N2.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        CategoriesDAO categoriesDAO = new CategoriesDAO();
       
        /// ///////////////////////////////////////////////////
        // GET: Admin/Category
        public ActionResult Index()
        {
            return View(categoriesDAO.getList("Index"));
        }

        // GET: Admin/Category/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                return HttpNotFound();
            }
            return View(categories);
        }

        // GET: Admin/Category/Create
        public ActionResult Create()
        {
            ViewBag.CatList = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");
            ViewBag.OrderList = new SelectList(categoriesDAO.getList("Index"), "Order", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Categories categories)
        {
            if (ModelState.IsValid)
            {
                //Xử lý một sô trường tự động
                //CreateAt
                categories.CreateAt=DateTime.Now;
                //UpdateAt
                categories.UpdateAt= DateTime.Now;
                //CreateBy
                categories.CreateBy = Convert.ToInt32(Session["UserID"]);
                //UpdateBy
                categories.UpdateBy = Convert.ToInt32(Session["UserID"]);
                //Slug
                categories.Slug = XString.Str_Slug(categories.Name);
                //ParentId
                if (categories.ParenId == null)
                {
                    categories.ParenId = 0;
                }
                //Oder
                if (categories.Order == null)
                {
                    categories.Order = 1;
                }
                else
                {
                    categories.Order  +=1;
                }
                //Thêm mới dòng dữ liệu
                categoriesDAO.Insert(categories);
                //thong bao
                TempData["message"] = new XMessage ("success","Thêm mới mẫu tin thành công");

                return RedirectToAction("Index");

            }
            ViewBag.CatList = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");
            ViewBag.OrderList = new SelectList(categoriesDAO.getList("Index"), "Order", "Name");
            return View(categories);
        }
        // GET: Admin/Category/Status/5
        public ActionResult Status(int? id)
        {
            if (id == null)
            {
                //thong bao
                TempData["message"] = new XMessage("danger", "Thay đổi status thất bại");
                return RedirectToAction("Index");
            }
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                //thong bao
                TempData["message"] = new XMessage("danger", "Thay đổi status thất bại");
                return RedirectToAction("Index");
            }
            //cap nhat
            //UpdateAt
             categories.UpdateAt= DateTime.Now;
            //UpdateBy
            categories.UpdateBy = Convert.ToInt32(Session["UserID"]);
            //Status
            categories.Status = (categories.Status == 1) ? 2 : 1;
            //Update DB
            categoriesDAO.Update(categories);
            //thong bao
            TempData["message"] = new XMessage("success", "Thay đổi status thành công");
            return RedirectToAction("Index");
        }
        // GET: Admin/Category/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.CatList = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");
            ViewBag.OrderList = new SelectList(categoriesDAO.getList("Index"), "Order", "Name");
            if (id == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
                //return HttpNotFound();
            }
            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Categories categories)
        {
            if (ModelState.IsValid)
            {
                //xu ly tu dong cho cac truong sau:
                //slug
                categories.Slug = XString.Str_Slug(categories.Name);
                //ParentID
                if (categories.ParenId == null)
                {
                    categories.ParenId = 0;
                }
                //Order
                if (categories.Order == null)
                {
                    categories.Order = 1;
                }
                else
                {
                    categories.Order += 1;
                }
                //UpdateAt
                categories.UpdateAt = DateTime.Now;
                //UpdateBy
                categories.UpdateBy = Convert.ToInt32(Session["UserID"]);
                //cap nhat DB
                categoriesDAO.Update(categories);
                //hien thi thong bao thanh cong
                TempData["message"] = new XMessage("success", "Cập nhật thông tin thành công");
                return RedirectToAction("Index");
            }
            ViewBag.CatList = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");
            ViewBag.OrderList = new SelectList(categoriesDAO.getList("Index"), "Order", "Name");
            return View(categories);
        }
        ///// ///////////////////////////////////////////////////
        //// GET: Admin/Category/Delete/5
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Categories categories = await db.Categories.FindAsync(id);
        //    if (categories == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(categories);
        //}

        //// POST: Admin/Category/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    Categories categories = await db.Categories.FindAsync(id);
        //    db.Categories.Remove(categories);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}
    }
}
