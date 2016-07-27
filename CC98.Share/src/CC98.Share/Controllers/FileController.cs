using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CC98.Share.Data;
using CC98.Share.ViewModels.FileController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sakura.AspNetCore;
using Sakura.AspNetCore.Authentication;

namespace CC98.Share.Controllers
{
	/// <summary>
	///     提供网站基本的上传和下载功能。
	/// </summary>
	public class FileController : Controller
	{
		public FileController(IHostingEnvironment environment, CC98ShareModel model,
			ExternalSignInManager externalSignInManager, IOptions<Setting> setting)
		{
			Environment = environment;
			Model = model;
			ExternalSignInManager = externalSignInManager;
			Setting = setting;
		}

		private ExternalSignInManager ExternalSignInManager { get; }

		private CC98ShareModel Model { get; }

		private IHostingEnvironment Environment { get; }

		private IOptions<Setting> Setting { get; }

		/// <summary>
		///     显示上传界面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize]
		public IActionResult Upload()
		{
			return View();
		}

		/// <summary>
		///     显示下载界面。
		/// </summary>
		/// <returns>操作结果。</returns>
		/// <summary>
		///     提供下载功能。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Route("Download/{id}")]
		public IActionResult Download(int id)
		{
			try
			{
				//将文件在数据库中的标识传入函数。
				//并在数据库中找到此文件，返回给用户
				var output = from i in Model.Items where i.Id == id select i;
				var result = output.SingleOrDefault();
				if (result != null && result.IsShared || result != null && result.UserName == User.Identity.Name)
				{
					var addressName = Setting.Value.StoreFolder + result.Path;
					return PhysicalFile(addressName, "application/octet-stream", result.Name);
				}
				return StatusCode(403);
			}
			catch
			{
				return StatusCode(404);
			}
		}

		/// <summary>
		///     提供删除功能。
		/// </summary>
		/// <returns>操作结果。</returns>
		//[ValidateAntiForgeryToken]
		[HttpPost]
		public async Task<IActionResult> DeleteFile(int id)
		{
			try
			{
				//将文件在数据库中的标识传入函数。
				//并在数据库中找到此文件，删除
				var output = from i in Model.Items where i.Id == id select i;
				var result = output.SingleOrDefault();
				if (result != null && result.UserName == User.Identity.Name)
				{
					Model.Items.Remove(result);
					await Model.SaveChangesAsync();
					return RedirectToAction("Index", "Home");
				}
				return StatusCode(403);
			}
			catch
			{
				return StatusCode(404);
			}
		}

		/// <summary>
		///     提供分享功能。
		/// </summary>
		/// <returns>操作结果。</returns>
		//[ValidateAntiForgeryToken]
		[HttpPost]
		public async Task<IActionResult> ShareFile(int id)
		{
			try
			{
				//将文件在数据库中的标识传入函数。
				//并在数据库中找到此文件，改为允许分享
				var output = from i in Model.Items where i.Id == id select i;
				var result = output.SingleOrDefault();
				if (result != null && result.UserName == User.Identity.Name)
				{
					result.IsShared = true;
					await Model.SaveChangesAsync();
					return RedirectToAction("Index", "Home");
				}
				return StatusCode(403);
			}
			catch
			{
				return StatusCode(404);
			}
		}

		/// <summary>
		///     提供取消分享功能。
		/// </summary>
		/// <returns>操作结果。</returns>
		//[ValidateAntiForgeryToken]
		[HttpPost]
		public async Task<IActionResult> CancelShare(int id)
		{
			try
			{
				//将文件在数据库中的标识传入函数。
				//并在数据库中找到此文件，改为允许分享
				var output = from i in Model.Items where i.Id == id select i;
				var result = output.SingleOrDefault();
				if (result != null && result.UserName == User.Identity.Name)
				{
					result.IsShared = false;
					await Model.SaveChangesAsync();
					return RedirectToAction("Index", "Home");
				}
				return StatusCode(403);
			}
			catch
			{
				return StatusCode(404);
			}
		}

		/// <summary>
		///     获取ContentDisposition中的文件名称。
		/// </summary>
		/// <returns>操作结果。</returns>
		public string GetFileName(string fileName)
		{
			var extension = Path.GetExtension(fileName);
			var file = Path.GetFileNameWithoutExtension(fileName);

			int extLength;
			if (extension.Length >= Setting.Value.FileNameLengh)
			{
				extLength = Setting.Value.FileNameLengh - 1;
			}
			else
			{
				extLength = extension.Length;
			}

			var fileLength = Setting.Value.FileNameLengh - extLength;

			if (fileLength > file.Length)
			{
				fileLength = file.Length;
			}

			return Path.ChangeExtension(file.Substring(0, fileLength), extension.Substring(0, extLength));
		}

		/// <summary>
		///     显示上传界面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[Authorize]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public async Task<IActionResult> Upload(UploadViewModel inputModel, [FromServices] IOperationMessageAccessor accessor)
		{
			// 首先检测是否有文件传入函数。
			if (inputModel.Files == null || inputModel.Files.Length == 0)
			{
				return StatusCode(400);
			}
			//随机生成一个文件名字，并将此文件插入数据库。
			var value = inputModel.Value;

			{
				foreach (var file in inputModel.Files)
				{
					var fileNameRandom = Path.GetRandomFileName();

					var tm = new ShareItem
					{
						Path = "\\" + fileNameRandom
					};

					var saveFileName = Path.Combine(Setting.Value.StoreFolder, fileNameRandom);

					using (var stream = System.IO.File.OpenWrite(saveFileName))
					{
						await file.CopyToAsync(stream);
					}
					tm.Size = file.Length;

					var currentUserName = ExternalSignInManager.GetUserName(User);

					var currentUserItems = from i in Model.Items
										   where i.UserName == currentUserName
										   select i.Size;

					var totalSize = currentUserItems.Sum();

					totalSize += tm.Size;

					if (tm.Size > Setting.Value.UserOnceSize)
					{
						accessor.Messages.Add(OperationMessageLevel.Error, "错误", "上传文件大小超过单次上传上限");
						return RedirectToAction("Index", "Home");
					}

					if (totalSize > Setting.Value.UserTotalSize)
					{
						accessor.Messages.Add(OperationMessageLevel.Error, "错误", "上传文件总大小超过网盘容量");
						return RedirectToAction("Index", "Home");
					}

					tm.Name = Path.GetFileName(file.FileName);
					tm.UserName = ExternalSignInManager.GetUserName(User);
					tm.IsShared = value;
					tm.UploadTime = DateTimeOffset.UtcNow;
					Model.Items.Add(tm);

					await Model.SaveChangesAsync();
				}
				return RedirectToAction("Index", "Home");
			}

		}
	}
}