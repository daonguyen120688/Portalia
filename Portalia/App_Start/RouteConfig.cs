using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Portalia
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var isUnderContructor = bool.Parse(ConfigurationManager.AppSettings["IsUnderContructor"]);
            if (isUnderContructor)
            {
                routes.MapRoute(
                    name: "Default",
                    url: "{controller}/{action}/{id}",
                    defaults: new { controller = "LandingPage", action = "Index", id = UrlParameter.Optional }
                );
            }
            else
            {
                routes.MapRoute(
                    name: "MySpace",
                    url: "Proposal/MySpace/{userId}",
                    defaults: new { controller = "Proposal", action = "MySpace", userId = UrlParameter.Optional }
                );

                routes.MapRoute(
                    name: "FileRoute",
                    url: "account/files/{fileName}",
                    defaults: new { controller = "account", action = "files", fileName = UrlParameter.Optional }
                );

                routes.MapRoute(
                    name: "Default",
                    url: "{controller}/{action}/{id}",
                    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                );
            }
        }
    }
}
