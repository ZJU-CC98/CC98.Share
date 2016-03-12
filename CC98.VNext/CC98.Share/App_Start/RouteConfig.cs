using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CC98.Share
{
	/// <summary>
	/// 提供路由注册的相关方法。该类型为静态类型。
	/// </summary>
    public static class RouteConfig
    {
		/// <summary>
		/// 向项目注册 MVC 路由规则。
		/// </summary>
		/// <param name="routes">项目的 MVC 路由表。</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
