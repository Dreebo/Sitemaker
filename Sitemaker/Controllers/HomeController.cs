using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNet.Identity;
using Sitemaker.Filters;
using Sitemaker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Sitemaker.Controllers
{
    [Culture]
    public class HomeController : Controller
    {
    
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateSite(int? id)
        {
            // TODO check is author.
            Site site;     
            using (var db = new MyDbContext())
            {
               site = db.Sites.Where(p => p.Id == id).SingleOrDefault();
            }
            if(site == null)
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

        public ActionResult CreatePage(int? id)
        {
            Site site;
            using(var db = new MyDbContext())
            {
                site = db.Sites.Where(p => p.Id == id).SingleOrDefault();
            }
            if (site == null)
            {
                site = new Site();
            }
            return View("CreatePage", site);
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
            return Json(new { result = "Redirect", url = Url.Action("CreateSite", "Home", new { id = site.Id }) });
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