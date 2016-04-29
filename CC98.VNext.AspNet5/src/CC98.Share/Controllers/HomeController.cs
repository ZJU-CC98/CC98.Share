using Microsoft.AspNet.Mvc;

namespace CC98.Share.Controllers
{
	/// <summary>
	/// 提供网站最常用功能的访问。
	/// </summary>
	public class HomeController : Controller
	{
		/// <summary>
		/// 显示网站主页。
		/// </summary>
		/// <returns>操作结果。</returns>
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult About()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		public IActionResult Contact()
		{
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		/// <summary>
		/// 当网站发生错误时显示的页面。
		/// </summary>
		/// <returns>操作结果。</returns>
		public IActionResult Error()
		{
			return View();
		}
	}
}
