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
using Microsoft.Ajax.Utilities;
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
            //Medal medal = new Medal();
            //using (MyDbContext db = new MyDbContext())
            //{
            //        medal.Description = "5s";
            //        db.Medals.Add(medal);
            //        db.SaveChanges();
            //        medal.Description = "10s";
            //        db.Medals.Add(medal);
            //        db.SaveChanges();
            //        medal.Description = "20s";
            //        db.Medals.Add(medal);
            //        db.SaveChanges();
            //        medal.Description = "maxkomment";
            //        db.Medals.Add(medal);
            //        db.SaveChanges();
            //        medal.Description = "topsite";
            //        db.Medals.Add(medal);
            //        db.SaveChanges();
            //}
            List<Site> sites;
            sites = new List<Site>();
            foreach (var x in db.Sites.Include(s => s.Pages).Include(s => s.Ratings))
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
            UserRating user;
            using (MyDbContext db = new MyDbContext())
            {
                user = db.Ratings.Include(m => m.Medals).Include(s => s.Sites).Where(p => p.UserName == siteCreator).FirstOrDefault();
                user.Medals = CheckMedal(siteCreator).Medals;
            }
            return View("ShowUser", user);
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

            Page page = site.Pages.FirstOrDefault();
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
            //Page page = site.Pages.Where(p => p.Id == pageId).SingleOrDefault();
            return View("CreatePage", site);
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
            //return Json(new { result = "Redirect", url = Url.Action("FillSite", "Sites", new { userName = site.UserName, id = site.Id }) });
        }

        public ActionResult PageView(string userName, int id, int? pageId)
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
            return View("PageView", page);
        }


        public ActionResult CreateMenu(string userName, int id)
        {
            Site site;
            using (var db = new MyDbContext())
            {
                site = db.Sites
                    .Include(s => s.Pages)
                    .Include(s => s.Menu)
                    .Include(s => s.Menu.TopBar)
                    .Include(s => s.Menu.SideBar)
                    .Where(p => p.Id == id)
                    .SingleOrDefault();
            }
            if (site == null)
            {
                site = new Site();
            }

            return View("CreateMenu", site);
        }

        public ActionResult FillSite(string userName, int id)
        {
            Site site;
            using (var db = new MyDbContext())
            {
                site = db.Sites.Where(p => p.Id == id).SingleOrDefault();
            }
            if (site == null)
            {
                site = new Site();
            }
            return View("FillSite", site);
        }

        public ActionResult Site(string userName, int id, int? pageId)
        {
            Site site;
            using (var db = new MyDbContext())
            {
                site = db.Sites
                    .Include(s => s.Pages)
                    .Include(s => s.Menu)
                    .Include(s => s.Menu.TopBar)
                    .Include(s => s.Menu.SideBar)
                    .Where(p => p.Id == id)
                    .SingleOrDefault();
            }
            if (site == null)
            {
                site = new Site();
            }

            Session.Add("TemplateId",site.TemplateId);
            Session.Add("Menu",site.Menu);
            //if(pageId!=null)
            //        Session.Add("pageId",pageId);

            //else if(site.Pages.Count>0)
            //{
            //    Session.Add("pageId", site.Pages.ElementAt(0).Id);
            //}
            Page page=null;
            if(pageId!=null)
                page = site.Pages.Where(s => s.Id == pageId).FirstOrDefault();
            if(page==null)
                if (site.Pages.Count > 0)
                    page = site.Pages.FirstOrDefault();
                else
                    page=new Page();
            return View("Site", page);
        }

        public ActionResult PageResult(int id, int? pageId)
        {
            Site site;
            using (var db = new MyDbContext())
            {
                site = db.Sites.Where(p => p.Id == id).SingleOrDefault();
            }
            if (site == null)
            {
                site = new Site();
            }
            Page page;
            if (pageId == null)
                page = site.Pages.FirstOrDefault();
            page = site.Pages.Where(p => p.Id == pageId).SingleOrDefault();
            //return View("Site", site);
            return Json(page);
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
                //db.Menus.Add(site.Menu);
                db.SaveChanges();

            }
            //return Json(new { result = "Redirect", url = Url.Action("CreateSite", "Sites", new { userName = site.UserName, id = site.Id }) });
            //return Json(new {result="Redirect", Url=Url.Action("FillSite","Sites",)})
            return Json(new { result = "Redirect", url = Url.Action("FillSite", "Sites", new { userName = site.UserName, id = site.Id }) });
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
        public void SaveMenu(Menu saveMenu)
        {
            Menu menu;
            using (MyDbContext db = new MyDbContext())
            {
                menu = db.Menus.Include(c => c.TopBar).Include(s => s.SideBar).Where(p => p.Id == saveMenu.Id).SingleOrDefault();
                menu.TopBar.Clear();
                menu.SideBar.Clear();
                saveMenu.TopBar.CopyItemsTo(menu.TopBar);
                saveMenu.SideBar.CopyItemsTo(menu.SideBar);
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

        public ActionResult Comments(string userName, int id)
        {
            Site site;
            using (MyDbContext db = new MyDbContext())
            {
                site = db.Sites
                    .Include(s => s.Comments)
                    .Include(s => s.Ratings)
                    .Where(p => p.Id == id)
                    .SingleOrDefault(); 
            }
           // Comment comment = site.Comments.Where(p => p.Id == id).SingleOrDefault();
            return View("Comments", site);
        }

        [HttpPost]
        public void AddComment(int Id, string UserName, string Comment)
        {
            Comment comment = new Comment();
            Site site;
            using (MyDbContext db = new MyDbContext())
            {
                site = db.Sites
                    .Include(s => s.Comments)
                    .Where(p => p.Id == Id)
                    .SingleOrDefault();
                comment.Date = DateTime.Now;
                comment.UserComment = Comment;
                comment.UserName = UserName;
                comment.Site = site;
                db.Comments.Add(comment);
                db.SaveChanges();
            }
        }

        bool result;
        [Authorize]
        public void AddRating(int id, string rating)
        {
            string user = User.Identity.GetUserName();
            int position = user.IndexOf("@");
            user = user.Remove(position);
            Site site;
            UserRating num = new UserRating();
            using (MyDbContext db = new MyDbContext())
            {
                site = db.Sites
                    .Include(s => s.Ratings)
                    .Where(p => p.Id == id)
                    .SingleOrDefault();
                foreach (var x in site.Ratings)
                {
                    if (x.UserName == user) { result = true; break; }
                }
                if (result == false)
                {
                    site.RatingCount++;
                    site.AverageRating = Math.Round((site.AverageRating*(site.RatingCount-1) + Int32.Parse(rating)) / site.RatingCount);
                    num.UserName = user;
                    num.Star = Int32.Parse(rating);
                    num.Sites.Add(site);
                    db.Ratings.Add(num);
                    db.SaveChanges();
                }
            }
         }

        public UserRating CheckMedal (string userName)
        {
            int count = 0;
            bool countMedal = false;
            Site site;
            UserRating userMedal;
            using (MyDbContext db = new MyDbContext())
            {
                userMedal = db.Ratings.Include(s => s.Medals).Where(p => p.UserName == userName).FirstOrDefault();
                foreach (var x in db.Sites)
                {
                    if (x.UserName == userName) count++;
                }
                if (count >= 5)
                {
                    foreach (var x in userMedal.Medals)
                    {
                        if (x.Description == "5") countMedal = true;
                    }
                    if (countMedal == false)
                    {
                        Medal medal = new Medal()
                        { Description = "5" };
                        userMedal.Medals.Add(medal);
                        db.SaveChanges();
                    }
                }
                if (count >= 10)
                {
                    foreach (var x in userMedal.Medals)
                    {
                        if (x.Description == "10") countMedal = true;
                    }
                    if (countMedal == false)
                    {
                        Medal medal = new Medal()
                        { Description = "10" };
                        userMedal.Medals.Add(medal);
                        db.SaveChanges();
                    }
                }
                if (count >= 20)
                {
                    foreach (var x in userMedal.Medals)
                    {
                        if (x.Description == "20") countMedal = true;
                    }
                    if (countMedal == false)
                    {
                        Medal medal = new Medal()
                        { Description = "20" };
                        userMedal.Medals.Add(medal);
                        db.SaveChanges();
                    }
                }
            }
            return userMedal;
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

