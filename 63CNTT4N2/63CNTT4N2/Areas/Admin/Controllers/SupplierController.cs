﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _63CNTT4N2.App_Start;
using _63CNTT4N2.Library;
using MyClass.DAO;
using MyClass.Model;
using UDW.Library;

namespace _63CNTT4N2.Areas.Admin.Controllers
{
    [Role]
    public class SupplierController : Controller
    {
        SuppliersDAO suppliersDAO = new SuppliersDAO();
        ////////////////////////////////////////////////////////////////////
        // GET: Admin/Supplier/Index
        public ActionResult Index()
        {
            return View(suppliersDAO.getList("Index"));
        }
        ////////////////////////////////////////////////////////////////////
        // GET: Admin/Supplier/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Không tìm thấy nhà cung cấp");
                return RedirectToAction("Index");
            }
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Không tìm thấy nhà cung cấp");
                return RedirectToAction("Index");
            }
            return View(suppliers);
        }
        ////////////////////////////////////////////////////////////////////
        // GET: Admin/Supplier/Create
        public ActionResult Create()
        {
            ViewBag.OrderList = new SelectList(suppliersDAO.getList("Index"), "Order", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Suppliers suppliers)
        {
            if (ModelState.IsValid)
            {
                //xu ly tu dong mot so truong
                //-----CreateAt
                suppliers.CreateAt = DateTime.Now;
                //-----CreateBy
                suppliers.CreateBy = Convert.ToInt32(Session["UserID"]);
                //UpdateAt
                suppliers.UpdateAt = DateTime.Now;
                //UpdateBy
                suppliers.UpdateBy = Convert.ToInt32(Session["UserID"]);
                //slug
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
                
                //xu ly cho phan upload hình ảnh
                var img = Request.Files["img"];//lay thong tin file
                if (img.ContentLength != 0)
                {
                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    //kiem tra tap tin co hay khong
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))//lay phan mo rong cua tap tin
                    {
                        string slug = suppliers.Slug;
                        //ten file = Slug + phan mo rong cua tap tin
                        string imgName = slug + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        suppliers.Image = imgName;
                        //upload hinh
                        string PathDir = "~/Public/img/supplier/";
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                }//ket thuc phan upload hinh anh

                //them moi mau tin vao DB
                suppliersDAO.Insert(suppliers);
                //hien thi thong bao thanh cong
                TempData["message"] = new XMessage("success", "Tạo mới nhà cung cấp thành công");
                return RedirectToAction("Index");
            }
            ViewBag.OrderList = new SelectList(suppliersDAO.getList("Index"), "Order", "Name");
            return View(suppliers);
        }

        ////////////////////////////////////////////////////////////////////
        // GET: Admin/Supplier/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Không tìm thấy nhà cung cấp");
                return RedirectToAction("Index");
            }
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Không tìm thấy nhà cung cấp");
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
                //xu ly tu dong mot so truong

                //Order
                if (suppliers.Order == null)
                {
                    suppliers.Order = 1;
                }
                else
                {
                    suppliers.Order += 1;
                }
                //UpdateAt
                suppliers.UpdateAt = DateTime.Now;
                //UpdateBy
                suppliers.UpdateBy = Convert.ToInt32(Session["UserID"]);

                //slug
                suppliers.Slug = XString.Str_Slug(suppliers.Name);

                //trước khi cập nhật lại ảnh mới thì xóa ảnh cũ
                var img = Request.Files["img"];//lay thong tin file
                string PathDir = "~/Public/img/supplier/";
                if (img.ContentLength != 0)//ton tai 1 logo của NCC từ trước
                {
                    if (suppliers.Image != null)
                    {
                        string DelPath = Path.Combine(Server.MapPath(PathDir),suppliers.Image);
                        System.IO.File.Delete(DelPath);
                    }

                    //upload ảnh mới nhà cung cấp
                    //xử lý cho phần upload ảnh 
                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    //kiem tra tap tin co hay khong
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))//lay phan mo rong cua tap tin
                    {
                        string slug = suppliers.Slug;
                        //ten file = Slug + phan mo rong cua tap tin
                        string imgName = slug +  img.FileName.Substring(img.FileName.LastIndexOf("."));
                        suppliers.Image = imgName;
                        //upload hinh
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                }//ket thuc phan upload hinh anh

                //cap nhat mau tin vao DB
                suppliersDAO.Update(suppliers);
                //hien thi thong bao thanh cong
                TempData["message"] = new XMessage("success", "Cập nhật thông tin thành công");
                return RedirectToAction("Index");
            }
            ViewBag.OrderList = new SelectList(suppliersDAO.getList("Index"), "Order", "Name");
            return View(suppliers);
        }

        ////////////////////////////////////////////////////////////////////
        // GET: Admin/Supplier/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Không tìm thấy nhà cung cấp");
                return RedirectToAction("Index");
            }
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Không tìm thấy nhà cung cấp");
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
            if (suppliersDAO.Delete(suppliers) == 1)
            {
                string PathDir = "~/Public/img/supplier/";
                if (suppliers.Image != null)//ton tai 1 logo của NCC từ trước
                {
                    // xóa ảnh cũ
                        string DelPath = Path.Combine(Server.MapPath(PathDir), suppliers.Image);
                        System.IO.File.Delete(DelPath);
                }
             }
            //hien thi thong bao thanh cong
            TempData["message"] = new XMessage("success", "Cập nhật thông tin thành công");
            return RedirectToAction("Trash");
        }

        ////////////////////////////////////////////////////////////////////
        // GET: Admin/Supplier/Status/5
        public ActionResult Status(int? id)
        {
            if (id == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }
            //cap nhat trang thai
            suppliers.Status = (suppliers.Status == 1) ? 2 : 1;
            //cap nhạt Update At
            suppliers.UpdateAt = DateTime.Now;
            //cap nhat Update By
            suppliers.UpdateBy = Convert.ToInt32(Session["UserID"]);
            //Update DB
            suppliersDAO.Update(suppliers);
            //hien thi thong bao
            TempData["message"] = new XMessage("success", "Cập nhật trạng thái thành công");
            //tro ve trang Index
            return RedirectToAction("Index");
        }

        ////////////////////////////////////////////////////////////////////
        // GET: Admin/Supplier/DelTrash/5
        public ActionResult DelTrash(int? id)
        {
            if (id == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Xóa mẩu tin thất bại");
                return RedirectToAction("Index");
            }
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Xóa mẩu tin thất bại");
                return RedirectToAction("Index");
            }
            //cap nhat trang thai
            suppliers.Status = 0;
            //cap nhạt Update At
            suppliers.UpdateAt = DateTime.Now;
            //cap nhat Update By
            suppliers.UpdateBy = Convert.ToInt32(Session["UserID"]);
            //Update DB
            suppliersDAO.Update(suppliers);
            //hien thi thong bao
            TempData["message"] = new XMessage("success", "Xóa mẩu tin thành công");
            //tro ve trang Index
            return RedirectToAction("Index");
        }

        ////////////////////////////////////////////////////////////////////
        // GET: Admin/Supplier/Trash = luc thung rac
        public ActionResult Trash()
        {
            return View(suppliersDAO.getList("Trash"));
        }

        ////////////////////////////////////////////////////////////////////
        // GET: Admin/Supplier/Undo/5
        public ActionResult Undo(int? id)
        {
            if (id == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Phục hồi mẩu tin thất bại");
                return RedirectToAction("Index");
            }
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                //hien thi thong bao
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
            return RedirectToAction("Trash");//o lai thung rac de tiep tuc Undo
        }
    }
}
