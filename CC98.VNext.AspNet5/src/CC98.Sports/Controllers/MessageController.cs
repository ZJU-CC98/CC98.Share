using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Sakura.AspNet;
using Sakura.AspNet.Mvc.Messages;

namespace CC98.Sports.Controllers
{
	/// <summary>
	/// 提供对消息的访问。
	/// </summary>
	public class MessageController : Controller
	{
		/// <summary>
		/// 数据库上下文对象。
		/// </summary>
		private SportDataModel DbContext { get; }

		/// <summary>
		/// 设置服务。
		/// </summary>
		private AppSettingService<SystemSetting> SettingService { get; }

		/// <summary>
		/// 消息服务。
		/// </summary>
		private ICollection<OperationMessage> Messages { get; }


		public MessageController(SportDataModel dbContext, AppSettingService<SystemSetting> settingService, IOperationMessageAccessor messageAccessor)
		{
			DbContext = dbContext;
			SettingService = settingService;
			Messages = messageAccessor.Messages;
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteMyAll()
		{
			var userName = User.GetUserName();

			var messages = from i in DbContext.Messages
				where i.Receiver == userName
				select i;

			DbContext.Messages.RemoveRange(messages);

			try
			{
				await DbContext.SaveChangesAsync();
				Messages.Add(OperationMessageLevel.Success, "操作成功。", "你的所有系统消息均已删除。");
			}
			catch (DbUpdateException ex)
			{
				Messages.Add(OperationMessageLevel.Error, "操作失败。", $"操作时发生错误。详细信息：{ex.Message}");
			}

			return RedirectToAction("Index", "Message");
		}

		/// <summary>
		/// 查看当前用户的消息。
		/// </summary>
		/// <param name="page">页面编号。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize]
		public IActionResult Index(int page = 1)
		{
			var userName = User.GetUserName();

			var items = from i in DbContext.Messages
						where i.Receiver == userName
						orderby i.Time descending
						select i;

			return View(items.ToPagedList(SettingService.Current.PageSize, page));
		}

		[Authorize]
		public async Task<IActionResult> SetRead(int id)
		{
			var item = await (from i in DbContext.Messages
							  where i.Id == id
							  select i).FirstOrDefaultAsync();

			// 找不到消息
			if (item == null)
			{
				return HttpNotFound();
			}

			// 不是所有者
			if (!string.Equals(item.Receiver, User.GetUserName(), StringComparison.OrdinalIgnoreCase))
			{
				return new HttpStatusCodeResult(403);
			}

			item.IsRead = true;
			try
			{
				await DbContext.SaveChangesAsync();
				Messages.Add(OperationMessageLevel.Success, "操作成功。", "已经将消息标记成为已读状态。");
			}
			catch(DbUpdateException)
			{
				Messages.Add(OperationMessageLevel.Error, "操作失败。", "将消息标记为已读过程中发生错误，请稍后再试一次。");
			}

			return RedirectToAction("Index", "Message");
		}

		[Authorize]
		public async Task<IActionResult> SetUnread(int id)
		{
			var item = await (from i in DbContext.Messages
							  where i.Id == id
							  select i).FirstOrDefaultAsync();

			// 找不到消息
			if (item == null)
			{
				return HttpNotFound();
			}

			// 不是所有者
			if (!string.Equals(item.Receiver, User.GetUserName(), StringComparison.OrdinalIgnoreCase))
			{
				return new HttpStatusCodeResult(403);
			}

			item.IsRead = false;
			try
			{
				await DbContext.SaveChangesAsync();
				Messages.Add(OperationMessageLevel.Success, "操作成功。", "已经将消息标记成为未读状态。");
			}
			catch (DbUpdateException)
			{
				Messages.Add(OperationMessageLevel.Error, "操作失败。", "将消息标记为未读过程中发生错误，请稍后再试一次。");
			}

			return RedirectToAction("Index", "Message");
		}

		[Authorize]
		public async Task<IActionResult> Delete(int id)
		{
			var item = await (from i in DbContext.Messages
							  where i.Id == id
							  select i).FirstOrDefaultAsync();

			// 找不到消息
			if (item == null)
			{
				return HttpNotFound();
			}

			// 不是所有者
			if (!string.Equals(item.Receiver, User.GetUserName(), StringComparison.OrdinalIgnoreCase))
			{
				return new HttpStatusCodeResult(403);
			}

			DbContext.Messages.Remove(item);

			try
			{
				await DbContext.SaveChangesAsync();
				Messages.Add(OperationMessageLevel.Success, "操作成功。", "已经删除了给定的消息。");
			}
			catch (DbUpdateException)
			{
				Messages.Add(OperationMessageLevel.Error, "操作失败。", "删除消息时发生错误，请稍后再试一次。");
			}

			return RedirectToAction("Index", "Message");
		}
	}
}
