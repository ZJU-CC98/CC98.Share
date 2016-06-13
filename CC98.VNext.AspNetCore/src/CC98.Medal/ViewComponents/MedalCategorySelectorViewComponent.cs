using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.Medal.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CC98.Medal.ViewComponents
{
	public class MedalCategorySelectorViewComponent : ViewComponent
	{
		public MedalCategorySelectorViewComponent(MedalDataModel dataModel)
		{
			DataModel = dataModel;
		}

		/// <summary>
		/// 数据库连接对象。
		/// </summary>
		private MedalDataModel DataModel { get; }

		/// <summary>
		/// 显示勋章分类选择列表，选择器使用给定的名称作为字段名。
		/// </summary>
		/// <param name="modelName">字段名。</param>
		/// <returns>操作结果。</returns>
		public async Task<IViewComponentResult> InvokeAsync(string modelName)
		{
			var items = from i in DataModel.MedalCategories
						select i;

			ViewBag.ModelName = modelName;

			return View(await items.ToArrayAsync());
		}
	}
}
