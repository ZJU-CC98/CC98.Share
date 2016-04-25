using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using CC98.Share.Models;
using PagedList;

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
		CC98ShareModel UserDb = new CC98ShareModel();
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Search(SearchModeViewModels search,int ?page)
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
						result = from i in UserDb.Items orderby i.Id where i.Description == search.Words && i.Name == search.Words select i;
						break;

					default:
						return Index();
						break;

				}

				ViewData["List"] = result.ToArray();
				ViewData["CHECK"] = search.Words;
				//TempData["list"] = result.ToArray();
				ViewData["SEARCH"] = search;

				var products = result.ToArray(); //returns IQueryable<Product> representing an unknown number of products. a thousand maybe?

				var pageNumber =page ?? 1; // if no page was specified in the querystring, default to the first page (1)
				var onePageOfProducts = products.OrderBy(p => p.Id).ToPagedList(pageNumber, 4); // will only contain 25 products max because of the pageSize

				ViewBag.onePageOfProducts = onePageOfProducts;

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
						result = from i in UserDb.Items orderby i.Id where i.Description.Contains(search.Words) && i.Name.Contains(search.Words) select i;
						break;

					default:
						return Index();
						break;

				}

				ViewData["List"] = result.ToArray();
				ViewData["CHECK"] = search.Words;
			//	TempData["list"] = result.ToArray();
				ViewData["SEARCH"] = search;

				var products = result.ToArray(); //returns IQueryable<Product> representing an unknown number of products. a thousand maybe?

				var pageNumber =page ?? 1; // if no page was specified in the querystring, default to the first page (1)
				var onePageOfProducts = products.OrderBy(p=>p.Id).ToPagedList(pageNumber, 4); // will only contain 25 products max because of the pageSize

				ViewBag.onePageOfProducts = onePageOfProducts;

				/*	var ChangedResult = result.ToArray();*/
				return View();
			}
			


		}


		/*  foreach(var item in result)
		  {   

			  TempData.Add("SearchResult", item.Name);
			  TempData.Add("SearchResultPath", item.Path);
		  }    */
		/*  return View(result);*/


		// GET: User/1




















	}
}

