using Microsoft.AspNet.Mvc;

namespace CC98.Sports.Controllers
{
	/// <summary>
	/// 主页控制器。
	/// </summary>
    public class HomeController : Controller
    {
		/// <summary>
		/// 应用程序首页。
		/// </summary>
		/// <returns></returns>
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

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}
