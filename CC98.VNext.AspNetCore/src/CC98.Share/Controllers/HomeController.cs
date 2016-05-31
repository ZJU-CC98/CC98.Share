using System.Linq;
using CC98.Share.Data;
using CC98.Share.ViewModels.SearchAll;
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

	
		private CC98ShareModel UserDb { get; set; }

		private IQueryable<ShareItem> GetUserFile(string username)
		{
			IQueryable<ShareItem> result;
			result = from i in UserDb.Items
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
			if(User.Identity.IsAuthenticated==false)
			{
				return View();
			}
			else
			{
				var result = GetUserFile(User.Identity.Name);
				int FileSize = 0;
				int FileCount = 0;
				int ShareCount = 0;
				var products = result.ToArray();
				var PageNumber = page;
				var PageSize = 10;
				var PageData = products.OrderBy(p => p.Id).ToPagedList(PageSize, PageNumber);
				IPagedList PagerSource=result.ToPagedList(PageSize,page);

				foreach (var i in products)
				{
					FileCount = FileCount + 1;
					//FileSize = FileSize + i.Size;
					if (i.IsShared==true)
					{
						ShareCount = ShareCount + 1;
					}
				}

				ViewData["datashow"] = PageData;
				ViewData["filecount"] = FileCount;
				ViewData["filesize"] = FileSize;
				ViewData["sharecount"] = ShareCount;
				ViewData["pagersource"] = PagerSource;

				return View();
			}
			
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
		///     当网站发生错误时显示的页面。
		/// </summary>
		/// <returns>操作结果。</returns>
		public IActionResult Error()
		{
			return View();
		}

		public IActionResult Search(SearchModeViewModel search, int? page,string username)
		{
			/*
						if (Select == "1")
						{
							var result1 = from i in UserDb.Items where i.Name == keywords select i;
							ShareItem[] ChangedResult1 = result1.ToArray();
							return View(ChangedResult1);
						}
						else if (Select == "0")
						{
							var result2 = from i in UserDb.Items where i.UserName == keywords select i;
							ShareItem[] ChangedResult2 = result2.ToArray();
							return View(ChangedResult2);
						}
						else
							return Index();
			*/
			IQueryable<ShareItem> result;
            result = from i in UserDb.Items orderby i.Id where i.Name == search.Words && i.UserName ==username select i;
						

					

				ViewData["List"] = result.ToArray();
				ViewData["CHECK"] = search.Words;

				//TempData["list"] = result.ToArray();
				ViewData["SEARCH"] = search;
			    ViewData["Check"] = 1;
				var products = result.ToArray();
            //returns IQueryable<Product> representing an unknown number of products. a thousand maybe?

            //var pageNumber = page ?? 1; if no page was specified in the querystring, default to the first page (1)
            var pageNumber = page ?? 1;
            var pageSize = 10;


            var pageData = products.OrderBy(p => p.Id).ToPagedList(pageSize, pageNumber);
            ViewData["datasource"] = pageData;
            //.ToPagedList(pageNumber, 4);  will only contain 25 products max because of the pageSize

            // ViewBag.onePageOfProducts = onePageOfProducts;

            /*	var ChangedResult = result.ToArray();*/
            return View();
			}
		
		
	}
}