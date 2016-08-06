using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sitemaker;
using Sitemaker.Models;
using CloudinaryDotNet;
using Sitemaker.Filters;

namespace Sitemaker.Controllers
{
    [Culture]
    public class SitesController : Controller
    {
        private MyDbContext db = new MyDbContext();

        // GET: Sites
        public ActionResult Index()
        {
            return View(db.Sites.ToList());
        }

        // GET: Sites/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Site site = db.Sites.Find(id);
            if (site == null)
            {
                return HttpNotFound();
            }
            return View(site);
        }

        // GET: Sites/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,Logo,Name,About,TemplateId,MenuId")] Site site)
        {
            if (ModelState.IsValid)
            {
                db.Sites.Add(site);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(site);
        }

        // GET: Sites/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Site site = db.Sites.Find(id);
            if (site == null)
            {
                return HttpNotFound();
            }
            return View(site);
        }

        // POST: Sites/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,Logo,Name,About,TemplateId,MenuId")] Site site)
        {
            if (ModelState.IsValid)
            {
                db.Entry(site).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(site);
        }

        // GET: Sites/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Site site = db.Sites.Find(id);
            if (site == null)
            {
                return HttpNotFound();
            }
            return View(site);
        }

        // POST: Sites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Site site = db.Sites.Find(id);
            db.Sites.Remove(site);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult CreateSite(int? id)
        {
            // TODO check is author.
            Site site;
            using (var db = new MyDbContext())
            {
                site = db.Sites
                    .Include(s => s.Pages)
                    .Where(p => p.Id == id)
                    .SingleOrDefault();
            }
            if (site == null)
            {
                site = new Site();
            }
            return View("CreateSite", site);
        }


        public string Upload(string data)
        {
            Account account = new Account("dgy6x5krf", "949232162798767", "oxvzYd03K1i8lEIi5MA2ByIf590");
            Cloudinary cloudinary = new Cloudinary(account);
            CloudinaryDotNet.Actions.ImageUploadParams uploadParams = new CloudinaryDotNet.Actions.ImageUploadParams()
            {
                File = new CloudinaryDotNet.Actions.FileDescription(data)
            };
            CloudinaryDotNet.Actions.ImageUploadResult uploadResult = cloudinary.Upload(uploadParams);
            return cloudinary.Api.UrlImgUp.BuildUrl(String.Format("{0}.{1}", uploadResult.PublicId, uploadResult.Format));
        }

        public ActionResult CreatePage1(int id)
        {
            Site site;
            using (var db = new MyDbContext())
            {
                site = db.Sites.Where(p => p.Id == id).SingleOrDefault();
                if (site != null)
                {
                    Page page = new Page();
                    site.Pages.Add(page);
                    db.SaveChanges();
                    return RedirectToAction("CreatePage", new { id = id, pageId = page.Id });
                }
            }
            throw new Exception("idi naxyi pidr");
        }

        public ActionResult CreatePage(int id, int? pageId)
        {
            if (pageId == null)
            {
                return RedirectToAction("CreateSite", new { id = id });
            }
            Site site;
            using (var db = new MyDbContext())
            {
                site = db.Sites
                    .Include(s => s.Pages)
                    .Where(p => p.Id == id)
                    .SingleOrDefault();
            }
            if (site == null)
            {
                throw new Exception("idi naxyi pidr");
            }
            Page page = site.Pages.Where(p => p.Id == pageId).SingleOrDefault();
            return View("CreatePage", page);
        }

        public ActionResult ShowPage(int id, int? pageId)
        {
            if (pageId == null)
            {
                return RedirectToAction("CreateSite", new { id = id });
            }
            Site site;
            using (var db = new MyDbContext())
            {
                site = db.Sites
                    .Include(s => s.Pages)
                    .Where(p => p.Id == id)
                    .SingleOrDefault();
            }
            if (site == null)
            {
                throw new Exception("idi naxyi pidr");
            }
            Page page = site.Pages.Where(p => p.Id == pageId).SingleOrDefault();
            return View("Page", page);
        }

        public ActionResult Page(int id, int? pageId)
        {
            Site site;
            using (var db = new MyDbContext())
            {
                site = db.Sites
                    .Include(s => s.Pages)
                    .Where(p => p.Id == id)
                    .SingleOrDefault();
            }
            Page page = site.Pages.Where(p => p.Id == pageId).FirstOrDefault();
            return RedirectToAction("ShowPage", new { id = id, pageId = page.Id });
        }

        [HttpPost]
        public ActionResult SaveSite(Site site)
        {
            using (MyDbContext db = new MyDbContext())
            {
                site.Logo = Upload(site.Logo);
                db.Sites.Add(site);
                db.SaveChanges();
            }
            return Json(new { result = "Redirect", url = Url.Action("CreateSite", "Sites", new { id = site.Id }) });
        }

        [HttpPost]
        public ActionResult LoadTemplate(string id)
        {
            return PartialView("template/" + id);
        }

        [HttpPost]
        public void SavePage(Page savePage)
        {
            Page page;
            using (MyDbContext db = new MyDbContext())
            {
                page = db.Pages.Where(p => p.Id == savePage.Id).SingleOrDefault();
                if (page.HtmlCode != null)
                {
                    page.HtmlCode = null;
                }
                page.HtmlCode = savePage.HtmlCode.Replace("<textarea ", "<textarea disabled ");
                db.SaveChanges();
            }
        }

        [HttpPost]
        public ActionResult LoadMenuEditor()
        {
            return PartialView("template/MenuEditor");
        }

        public ActionResult ChangeCulture(string lang)
        {
            string returnUrl = Request.UrlReferrer.AbsolutePath;
            List<string> cultures = new List<string>() { "ru", "en", "de" };
            if (!cultures.Contains(lang))
            {
                lang = "ru";
            }
            HttpCookie cookie = Request.Cookies["lang"];
            if (cookie != null)
                cookie.Value = lang;
            else
            {
                cookie = new HttpCookie("lang");
                cookie.HttpOnly = false;
                cookie.Value = lang;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);
            return Redirect(returnUrl);
        }
    }

}

