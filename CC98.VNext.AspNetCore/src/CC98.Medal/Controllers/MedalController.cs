using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.Medal.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sakura.AspNetCore;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CC98.Medal.Controllers
{
	/// <summary>
	/// 提供勋章相关功能。
	/// </summary>
	public class MedalController : Controller
	{
		public MedalController(MedalDataModel dataModel)
		{
			DataModel = dataModel;
		}

		/// <summary>
		/// 数据库连接对象。
		/// </summary>
		private MedalDataModel DataModel { get; }

		/// <summary>
		/// 显示勋章主页。
		/// </summary>
		/// <returns>操作结果。</returns>
		[AllowAnonymous]
		public IActionResult Index(int page = 1)
		{
			var items = from i in DataModel.Medals.Include(p => p.Category)
						select i;

			return View(items.ToPagedList(20, page));
		}

		/// <summary>
		/// 创建新勋章。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(Policies.Edit)]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policies.Edit)]
		public IActionResult Create(Data.Medal model)
		{
			return RedirectToAction("Index", "Medal");
		}
	}
}
