using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using System.IO;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using System.Security.Claims;
using Microsoft.AspNet.Authorization;
using System.Threading.Tasks;
using CC98.Share.ViewModels;
using Microsoft.Extensions.OptionsModel;

namespace CC98.Share.Controllers
{
	/// <summary>
	/// 提供网站基本的上传和下载功能。
	/// </summary>
	public class FileController : Controller
	{
		/// <summary>
		/// 显示上传界面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize]
		public IActionResult Upload()
		{
			return View();

		}
		/// <summary>
		/// 显示下载界面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		public IActionResult Download()
		{
			return View();
		}
		[FromServices]
		public CC98ShareModel Model
		{
			get;
			set;
		}
		[FromServices]
		public IHostingEnvironment Address
		{
			get;
			set;
		}
		[FromServices]
		public IOptions<Setting> Folder
		{
			get;
			set;
		}
		/// <summary>
		/// 提供下载功能。
		/// </summary>
		/// <returns>操作结果。</returns>
		[ValidateAntiForgeryToken]
		[HttpPost]
		public IActionResult Download(int id)
		{
			//try
		   // {
				//将文件在数据库中的标识传入函数。
				//并在数据库中找到此文件，返回给用户
				var output = from i in Model.Items where i.Id == id select i;
				var result = output.SingleOrDefault();
				if (result != null)
				{
					string addressName = result.Path;
					return File(addressName, "application/octet-stream");
				}
				else
				{
					return new HttpStatusCodeResult(404);
				}
		   // }
			//catch
			//{
			//    return new HttpStatusCodeResult(404);
			//}
		}
		/// <summary>
		/// 获取ContentDisposition中的文件名称。
		/// </summary>
		/// <returns>操作结果。</returns>
		public string GetFileName(string contentDisposition)
		{
			//获取文件名称的起始位置。
			var startIndex = contentDisposition.IndexOf("filename=\"", StringComparison.OrdinalIgnoreCase) + 10;
			//获取文件名称的结尾位置。
			var endIndex = contentDisposition.LastIndexOf("\"", StringComparison.OrdinalIgnoreCase);
			var length = endIndex - startIndex;
			//通过起始位置和结尾位置获得名称。
			var fileName = contentDisposition.Substring(startIndex, length);
			return fileName;
		}
		/// <summary>
		/// 显示上传界面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[Authorize]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public async Task<IActionResult> Upload(IFormFile file)
		{
			//首先检测是否有文件传入函数。
			if (file == null)
			{
				return new HttpStatusCodeResult(404);
			}
			//随机生成一个文件名字，并将此文件插入数据库。
			string fileNameRandom = Path.GetRandomFileName();
			var fileName = Path.Combine(Address.MapPath("Upload"), fileNameRandom);
			ShareItem tm = new ShareItem();
			//try
			//{
				string s = GetFileName(file.ContentDisposition);
				tm.Path = "Upload" + "\\" + fileNameRandom;
				file.SaveAs(Folder.Value.StoreFolder + "\\" + fileNameRandom);
				tm.Name = s;
				tm.UserName = User.GetUserName();
				Model.Items.Add(tm);
				await Model.SaveChangesAsync();
				return new HttpStatusCodeResult(200);
			//}
			//catch
			//{
			//    return new HttpStatusCodeResult(404);
			//}
		}
	}
}
