using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.Share.Models;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Sakura.AspNet;
using Sakura.AspNet.Mvc;
using Sakura.AspNet.Mvc.PagedList;


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

        [FromServices]
        public IHostingEnvironment Address { get; set; }

        [FromServices]
        public CC98ShareModel UserDb { get; set; }

        /// <summary>
        /// 当网站发生错误时显示的页面。
        /// </summary>
        /// <returns>操作结果。</returns>
        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Search(SearchModeViewModels search, int? page)
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


            if (search.Acc == Accuracy.Accurate)
            {
                switch (search.Mode)
                {
                    case Valuation.UserName:
                        result = from i in UserDb.Items orderby i.Id where i.UserName == search.Words select i;
                        break;


                    case Valuation.FileName:
                        result = from i in UserDb.Items orderby i.Id where i.Name == search.Words select i;
                        break;

                    case Valuation.Discription:
                        result = from i in UserDb.Items orderby i.Id where i.Description == search.Words select i;
                        break;

                    case Valuation.FileNameAndDis:
                        result = from i in UserDb.Items
                            orderby i.Id
                            where i.Description == search.Words && i.Name == search.Words
                            select i;
                        break;

                    default:
                        return Index();
                        break;

                }

                ViewData["List"] = result.ToArray();
                ViewData["CHECK"] = search.Words;

                //TempData["list"] = result.ToArray();
                ViewData["SEARCH"] = search;

                var products = result.ToArray();
                //returns IQueryable<Product> representing an unknown number of products. a thousand maybe?

                //var pageNumber = page ?? 1; if no page was specified in the querystring, default to the first page (1)
                var onePageOfProducts = products.OrderBy(p => p.Id);
                    //.ToPagedList(pageNumber, 4);  will only contain 25 products max because of the pageSize

                // ViewBag.onePageOfProducts = onePageOfProducts;

                /*	var ChangedResult = result.ToArray();*/
                return View();

            }
            else
            {
                switch (search.Mode)
                {
                    case Valuation.UserName:
                        result = from i in UserDb.Items orderby i.Id where i.UserName.Contains(search.Words) select i;
                        break;


                    case Valuation.FileName:
                        result = from i in UserDb.Items orderby i.Id where i.Name.Contains(search.Words) select i;
                        break;

                    case Valuation.Discription:
                        result = from i in UserDb.Items orderby i.Id where i.Description.Contains(search.Words) select i;
                        break;

                    case Valuation.FileNameAndDis:
                        result = from i in UserDb.Items
                            orderby i.Id
                            where i.Description.Contains(search.Words) && i.Name.Contains(search.Words)
                            select i;
                        break;

                    default:
                        return Index();
                        break;

                }

                ViewData["List"] = result.ToArray();
                ViewData["CHECK"] = search.Words;
             // TempData["list"] = result.ToArray();
                ViewData["SEARCH"] = search;


                //returns IQueryable<Product> representing an unknown number of products. a thousand maybe?
                var products = result.ToArray();
                //var pageNumber = page ?? 1;  if no page was specified in the querystring, default to the first page (1)
                var onePageOfProducts = products.OrderBy(p => p.Id);

                // ViewBag.onePageOfProducts = onePageOfProducts;

                /*	var ChangedResult = result.ToArray();*/
                var pageNumber = 1;
                var pageSize = 10;
             
             
                var pageData = products.ToPagedList(pageSize, pageNumber);
                return View();
            }
        }

     

    }

}
    