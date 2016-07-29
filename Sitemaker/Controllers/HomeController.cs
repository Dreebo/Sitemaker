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
        MyDbContext db = new MyDbContext();
        int count = 0;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateSite()
        {
            return View("CreateSite");
        }

        [HttpPost]
        public JsonResult Upload()
        {
            Account account = new Account("dgy6x5krf", "949232162798767", "oxvzYd03K1i8lEIi5MA2ByIf590");
            Cloudinary cloudinary = new Cloudinary(account);
            foreach (string file in Request.Files)
            {
                var upload = Request.Files[file];
                if (upload != null)
                {
                    string fileName = System.IO.Path.GetFileName(upload.FileName);
                    upload.SaveAs(Server.MapPath("~/Files/" + fileName));
                    string path = (Server.MapPath("~/Files/" + fileName));

                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(path)
                    };
                    var uploadResult = cloudinary.Upload(uploadParams);
                    ViewBag.Head = uploadResult.Uri.AbsolutePath;
                }
            }
            return Json("файл загружен");
        }



        [HttpPost]
        public RedirectToRouteResult SaveSite(string id, string name, string about, string dataid)
        {
            Site site = new Site();
            site.Name = name;
            site.TemplateId = id;
            site.About = about;
            site.MenuId = dataid;
            db.Sites.Add(site);
            db.SaveChanges();
            return RedirectToAction("CreateSite", "HomeController");         
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