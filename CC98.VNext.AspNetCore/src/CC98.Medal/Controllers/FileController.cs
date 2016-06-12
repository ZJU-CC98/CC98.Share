using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Editor;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CC98.Medal.Controllers
{
	/// <summary>
	/// 提供文件服务。
	/// </summary>
	public class FileController : Controller
	{
		public FileController(IOptions<AppSetting> appSetting)
		{
			AppSetting = appSetting.Value;
		}

		private AppSetting AppSetting { get; }

		/// <summary>
		/// 提供上传文件服务。
		/// </summary>
		/// <param name="file">上传的文件。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policies.Edit)]
		public async Task<IActionResult> Upload(IFormFile file)
		{
			// 随机名称，保持原有后缀
			var randomName = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(file.FileName));
			var storePath = Path.Combine(AppSetting.MedalImageStoreFolder, randomName);
			var displayPath = new PathString(AppSetting.MedalImagePath) + new PathString("/" + randomName);

			using (var stream = System.IO.File.OpenWrite(storePath))
			{
				try
				{
					await file.CopyToAsync(stream);
					return Ok(displayPath.ToString());
				}
				catch
				{
					// ignored
				}
			}

			return BadRequest();

		}
	}
}
