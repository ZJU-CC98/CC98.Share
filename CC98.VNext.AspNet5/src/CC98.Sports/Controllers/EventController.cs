using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Authorization;
using Sakura.AspNet;
using Sakura.AspNet.Mvc.Messages;
using System.Collections.Generic;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CC98.Sports.Controllers
{
	/// <summary>
	/// 提供对赛事信息的访问。
	/// </summary>
	public class EventController : Controller
	{

		/// <summary>
		/// 获取该控制器相关的数据库上下文对象。
		/// </summary>
		private SportDataModel DbContext { get; }

		private AppSettingService<SystemSetting> SettingService { get; }

		private ICollection<OperationMessage> Messages { get; }

		/// <summary>
		/// 初始化一个控制器对象的新实例。
		/// </summary>
		/// <param name="dbContext">数据库上下文对象。</param>
		public EventController(SportDataModel dbContext, AppSettingService<SystemSetting> settingService, IOperationMessageAccessor messageAccessor)
		{
			DbContext = dbContext;
			SettingService = settingService;
			Messages = messageAccessor.Messages;
		}

		/// <summary>
		/// 释放该对象占用的所有资源。
		/// </summary>
		~EventController()
		{
			// 释放数据库对象。
			DbContext.Dispose();
		}


		/// <summary>
		/// 首页。
		/// </summary>
		/// <returns>操作结果。</returns>
		public IActionResult Index(int page = 1)
		{
			var items = from i in DbContext.Events.Include(p => p.TeamRegistrations)
						orderby i.Id descending
						select i;

			ViewBag.ShowManage = User.CanAdmin();
			return View(items.ToPagedList(SettingService.Current.PageSize, page));
		}

		/// <summary>
		/// 创建赛事视图。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(UserUtility.OrganizePolicy)]
		public IActionResult Create()
		{
			return View();
		}



		/// <summary>
		/// 提交创建操作。
		/// </summary>
		/// <param name="model">数据模型。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize(UserUtility.OrganizePolicy)]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Event model)
		{
			if (model.PlayerMax < model.PlayerMin)
			{
				ModelState.AddModelError("", "运动员上限数量不能低于下限数量。");
			}

			if (ModelState.IsValid)
			{
				DbContext.Events.Add(model);
				DbContext.Logs.Add(new Log
				{
					ActionType = ActionType.CreateEvent,
					CC98Id = User.GetUserName(),
					RelatedEvent = model,
					Time = DateTime.Now
				});

				try
				{
					await DbContext.SaveChangesAsync();
					return RedirectToAction("Index");
				}
				catch (DbUpdateException ex)
				{
					ModelState.AddModelError("", ex.Message);
				}
			}

			return View(model);
		}

		/// <summary>
		/// 显示赛事的详细信息。
		/// </summary>
		/// <param name="id">赛事的标识。</param>
		/// <param name="teamPage">队伍列表的分页。</param>
		/// <param name="gamePage">赛事列表的分页。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		public async Task<IActionResult> Detail(int id, int teamPage = 1, int gamePage = 1)
		{
			var item = await (from i in DbContext.Events
								.Include(p => p.Games)
								.Include(p => p.TeamRegistrations).ThenInclude(p => p.Team)
								.Include(p => p.TeamRegistrations).ThenInclude(p => p.Members).ThenInclude(p => p.Member)
							  where i.Id == id
							  select i).FirstOrDefaultAsync();

			// 找不到赛事
			if (item == null)
			{
				return HttpNotFound();
			}

			ViewBag.TeamPage = teamPage;
			ViewBag.GamePage = gamePage;

			return View(item);
		}

		/// <summary>
		/// 显示赛事编辑界面。
		/// </summary>
		/// <param name="id">赛事标识。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(UserUtility.OrganizePolicy)]
		public async Task<IActionResult> Edit(int id)
		{
			var item = await (from i in DbContext.Events
							  where i.Id == id
							  select i).FirstOrDefaultAsync();

			if (item == null)
			{
				return HttpNotFound();
			}

			return View(item);
		}

		/// <summary>
		/// 执行赛事编辑操作。
		/// </summary>
		/// <param name="model">数据对象。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize(UserUtility.OrganizePolicy)]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Event model)
		{
			if (model.PlayerMax < model.PlayerMin)
			{
				ModelState.AddModelError("", "运动员上限数量不能低于下限数量。");
			}

			if (ModelState.IsValid)
			{

				var item = await (from i in DbContext.Events
								  where i.Id == model.Id
								  select i).FirstOrDefaultAsync();

				if (item == null)
				{
					return HttpNotFound();
				}

				DbContext.Entry(item).State = EntityState.Detached;
				DbContext.Update(model);

				DbContext.Logs.Add(new Log
				{
					ActionType = ActionType.EditEvent,
					CC98Id = User.GetUserName(),
					RelatedEvent = model,
					Time = DateTime.Now
				});

				try
				{
					await DbContext.SaveChangesAsync();
					return RedirectToAction("Index");
				}
				catch (DbUpdateException ex)
				{
					ModelState.AddModelError("", ex.Message);
				}
			}

			return View(model);
		}

		/// <summary>
		/// 管理队伍报名分组。
		/// </summary>
		/// <param name="id">赛事标识。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(UserUtility.OrganizePolicy)]
		public async Task<IActionResult> GroupTeams(int id)
		{
			var item = await (from i in DbContext.Events
								.Include(p => p.TeamRegistrations).ThenInclude(p => p.Captain)
								.Include(p => p.TeamRegistrations).ThenInclude(p => p.Coach)
								.Include(p => p.TeamRegistrations).ThenInclude(p => p.Skipper)
								.Include(p => p.TeamRegistrations).ThenInclude(p => p.Team)
							  where i.Id == id
							  select i).FirstOrDefaultAsync();

			if (item == null)
			{
				throw new ActionResultException(404);
			}

			return View(item);

		}

		/// <summary>
		/// 更新队伍分组信息。
		/// </summary>
		/// <param name="id">赛事标识。</param>
		/// <param name="model">操作结果。</param>
		/// <returns></returns>
		[HttpPost]
		[Authorize(UserUtility.OrganizePolicy)]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> GroupTeams(int id, EventTeamRegistration[] model)
		{
			// 检测项目
			var item = await (from i in DbContext.Events
								.Include(p => p.TeamRegistrations)
							  where i.Id == id
							  select i).FirstOrDefaultAsync();

			if (item == null)
			{
				throw new ActionResultException(404);
			}

			// 循环更新，放入内存中更快
			foreach (var updateRecord in model)
			{
				var dataItem = item.TeamRegistrations.FirstOrDefault(i => i.TeamId == updateRecord.TeamId);

				if (dataItem != null)
				{
					dataItem.Group = updateRecord.Group;
					dataItem.GroupNumber = updateRecord.GroupNumber;
				}
			}

			await DbContext.SaveChangesAsync();
			Messages.Add(OperationMessageLevel.Success, "操作成功。", "队伍分组信息已经更新。");

			return RedirectToAction("GroupTeams", "Event", new { id });
		}


		/// <summary>
		/// 执行删除操作。
		/// </summary>
		/// <param name="id">赛事标识。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize(UserUtility.OrganizePolicy)]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{

			var item = await (from i in DbContext.Events.Include(p => p.Logs)
							  where i.Id == id
							  select i).FirstOrDefaultAsync();

			if (item == null)
			{
				return HttpNotFound();
			}

			try
			{
				DbContext.Logs.Add(new Log
				{
					ActionType = ActionType.DeleteEvent,
					CC98Id = User.GetUserName(),
					RelatedEvent = item,
					Time = DateTime.Now
				});

				DbContext.Remove(item);

				// 取消关联并放入备注
				foreach (var i in item.Logs)
				{
					i.RelatedEvent = null;
					i.Remark += string.Format(CultureInfo.InvariantCulture, "{0} (#{1})", item.Name, item.Id);
				}

				await DbContext.SaveChangesAsync();
				return Ok();
			}
			catch (DbUpdateException ex)
			{
				return HttpBadRequest(ex.Message);
			}
		}

		/// <summary>
		/// 启动一项赛事。
		/// </summary>
		/// <param name="id">赛事标识。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize(UserUtility.OrganizePolicy)]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Run(int id)
		{
			var item = await (from i in DbContext.Events.Include(p => p.TeamRegistrations)
							  where i.Id == id
							  select i).FirstOrDefaultAsync();

			if (item == null)
			{
				throw new ActionResultException(404);
			}

			switch (item.State)
			{
				case EventState.Preparing:
				case EventState.Registring:
					break;
				default:
					throw new ActionResultException(400, "只有准备和注册状态的赛事可以启动。");
			}


			item.State = EventState.Running;

			foreach (var i in item.TeamRegistrations)
			{
				if (i.AuditState == AuditState.Accepted)
				{
					i.EventState = TeamEventState.Playing;
				}
			}

			DbContext.Logs.Add(new Log
			{
				ActionType = ActionType.RunEvent,
				CC98Id = User.GetUserName(),
				RelatedEvent = item,
				Time = DateTime.Now
			});

			try
			{
				await DbContext.SaveChangesAsync();
				return Ok();
			}
			catch (DbUpdateException ex)
			{
				return HttpBadRequest(ex.Message);
			}
		}

		/// <summary>
		/// 结束一项赛事。
		/// </summary>
		/// <param name="id">赛事标识。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize(UserUtility.OrganizePolicy)]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Close(int id)
		{
			var item = await (from i in DbContext.Events.Include(p => p.TeamRegistrations)
							  where i.Id == id
							  select i).FirstOrDefaultAsync();

			if (item == null)
			{
				throw new ActionResultException(404);
			}

			if (item.State != EventState.Running)
			{
				throw new ActionResultException(400, "只有正在运行的赛事可以关闭。");
			}

			item.State = EventState.Closed;

			foreach (var i in item.TeamRegistrations)
			{
				if (i.AuditState == AuditState.Accepted)
				{
					i.EventState = TeamEventState.Ended;
				}
			}

			DbContext.Logs.Add(new Log
			{
				ActionType = ActionType.CloseEvent,
				CC98Id = User.GetUserName(),
				RelatedEvent = item,
				Time = DateTime.Now
			});

			try
			{
				await DbContext.SaveChangesAsync();
				return Ok();
			}
			catch (DbUpdateException ex)
			{
				return HttpBadRequest(ex.Message);
			}

		}
	}
}
