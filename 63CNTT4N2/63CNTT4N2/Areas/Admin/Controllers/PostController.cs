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
using _63CNTT4N2.App_Start;
using UDW.Library;
using System.IO;

namespace _63CNTT4N2.Areas.Admin.Controllers
{
    [Role]
    public class PostController : Controller
    {
        PostsDAO postsDAO = new PostsDAO();
        LinksDAO linksDAO = new LinksDAO();
        TopicsDAO topicsDAO = new TopicsDAO();

        public ActionResult Index()
        {
            return View(postsDAO.getList("Index", "Post"));
        }
        public ActionResult Create()
        {
            ViewBag.TopList = new SelectList(topicsDAO.getList("Index"), "ID", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Posts posts)
        {
            if (ModelState.IsValid)
            {
                posts.Slug = XString.Str_Slug(posts.Tittle);

                var img = Request.Files["img"];
                if (img.ContentLength != 0)
                {
                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };

                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))
                    {
                        string slug = posts.Slug;
                        string id = posts.Id.ToString();
                        string imgName = slug + id + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        posts.Img = imgName;

                        string PathDir = "~/Public/img/post/";
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                }
                posts.PostType = "post";
                posts.CreateAt = DateTime.Now;
                posts.CreateBy = Convert.ToInt32(Session["UserId"]);

                if (postsDAO.Insert(posts) == 1)
                {
                    Links links = new Links();
                    links.Slug = posts.Slug;
                    links.TableId = posts.Id;
                    links.Type = "post";
                    linksDAO.Insert(links);
                }
                TempData["message"] = new XMessage("success", "Thêm bài viết thành công");
                return RedirectToAction("Index");
            }
            ViewBag.TopList = new SelectList(topicsDAO.getList("Index"), "ID", "Name");
            return View(posts);
        }

        public ActionResult Status(int? id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index", "Post");
            }

            Posts posts = postsDAO.getRow(id);
            if (posts == null)
            {
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index", "Post");
            }
            posts.Status = (posts.Status == 1) ? 2 : 1;
            posts.UpdateBy = Convert.ToInt32(Session["UserId"].ToString());
            posts.UpdateAt = DateTime.Now;
            postsDAO.Update(posts);
            TempData["message"] = new XMessage("success", "Cập nhật trạng thái thành công");
            return RedirectToAction("Index", "Post");
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Posts posts = postsDAO.getRow(id);
            if (posts == null)
            {
                return HttpNotFound();
            }
            return View(posts);
        }
        public ActionResult Edit(int? id)
        {
            ViewBag.TopList = new SelectList(topicsDAO.getList("Index"), "ID", "Name");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Posts posts = postsDAO.getRow(id);

            if (posts == null)
            {
                return HttpNotFound();
            }

            return View(posts);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Posts posts)
        {
            if (ModelState.IsValid)
            {
                posts.Slug = XString.Str_Slug(posts.Tittle);
                var img = Request.Files["img"];
                if (img.ContentLength != 0)
                {
                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))
                    {
                        string slug = posts.Slug;
                        string id = posts.Id.ToString();
                        string imgName = slug + id + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        posts.Img = imgName;

                        string PathDir = "~/Public/img/post/";
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                }
                posts.UpdateAt = DateTime.Now;
                posts.UpdateBy = Convert.ToInt32(Session["UserId"]);
                if (postsDAO.Update(posts) == 1)
                {
                    Links links = new Links();
                    links.Slug = posts.Slug;
                    links.TableId = posts.Id;
                    links.Type = "post";
                    linksDAO.Insert(links);
                }
                TempData["message"] = new XMessage("success", "Sửa bài viết thành công");
                return RedirectToAction("Index");
            }
            ViewBag.TopList = new SelectList(topicsDAO.getList("Index"), "ID", "Name");
            return View(posts);
        }
        public ActionResult DelTrash(int? id)
        {
            Posts posts = postsDAO.getRow(id);
            posts.Status = 0;
            posts.UpdateBy = Convert.ToInt32(Session["UserId"].ToString());
            posts.UpdateAt = DateTime.Now;
            postsDAO.Update(posts);
            TempData["message"] = new XMessage("success", "Xóa bài viết thành công");
            return RedirectToAction("Index", "Post");
        }
        public ActionResult Trash(int? id)
        {
            return View(postsDAO.getList("Trash"));
        }
        public ActionResult Undo(int? id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index", "Post");
            }

            Posts posts = postsDAO.getRow(id);
            if (posts == null)
            {
                TempData["message"] = new XMessage("danger", "Phục hồi dữ liệu thất bại");
                return RedirectToAction("Index", "Post");
            }
            posts.Status = 2;
            posts.UpdateBy = Convert.ToInt32(Session["UserId"].ToString());
            posts.UpdateAt = DateTime.Now;
            postsDAO.Update(posts);
            TempData["message"] = new XMessage("success", "Phục hồi dữ liệu thành công");
            return RedirectToAction("Trash", "Post");
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Posts posts = postsDAO.getRow(id);
            if (posts == null)
            {
                return HttpNotFound();
            }
            return View(posts);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Posts posts = postsDAO.getRow(id);
            if (postsDAO.Delete(posts) == 1)
            {
                Links links = linksDAO.getRow(posts.Id, "post");
                linksDAO.Delete(links);
                string PathDir = "~/Public/img/post/";
                if (posts.Img != null)
                {
                    string DelPath = Path.Combine(Server.MapPath(PathDir), posts.Img);
                    System.IO.File.Delete(DelPath);
                }
            }
            TempData["message"] = new XMessage("success", "Xóa danh mục thành công");
            return RedirectToAction("Trash");
        }
    }
}
