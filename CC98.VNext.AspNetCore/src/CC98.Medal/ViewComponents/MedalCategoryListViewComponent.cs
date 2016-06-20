using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.Medal.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CC98.Medal.ViewComponents
{
	/// <summary>
	/// 显示勋章分类列表的视图组件。
	/// </summary>
	public class MedalCategoryListViewComponent : ViewComponent
	{
		public MedalCategoryListViewComponent(MedalDataModel dataModel)
		{
			DataModel = dataModel;
		}

		/// <summary>
		/// 数据库连接对象。
		/// </summary>
		private MedalDataModel DataModel { get; }

		/// <summary>
		/// 异步执行调用。
		/// </summary>
		/// <returns>表示操作结果的异步任务。</returns>
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var items = from i in DataModel.MedalCategories
						orderby i.Name ascending
						select i;

			return View(await items.ToArrayAsync());
		}
	}
}
