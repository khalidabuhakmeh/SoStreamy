using System.Web.Mvc;
using System.Web.Routing;

namespace SoStreamy
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapHubs();
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Thoughts", action = "Index", id = UrlParameter.Optional }
            );

            
        }
    }
}