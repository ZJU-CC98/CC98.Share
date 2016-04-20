using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using CC98.Sports.Models;
using Sakura.AspNet;
using Sakura.AspNet.Mvc.Messages;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CC98.Sports.Controllers
{
	/// <summary>
	/// 管理相关功能的控制器。
	/// </summary>
	public class ManageController : Controller
	{
		/// <summary>
		/// 应用程序设置服务对象。
		/// </summary>
		private AppSettingService<SystemSetting> SettingService { get; }

		/// <summary>
		/// 消息集合。
		/// </summary>
		private ICollection<OperationMessage> Messages { get; }

		/// <summary>
		/// 数据库对象。
		/// </summary>
		private SportDataModel DbContext { get; }

		/// <summary>
		/// 初始化一个控制器的新实例。
		/// </summary>
		/// <param name="dbContext">数据库上下文对象。</param>
		/// <param name="settingService">设置服务对象、</param>
		public ManageController(SportDataModel dbContext, AppSettingService<SystemSetting> settingService, IOperationMessageAccessor messageAccessor)
		{
			DbContext = dbContext;
			SettingService = settingService;
			Messages = messageAccessor.Messages;
		}


		// GET: /<controller>/
		public IActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// 当前用户管理界面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Me()
		{
			var myUserName = User.GetUserName();

			var members = await (from i in DbContext.Members
									.Include(p => p.EventRegistrations).ThenInclude(p => p.TeamRegistration)
								 where i.CC98Id == myUserName
								 select i).ToArrayAsync();

			var teams = await (from i in DbContext.Teams
								.Include(p => p.MemberRegistrations).ThenInclude(p => p.Member)
								.Include(p => p.EventTeamRegistrations).ThenInclude(p => p.Event)
							   where i.Skipper != null && i.Skipper.CC98Id == myUserName
							   select i).ToArrayAsync();

			var teamIds = teams.Select(i => i.Id);

			var eventRegs = await (from i in DbContext.EventTeamRegistrations
									.Include(p => p.Event)
									.Include(p => p.Team)
									.Include(p => p.Skipper)
									.Include(p => p.Captain)
									.Include(p => p.Coach)
									.Include(p => p.Members)
								   where teamIds.Contains(i.TeamId)
								   select i).ToArrayAsync();

			var model = new ManageMeViewModel
			{
				Members = members,
				Teams = teams,
				EventTeamRegistrations = eventRegs
			};

			return View(model);
		}

		/// <summary>
		/// 显示系统设置页面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(UserUtility.AdminPolicy)]
		public IActionResult SystemSetting()
		{
			return View(SettingService.Current);
		}

		/// <summary>
		/// 显示系统设置页面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize(UserUtility.AdminPolicy)]
		public async Task<IActionResult> SystemSetting(SystemSetting model)
		{
			try
			{
				// 保存设置
				SettingService.Current = model;
				await AddChangeSystemSettingLog();

				Messages.Add(new OperationMessage(OperationMessageLevel.Success, "操作成功。", "系统设置已经成功修改。"));

				// 返回显示页面
				return RedirectToAction("SystemSetting");
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
			}

			return View(model);
		}
		/// <summary>
		/// 为系统添加修改系统设置日志。
		/// </summary>
		/// <returns>操作结果。</returns>
		private async Task AddChangeSystemSettingLog()
		{
			DbContext.Logs.Add(new Log
			{
				ActionType = ActionType.ChangeSystemSetting,
				CC98Id = User.GetUserName(),
				Time = DateTime.Now,
			});

			await DbContext.SaveChangesAsync();
		}

		/// <summary>
		/// 查看审核成员列表。
		/// </summary>
		/// <param name="page">页面编号。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(UserUtility.ReviewPolicy)]
		public IActionResult ReviewMember(int page = 1)
		{
			var items = from i in DbContext.Members
						where i.AuditState == AuditState.Pending
						orderby i.AuditCommitTime ascending
						select i;

			return View(items.ToPagedList(SettingService.Current.PageSize, page));
		}

		/// <summary>
		/// 查看审核球队参赛申请列表。
		/// </summary>
		/// <param name="page">页面编号。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(UserUtility.ReviewPolicy)]
		public IActionResult ReviewTeamEventApply(int page = 1)
		{
			var items = from i in DbContext.EventTeamRegistrations
							.Include(p => p.Team)
							.Include(p => p.Event)
							.Include(p => p.Members).ThenInclude(p => p.Member)
						where i.AuditState == AuditState.Pending
						orderby i.AuditCommitTime ascending
						select i;

			return View(items.ToPagedList(SettingService.Current.PageSize, page));
		}

		/// <summary>
		/// 显示日志界面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(UserUtility.OperatePolicy)]
		public IActionResult Log(int page = 1)
		{
			var items =
				from i in DbContext.Logs.Include(p => p.RelatedMember).Include(p => p.RelatedEvent).Include(p => p.RelatedTeam)
				orderby i.Id descending
				select i;

			return View(items.ToPagedList(SettingService.Current.PageSize, page));
		}

		/// <summary>
		/// 执行删除日志操作。
		/// </summary>
		/// <param name="remark">删除原因说明。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(UserUtility.AdminPolicy)]
		public async Task<IActionResult> ClearLog(string remark)
		{
			try
			{
				// 删除所有项目
				DbContext.Logs.RemoveRange(DbContext.Logs);

				DbContext.Logs.Add(new Log
				{
					ActionType = ActionType.ClearLog,
					CC98Id = User.GetUserName(),
					Time = DateTime.Now,
					Remark = remark
				});

				await DbContext.SaveChangesAsync();
				Messages.Add(new OperationMessage(OperationMessageLevel.Success, "操作成功。", "系统日志已经被清除。"));
			}
			catch (DbUpdateException ex)
			{
				Messages.Add(new OperationMessage(OperationMessageLevel.Error, "清除日志时发生错误。", ex.Message));
			}

			return RedirectToAction("Log", "Manage");
		}

		/// <summary>
		/// 删除给定的日志。
		/// </summary>
		/// <param name="id">要删除的日志项目。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(UserUtility.SystemAdminPolicy)]
		public async Task<IActionResult> DeleteLog(int id)
		{
			var item = await (from i in DbContext.Logs
							  where i.Id == id
							  select i).SingleOrDefaultAsync();

			if (item == null)
			{
				return HttpNotFound();
			}

			DbContext.Logs.Remove(item);
			await DbContext.SaveChangesAsync();

			Messages.Add(OperationMessageLevel.Success, "操作成功。", "选中的系统日志项目已经被删除。");
			return Ok();
		}

		/// <summary>
		/// 显示权限信息。
		/// </summary>
		/// <returns>操作结果。</returns>
		public IActionResult Permission()
		{
			return View();
		}

		/// <summary>
		/// 管理功能设定。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(UserUtility.AdminPolicy)]
		public IActionResult Feature()
		{
			return View(SettingService.Current);
		}

		/// <summary>
		/// 设置用户审核申请状态。
		/// </summary>
		/// <param name="value">新状态的值。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(UserUtility.AdminPolicy)]
		public async Task<IActionResult> SetUserReviewRequest(bool value)
		{
			SettingService.Current.OpenUserReviewRequest = value;
			await SettingService.SaveAsync();
			await AddChangeSystemSettingLog();

			Messages.Add(OperationMessageLevel.Success, "操作成功。", "用户审核申请状态已经更改。");
			return RedirectToAction("Feature");
		}

		/// <summary>
		/// 设置球队审核申请状态。
		/// </summary>
		/// <param name="value">新状态的值。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(UserUtility.AdminPolicy)]
		public async Task<IActionResult> SetTeamReviewRequest(bool value)
		{
			SettingService.Current.OpenTeamReviewRequest = value;
			await SettingService.SaveAsync();
			await AddChangeSystemSettingLog();

			Messages.Add(OperationMessageLevel.Success, "操作成功。", "球队审核申请状态已经更改。");
			return RedirectToAction("Feature");
		}

		/// <summary>
		/// 重置所有人的审核状态。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(UserUtility.AdminPolicy)]
		public async Task<IActionResult> ResetAllMemberAuditState()
		{
			foreach (var i in DbContext.Members)
			{
				i.AuditState = AuditState.NotCommitted;
				i.AuditCommitTime = null;
			}

			await DbContext.SaveChangesAsync();

			Messages.Add(OperationMessageLevel.Success, "操作成功。", "所有成员的审核状态都已经重置为未提交。。");
			return RedirectToAction("Feature");
		}
	}
}
