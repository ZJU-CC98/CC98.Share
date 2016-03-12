using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CC98.Share.Controllers
{
	/// <summary>
	/// Home 控制器。
	/// </summary>
	public class HomeController : Controller
	{
		/// <summary>
		/// 显示应用程序主页。
		/// </summary>
		/// <returns>操作结果。</returns>
		public ActionResult Index()
		{
			return View();
		}
	}
}