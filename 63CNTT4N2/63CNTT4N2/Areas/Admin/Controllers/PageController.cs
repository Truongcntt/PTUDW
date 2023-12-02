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
    public class PageController : Controller
    {
        PostsDAO postsDAO = new PostsDAO();
        LinksDAO linksDAO = new LinksDAO();

        public ActionResult Index()
        {
            return View(postsDAO.getList("Index", "Page"));
        }

        public ActionResult Create()
        {
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

                        string PathDir = "~/Public/img/page/";
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                }
                posts.PostType = "page";
                posts.CreateAt = DateTime.Now;
                posts.CreateBy = Convert.ToInt32(Session["UserId"]);
                if (postsDAO.Insert(posts) == 1)
                {
                    Links links = new Links();
                    links.Slug = posts.Slug;
                    links.TableId = posts.Id;
                    links.Type = "page";
                    linksDAO.Insert(links);
                }
                TempData["message"] = new XMessage("success", "Thêm trang đơn thành công");
                return RedirectToAction("Index");
            }
            return View(posts);
        }
        public ActionResult Status(int? id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index", "Page");
            }

            Posts posts = postsDAO.getRow(id);
            if (posts == null)
            {
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index", "Page");
            }
            posts.Status = (posts.Status == 1) ? 2 : 1;
            posts.UpdateBy = Convert.ToInt32(Session["UserId"].ToString());
            posts.UpdateAt = DateTime.Now;
            postsDAO.Update(posts);
            TempData["message"] = new XMessage("success", "Cập nhật trạng thái thành công");
            return RedirectToAction("Index", "Page");
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

                        string PathDir = "~/Public/img/page/";
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
                    links.Type = "page";
                    linksDAO.Insert(links);
                }
                TempData["message"] = new XMessage("success", "Sửa trang đơn thành công");
                return RedirectToAction("Index");
            }
            return View(posts);
        }
        public ActionResult DelTrash(int? id)
        {
            Posts posts = postsDAO.getRow(id);
            posts.Status = 0;
            posts.UpdateBy = Convert.ToInt32(Session["UserId"].ToString());
            posts.UpdateAt = DateTime.Now;
            postsDAO.Update(posts);
            TempData["message"] = new XMessage("success", "Xóa trang đơn thành công");
            return RedirectToAction("Index", "Page");
        }
        public ActionResult Trash(int? id)
        {
            return View(postsDAO.getList("Trash", "page"));
        }      
        public ActionResult Undo(int? id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index", "Page");
            }
            Posts posts = postsDAO.getRow(id);
            if (posts == null)
            {
                TempData["message"] = new XMessage("danger", "Phục hồi dữ liệu thất bại");
                return RedirectToAction("Index", "Page");
            }
            posts.Status = 2;
            posts.UpdateBy = Convert.ToInt32(Session["UserId"].ToString());
            posts.UpdateAt = DateTime.Now;
            postsDAO.Update(posts);
            TempData["message"] = new XMessage("success", "Phục hồi dữ liệu thành công");
            return RedirectToAction("Trash", "Page");
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
                Links links = linksDAO.getRow(posts.Id, "page");
                linksDAO.Delete(links);
                string PathDir = "~/Public/img/page/";
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