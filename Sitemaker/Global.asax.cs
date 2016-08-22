using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Sitemaker.Files.Filters;
using Sitemaker.Models;

namespace Sitemaker
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //Database.SetInitializer<MyDbContext>(new AppDbInitializer());
            AreaRegistration.RegisterAllAreas();
            //filters.Add(new HandleAllErrorAttribute());
            GlobalFilters.Filters.Add(new HandleAllErrorAttribute());
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    HttpContext oHttpContext;
        //    Exception oException;

        //    oHttpContext = HttpContext.Current;

        //    oException = oHttpContext.Server.GetLastError();

        //    if (oException is HttpException)
        //    {
        //                oHttpContext.Response.StatusCode = (oException as HttpException).GetHttpCode();
        //                oHttpContext.Response.StatusDescription = (oException as HttpException).GetHtmlErrorMessage();
        //                oHttpContext.Server.Execute("/Home/Error");
        //                oHttpContext.Server.ClearError();
                
        //    }
        //}
    }
}
