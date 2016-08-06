using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sitemaker
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{userId}/{siteId}/{pageId}",
            //    defaults: new { userId = UrlParameter.Optional, siteId = UrlParameter.Optional, pageId = UrlParameter.Optional }
            //);

            routes.MapRoute(
               name: "ForPage",
               url: "{controller}/{action}/{id}/{pageId}",
               defaults: new { controller = "Sites", action = "Index", id = UrlParameter.Optional, pageId = UrlParameter .Optional}
           );

            

            
        }
    }
}
