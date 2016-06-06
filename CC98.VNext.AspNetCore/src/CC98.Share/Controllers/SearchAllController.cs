using System.Linq;
using CC98.Share.Data;
using CC98.Share.ViewModels.SearchAll;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CC98.Share.Controllers
{
    public class SearchAllController : Controller
    {
        public SearchAllController(IHostingEnvironment environment, CC98ShareModel userDb)
        {
            Environment = environment;
            UserDb = userDb;
        }

        private IHostingEnvironment Environment { get; }

        private CC98ShareModel UserDb { get; }

        // GET: /<controller>/
        public IActionResult Index()
        {
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

        public IActionResult Search(SearchModeViewModel search, int? page)
        {
            IQueryable<ShareItem> result;

            //if (username == this.User.Identity.Name)
            //{
            //    if (search.Acc == Accuracy.Accurate)
            //    {
            //        switch (search.Mode)
            //        {
            //            case Valuation.UserName:
            //                result = from i in UserDb.Items
            //                    orderby i.Id
            //                    where i.UserName == search.Words
            //                    select i;
            //                break;


            //            case Valuation.FileName:
            //                result = from i in UserDb.Items
            //                    orderby i.Id
            //                    where i.Name == search.Words
            //                    select i;
            //                break;

            //            case Valuation.Discription:
            //                result = from i in UserDb.Items
            //                    orderby i.Id
            //                    where i.Description == search.Words
            //                    select i;
            //                break;

            //            case Valuation.FileNameAndDis:
            //                result = from i in UserDb.Items
            //                    orderby i.Id
            //                    where i.Description == search.Words && i.Name == search.Words
            //                    select i;
            //                break;

            //            default:
            //                return Index();
            //        }

            //        ViewData["List"] = result.ToArray();
            //        ViewData["CHECK"] = search.Words;

            //        //TempData["list"] = result.ToArray();
            //        ViewData["SEARCH"] = search;

            //        var products = result.ToArray();
            //        //returns IQueryable<Product> representing an unknown number of products. a thousand maybe?

            //        //var pageNumber = page ?? 1; if no page was specified in the querystring, default to the first page (1)

            //        //.ToPagedList(pageNumber, 4);  will only contain 25 products max because of the pageSize

            //        // ViewBag.onePageOfProducts = onePageOfProducts;
            //        var pageNumber = page ?? 1;
            //        var pageSize = 10;


            //        var pageData = products.OrderBy(p => p.Id).ToPagedList(pageSize, pageNumber);
            //        /*	var ChangedResult = result.ToArray();*/
            //        ViewData["datasource"] = pageData;
            //        return View();
            //    }
            //    else
            //    {
            //        switch (search.Mode)
            //        {
            //            case Valuation.UserName:
            //                result = from i in UserDb.Items
            //                    orderby i.Id
            //                    where i.UserName.Contains(search.Words)
            //                    select i;
            //                break;


            //            case Valuation.FileName:
            //                result = from i in UserDb.Items
            //                    orderby i.Id
            //                    where i.Name.Contains(search.Words)
            //                    select i;
            //                break;

            //            case Valuation.Discription:
            //                result = from i in UserDb.Items
            //                    orderby i.Id
            //                    where i.Description.Contains(search.Words)
            //                    select i;
            //                break;

            //            case Valuation.FileNameAndDis:
            //                result = from i in UserDb.Items
            //                    orderby i.Id
            //                    where
            //                        i.Description.Contains(search.Words) && i.Name.Contains(search.Words) &&
            //                        i.IsShared
            //                    select i;
            //                break;

            //            default:
            //                return Index();
            //        }

            //        ViewData["List"] = result.ToArray();
            //        ViewData["CHECK"] = search.Words;
            //        // TempData["list"] = result.ToArray();
            //        ViewData["SEARCH"] = search;


            //        //returns IQueryable<Product> representing an unknown number of products. a thousand maybe?
            //        var products = result.ToArray();
            //        //var pageNumber = page ?? 1;  if no page was specified in the querystring, default to the first page (1)


            //        // ViewBag.onePageOfProducts = onePageOfProducts;

            //        /*	var ChangedResult = result.ToArray();*/
            //        var pageNumber = page ?? 1;
            //        var pageSize = 10;


            //        var pageData = products.OrderBy(p => p.Id).ToPagedList(pageSize, pageNumber);
            //        ViewData["datasource"] = pageData;
            //        return View();
            //    }
            //}

            if (search.Acc == Accuracy.Accurate)
            {
                switch (search.Mode)
                {
                    case Valuation.UserName:
                        result = from i in UserDb.Items
                                 orderby i.Id
                                 where i.UserName == search.Words && (i.IsShared || i.UserName == User.Identity.Name)
                                 select i;
                        break;


                    case Valuation.FileName:
                        result = from i in UserDb.Items
                                 orderby i.Id
                                 where i.Name == search.Words && (i.IsShared || i.UserName == User.Identity.Name)
                                 select i;
                        break;

                    case Valuation.Discription:
                        result = from i in UserDb.Items
                                 orderby i.Id
                                 where i.Description == search.Words && (i.IsShared || i.UserName == User.Identity.Name)
                                 select i;
                        break;

                    case Valuation.FileNameAndDis:
                        result = from i in UserDb.Items
                                 orderby i.Id
                                 where
                                 i.Description == search.Words && i.Name == search.Words &&
                                 (i.IsShared || i.UserName == User.Identity.Name)
                                 select i;
                        break;

                    default:
                        return Index();
                }

                ViewData["List"] = result.ToArray();
                ViewData["CHECK"] = search.Words;

                //TempData["list"] = result.ToArray();
                ViewData["SEARCH"] = search;

                var products = result.ToArray();
                //returns IQueryable<Product> representing an unknown number of products. a thousand maybe?

                //var pageNumber = page ?? 1; if no page was specified in the querystring, default to the first page (1)

                //.ToPagedList(pageNumber, 4);  will only contain 25 products max because of the pageSize

                // ViewBag.onePageOfProducts = onePageOfProducts;

                /*	var ChangedResult = result.ToArray();*/
                var pageNumber = page ?? 1;
                var pageSize = 10;


                var pageData = products.OrderBy(p => p.Id).ToPagedList(pageSize, pageNumber);
                ViewData["datasource"] = pageData;
                return View();
            }
            else
            {
                switch (search.Mode)
                {
                    case Valuation.UserName:
                        result = from i in UserDb.Items
                                 orderby i.Id
                                 where
                                 i.UserName.Contains(search.Words) && (i.IsShared || i.UserName == User.Identity.Name)
                                 select i;
                        break;


                    case Valuation.FileName:
                        result = from i in UserDb.Items
                                 orderby i.Id
                                 where i.Name.Contains(search.Words) && (i.IsShared || i.UserName == User.Identity.Name)
                                 select i;
                        break;

                    case Valuation.Discription:
                        result = from i in UserDb.Items
                                 orderby i.Id
                                 where
                                 i.Description.Contains(search.Words) &&
                                 (i.IsShared || i.UserName == User.Identity.Name)
                                 select i;
                        break;

                    case Valuation.FileNameAndDis:
                        result = from i in UserDb.Items
                                 orderby i.Id
                                 where
                                 i.Description.Contains(search.Words) && i.Name.Contains(search.Words) &&
                                 (i.IsShared || i.UserName == User.Identity.Name)
                                 select i;
                        break;

                    default:
                        return Index();
                }

                ViewData["List"] = result.ToArray();
                ViewData["CHECK"] = search.Words;
                // TempData["list"] = result.ToArray();
                ViewData["SEARCH"] = search;


                //returns IQueryable<Product> representing an unknown number of products. a thousand maybe?

                //var pageNumber = page ?? 1;  if no page was specified in the querystring, default to the first page (1)


                // ViewBag.onePageOfProducts = onePageOfProducts;

                /*	var ChangedResult = result.ToArray();*/
                var products = result.ToArray();
                var pageNumber = page ?? 1;
                var pageSize = 10;

                var pageData = products.OrderBy(p => p.Id).ToPagedList(pageSize, pageNumber);
                ViewData["datasource"] = pageData;
                return View();
            }
        }
    }
}