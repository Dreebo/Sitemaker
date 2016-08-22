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
        private ApplicationDbContext adb = new ApplicationDbContext();

        // GET: Sites
        [AllowAnonymous]
        public ActionResult Index(int? page)
        {
            List<Site> sites = db.Sites.Include(s => s.Ratings).Where(x => x.Pablish).ToList();
            
            switch ((string) Session["sort"])
            {
                case "rating":
                    sites.Sort(delegate(Site x, Site y) { return y.AverageRating.CompareTo(x.AverageRating); });
                    break;
                default:
                    sites.Reverse();
                    break;
            }
            LuceneSearch.ClearLuceneIndex();
            LuceneSearch.AddUpdateLuceneIndex(db.Sites.Include("Comments").Include("Tags"));
            Session["CurrentUserName"] = GetUserName();
            Session["userBlock"] = GetApplicationUser().IsBlock;
            Session.Add("Author", false);
            Session["sortEnable"] = "true";
            ViewBag.Tags = db.Tags.Select(x=>x.Name).ToList();
            return View(sites.ToPagedList((page ?? 1), 9));
        }

        
        private string GetUserName()
        {
            return GetUserName(User.Identity.GetUserName());
        }

        private string GetUserName(string userName)
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
            if (id == null) throw new HttpException(HttpStatusCode.BadRequest.ToString());
            Site site = db.Sites.Include(x=>x.Pages).SingleOrDefault(p => p.Id == id);
            if (site == null) throw new HttpException(HttpStatusCode.NotFound.ToString());
            if (CheckRoot(site))
            {
                Session.Add("Author",true);
                return View(site);
            }
            throw new HttpException(HttpStatusCode.Forbidden.ToString()); ;
        }

        private bool CheckRoot(Site site)
        {
            return User.IsInRole("admin") || User.Identity.GetUserId().Equals(site.CreaterId);
        }

        // GET: Sites/Delete/5

        public ActionResult Delete(string userName, int? id)
        {
            if (id == null) throw new HttpException(HttpStatusCode.BadRequest.ToString());
            using (MyDbContext db = new MyDbContext())
            {
                Site site = db.Sites
                    .Include(s => s.Pages)
                    .Include(s => s.Comments)
                    .Include(s => s.Medals)
                    .Include(s => s.Tags)
                    .SingleOrDefault(p => p.Id == id);
                if (site == null) throw new HttpException(HttpStatusCode.NotFound.ToString());
                if (CheckRoot(site)) return View(site);
            }
            throw new HttpException(HttpStatusCode.NotFound.ToString());
        }

        // POST: Sites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (MyDbContext db = new MyDbContext())
            {
            Site site = db.Sites
                   .Include(s => s.Pages)
                   .Include(s => s.Comments)
                    .Include(s => s.Medals)
                    .Include(s => s.Tags)
                   .SingleOrDefault(p => p.Id == id);
            if (CheckRoot(site))
            {
                db.Sites.Remove(site);
                db.SaveChanges();
                return RedirectToAction("ShowMySite", new { userName = site.UserName });
            }    
            }
            
            throw new HttpException(HttpStatusCode.NotFound.ToString());
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
            if (User.IsInRole("admin") || GetUserName().Equals(userName))
                return View("ShowMySite", db.Sites.Where(p => p.UserName.Equals(userName)).ToList());
            throw new HttpException(HttpStatusCode.Forbidden.ToString());
        }

        [HttpPost]
        public void SaveChangeSite(Site site)
        {
            if (CheckRoot(site))
                using (MyDbContext db = new MyDbContext())
                {
                    Site currentSite = db.Sites.FirstOrDefault(p => p.Id == site.Id);
                    currentSite.Name = site.Name;
                    currentSite.About = site.About;
                    currentSite.Logo = Upload(site.Logo);
                    currentSite.Date = DateTime.Now;
                    db.SaveChanges();
                }
            throw new HttpException(HttpStatusCode.Forbidden.ToString());
        }

        [AllowAnonymous]
        public ActionResult ShowUser(string userName)
        {
            using (ApplicationDbContext adb = new ApplicationDbContext())
            {
                ApplicationUser applicationUser = adb.Users.ToList().FirstOrDefault(x => GetUserName(x.UserName).Equals(userName));
                if(applicationUser!=null) { Session.Add("imgUrl", applicationUser.PhotoUrl); Session.Add("authorName", GetUserName(applicationUser.UserName)); }
            }

            UserRating user;
            List<Site> sites;
            using (MyDbContext db = new MyDbContext())
            {
                sites = db.Sites
                        .Include(s => s.Ratings)
                        .Where(p => p.UserName.Equals(userName) && p.Pablish).ToList();
                sites.Reverse();
                user = db.Ratings.Include(m => m.Medals).Include(m => m.Sites).FirstOrDefault(p => p.UserName.Equals(userName));
                UserRating userRating = CheckMedal(userName);
                user.Medals = userRating != null ? CheckMedal(userName).Medals : user.Medals;
                user.Sites = sites;
            }
            return View("ShowUser", user);
        }


        [Authorize]
        public ActionResult CreateSite(string userName, int? id)
        {
            if (GetApplicationUser().IsBlock)
                throw new HttpException(HttpStatusCode.Forbidden.ToString());
            Site site;
            using (MyDbContext db = new MyDbContext())
            {
                site = db.Sites.SingleOrDefault(p => p.Id == id);
            }
            if (site == null)
                site = new Site();
            return View("CreateSite", site);
        }

        private ApplicationUser GetApplicationUser()
        {
            ApplicationUser user;
            using (ApplicationDbContext adb = new ApplicationDbContext())
            {
                user = adb.Users.ToList().FirstOrDefault(x => x.Id.Equals(User.Identity.GetUserId()));
            }
            return user;
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
            using (MyDbContext db = new MyDbContext())
            {
                Site site = db.Sites.SingleOrDefault(p => p.Id == id);
                if (site != null && CheckRoot(site))
                {
                    Page page = new Page();
                    site.Pages.Add(page);
                    db.SaveChanges();
                    return RedirectToAction("CreatePage", new { userName = site.UserName, id = id, pageId = page.Id });
                }
            }
            throw new HttpException(HttpStatusCode.InternalServerError.ToString());
        }

        public ActionResult CreatePage(string userName, int id, int? pageId)
        {
            if (pageId == null)
                throw new HttpException(HttpStatusCode.BadRequest.ToString());
            Site site;
            using (MyDbContext db = new MyDbContext())
            {
                site = db.Sites.Include(s => s.Pages).SingleOrDefault(p => p.Id == id);
            }
            if (site == null)
                throw new HttpException(HttpStatusCode.NotFound.ToString());
            if (CheckRoot(site))
            {
                Session.Add("TemplateId", site.TemplateId);
                return View("CreatePage", site.Pages.SingleOrDefault(p => p.Id == pageId));
            }
            throw new HttpException(HttpStatusCode.NotFound.ToString());
        }



        public ActionResult PageView(string userName, int id, int? pageId)
        {
            Page page;
            using (MyDbContext db = new MyDbContext())
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
            using (MyDbContext db = new MyDbContext())
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
            if (CheckRoot(site))
                return View("CreateMenu", site);
            throw new HttpException(HttpStatusCode.Forbidden.ToString());
        }

        [AllowAnonymous]
        public ActionResult FillSite(string userName, int id)
        {
            Site site;
            using (MyDbContext db = new MyDbContext())
            {
                site = db.Sites.Include(x=>x.Pages).SingleOrDefault(p => p.Id == id);
            }
            if (site == null)
                site = new Site();
            if (CheckRoot(site))
                return View("FillSite", site);
            throw new HttpException(HttpStatusCode.Forbidden.ToString());
        }

        [AllowAnonymous]
        public ActionResult Site(string userName, int id, int? pageId)
        {
            Site site;
            using (MyDbContext db = new MyDbContext())
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
            Page page;
            using (MyDbContext db = new MyDbContext())
            {
                Site site = db.Sites.SingleOrDefault(p => p.Id == id);
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
                        temp = new Tag() { Name = x };
                        db.Tags.Add(temp);
                    }
                    tags.Add(temp);
                }
                site.UserName = GetUserName();
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
            using (MyDbContext db = new MyDbContext())
            {
                Menu menu = db.Menus.Include(c => c.TopBar).Include(s => s.SideBar).SingleOrDefault(p => p.Id == saveMenu.Id);
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
            if (GetApplicationUser().IsBlock)
                throw new HttpException(HttpStatusCode.Forbidden.ToString());
            using (MyDbContext db = new MyDbContext())
            {
                Site site = db.Sites.SingleOrDefault(p => p.Id == id);
                if (CheckRoot(site))
                {
                    site.Pablish = true;
                    site.Date = DateTime.Now;
                    db.SaveChanges();
                }
                else throw new HttpException(HttpStatusCode.Forbidden.ToString());
            }
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public ActionResult Comments(string userName, int id)
        {
            var user = GetApplicationUser();
            Session.Add("userBlock", user.IsBlock == null ? true : user.IsBlock);
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
        public ActionResult AddComment(int Id, string UserName, string Comment)
        {
            
            Comment comment = new Comment() { Date = DateTime.Now, UserComment = Comment, UserName = GetUserName() };
            using (MyDbContext db = new MyDbContext())
            {
                comment.Site = db.Sites
                    .Include(s => s.Comments)
                    .SingleOrDefault(p => p.Id == Id);
                db.Comments.Add(comment);
                db.SaveChanges();
            }
            return Redirect(Request.UrlReferrer.AbsolutePath);
        }


        [Authorize]
        public void AddRating(int id, string rating)
        {
            string user = GetUserName();

            using (MyDbContext db = new MyDbContext())
            {
                bool result = false;
                Site site = db.Sites.Include(s => s.Ratings).SingleOrDefault(p => p.Id == id);
                foreach (var x in site.Ratings)
                    if(result = x.UserName.Equals(user)) break;
                    
                if (!result)
                {
                    site.RatingCount++;
                    site.AverageRating = Math.Round((site.AverageRating * (site.RatingCount - 1) + Int32.Parse(rating)) / site.RatingCount);
                    UserRating num = new UserRating() { UserName = user, Star = Int32.Parse(rating) };
                    num.Sites.Add(site);
                    db.Ratings.Add(num);
                    db.SaveChanges();
                }
            }
        }

        public UserRating CheckMedal(string userName)
        {

            using (MyDbContext db = new MyDbContext())
            {
                UserRating userMedal = db.Ratings.Include(s => s.Medals).FirstOrDefault(p => p.UserName.Equals(userName));
                int count = db.Sites.Where(x => x.UserName.Equals(userName) && x.Pablish).Count();
                for (int i = 5; i <= 20; i *= 2)
                    if (count >= i)
                        Medal(userMedal, i);
                    else
                        db.Medals.RemoveRange(userMedal.Medals.Where(x => x.Description.Equals(i.ToString())).ToList());
                db.SaveChanges();
                return userMedal;
            }
        }

        private static void Medal(UserRating userMedal, int medalId)
        {
            List<Medal> medal = userMedal.Medals.Where(x => medalId.ToString().Equals(x.Description)).ToList();
            if (medal.Count <= 0)
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
        public ActionResult SortByDate()
        {
            Session.Add("sort", "date");
            return Redirect(Request.UrlReferrer.AbsolutePath);
        }

        [AllowAnonymous]
        public ActionResult SortByRating()
        {
            Session.Add("sort", "rating");
            return Redirect(Request.UrlReferrer.AbsolutePath);
        }

        [AllowAnonymous]
        public PartialViewResult Search(string SearchValue, bool tagSearch)
        {
            List<Site> resultSearch = new List<Site>();
            IEnumerable<Site> result = tagSearch ? LuceneSearch.Search(SearchValue, "Tags") : LuceneSearch.Search(SearchValue);
            using (MyDbContext db = new MyDbContext())
            {
                foreach (var x in result)
                {
                    Site site = db.Sites.Include(s => s.Comments)
                            .Include(m => m.Pages).Include(n => n.Ratings)
                            .FirstOrDefault(p => p.Id == x.Id && p.Pablish);
                    if(site!=null)
                        resultSearch.Add(site);
                }
            }
            TempData["sites"] = resultSearch;
            Session["sortEnable"] = null;
            return PartialView("TableSearch", new PagedList<Site>(resultSearch, 1, 99));
        }

        [HttpPost]
        public JsonResult ChangePhoto(string data, string userName)
        {
            var cloudinary = new Cloudinary(new Account("dgy6x5krf", "949232162798767", "oxvzYd03K1i8lEIi5MA2ByIf590"));
            var uploadResult = cloudinary.Upload(new ImageUploadParams
            {
                File = new FileDescription(data)
            });
            UserRating user = db.Ratings.FirstOrDefault(p => p.UserName.Equals(userName));
            user.PhotoUrl = uploadResult.SecureUri.ToString();
            using (var dbContext = new ApplicationDbContext())
            {
                dbContext.SaveChanges();
            }

            return new JsonResult();
        }
    }
}

