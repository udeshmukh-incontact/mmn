using InContact.Common.Branding;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using System.Web.Routing;

namespace ManageMyNotificationsMVC
{
    [ExcludeFromCodeCoverage]
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("PartnerUrls", "pb/{partner}", new { controller = "Notifications", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Notifications", action = "index", id = UrlParameter.Optional },
                namespaces: new[]{
                    typeof(BrandingController).Namespace
                    }
            );
        }
    }
}
