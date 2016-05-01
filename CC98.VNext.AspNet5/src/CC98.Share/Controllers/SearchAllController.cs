using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.Share.Models;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet;
using Sakura.AspNet;
using JetBrains.Annotations;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CC98.Share.Controllers
{
    public class SearchAllController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
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

        public IActionResult Search(SearchModeViewModels search, int? page, string username)
        {


            IQueryable<ShareItem> result;

            if (username == User.Identity.Name)
            {
                if (search.Acc == Accuracy.Accurate)
                {
                    switch (search.Mode)
                    {
                        case Valuation.UserName:
                            result = from i in UserDb.Items
                                orderby i.Id
                                where i.UserName == search.Words
                                select i;
                            break;


                        case Valuation.FileName:
                            result = from i in UserDb.Items
                                orderby i.Id
                                where i.Name == search.Words
                                select i;
                            break;

                        case Valuation.Discription:
                            result = from i in UserDb.Items
                                orderby i.Id
                                where i.Description == search.Words
                                select i;
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

                    //.ToPagedList(pageNumber, 4);  will only contain 25 products max because of the pageSize

                    // ViewBag.onePageOfProducts = onePageOfProducts;
                    var pageNumber =page ?? 1;
                    var pageSize = 10;


                    var pageData = products.OrderBy(p=>p.Id).ToPagedList(pageSize, pageNumber);
                    /*	var ChangedResult = result.ToArray();*/
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
                                where i.UserName.Contains(search.Words)
                                select i;
                            break;


                        case Valuation.FileName:
                            result = from i in UserDb.Items
                                orderby i.Id
                                where i.Name.Contains(search.Words)
                                select i;
                            break;

                        case Valuation.Discription:
                            result = from i in UserDb.Items
                                orderby i.Id
                                where i.Description.Contains(search.Words)
                                select i;
                            break;

                        case Valuation.FileNameAndDis:
                            result = from i in UserDb.Items
                                orderby i.Id
                                where
                                    i.Description.Contains(search.Words) && i.Name.Contains(search.Words) &&
                                    i.IsShared == true
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
                    

                    // ViewBag.onePageOfProducts = onePageOfProducts;

                    /*	var ChangedResult = result.ToArray();*/
                    var pageNumber =page ?? 1;
                    var pageSize = 10;


                    var pageData = products.OrderBy(p=>p.Id).ToPagedList(pageSize, pageNumber);
                    ViewData["datasource"] = pageData;
                    return View();
                }
            }
            else 
            {
                if (search.Acc == Accuracy.Accurate)
                {
                    switch (search.Mode)
                    {
                        case Valuation.UserName:
                            result = from i in UserDb.Items
                                orderby i.Id
                                where i.UserName == search.Words && i.IsShared == true
                                select i;
                            break;


                        case Valuation.FileName:
                            result = from i in UserDb.Items
                                orderby i.Id
                                where i.Name == search.Words && i.IsShared == true
                                select i;
                            break;

                        case Valuation.Discription:
                            result = from i in UserDb.Items
                                orderby i.Id
                                where i.Description == search.Words && i.IsShared == true
                                select i;
                            break;

                        case Valuation.FileNameAndDis:
                            result = from i in UserDb.Items
                                orderby i.Id
                                where i.Description == search.Words && i.Name == search.Words && i.IsShared == true
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
                                where i.UserName.Contains(search.Words) && i.IsShared == true
                                select i;
                            break;


                        case Valuation.FileName:
                            result = from i in UserDb.Items
                                orderby i.Id
                                where i.Name.Contains(search.Words) && i.IsShared == true
                                select i;
                            break;

                        case Valuation.Discription:
                            result = from i in UserDb.Items
                                orderby i.Id
                                where i.Description.Contains(search.Words) && i.IsShared == true
                                select i;
                            break;

                        case Valuation.FileNameAndDis:
                            result = from i in UserDb.Items
                                orderby i.Id
                                where
                                    i.Description.Contains(search.Words) && i.Name.Contains(search.Words) &&
                                    i.IsShared == true
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
                    
                    //var pageNumber = page ?? 1;  if no page was specified in the querystring, default to the first page (1)
                    

                    // ViewBag.onePageOfProducts = onePageOfProducts;
                    
                    /*	var ChangedResult = result.ToArray();*/
                    var products = result.ToArray();
                    var onePageOfProducts = products.OrderBy(p => p.Id);
                    var pageNumber =page ?? 1;
                    var pageSize = 10;


                    var pageData = products.OrderBy(p => p.Id).ToPagedList(pageSize, pageNumber);
                    ViewData["datasource"] = pageData;
                    return View();
                }




            }
        }
    }
}
