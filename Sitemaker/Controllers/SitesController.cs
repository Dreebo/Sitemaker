using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Sitemaker;
using Sitemaker.Models;
using CloudinaryDotNet;
using Microsoft.Ajax.Utilities;
using Sitemaker.Filters;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList.Mvc;
using PagedList;
using MvcLuceneSampleApp.Search;
using CloudinaryDotNet.Actions;
using Microsoft.AspNet.Identity.Owin;

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
            List<Site> sites = db.Sites.Include(s => s.Pages).Include(s => s.Ratings).Include(s => s.Tags).Where(x => x.Pablish).ToList();
            sites.Reverse();
            LuceneSearch.ClearLuceneIndex();
            LuceneSearch.AddUpdateLuceneIndex(db.Sites.Include("Comments").Include("Tags"));
            int pageSize = 9;
            int pageNumber = (page ?? 1);
            Session["CurrentUserName"] = GetUserName(User.Identity.GetUserName());
            return View(sites.ToPagedList(pageNumber, pageSize));
        }

        private static string GetUserName(string userName)
        {
            if (userName != "")
            {
                int position = userName.IndexOf("@");
                userName = userName.Remove(position);
            }
            else
            {
                userName = "";
            }
            return userName;
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
                    .Include(s => s.Comments)
                    .Include(s => s.Medals)
                    .Include(s => s.Tags)
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
                   .Include(s => s.Comments)
                    .Include(s => s.Medals)
                    .Include(s => s.Tags)
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
            string Name = GetUserName(User.Identity.GetUserName());
            Session["CurrentUserName"] = Name;
            return View("ShowMySite", db.Sites.Where(p => p.UserName == Name).ToList());
        }
        
        [HttpPost]
        public void SaveChangeSite(Site site)
        {
            Site currentSite;
            using (MyDbContext db = new MyDbContext())
            {
                currentSite = db.Sites.Where(p => p.Id == site.Id).FirstOrDefault();
                currentSite.Name = site.Name;
                currentSite.About = site.About;
                currentSite.Logo = Upload(site.Logo);
                currentSite.Date = DateTime.Now;
                db.SaveChanges();
            }
        }

        public ActionResult ShowUser(string userName, string siteCreator)
        {
            string Name = GetUserName(User.Identity.GetUserName());
            Session["CurrentUserName"] = Name;
            UserRating user;
            List<Site> sites;
            
            using (MyDbContext db = new MyDbContext())
            {
                sites =
                    db.Sites.Include(s => s.Pages)
                        .Include(s => s.Ratings)
                        .Include(s => s.Tags)
                        .Where(p => p.UserName == siteCreator)
                        .Where(x=>x.Pablish).ToList();
                sites.Reverse();
                user = db.Ratings.Include(m => m.Medals).Include(m => m.Sites).Where(p => p.UserName == siteCreator).FirstOrDefault();
                if (CheckMedal(siteCreator) != null)
                    user.Medals = CheckMedal(siteCreator).Medals;
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

        public ActionResult LoadPage(string userName, int id)
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
            Session.Add("TemplateId",site.TemplateId);
            Page page = site.Pages.Where(p => p.Id == pageId).SingleOrDefault();
            return View("CreatePage", page);
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
            Session.Add("TemplateId",site.TemplateId);
            return View("Page", page);
            //return RedirectToAction("ShowPage", new { userName = site.UserName, id = id, pageId = page.Id });
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
                    .Where(p => p.Id == id)
                    .SingleOrDefault();
            }
            if (site == null)
            {
                site = new Site();
            }

            return View("CreateMenu", site);
        }

        [AllowAnonymous]
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
                    .Where(p => p.Id == id)
                    .SingleOrDefault();
            }
            if (site == null)
            {
                site = new Site();
            }
            Page page=null;
            if(pageId!=null)
                page = site.Pages.Where(s => s.Id == pageId).FirstOrDefault();
            if(page==null)
                if (site.Pages.Count > 0)
                    page = site.Pages.FirstOrDefault();
                else
                    page=new Page();
            Session.Add("TemplateId", site.TemplateId);
            Session.Add("Menu", site.Menu);
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
            else page = site.Pages.Where(p => p.Id == pageId).SingleOrDefault();
            return Json(page);
        }



        [HttpPost]
        public ActionResult SaveSite(Site site, string[] tagsArray)
        {
            using (MyDbContext db = new MyDbContext())
            {
                Tag tag;
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
                string user = User.Identity.GetUserName();
                int position = user.IndexOf("@");
                user = user.Remove(position);
                site.UserName = user;
                site.Logo = Upload(site.Logo);
                site.Date = DateTime.Now;
                string userId = User.Identity.GetUserId();
                site.CreaterId = userId;
                site.Tags = tags;
                db.Sites.Add(site);
                var usr = System.Web.HttpContext.Current.GetOwinContext().
                GetUserManager<ApplicationUserManager>().
                FindById(System.Web.HttpContext.
                Current.User.Identity.GetUserId());
               // usr.Site.Add(site);
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
                    .Include(s => s.Tags)
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
            UserRating userMedal;
            using (MyDbContext db = new MyDbContext())
            {
                userMedal = db.Ratings.Include(s => s.Medals).Where(p => p.UserName == userName).FirstOrDefault();
                int count = db.Sites.Where(x => x.UserName.Equals(userName)).Where(t=>t.Pablish).Count();
                for (int i = 5; i <= 20; i *= 2)
                {
                    if (count >= i)
                    {
                        Medal(userMedal, i);
                    }
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
            {
                userMedal.Medals.Add(new Medal()
                { Description = medalId.ToString() });
            }
            
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

        public PartialViewResult Search(string SearchValue, bool tagSearch)
        {
            List<Site> resultSearch = new List<Site>();
            IEnumerable<Site> result;
            if (tagSearch)
            {
                result = LuceneSearch.Search(SearchValue, "Tags");
            }
            else
            {
                result = LuceneSearch.Search(SearchValue);
            }
            using (MyDbContext db = new MyDbContext())
            {
                foreach (var x in result)
                {

                    Site site =db.Sites.Include(s => s.Comments)
                            .Include(m => m.Pages)
                            .Include(n => n.Ratings)
                            .Where(p => p.Id == x.Id)
                            .Where(t=>t.Pablish)
                            .FirstOrDefault();
                        resultSearch.Add(site);
                }
            };          
            TempData["sites"] = resultSearch;
            return PartialView("TableSearch", new PagedList<Site>(resultSearch, 1, 99));
        }

        //public PartialViewResult SearchTags(string SearchValue)
        //{
        //    List<Site> resultSearch = new List<Site>();
        //    var result = LuceneSearch.Search(SearchValue, "Tags");
        //    using (MyDbContext db = new MyDbContext())
        //    {
        //        foreach (var x in result)
        //        {

        //            Site site =db.Sites.Include(s => s.Comments)
        //                    .Include(m => m.Pages)
        //                    .Include(n => n.Ratings)
        //                    .Where(p => p.Id == x.Id)
        //                    .Where(t=>t.Pablish)
        //                    .FirstOrDefault();
        //                resultSearch.Insert(0, site);
        //        }
        //    };
        //    TempData["sites"] = resultSearch;
        //    return PartialView("TableSearch", new PagedList<Site>(resultSearch, 1, 9));
        //}

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
            UserRating user = db.Ratings.Where(p => p.UserName == userName).FirstOrDefault();
            user.PhotoUrl = uploadResult.SecureUri.ToString();
            
            dbContext.SaveChanges();
            return new JsonResult();
        }
    }
}

