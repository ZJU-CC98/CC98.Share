using System;
using Microsoft.AspNet.Mvc;

namespace CC98.Sports.Components
{
	/// <summary>
	/// 文件列表服务
	/// </summary>
	public class FileListViewComponent : ViewComponent
	{
		public IViewComponentResult Invoke(string fileList)
		{
			var item = (fileList ?? string.Empty).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
			return View(item);
		}
	}
}
