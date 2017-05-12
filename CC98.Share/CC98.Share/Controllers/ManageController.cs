using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CC98.Share.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sakura.AspNetCore;

namespace CC98.Share.Controllers
{
	/// <summary>
	/// 管理相关功能。
	/// </summary>
	public class ManageController : Controller
	{
		public ManageController(CC98ShareModel dbContext, IOptions<Setting> setting, IOperationMessageAccessor messageAccessor)
		{
			DbContext = dbContext;
			MessageAccessor = messageAccessor;
			Setting = setting.Value;
		}

		/// <summary>
		/// 数据库上下文对象。
		/// </summary>
		private CC98ShareModel DbContext { get; }

		private Setting Setting { get; }

		private IOperationMessageAccessor MessageAccessor { get; }

		/// <summary>
		/// 显示管理概览页面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[Authorize(Policies.Administrate)]
		public async Task<IActionResult> Overview()
		{
			ViewBag.UsedSpace = await DbContext.Items.Select(i => i.Size).SumAsync();
			var drive = new DriveInfo(Path.GetPathRoot(Setting.StoreFolder));
			ViewBag.FreeSpace = drive.AvailableFreeSpace;

			return View();
		}

		private async Task<FileInfo[]> GetRemainFilesAsync()
		{
			var existFiles = new HashSet<string>(Directory.GetFiles(Setting.StoreFolder).Select(Path.GetFileName), StringComparer.OrdinalIgnoreCase);
			var databaseRecords = await DbContext.Items.Select(i => i.Path).ToArrayAsync();

			existFiles.ExceptWith(databaseRecords.Select(Path.GetFileName));

			var remainFileNames = existFiles.ToArray();
			var remainFiles = remainFileNames.OrderBy(i => i).Select(i => new FileInfo(Path.Combine(Setting.StoreFolder, i))).ToArray();

			return remainFiles;
		}

		[Authorize(Policies.Administrate)]
		public async Task<IActionResult> CleanUp(int page = 1)
		{
			var files = await GetRemainFilesAsync();
			ViewBag.TotalSize = files.Sum(i => i.Length);
			return View(files.ToPagedList(20, page));
		}

		[Authorize(Policies.Administrate)]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CleanUpAll()
		{
			var files = await GetRemainFilesAsync();

			try
			{
				foreach (var file in files)
				{
					file.Delete();
				}

				MessageAccessor.Messages.Add(OperationMessageLevel.Success, "清理文件成功", "你已经成功清理了所有冗余文件");
			}
			catch (Exception ex)
			{
				MessageAccessor.Messages.Add(OperationMessageLevel.Error, "清理文件失败",
					string.Format(CultureInfo.CurrentUICulture, "清理冗余文件时发生错误。错误消息：{0}", ex.Message));
			}

			return RedirectToAction("CleanUp", "Manage");
		}

		/// <summary>
		/// 删除冗余文件。
		/// </summary>
		/// <param name="fileName">要删除的文件名。</param>
		/// <returns>操作结果。</returns>
		[Authorize(Policies.Administrate)]
		[HttpGet]
		public IActionResult Download(string fileName)
		{
			var path = Path.Combine(Setting.StoreFolder, Path.GetFileName(fileName));
			return PhysicalFile(path, "application/octet-stream", fileName);
		}

		/// <summary>
		/// 删除冗余文件。
		/// </summary>
		/// <param name="fileName">要删除的文件名。</param>
		/// <returns>操作结果。</returns>
		[Authorize(Policies.Administrate)]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Delete(string fileName)
		{
			try
			{
				var path = Path.Combine(Setting.StoreFolder, Path.GetFileName(fileName));
				System.IO.File.Delete(path);

				MessageAccessor.Messages.Add(OperationMessageLevel.Success, "删除文件成功",
					string.Format(CultureInfo.CurrentUICulture, "你已经成功删除了文件 {0}", fileName));

			}
			catch (Exception ex)
			{
				MessageAccessor.Messages.Add(OperationMessageLevel.Error, "删除文件失败",
					string.Format(CultureInfo.CurrentUICulture, "删除文件 {0} 时发生错误。错误消息：{1}", fileName, ex.Message));
			}

			return RedirectToAction("CleanUp", "Manage");
		}
	}
}