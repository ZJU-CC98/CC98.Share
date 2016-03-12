using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using JetBrains.Annotations;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(CC98.Share.Startup))]

namespace CC98.Share
{
	/// <summary>
	/// OWIN 应用程序的启动类型。
	/// </summary>
	public class Startup
	{
		/// <summary>
		/// 配置应用程序的必要信息。
		/// </summary>
		/// <param name="app">OWIN 应用程序对象。</param>
		[UsedImplicitly(ImplicitUseKindFlags.Access)]
		public void Configuration(IAppBuilder app)
		{
			// 有关如何配置应用程序的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=316888

			AreaRegistration.RegisterAllAreas();
			RouteConfig.RegisterRoutes(RouteTable.Routes);
		}
	}
}
