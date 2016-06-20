using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CC98.Medal.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Sakura.AspNetCore;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CC98.Medal.Controllers
{
	/// <summary>
	/// 提供勋章相关功能。
	/// </summary>
	public class MedalController : Controller
	{
		public MedalController(MedalDataModel dataModel, IOperationMessageAccessor messageAccessor)
		{
			DataModel = dataModel;
			MessageAccessor = messageAccessor;
		}

		/// <summary>
		/// 数据库连接对象。
		/// </summary>
		private MedalDataModel DataModel { get; }

		private IOperationMessageAccessor MessageAccessor { get; }

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

		/// <summary>
		/// 执行添加勋章操作。
		/// </summary>
		/// <param name="model">数据模型。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policies.Edit)]
		public async Task<IActionResult> Create(Data.Medal model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					DataModel.Medals.Add(model);
					await DataModel.SaveChangesAsync();

					MessageAccessor.Messages.Add(OperationMessageLevel.Success, "操作成功。",
						string.Format(CultureInfo.CurrentUICulture, "你已经成功添加了勋章 {0}。", model.Name));

					return RedirectToAction("Index", "Medal");
				}
				catch (Exception ex)
				{
					ModelState.AddModelError(string.Empty, ex.Message);
				}
			}

			return View(model);
		}

	}
}
