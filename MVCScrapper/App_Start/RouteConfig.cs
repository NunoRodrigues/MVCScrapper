using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVCScrapper
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "FIDCControllerDefault",
                url: "{controller}/{action}/{serviceId}/{competencia}",
                defaults: new { controller = "FIDCController", action = "Mensal", serviceId = UrlParameter.Optional, competencia = UrlParameter.Optional }
            );
            /*
            routes.MapRoute(
                name: "FIDCController_serviceId",
                url: "{controller}/{action}/{serviceId}",
                defaults: new { controller = "FIDCController", action = "Mensal", serviceId = UrlParameter.Optional }
            );
            */
        }
    }
}