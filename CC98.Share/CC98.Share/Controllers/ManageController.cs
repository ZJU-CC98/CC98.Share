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
	/// ������ع��ܡ�
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
		/// ���ݿ������Ķ���
		/// </summary>
		private CC98ShareModel DbContext { get; }

		private Setting Setting { get; }

		private IOperationMessageAccessor MessageAccessor { get; }

		/// <summary>
		/// ��ʾ�������ҳ�档
		/// </summary>
		/// <returns>���������</returns>
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

				MessageAccessor.Messages.Add(OperationMessageLevel.Success, "�����ļ��ɹ�", "���Ѿ��ɹ����������������ļ�");
			}
			catch (Exception ex)
			{
				MessageAccessor.Messages.Add(OperationMessageLevel.Error, "�����ļ�ʧ��",
					string.Format(CultureInfo.CurrentUICulture, "���������ļ�ʱ�������󡣴�����Ϣ��{0}", ex.Message));
			}

			return RedirectToAction("CleanUp", "Manage");
		}

		/// <summary>
		/// ɾ�������ļ���
		/// </summary>
		/// <param name="fileName">Ҫɾ�����ļ�����</param>
		/// <returns>���������</returns>
		[Authorize(Policies.Administrate)]
		[HttpGet]
		public IActionResult Download(string fileName)
		{
			var path = Path.Combine(Setting.StoreFolder, Path.GetFileName(fileName));
			return PhysicalFile(path, "application/octet-stream", fileName);
		}

		/// <summary>
		/// ɾ�������ļ���
		/// </summary>
		/// <param name="fileName">Ҫɾ�����ļ�����</param>
		/// <returns>���������</returns>
		[Authorize(Policies.Administrate)]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Delete(string fileName)
		{
			try
			{
				var path = Path.Combine(Setting.StoreFolder, Path.GetFileName(fileName));
				System.IO.File.Delete(path);

				MessageAccessor.Messages.Add(OperationMessageLevel.Success, "ɾ���ļ��ɹ�",
					string.Format(CultureInfo.CurrentUICulture, "���Ѿ��ɹ�ɾ�����ļ� {0}", fileName));

			}
			catch (Exception ex)
			{
				MessageAccessor.Messages.Add(OperationMessageLevel.Error, "ɾ���ļ�ʧ��",
					string.Format(CultureInfo.CurrentUICulture, "ɾ���ļ� {0} ʱ�������󡣴�����Ϣ��{1}", fileName, ex.Message));
			}

			return RedirectToAction("CleanUp", "Manage");
		}
	}
}