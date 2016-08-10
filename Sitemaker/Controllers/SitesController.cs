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
using Microsoft.AspNet.Identity;
using PagedList.Mvc;
using PagedList;

namespace Sitemaker.Controllers
{
    [Culture]
    public class SitesController : Controller
    {
        private MyDbContext db = new MyDbContext();

        // GET: Sites
        public ActionResult Index(int? page)
        {
            List<Site> sites;
            sites = new List<Site>();
            foreach (var x in db.Sites.Include(s => s.Pages))
            {
                if (x.Pablish == true)
                {
                    sites.Add(x);
                }
                }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            string userName = User.Identity.GetUserName();
            if (userName != "")
            {
                int position = userName.IndexOf("@");
                userName = userName.Remove(position);
            }
            else { userName = ""; }
            Session["CurrentUserName"] = userName;
            return View(sites.ToPagedList(pageNumber, pageSize));
        }

        // GET: Sites/Details/5
        public ActionResult Details(string userName, int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Site site = db.Sites
                    .Include(s => s.Pages)
                    .Where(p => p.Id == id)
                    .SingleOrDefault(); 
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
        public ActionResult Create([Bind(Include = "Id,UserName,Logo,Name,About,TemplateId,MenuId")] Site site)
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
        public ActionResult Edit(string userName, int? id)
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
        public ActionResult Edit([Bind(Include = "Id,UserName,Logo,Name,About,TemplateId,MenuId")] Site site)
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
        public ActionResult Delete(string userName, int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Site site = db.Sites
                    .Include(s => s.Pages)
                    .Where(p => p.Id == id)
                    .SingleOrDefault();
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
            Site site = db.Sites
                   .Include(s => s.Pages)
                   .Where(p => p.Id == id)
                   .SingleOrDefault();
            db.Sites.Remove(site);
            db.SaveChanges();
            return RedirectToAction("ShowMySite");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Authorize]
        public ActionResult ShowMySite(string userName)
        {
            string Name = User.Identity.GetUserName();
            int position = Name.IndexOf("@");
            Name = Name.Remove(position);
            Session["CurrentUserName"] = Name;
            return View("ShowMySite", db.Sites.Where(p => p.UserName == Name).ToList());
        }

        public ActionResult ShowUser(string userName, string siteCreator)
        {
            string Name = User.Identity.GetUserName();
            int position = Name.IndexOf("@");
            Name = Name.Remove(position);
            Session["CurrentUserName"] = Name;
            return View("ShowUser", db.Sites.Where(p => p.UserName == siteCreator).ToList());
        }

        public ActionResult OpenSite(string userName, int id)
        {
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
            
            Page page = site.Pages.Where(p => p.Site.Id == id).FirstOrDefault();
            return View("OpenSite", page);
        }


        [Authorize]
        public ActionResult CreateSite(string userName, int? id )
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

        public ActionResult CreatePage1(string userName, int id)
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
                    return RedirectToAction("CreatePage", new { userName = site.UserName , id = id, pageId = page.Id });
                }
            }
            throw new Exception("idi naxyi pidr");
        }

        public ActionResult CreatePage(string userName, int id, int? pageId)
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

        public ActionResult ShowPage(string userName, int id, int? pageId)
        {
            string Name = User.Identity.GetUserName();
            int position = Name.IndexOf("@");
            Name = Name.Remove(position);
            Session["CurrentUserName"] = Name;
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

        public ActionResult Page(string userName, int id, int? pageId)
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
            return RedirectToAction("ShowPage", new { userName = site.UserName, id = id, pageId = page.Id });
        }

        [HttpPost]
        public ActionResult SaveSite(Site site)
        {
            using (MyDbContext db = new MyDbContext())
            {
                string user = User.Identity.GetUserName();
                int position = user.IndexOf("@");
                user = user.Remove(position);
                site.UserName = user;
                site.Logo = Upload(site.Logo);
                site.Date = DateTime.Now;
                db.Sites.Add(site);
                db.SaveChanges();
            }
            return Json(new { result = "Redirect", url = Url.Action("CreateSite", "Sites", new { userName = site.UserName, id = site.Id  }) });
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
                page.HtmlCode = savePage.HtmlCode;
                db.SaveChanges();
            }
        }

        [HttpPost]
        public ActionResult LoadMenuEditor()
        {
            return PartialView("template/MenuEditor");
        }

        public ActionResult PublishSite(string userName, int id)
        {
            Site site;
            using (MyDbContext db = new MyDbContext())
            {
                site = db.Sites
                    .Include(s => s.Pages)
                    .Where(p => p.Id == id)
                    .SingleOrDefault(); ;
                site.Pablish = true;
                site.Date = DateTime.Now;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        //public ActionResult PublishSite(string userName, int id)
        //{
        //    Site site;
        //    using (MyDbContext db = new MyDbContext())
        //    {
        //        site = db.Sites
        //            .Include(s => s.Pages)
        //            .Where(p => p.Id == id)
        //            .SingleOrDefault(); ;
        //        site.Pablish = true;
        //        site.Date = DateTime.Now;
        //    }
        //    return View("Index", site);
        //}

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

