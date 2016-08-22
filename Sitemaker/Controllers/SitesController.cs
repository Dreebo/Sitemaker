using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sitemaker.Models;
using CloudinaryDotNet;
using Microsoft.Ajax.Utilities;
using Sitemaker.Filters;
using Microsoft.AspNet.Identity;
using PagedList;
using MvcLuceneSampleApp.Search;
using CloudinaryDotNet.Actions;

namespace Sitemaker.Controllers
{
    [Culture]
    public class SitesController : Controller
    {
        private MyDbContext db = new MyDbContext();

        // GET: Sites
        [AllowAnonymous]
        public ActionResult Index(int? page)
        {   
            List<Site> sites = db.Sites.Include(s => s.Ratings).Where(x => x.Pablish).ToList();
            sites.Reverse();
            List<string> tags = db.Tags.Select(name => name.Name).ToList();
            LuceneSearch.ClearLuceneIndex();
            LuceneSearch.AddUpdateLuceneIndex(db.Sites.Include("Comments").Include("Tags"));
            Session["CurrentUserName"] = GetUserName(User.Identity.GetUserName());
            Session.Add("Author", false);
            ViewBag.Tags = tags;
            return View(sites.ToPagedList((page ?? 1), 9));
        }

        private static string GetUserName(string userName)
        {
            if (!"".Equals(userName))
            {
                int position = userName.IndexOf("@");
                userName = userName.Remove(position);
            }
            return userName;
        }

        // GET: Sites/Details/5
        public ActionResult Details(string userName, int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Site site = db.Sites.Include(p => p.Pages).SingleOrDefault(p => p.Id == id); 
            if (site == null)
                return HttpNotFound();
            if (User.IsInRole("admin") || User.Identity.GetUserId().Equals(site.CreaterId))
            {
                Session.Add("Author", true);
                return View(site);
            }
                return HttpNotFound();
        }

        // GET: Sites/Delete/5
        
        public ActionResult Delete(string userName, int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Site site = db.Sites
                    .Include(s => s.Pages)
                    .Include(s => s.Comments)
                    .Include(s => s.Medals)
                    .Include(s => s.Tags)
                    .SingleOrDefault(p => p.Id == id);
            if (site == null)
                return HttpNotFound();
            if (User.IsInRole("admin") || User.Identity.GetUserId().Equals(site.CreaterId))
                return View(site);
            return HttpNotFound();
        }

        // POST: Sites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Site site = db.Sites
                   .Include(s => s.Pages)
                   .Include(s => s.Comments)
                    .Include(s => s.Medals)
                    .Include(s => s.Tags)
                   .SingleOrDefault(p => p.Id == id);
            if (User.IsInRole("admin") || User.Identity.GetUserId().Equals(site.CreaterId))
            {
                db.Sites.Remove(site);
                db.SaveChanges();
                return RedirectToAction("ShowMySite", new { userName= site.UserName});
            }
            return HttpNotFound();
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
            if (User.IsInRole("admin"))
                return View("ShowMySite", db.Sites.Where(p => p.UserName.Equals(userName)).ToList());
            return View("ShowMySite", db.Sites.Where(p =>p.UserName.Equals(userName)).ToList());
        }
        
        [HttpPost]
        public void SaveChangeSite(Site site)
        {
            if (User.IsInRole("admin") || User.Identity.GetUserId().Equals(site.CreaterId))
            {
                Site currentSite;
                using (MyDbContext db = new MyDbContext())
                {
                    currentSite = db.Sites.FirstOrDefault(p => p.Id == site.Id);
                    currentSite.Name = site.Name;
                    currentSite.About = site.About;
                    currentSite.Logo = Upload(site.Logo);
                    currentSite.Date = DateTime.Now;
                    db.SaveChanges();
                }
            }
        }

        [AllowAnonymous]
        public ActionResult ShowUser(string userName, string siteCreator)
        {
            //Session["CurrentUserName"] = GetUserName(User.Identity.GetUserName());
            UserRating user;
            List<Site> sites;
            
            using (MyDbContext db = new MyDbContext())
            {
                sites =db.Sites
                        .Include(s => s.Ratings)
                        .Where(p => p.UserName.Equals(siteCreator)&& p.Pablish).ToList();
                sites.Reverse();
                user = db.Ratings.Include(m => m.Medals).Include(m => m.Sites).FirstOrDefault(p => p.UserName.Equals(siteCreator));
                UserRating userRating = CheckMedal(siteCreator);
                user.Medals = userRating != null ? CheckMedal(siteCreator).Medals:user.Medals;
                user.Sites = sites;
            }
            return View("ShowUser", user);
        }


        [Authorize]
        public ActionResult CreateSite(string userName, int? id )
        {
            // TODO check is author.
            Site site;
            using (var db = new MyDbContext())
            {
                site = db.Sites.SingleOrDefault(p => p.Id == id);
            }
            if (site == null)
                site = new Site(); 
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

        public ActionResult LoadPage(string userName, int id)
        {
                Site site;
            using (var db = new MyDbContext())
            {
                site = db.Sites.SingleOrDefault(p => p.Id == id);
                if (site != null)
                {
                    if (User.IsInRole("admin") || User.Identity.GetUserId().Equals(site.CreaterId))
                    {
                        Page page = new Page();
                        site.Pages.Add(page);
                        db.SaveChanges();
                        return RedirectToAction("CreatePage", new {userName = site.UserName, id = id, pageId = page.Id});
                    }
                }
            }
            throw new Exception("idi naxyi pidr");
        }

        public ActionResult CreatePage(string userName, int id, int? pageId)
        {
            if (pageId == null)
                return RedirectToAction("CreateSite", new { id = id });
            Site site;
            using (var db = new MyDbContext())
            {
                site = db.Sites
                    .Include(s => s.Pages)
                    .SingleOrDefault(p => p.Id == id);
            }
            if (site == null)
            {
                throw new Exception("idi naxyi pidr");
            }
            if (User.IsInRole("admin") || User.Identity.GetUserId().Equals(site.CreaterId))
            {
                Session.Add("TemplateId", site.TemplateId);
                Page page = site.Pages.SingleOrDefault(p => p.Id == pageId);
                return View("CreatePage", page);
            }
            return HttpNotFound();
        }

        

        public ActionResult PageView(string userName, int id, int? pageId)
        {
            Page page;
            using (var db = new MyDbContext())
            {
                page = db.Sites
                    .Include(s => s.Pages)
                    .SingleOrDefault(p => p.Id == id).Pages.FirstOrDefault(p => p.Id == pageId);
            }
            return View("PageView", page);
        }

        [Authorize]
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
                    .SingleOrDefault(p => p.Id == id);
            }
            if (site == null)
                site = new Site();
            if (User.IsInRole("admin") || User.Identity.GetUserId().Equals(site.CreaterId))
                return View("CreateMenu", site);
            return HttpNotFound();
        }

        [AllowAnonymous]
        public ActionResult FillSite(string userName, int id)
        {
            Site site;
            using (var db = new MyDbContext())
            {
                site = db.Sites.Include(p => p.Pages).SingleOrDefault(p => p.Id == id);
            }
            if (site == null)
                site = new Site();
            if (User.IsInRole("admin") || User.Identity.GetUserId().Equals(site.CreaterId))
                return View("FillSite", site);
            return HttpNotFound();
        }

        [AllowAnonymous]
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
                    .SingleOrDefault(p => p.Id == id);
            }
            if (site == null)
                site = new Site();
                Page page = pageId != null ? site.Pages.FirstOrDefault(s => s.Id == pageId) : null;
                if (page == null)
                    page = site.Pages.Count > 0 ? site.Pages.FirstOrDefault() : new Page();
                Session.Add("TemplateId", site.TemplateId);
                Session.Add("Menu", site.Menu);             
                return View("Site", page);
        }

        [AllowAnonymous]
        public ActionResult PageResult(int id, int? pageId)
        {
            Site site;
            Page page;
            using (var db = new MyDbContext())
            {
                site = db.Sites.SingleOrDefault(p => p.Id == id);
                page = pageId == null ? site.Pages.FirstOrDefault() : site.Pages.SingleOrDefault(p => p.Id == pageId);
            }
            return Json(page);
        }



        [HttpPost]
        public ActionResult SaveSite(Site site, string[] tagsArray)
        {
            using (MyDbContext db = new MyDbContext())
            {
                List<Tag> tags = new List<Tag>();
                foreach (var x in tagsArray)
                {
                    var temp = db.Tags.FirstOrDefault(t => t.Name.Equals(x));
                    if (temp == null)
                    {
                        temp = new Tag();
                        temp.Name = x;
                        db.Tags.Add(temp);
                    }
                    tags.Add(temp);        
                }
                site.UserName = GetUserName(User.Identity.GetUserName());
                site.Logo = Upload(site.Logo);
                site.Date = DateTime.Now;
                site.CreaterId = User.Identity.GetUserId();
                site.Tags = tags;
                db.Sites.Add(site);
                db.SaveChanges();
            }
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

            using (MyDbContext db = new MyDbContext())
            {
                db.Pages.SingleOrDefault(p => p.Id == savePage.Id).HtmlCode = savePage.HtmlCode;
                db.SaveChanges();
            }
        }

        [HttpPost]
        public void SaveMenu(Menu saveMenu)
        {
            Menu menu;
            using (MyDbContext db = new MyDbContext())
            {
                menu = db.Menus.Include(c => c.TopBar).Include(s => s.SideBar).SingleOrDefault(p => p.Id == saveMenu.Id);
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
                site = db.Sites.SingleOrDefault(p => p.Id == id);
                if (User.IsInRole("admin") || User.Identity.GetUserId().Equals(site.CreaterId))
                {
                    site.Pablish = true;
                    site.Date = DateTime.Now;
                    db.SaveChanges();
                }
                else return HttpNotFound();
            }
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public ActionResult Comments(string userName, int id)
        {
            Site site;
            using (MyDbContext db = new MyDbContext())
            {
                site = db.Sites
                    .Include(s => s.Comments)
                    .Include(s => s.Ratings)
                    .Include(s => s.Tags)
                    .SingleOrDefault(p => p.Id == id); 
            }
            return View("Comments", site);
        }

        [HttpPost]
        public void AddComment(int Id, string UserName, string Comment)
        {
            Comment comment = new Comment();
            using (MyDbContext db = new MyDbContext())
            {
                comment.Date = DateTime.Now;
                comment.UserComment = Comment;
                comment.UserName = GetUserName(User.Identity.GetUserName());
                comment.Site = db.Sites
                    .Include(s => s.Comments)
                    .SingleOrDefault(p => p.Id == Id); 
                db.Comments.Add(comment);
                db.SaveChanges();
            }
        }

        
        [Authorize]
        public void AddRating(int id, string rating)
        {
            string user = GetUserName(User.Identity.GetUserName());
            Site site;
            UserRating num = new UserRating();
            using (MyDbContext db = new MyDbContext())
            {
                bool result = false;
                site = db.Sites
                    .Include(s => s.Ratings)
                    .SingleOrDefault(p => p.Id == id);
                foreach (var x in site.Ratings)
                    if (x.UserName.Equals(user)) { result = true; break; }
                if (!result)
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
            UserRating userMedal;
            using (MyDbContext db = new MyDbContext())
            {
                userName = GetUserName(User.Identity.GetUserName());
                userMedal = db.Ratings.Include(s => s.Medals).FirstOrDefault(p => p.UserName.Equals(userName));
                int count = db.Sites.Where(x => x.UserName.Equals(userName)&&x.Pablish).Count();
                for (int i = 5; i <= 20; i *= 2)
                {
                    if (count >= i)
                        Medal(userMedal, i);
                    else
                    {
                        List<Medal> medal =userMedal.Medals.Where(x => x.Description.Equals(i.ToString())).ToList();
                        if (medal.Count != 0)
                            db.Medals.RemoveRange(medal);
                    }
                }
                db.SaveChanges();
            }
            return userMedal;
        }

        private static void Medal(UserRating userMedal, int medalId)
        {
            List<Medal> medal = userMedal.Medals.Where(x => medalId.ToString().Equals(x.Description)).ToList();
            if (medal.Count<=0)
                userMedal.Medals.Add(new Medal()
                { Description = medalId.ToString() });
            
        }

        [AllowAnonymous]
        public ActionResult ChangeCulture(string lang)
        {
            List<string> cultures = new List<string>() { "ru", "en" };
            lang = !cultures.Contains(lang) ? "ru" : lang;
            HttpCookie cookie = Request.Cookies["lang"];
            if (cookie != null)
                cookie.Value = lang;
            else
            {
                cookie = new HttpCookie("lang",lang);
                cookie.HttpOnly = false;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);
            return Redirect(Request.UrlReferrer.AbsolutePath);
        }

        [AllowAnonymous]
        public PartialViewResult Search(string SearchValue, bool tagSearch)
        {
            List<Site> resultSearch = new List<Site>();
            IEnumerable<Site> result = tagSearch? LuceneSearch.Search(SearchValue, "Tags"): LuceneSearch.Search(SearchValue);
            using (MyDbContext db = new MyDbContext())
            {
                foreach (var x in result)
                {
                    Site site =db.Sites.Include(s => s.Comments)
                            .Include(m => m.Pages)
                            .Include(n => n.Ratings)
                            .FirstOrDefault(p => p.Id == x.Id && p.Pablish);
                    if(site!=null)
                        resultSearch.Add(site);
                }
            }
            TempData["sites"] = resultSearch;
            return PartialView("TableSearch", new PagedList<Site>(resultSearch, 1, 99));
        }

        [HttpPost]
        public JsonResult ChangePhoto(string data, string userName)
        {
            var dbContext = new ApplicationDbContext();
            var account = new Account("dgy6x5krf", "949232162798767", "oxvzYd03K1i8lEIi5MA2ByIf590");

            var cloudinary = new Cloudinary(account);

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(data)
            };
            var uploadResult = cloudinary.Upload(uploadParams);
            UserRating user = db.Ratings.FirstOrDefault(p => p.UserName.Equals(userName));
            user.PhotoUrl = uploadResult.SecureUri.ToString();
            
            dbContext.SaveChanges();
            return new JsonResult();
        }
    }
}

