﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Sitemaker.Models;
using HtmlHelper = PagedList.Mvc.HtmlHelper;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Sitemaker.Controllers
{
    public class AdminController : Controller
    {
        [Authorize(Roles ="admin")]
        public ActionResult Users()
        {

            List<ApplicationUser> users = new List<ApplicationUser>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                users = db.Users.ToList();
            }
            return View("Users",users);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult DeleteUsers()
        {
            
            string blockString = Request["delete"];
            string[] block = blockString.Split(',');
            
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                foreach (string id in block)
                {
                    ApplicationUser user = db.Users.Where(x => x.Id.Equals(id)).First();
                    db.Users.Remove(user);     
                }
                db.SaveChanges();
            }

            using (MyDbContext db = new MyDbContext())
            {
                foreach (string id in block)
                {
                    List<Site> site = db.Sites
                    .Include(s => s.Pages)
                    .Include(s => s.Menu)
                    .Include(s => s.Menu.TopBar)
                    .Include(s => s.Menu.SideBar)
                    .Include(s => s.Medals)
                    .Include(s => s.Ratings)
                    .Include(s => s.Tags)
                    .Where(x => x.CreaterId.Equals(id)).ToList();
                    db.Sites.RemoveRange(site);
                }
                db.SaveChanges();
            }
            return RedirectToAction("Users", new {});
        }

        [HttpPost]
        public ActionResult ChangeBlock(string blockId, bool block)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ApplicationUser user = db.Users.Where(x => x.Id.Equals(blockId)).First();
                user.IsBlock = block;
                db.SaveChanges();
            }
            return RedirectToAction("Users", new { });
        }
    }

    
}