using System.Web.Mvc;
using System.Web.Routing;

namespace Fooble.Web
{
    static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            const string nonEmptyGuidPattern =
                @"(?i:(?(^0{8}-(?:0{4}-){3}0{12}$)^$|^[\da-f]{8}-(?:[\da-f]{4}-){3}[\da-f]{12})$)";

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "MemberRegister",
                url: "member/register",
                defaults: new { controller = "member", action = "register" });

            routes.MapRoute(
                name: "MemberChangeEmail",
                url: "member/{id}/changeemail",
                defaults: new { controller = "member", action = "changeemail" },
                constraints: new { id = nonEmptyGuidPattern });

            routes.MapRoute(
                name: "MemberChangeOther",
                url: "member/{id}/changeother",
                defaults: new { controller = "member", action = "changeother" },
                constraints: new { id = nonEmptyGuidPattern });

            routes.MapRoute(
                name: "MemberChangePassword",
                url: "member/{id}/changepassword",
                defaults: new { controller = "member", action = "changepassword" },
                constraints: new { id = nonEmptyGuidPattern });

            routes.MapRoute(
                name: "MemberChangeUsername",
                url: "member/{id}/changeusername",
                defaults: new { controller = "member", action = "changeusername" },
                constraints: new { id = nonEmptyGuidPattern });

            routes.MapRoute(
                name: "MemberDetail",
                url: "member/{id}",
                defaults: new { controller = "member", action = "detail" },
                constraints: new { id = nonEmptyGuidPattern });

            routes.IgnoreRoute("member/{*pathInfo}");

            routes.MapRoute(
                name: "MemberList",
                url: "members",
                defaults: new { controller = "member", action = "list" });

            routes.IgnoreRoute("members/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
