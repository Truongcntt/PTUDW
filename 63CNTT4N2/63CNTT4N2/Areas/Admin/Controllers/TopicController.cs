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
    public class TopicController : Controller
    {
        TopicsDAO topicsDAO = new TopicsDAO();
        LinksDAO linksDAO = new LinksDAO();
        public ActionResult Index()
        {
            return View(topicsDAO.getList("Index"));//hien thi toan bo danh sach loai SP
        }
        public ActionResult Create()
        {
            ViewBag.ListTopic = new SelectList(topicsDAO.getList("Index"), "ID", "Name");
            ViewBag.OrderTopic = new SelectList(topicsDAO.getList("Index"), "Order", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Topics topics)
        {
            if (ModelState.IsValid)
            {
                topics.Slug = XString.Str_Slug(topics.Name);
                if (topics.ParenId == null)
                {
                    topics.ParenId = 0;
                }                
                if (topics.Order == null)
                {
                    topics.Order = 1;
                }
                else
                {
                    topics.Order = topics.Order + 1;
                }
                topics.CreateAt = DateTime.Now;
                topics.CreateBy = Convert.ToInt32(Session["UserId"]);
                if (topicsDAO.Insert(topics) == 1)
                {
                    Links links = new Links();
                    links.Slug = topics.Slug;
                    links.TableId = topics.Id;
                    links.Type = "topic";
                    linksDAO.Insert(links);
                }
                TempData["message"] = new XMessage("success", "Thêm chủ đề thành công");
                return RedirectToAction("Index");
            }
            ViewBag.ListTopic = new SelectList(topicsDAO.getList("Index"), "ID", "Name");
            ViewBag.OrderTopic = new SelectList(topicsDAO.getList("Index"), "Order", "Name");
            return View(topics);
        }
        public ActionResult Status(int? id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Cập nhật chủ đề thất bại");
                return RedirectToAction("Index", "Topic");
            }
            Topics topics = topicsDAO.getRow(id);
            if (topics == null)
            {
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index", "Topic");
            }
            topics.Status = (topics.Status == 1) ? 2 : 1;
            topics.UpdateBy = Convert.ToInt32(Session["UserId"].ToString());
            topics.UpdateAt = DateTime.Now;
            topicsDAO.Update(topics);
            TempData["message"] = new XMessage("success", "Cập nhật trạng thái thành công");
            return RedirectToAction("Index", "Topic");
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Topics topics = topicsDAO.getRow(id);
            if (topics == null)
            {
                return HttpNotFound();
            }
            return View(topics);
        }
        public ActionResult Edit(int? id)
        {
            ViewBag.ListTopic = new SelectList(topicsDAO.getList("Index"), "ID", "Name");
            ViewBag.OrderTopic = new SelectList(topicsDAO.getList("Index"), "Order", "Name");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Topics topics = topicsDAO.getRow(id);

            if (topics == null)
            {
                return HttpNotFound();
            }

            return View(topics);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Topics topics)
        {
            if (ModelState.IsValid)
            {
                topics.Slug = XString.Str_Slug(topics.Name);
                if (topics.ParenId == null)
                {
                    topics.ParenId = 0;
                }

                if (topics.Order == null)
                {
                    topics.Order = 1;
                }
                else
                {
                    topics.Order = topics.Order + 1;
                }
                topics.UpdateAt = DateTime.Now;
                topics.UpdateBy = Convert.ToInt32(Session["UserId"]);
                TempData["message"] = new XMessage("success", "Sửa danh mục thành công");
                if (topicsDAO.Update(topics) == 1)
                {
                    Links links = linksDAO.getRow(topics.Id, "topic");
                    links.Slug = topics.Slug;
                    linksDAO.Update(links);
                }

                return RedirectToAction("Index");
            }
            return View(topics);
        }
        public ActionResult DelTrash(int? id)
        {
            Topics topics = topicsDAO.getRow(id);
            topics.Status = 0;
            topics.UpdateBy = Convert.ToInt32(Session["UserId"].ToString());
            topics.UpdateAt = DateTime.Now;
            topicsDAO.Update(topics);
            TempData["message"] = new XMessage("success", "Xóa mẩu tin thành công");
            return RedirectToAction("Index", "Topic");
        }
        public ActionResult Trash(int? id)
        {
            return View(topicsDAO.getList("Trash"));
        }
        public ActionResult Undo(int? id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index", "Topic");
            }
            Topics topics = topicsDAO.getRow(id);
            if (topics == null)
            {
                TempData["message"] = new XMessage("danger", "Phục hồi dữ liệu thất bại");
                return RedirectToAction("Index", "Topic");
            }
            topics.Status = 2;
            topics.UpdateBy = Convert.ToInt32(Session["UserId"].ToString());
            topics.UpdateAt = DateTime.Now;
            topicsDAO.Update(topics);
            TempData["message"] = new XMessage("success", "Phục hồi dữ liệu thành công");
            return RedirectToAction("Trash", "Topic");
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Xóa mẫu tin thất bại!");
                return RedirectToAction("Trash");
            }
            Topics topics = topicsDAO.getRow(id);
            if (topics == null)
            {
                TempData["message"] = new XMessage("danger", "Xóa mẫu tin thất bại!");
                return RedirectToAction("Trash");
            }
            return View(topics);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Topics topics = topicsDAO.getRow(id);
            if (topicsDAO.Delete(topics) == 1)
            {
                Links links = linksDAO.getRow(topics.Id, "topic");
                linksDAO.Delete(links);
            }
            TempData["message"] = new XMessage("success", "Xóa chủ đề thành công");
            return RedirectToAction("Trash");
        }
    }
}
