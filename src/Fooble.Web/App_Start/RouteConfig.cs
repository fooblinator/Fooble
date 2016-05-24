using System.Web.Mvc;
using System.Web.Routing;

namespace Fooble.Web
{
    static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("MemberList", "members", new { controller = "member", action = "list" });
            routes.MapRoute("MemberDetail", "member/{id}", new { controller = "member", action = "detail" });

            routes.IgnoreRoute("member/{*pathInfo}");

            routes.MapRoute("SelfServiceRegister", "self/register", new { controller = "selfservice", action = "register" });

            routes.IgnoreRoute("self/{*pathInfo}");
            routes.IgnoreRoute("selfservice/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
