using System;
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
        // GET: Admin
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult Users()
        {

            List<ApplicationUser> users = new List<ApplicationUser>();
            //using (MyDbContext db = new MyDbContext())
            //{
            //    if (!db.Database.Exists())
            //    {
            //    }
            //    MembershipUserCollection userCollection = Membership.GetAllUsers();
            //    foreach (MembershipUser user in userCollection)
            //    {
            //        users.Add(user);
            //    }
            //}


            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                IDbSet<ApplicationUser> usersDb = db.Users;
                foreach (var user in usersDb)
                {
                    users.Add(user);
                }
            }


            return View("Users",users);
        }


        [HttpPost]
        public ActionResult DeleteUsers()
        {
            
            string blockString = Request["delete"];
            string[] block = blockString.Split(',');
            
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                
                IDbSet<ApplicationUser> usersDb = db.Users;
                var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                foreach (string id in block)
                {
                    //var user = um.FindByIdAsync(id);
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
                    .Where(x => x.CreaterId.Equals(id)).ToList();
                    db.Sites.RemoveRange(site);
                }
                db.SaveChanges();
            }
            return RedirectToAction("Users", new {});
        }

        [HttpPost]
        public ActionResult ChangeBlock(BlockInfo block)
        {
            //string blockId = Request["blockId"];
            //bool block = Boolean.Parse(Request["block"]);

            
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ApplicationUser user = db.Users.Where(x => x.Id.Equals(block.BlockId)).First();
                user.IsBlock = block.Block;
                db.SaveChanges();
            }
            
            return RedirectToAction("Users", new { });
        }
    }

    
}