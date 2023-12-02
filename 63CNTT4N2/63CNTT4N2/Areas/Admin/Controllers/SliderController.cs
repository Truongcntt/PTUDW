using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyClass.DAO;
using MyClass.Model;
using _63CNTT4N2.App_Start;
using _63CNTT4N2.Library;
using UDW.Library;

namespace _63CNTT4N2.Areas.Admin.Controllers
{
    [Role]
    public class SliderController : Controller
    {
        SlidersDAO slidersDAO = new SlidersDAO();
        public ActionResult Index()
        {
            return View(slidersDAO.getList("Index"));//hien thi toan bo danh sach Slider
        }
        public ActionResult Create()
        {
            ViewBag.OrderList = new SelectList(slidersDAO.getList("Index"), "Order", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Sliders sliders)
        {
            if (ModelState.IsValid)
            {
                //Xu ly cho muc Order
                if (sliders.Order == null)
                {
                    sliders.Order = 1;
                }
                else
                {
                    sliders.Order = sliders.Order + 1;
                }

                string slug = XString.Str_Slug(sliders.Name);

                var img = Request.Files["img"];
                if (img.ContentLength != 0)
                {
                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))
                    {
                        string imgName = slug + sliders.Id + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        sliders.Img = imgName;
                        string PathDir = "~/Public/img/slider/";
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                }
                sliders.CreateAt = DateTime.Now;
                sliders.CreateBy = Convert.ToInt32(Session["UserId"]);
                slidersDAO.Insert(sliders);
                TempData["message"] = new XMessage("success", "Thêm danh mục thành công");
                return RedirectToAction("Index");
            }
            ViewBag.OrderList = new SelectList(slidersDAO.getList("Index"), "Order", "Name");
            return View(sliders);
        }
        public ActionResult Status(int? id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index", "Slider");
            }
            Sliders sliders = slidersDAO.getRow(id);
            if (sliders == null)
            {
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index", "Slider");
            }
            sliders.Status = (sliders.Status == 1) ? 2 : 1;
            sliders.UpdateBy = Convert.ToInt32(Session["UserId"].ToString());
            sliders.UpdateAt = DateTime.Now;
            slidersDAO.Update(sliders);

            TempData["message"] = new XMessage("success", "Cập nhật trạng thái thành công");
            return RedirectToAction("Index", "Slider");
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sliders sliders = slidersDAO.getRow(id);
            if (sliders == null)
            {
                return HttpNotFound();
            }
            return View(sliders);
        }
        public ActionResult Edit(int? id)
        {
            ViewBag.OrderList = new SelectList(slidersDAO.getList("Index"), "Order", "Name");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sliders sliders = slidersDAO.getRow(id);
            if (sliders == null)
            {
                return HttpNotFound();
            }
            return View(sliders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Sliders sliders)
        {
            if (ModelState.IsValid)
            {
                string slug = XString.Str_Slug(sliders.Name);
                if (sliders.Order == null)
                {
                    sliders.Order = 1;
                }
                else
                {
                    sliders.Order = sliders.Order + 1;
                }

                var img = Request.Files["img"];
                if (img.ContentLength != 0)
                {
                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))
                    {
                        string imgName = slug + sliders.Id + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        sliders.Img = imgName;
                        string PathDir = "~/Public/img/slider/";
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        if (sliders.Img != null)
                        {
                            string DelPath = Path.Combine(Server.MapPath(PathDir), sliders.Img);
                            System.IO.File.Delete(DelPath);
                        }

                        img.SaveAs(PathFile);
                    }
                }
                sliders.UpdateAt = DateTime.Now;
                sliders.UpdateBy = Convert.ToInt32(Session["UserId"]);

                slidersDAO.Update(sliders);
                TempData["message"] = new XMessage("success", "Sửa danh mục thành công");
                return RedirectToAction("Index");
            }
            return View(sliders);
        }
        public ActionResult DelTrash(int? id)
        {
            Sliders sliders = slidersDAO.getRow(id);
            sliders.Status = 0;
            sliders.UpdateBy = Convert.ToInt32(Session["UserId"].ToString());
            sliders.UpdateAt = DateTime.Now;
            slidersDAO.Update(sliders);
            TempData["message"] = new XMessage("success", "Xóa mẩu tin thành công");
            return RedirectToAction("Index", "Slider");
        }
        public ActionResult Trash(int? id)
        {
            return View(slidersDAO.getList("Trash"));
        }
        public ActionResult Undo(int? id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Phục hồi danh mục thất bại");
                return RedirectToAction("Index", "Slider");
            }
            Sliders sliders = slidersDAO.getRow(id);
            if (sliders == null)
            {
                TempData["message"] = new XMessage("danger", "Phục hồi danh mục thất bại");
                return RedirectToAction("Index", "Page");
            }
            sliders.Status = 2;
            sliders.UpdateBy = Convert.ToInt32(Session["UserId"].ToString());
            sliders.UpdateAt = DateTime.Now;
            slidersDAO.Update(sliders);
            TempData["message"] = new XMessage("success", "Phục hồi danh mục thành công");
            return RedirectToAction("Trash", "Slider");
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sliders sliders = slidersDAO.getRow(id);

            if (sliders == null)
            {
                return HttpNotFound();
            }
            return View(sliders);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sliders sliders = slidersDAO.getRow(id);

            if (slidersDAO.Delete(sliders) == 1)
            {
                string PathDir = "~/Public/img/slider/";
                if (sliders.Img != null)
                {
                    string DelPath = Path.Combine(Server.MapPath(PathDir), sliders.Img);
                    System.IO.File.Delete(DelPath);
                }
            }
            TempData["message"] = new XMessage("success", "Xóa danh mục thành công");
            return RedirectToAction("Trash");
        }
    }
}
