using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.PlatformAbstractions;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CC98.Sports.Controllers
{
	/// <summary>
	/// 上传文件的控制器。
	/// </summary>
	public class FileController : Controller
	{
		/// <summary>
		/// 获取应用程序设置。
		/// </summary>
		private AppSetting AppSetting { get; }

		/// <summary>
		/// 宿主环境对象。
		/// </summary>
		private IHostingEnvironment Enviroment { get; }

		/// <summary>
		/// 应用程序环境对象。
		/// </summary>
		private IApplicationEnvironment AppEnvironment { get; set; }

		/// <summary>
		/// 初始化一个控制器的新实例。
		/// </summary>
		/// <param name="environment">宿主环境对象。</param>
		/// <param name="appSetting">应用程序设置。</param>
		public FileController(IHostingEnvironment environment, IOptions<AppSetting> appSetting)
		{
			Enviroment = environment;
			AppSetting = appSetting.Value;
		}

		/// <summary>
		/// 上传一个制定的文件，并返回服务器端的文件名。
		/// </summary>
		/// <param name="files">要上传的文件。</param>
		/// <returns>服务器端的文件名。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Upload(IFormFile[] files)
		{
			var fileResultList = new List<SingleFileResultBase>();

			// 最大尺寸
			var maxSize = 1024 * 1024L;

			foreach (var file in files)
			{
				if (file.Length > maxSize)
				{
					fileResultList.Add(new SingleFileErrorResult
					{
						Name = file.GetFileName(),
						Size = file.Length,
						Error = "文件尺寸过大"
					});
				}

				else
				{
					// 随机文件名，保留原始扩展名
					var fileName = Path.GetRandomFileName();
					fileName = Path.ChangeExtension(fileName, Path.GetExtension(file.GetFileName()));

					// 上传的完整路径
					var fullPath = Path.Combine(AppSetting.UploadRootFolder, fileName).Replace(Path.DirectorySeparatorChar, '/');

					// 文件存储路径
					var filePath = Path.Combine(AppSetting.UploadPhysicalPath, fileName);

					// 创建目录
					// ReSharper disable once AssignNullToNotNullAttribute
					Directory.CreateDirectory(Path.GetDirectoryName(filePath));

					// 保存文件
					await file.SaveAsAsync(filePath);

					var newItem = new SingleFileResult
					{
						Name = fileName,
						Size = file.Length,
						Url = fullPath,
						DeleteUrl = null,
						DeleteType = null
					};

					fileResultList.Add(newItem);
				}

			}

			var result = new FileResult
			{
				Files = fileResultList.ToArray()
			};

			var settings = new Newtonsoft.Json.JsonSerializerSettings
			{
				ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
			};

			return Json(result, settings);
		}

		private class FileResult
		{
			public SingleFileResultBase[] Files { get; set; }
		}

		private class SingleFileResultBase
		{
			public string Name { get; set; }
			public long Size { get; set; }
		}

		private class SingleFileResult : SingleFileResultBase
		{
			public string Url { get; set; }
			public string ThumbnailUrl { get; set; }

			public string DeleteUrl { get; set; }

			public string DeleteType { get; set; }
		}

		private class SingleFileErrorResult : SingleFileResultBase
		{
			public string Error { get; set; }
		}
	}
}
