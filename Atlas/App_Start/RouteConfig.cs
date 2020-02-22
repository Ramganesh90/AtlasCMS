using System.Web.Mvc;
using System.Web.Routing;

namespace Atlas
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "Project",
               url: "Project",
               defaults: new { controller = "Project", action = "Index", id = UrlParameter.Optional }
           );
        }
    }
}