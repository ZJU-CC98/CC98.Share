using System.Linq;
using CC98.Share.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Sakura.AspNetCore;

namespace CC98.Share.Controllers
{
    /// <summary>
    ///     提供网站最常用功能的访问。
    /// </summary>
    public class HomeController : Controller
    {
        public HomeController(IHostingEnvironment environment, CC98ShareModel userDb)
        {
            Environment = environment;
            UserDb = userDb;
        }


        private IHostingEnvironment Environment { get; }


        private CC98ShareModel UserDb { get; }

        private IQueryable<ShareItem> GetUserFile(string username)
        {
            var result = from i in UserDb.Items
                         orderby i.Id
                         where i.UserName == username
                         select i;
            return result;
        }

        /// <summary>
        ///     显示网站主页。
        /// </summary>
        /// <returns>操作结果。</returns>
        public IActionResult Index(int page = 1)
        {
            if (User.Identity.IsAuthenticated == false)
            {
                return View();
            }
            var result = GetUserFile(User.Identity.Name);
            const long fileSize = 0;
            var fileCount = 0;
            var shareCount = 0;
            var products = result.ToArray();
            var pageNumber = page;
            var pageSize = 10;
            var pageData = products.OrderBy(p => p.Id).ToPagedList(pageSize, pageNumber);
            IPagedList pagerSource = result.ToPagedList(pageSize, page);

            foreach (var i in products)
            {
                fileCount = fileCount + 1;
                //FileSize = FileSize + i.Size;
                if (i.IsShared)
                {
                    shareCount = shareCount + 1;
                }
            }

            ViewData["datashow"] = pageData;
            ViewData["filecount"] = fileCount;
            ViewData["filesize"] = fileSize;
            ViewData["sharecount"] = shareCount;
            ViewData["pagersource"] = pagerSource;

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

        public IActionResult Fileinfo()
        {
            ViewData["Message"] = "开发中";

            return View();
        }

        /// <summary>
        ///     当网站发生错误时显示的页面。
        /// </summary>
        /// <returns>操作结果。</returns>
        public IActionResult Error()
        {
            return View();
        }
    }
}