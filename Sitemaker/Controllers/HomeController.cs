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

        public ActionResult Index(int? id)
        {
            return View();
        }
    }
}