using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CC98.Sports.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Sakura.AspNet;
using Sakura.AspNet.Mvc.Messages;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CC98.Sports.Controllers
{
	/// <summary>
	/// 提供对于球队信息的访问和管理。
	/// </summary>
	public class TeamController : Controller
	{
		private SportDataModel DbContext { get; }

		private AppSettingService<SystemSetting> SettingService { get; }

		private ICollection<OperationMessage> Messages { get; }

		public TeamController(SportDataModel dbContext, AppSettingService<SystemSetting> settingService, IOperationMessageAccessor messageAccessor)
		{
			DbContext = dbContext;
			SettingService = settingService;
			Messages = messageAccessor.Messages;
		}

		/// <summary>
		/// 显示球队首页。
		/// </summary>
		/// <param name="page">页码。</param>
		/// <returns>操作结果。</returns>
		// GET: /<controller>/
		public IActionResult Index(int page = 1)
		{
			var items = from i in DbContext.Teams
							.Include(p => p.Skipper)
							.Include(p => p.Coach)
							.Include(p => p.Captain)
							.Include(p => p.MemberRegistrations)
							.Include(p => p.EventTeamRegistrations).ThenInclude(p => p.Event)
						select i;

			ViewBag.ShowAdmin = User.CanOperate();

			return View(items.ToPagedList(SettingService.Current.PageSize, page));
		}

		/// <summary>
		/// 显示创建球队页面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize]
		public IActionResult Create()
		{
			return View();
		}

		/// <summary>
		/// 执行创建球队操作。
		/// </summary>
		/// <param name="model">数据模型。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public async Task<IActionResult> Create(Team model)
		{
			HandleModel(model);

			// 如果用户没有操作权限，则不能修改锁定状态。
			if (!User.CanOperate())
			{
				model.IsLocked = false;
			}

			if (model.SkipperId != null)
			{
				var skipperIdValue = model.SkipperId.Value;
				var userName = User.GetUserName();

				// 检查当前用户是否注册领队。
				var isCurrentUser = await (from i in DbContext.Members
										   where i.CC98Id == userName && i.Id == skipperIdValue && i.Type == MemberType.Skipper
										   select i).AnyAsync();

				if (!isCurrentUser)
				{
					ModelState.AddModelError(nameof(model.SkipperId), "选中的领队不是由你注册的。");
				}
			}

			if (ModelState.IsValid)
			{
				DbContext.Teams.Add(model);

				// 如果提供了领队，则领队将作为第一个成员加入队伍。
				if (model.SkipperId != null)
				{
					model.MemberRegistrations.Add(new TeamMemberRegistration
					{
						MemberId = model.SkipperId.Value,
						Team = model,
						TeamAuditState = AuditState.Accepted,
						MemberAuditState = AuditState.Accepted,
						Time = DateTime.Now
					});
				}

				try
				{
					await DbContext.SaveChangesAsync();
					Messages.Add(OperationMessageLevel.Success, "操作成功。", "你已经成功创建了新的球队。");
					return RedirectToAction("Index", "Team");
				}
				catch (DbUpdateException ex)
				{
					ModelState.AddModelError("", ex.Message);
				}

			}

			return View(model);
		}

		/// <summary>
		/// 显示编辑界面。
		/// </summary>
		/// <param name="id">要编辑的队伍的标识。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Edit(int id)
		{
			var item = await GetItemAndCheckUnlocked(id);
			return View(item);
		}

		/// <summary>
		/// 获取具有指定标识的 <see cref="Team"/> 对象，并且检查当前用户是否有权管理。如果当前用户不具有管理权限，则还要求项目未被锁定。
		/// </summary>
		/// <param name="id">要检索的 <see cref="Team"/> 的标识。</param>
		/// <returns>具有给定标识的 <see cref="Team"/> 对象。</returns>
		private async Task<Team> GetItemAndCheckUnlocked(int id)
		{
			var item = await GetItemAndCheckManage(id);

			// 如果项目被锁定但用户不是操作员，则不能编辑项目
			if (item.IsLocked && !User.CanOperate())
			{
				throw new ActionResultException(403);
			}
			return item;
		}

		/// <summary>
		/// 检查球队中是否包含了具有给定要求的用户。
		/// </summary>
		/// <param name="model">球队对象。</param>
		/// <param name="memberId">成员标识。</param>
		/// <param name="memberType">成员类型。</param>
		/// <returns>如果球队中具有给定标识的成员，其类型符合 <paramref name="memberType"/> 的定义，且已经通过双向审核，返回 true；否则返回 false。</returns>
		private bool CheckMemberOwnerAndType(Team model, int? memberId, MemberType memberType)
		{
			// 如果 ID 为空则始终成立。
			if (memberId == null)
			{
				return true;
			}

			return (from i in model.MemberRegistrations
					where i.TeamAuditState == AuditState.Accepted && i.TeamAuditState == AuditState.Accepted
					select i.Member
				into j
					where j.Id == memberId && j.Type == memberType
					select j).Any();

		}

		/// <summary>
		/// 执行编辑操作。
		/// </summary>
		/// <param name="model">编辑后的数据。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Team model)
		{
			HandleModel(model);

			var item = await GetItemAndCheckUnlocked(model.Id);

			if (!CheckMemberOwnerAndType(item, model.SkipperId, MemberType.Skipper))
			{
				ModelState.AddModelError(nameof(model.SkipperId), "选定的成员不是有效的领队。");
			}

			if (!CheckMemberOwnerAndType(item, model.CaptainId, MemberType.Player))
			{
				ModelState.AddModelError(nameof(model.CaptainId), "选定的成员不是有效的队长。");
			}

			if (!CheckMemberOwnerAndType(item, model.CoachId, MemberType.Coach))
			{
				ModelState.AddModelError(nameof(model.CoachId), "选定的成员不是有效的教练。");
			}


			// 非管理用户不能修改锁定状态。
			if (!User.CanOperate())
			{
				model.IsLocked = false;
			}

			if (ModelState.IsValid)
			{

				// 更换数据
				DbContext.Replace(item, model);

				// 追加日志
				DbContext.Logs.Add(new Log
				{
					ActionType = ActionType.EditTeam,
					CC98Id = User.GetUserName(),
					RelatedTeam = model,
					Time = DateTime.Now
				});

				try
				{
					// 保存
					await DbContext.SaveChangesAsync();

					// 界面消息
					Messages.Add(OperationMessageLevel.Success, "操作成功。", "队伍信息已经成功更新。");

					// 返回详细信息
					return RedirectToAction("Index", "Team");

				}
				catch (DbUpdateException ex)
				{
					ModelState.AddModelError("", ex.Message);
				}
			}

			// 返回
			return View(model);
		}

		/// <summary>
		/// 执行删除操作。
		/// </summary>
		/// <param name="id">要删除的对象的标识。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{
			// 获取对象并检查权限
			var item = await GetItemAndCheckUnlocked(id);

			foreach (var eventTeamRegistration in item.EventTeamRegistrations)
			{
				if (eventTeamRegistration.EventState != TeamEventState.NotStarted)
				{
					return HttpBadRequest($"该球队正在参与赛事 {eventTeamRegistration.Event.Name} 或在该赛事留有参赛记录。必须清除记录才能删除球队。");
				}
			}

			item.EventTeamRegistrations.Clear();

			foreach (var game in item.Game1s.Concat(item.Game2s))
			{
				if (game.State != GameState.NotStarted)
				{
					return HttpBadRequest($"该球队正在参与比赛 {game.Name}。必须手动删除比赛记录才能删除球队。");
				}
			}

			item.Game1s.Clear();
			item.Game2s.Clear();

			// 删除队伍
			DbContext.Teams.Remove(item);

			// 追加日志
			DbContext.Logs.Add(new Log
			{
				ActionType = ActionType.DeleteTeam,
				CC98Id = User.GetUserName(),
				RelatedTeam = item,
				Time = DateTime.Now
			});

			// 删除所有日志
			foreach (var log in item.Logs)
			{
				log.RelatedTeam = null;
				log.Remark += string.Format(CultureInfo.CurrentCulture, "队伍 {0}(#{1})", item.Name, item.Id);
			}

			try
			{
				// 保存更改
				await DbContext.SaveChangesAsync();
				Messages.Add(OperationMessageLevel.Success, "操作成功。",
					string.Format(CultureInfo.CurrentCulture, "队伍 {0}(#{1}) 已经成功删除。", item.Name, item.Id));
			}
			catch (DbUpdateException ex)
			{
				return HttpBadRequest(ex.Message);
			}

			// 成功
			return Ok();
		}

		/// <summary>
		/// 获取具有指定标识的 <see cref="Team"/> 对象，并且检查当前用户是否有权管理。
		/// </summary>
		/// <param name="id">要检索的 <see cref="Team"/> 的标识。</param>
		/// <returns>具有给定标识的 <see cref="Team"/> 对象。</returns>
		private async Task<Team> GetItemAndCheckManage(int id)
		{
			var item = await (from i in DbContext.Teams
								.Include(p => p.MemberRegistrations).ThenInclude(p => p.Member)
								.Include(p => p.Logs)
								.Include(p => p.EventTeamRegistrations).ThenInclude(p => p.Event)
								.Include(p => p.Game1s)
								.Include(p => p.Game2s)
							  where i.Id == id
							  select i).FirstOrDefaultAsync();

			// 找不到项目
			if (item == null)
			{
				throw new ActionResultException(404);
			}

			// 无法管理
			if (!CanManage(item))
			{
				throw new ActionResultException(403);
			}

			return item;
		}

		/// <summary>
		/// 获取一个值，指示当前用户是否对于给定的项目具有管理权限。
		/// </summary>
		/// <param name="item">要判断的 <see cref="Team"/> 对象。</param>
		/// <returns>如果当前用户对 <paramref name="item"/> 表示的项目具有管理权，返回 true；否则返回 false。</returns>
		private bool CanManage(Team item)
		{
			// 如果用户具有操作权限，始终可以管理
			// 否则，用户必须是所有者
			return User.CanOperate() || IsOwner(item);
		}

		/// <summary>
		/// 获取一个值，指示当前用户是否是是给定项目的所有者。
		/// </summary>
		/// <param name="item">要判断的 <see cref="Team"/> 对象。</param>
		/// <returns>如果当前用户是 <paramref name="item"/> 的所有者，返回 true；否则返回 false。</returns>
		private bool IsOwner(Team item)
		{
			return User.Identity.IsAuthenticated && string.Equals(item.Skipper?.CC98Id, User.GetUserName(), StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// 检查某个成员是否属于某个类型。
		/// </summary>
		/// <param name="memberId">要检查的成员。</param>
		/// <param name="type">要检查的类型。</param>
		/// <returns></returns>
		private Task<bool> CheckMemberType(int memberId, MemberType type)
		{
			return (from i in DbContext.Members
					where i.Id == memberId && i.Type == type
					select i).AnyAsync();
		}

		/// <summary>
		/// 对模型进行必要的数据处理。补充检测错误并且覆盖重要数据。
		/// </summary>
		/// <param name="model"></param>
		private void HandleModel(Team model)
		{
			//model.CC98Id = User.GetUserName();
			// 去掉关联属性关联防止模型报错
			ModelState.Remove(nameof(Team.Skipper));
			ModelState.Remove(nameof(Team.Captain));
			ModelState.Remove(nameof(Team.Coach));

			if (!User.CanOperate() && model.SkipperId == null)
			{
				ModelState.AddModelError(nameof(model.SkipperId), "你必须设定一个领队。");
			}

			//if (model.SkipperId != null && !await CheckMemberType(model.SkipperId.Value, MemberType.Skipper))
			//{
			//	ModelState.AddModelError(nameof(model.SkipperId), "选中的领队并没有注册为领队身份。");
			//}

			//if (model.CaptainId != null && !await CheckMemberType(model.CaptainId.Value, MemberType.Player))
			//{
			//	ModelState.AddModelError(nameof(model.SkipperId), "选中的队长并没有注册为运动员身份。");
			//}

			//if (model.CoachId != null && !await CheckMemberType(model.CoachId.Value, MemberType.Coach))
			//{
			//	ModelState.AddModelError(nameof(model.SkipperId), "选中的领队并没有注册为教练身份。");
			//}

		}

		/// <summary>
		/// 显示球队的详细信息。
		/// </summary>
		/// <param name="id">要显示详细信息的球队的标识。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		public async Task<IActionResult> Detail(int id)
		{
			var item = await (from i in DbContext.Teams
								.Include(p => p.MemberRegistrations).ThenInclude(p => p.Member).ThenInclude(p => p.EventRegistrations).ThenInclude(p => p.TeamRegistration)
								.Include(p => p.EventTeamRegistrations).ThenInclude(p => p.Event)
							  where i.Id == id
							  select i).FirstOrDefaultAsync();

			if (item == null)
			{
				return HttpNotFound();
			}

			// 是否具有管理权限
			ViewBag.ShowAdmin = CanManage(item);
			return View(item);
		}

		#region 申请相关

		/// <summary>
		/// 球员向球队发出入队申请。
		/// </summary>
		/// <param name="memberId">要申请的球员的标识。</param>
		/// <param name="teamId">要申请的球队的标识。</param>
		/// <param name="reason">申请理由。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> InviteMember(int memberId, int teamId, string reason)
		{
			var member = await (from i in DbContext.Members
								where i.Id == memberId
								select i).FirstOrDefaultAsync();

			var team = await (from i in DbContext.Teams.Include(p => p.Skipper)
							  where i.Id == teamId
							  select i).FirstOrDefaultAsync();

			if (member == null || team == null)
			{
				throw new ActionResultException(400, "提供的球员或者球队信息有误。");
			}

			// 管理权限不足
			if (!CanManage(team))
			{
				throw new ActionResultException(403);
			}

			var existItem = from i in DbContext.TeamMemberRegistrations
							where i.TeamId == teamId && i.MemberId == memberId
							select i;

			if (await existItem.AnyAsync())
			{
				throw new ActionResultException(400, "已经存在球员和球队的双向申请关系。");
			}

			var newItem = new TeamMemberRegistration
			{
				TeamId = teamId,
				MemberId = memberId,
				MemberAuditState = AuditState.Pending,
				TeamAuditState = AuditState.Accepted,
				Time = DateTime.Now,
			};

			DbContext.TeamMemberRegistrations.Add(newItem);

			await AddMessageAndLogCore(newItem, ActionType.TeamCreateInvite, reason,
				string.Format(CultureInfo.CurrentCulture, "队伍 {0} 向成员 {1} 发出了入队邀请。", team.Name, member.Name),
				string.Format(CultureInfo.CurrentCulture, "队伍 {0} 已经向成员 {1} 发出了入队邀请。", team.Name, member.Name));

			await DbContext.SaveChangesAsync();
			return Ok();
		}


		/// <summary>
		/// 球队执行通过申请操作。
		/// </summary>
		/// <param name="memberId">球员标识。</param>
		/// <param name="teamId">球队标识。</param>
		/// <param name="reason">操作原因。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public Task<IActionResult> AcceptApply(int memberId, int teamId, string reason)
		{
			return AuditMemberApplyCore(memberId, teamId, reason, true);
		}

		/// <summary>
		/// 球队执行拒绝申请操作。
		/// </summary>
		/// <param name="memberId">球员标识。</param>
		/// <param name="teamId">球队标识。</param>
		/// <param name="reason">操作原因。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public Task<IActionResult> RejectApply(int memberId, int teamId, string reason)
		{
			return AuditMemberApplyCore(memberId, teamId, reason, false);
		}

		/// <summary>
		/// 球队执行取消申请操作。
		/// </summary>
		/// <param name="memberId">球员标识。</param>
		/// <param name="teamId">球队标识。</param>
		/// <param name="reason">操作原因。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CancelApply(int memberId, int teamId, string reason)
		{
			var item = await GetRegistrationItem(memberId, teamId);

			if (item == null)
			{
				throw new ActionResultException(400, "当前不存在对应的申请。");
			}

			// 球员发起，球队未通过
			if (item.MemberAuditState == AuditState.Accepted || item.TeamAuditState != AuditState.Accepted)
			{
				throw new ActionResultException(400, "必须是球队发起的申请才能执行这个操作。");
			}

			// 删除对象
			DbContext.TeamMemberRegistrations.Remove(item);

			// 创建日志并保存结果
			return await AddMessageAndLogCore(
				item,
				ActionType.MemberDeleteApply,
				reason,
				string.Format(CultureInfo.CurrentCulture, "队伍 {0} 取消了向成员 {1} 发出的加入邀请。", item.Team.Name, item.Member.Name),
				string.Format(CultureInfo.InvariantCulture, "队伍 {0} 已经取消了向成员 {1} 发出的加入邀请。", item.Team.Name, item.Member.Name));
		}

		/// <summary>
		/// 修正队员离队带来的成员变动影响。
		/// </summary>
		/// <param name="team">队伍。</param>
		/// <param name="memberId">队员标识。</param>
		private static void FixMemberLeave(Team team, int memberId)
		{
			if (team.CoachId == memberId)
			{
				team.CoachId = null;
			}

			if (team.CaptainId == memberId)
			{
				team.CaptainId = null;
			}

			if (team.SkipperId == memberId)
			{
				team.SkipperId = null;
			}
		}

		/// <summary>
		/// 球队执行删除成员操作。
		/// </summary>
		/// <param name="memberId">球员标识。</param>
		/// <param name="teamId">球队标识。</param>
		/// <param name="reason">操作原因。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteMember(int memberId, int teamId, string reason)
		{
			var item = await GetRegistrationItem(memberId, teamId);

			if (item == null)
			{
				throw new ActionResultException(400, "当前不存在对应的申请。");
			}

			// 球员和球队均通过
			if (item.MemberAuditState != AuditState.Accepted || item.TeamAuditState != AuditState.Accepted)
			{
				throw new ActionResultException(400, "必须已经是球队的成员才能执行离队操作。");
			}

			if (item.Member.IsLockedByTeam(item.TeamId))
			{
				throw new ActionResultException(400, "参赛期间不能删除参赛成员。");
			}

			// 删除对象
			DbContext.TeamMemberRegistrations.Remove(item);

			FixMemberLeave(item.Team, memberId);

			// 创建日志并保存结果
			return await AddMessageAndLogCore(
				item,
				ActionType.MemberLeaveTeam,
				reason,
				string.Format(CultureInfo.CurrentCulture, "队伍 {0} 删除了成员 {1}。", item.Team.Name, item.Member.Name),
				string.Format(CultureInfo.InvariantCulture, "队伍 {0} 删除了成员 {1}。", item.Team.Name, item.Member.Name));
		}

		/// <summary>
		/// 执行通过或者拒绝球队申请的核心方法。
		/// </summary>
		/// <param name="memberId">球员标识。</param>
		/// <param name="teamId">球队标识。</param>
		/// <param name="reason">操作原因。</param>
		/// <param name="isAccept">true 表示通过申请，false 表示拒绝申请。</param>
		/// <returns>操作结果。</returns>
		private async Task<IActionResult> AuditMemberApplyCore(int memberId, int teamId, string reason, bool isAccept)
		{
			var item = await GetRegistrationItem(memberId, teamId);

			if (item == null)
			{
				throw new ActionResultException(400, "当前不存在对应的申请。");
			}

			if (item.MemberAuditState != AuditState.Accepted)
			{
				throw new ActionResultException(400, "必须是球员发起的申请才能执行这个操作。");
			}

			// 修改状态
			item.TeamAuditState = isAccept ? AuditState.Accepted : AuditState.Rejected;

			return isAccept
				? await AddMessageAndLogCore(
					item,
					ActionType.MemberAcceptTeam,
					reason,
					string.Format(CultureInfo.CurrentCulture, "队伍 {0} 接受了成员 {1} 的加入邀请。", item.Team.Name, item.Member.Name),
					string.Format(CultureInfo.InvariantCulture, "队伍 {0} 已经通过了成员 {1} 的加入邀请。", item.Team.Name, item.Member.Name))
				: await AddMessageAndLogCore(
					item,
					ActionType.MemberRejectTeam,
					reason,
					string.Format(CultureInfo.CurrentCulture, "队伍 {0} 拒绝了成员 {1} 的加入邀请。", item.Team.Name, item.Member.Name),
					string.Format(CultureInfo.InvariantCulture, "队伍 {0} 已经拒绝了成员 {1} 的加入邀请。", item.Team.Name, item.Member.Name));
		}

		/// <summary>
		/// 添加用户消息，日志，以及执行数据库更新操作，并返回结果。
		/// </summary>
		/// <param name="item">注册项目对象。</param>
		/// <param name="actionType">日志记录的操作原因。</param>
		/// <param name="reason">用户提供的操作原因字符串。</param>
		/// <param name="messageContent">用户短消息的内容正文。</param>
		/// <param name="messageDescription">用户界面的提示消息正文。</param>
		/// <returns>操作结果。</returns>
		private async Task<IActionResult> AddMessageAndLogCore(TeamMemberRegistration item, ActionType actionType, string reason, string messageContent, string messageDescription)
		{
			// 日志
			DbContext.Logs.Add(new Log
			{
				ActionType = actionType,
				CC98Id = User.GetUserName(),
				RelatedMember = item.Member,
				RelatedTeam = item.Team,
				Time = DateTime.Now
			});

			// 消息
			var message = new Message
			{
				Receiver = item.Member.CC98Id,
				Content = messageContent,
				Time = DateTime.Now,
			};

			// 追加原因
			if (!string.IsNullOrEmpty(reason))
			{
				message.Content += string.Format(CultureInfo.CurrentCulture, "原因：{0}。", reason);
			}

			DbContext.Messages.Add(message);

			// 保存结果
			await DbContext.SaveChangesAsync();

			// 界面消息
			Messages.Add(OperationMessageLevel.Success, "操作成功。", messageDescription);

			return Ok();
		}

		/// <summary>
		/// 获取的注册对象。如果参数不正确或者当前成员没有权限管理，则返回错误响应。
		/// </summary>
		/// <param name="memberId">成员标识。</param>
		/// <param name="teamId">队伍标识。</param>
		/// <returns>注册对象。</returns>
		private async Task<TeamMemberRegistration> GetRegistrationItem(int memberId, int teamId)
		{
			var member = await (from i in DbContext.Members
								where i.Id == memberId
								select i).FirstOrDefaultAsync();

			var team = await (from i in DbContext.Teams
								.Include(p => p.Skipper)
							  where i.Id == teamId
							  select i).FirstOrDefaultAsync();

			if (member == null || team == null)
			{
				throw new ActionResultException(400, "提供的球员或者球队信息有误。");
			}

			// 管理权限不足
			if (!CanManage(team))
			{
				throw new ActionResultException(403);
			}

			var item = await (from i in DbContext.TeamMemberRegistrations
								.Include(p => p.Member)
								.Include(p => p.Team).ThenInclude(p => p.EventTeamRegistrations).ThenInclude(p => p.Event)
							  where i.MemberId == memberId && i.TeamId == teamId
							  select i).FirstOrDefaultAsync();
			return item;
		}

		#endregion

		/// <summary>
		/// 代表球队报名参加赛事。
		/// </summary>
		/// <param name="teamId">球队标识。</param>
		/// <param name="eventId">赛事标识。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ApplyEvent(int teamId, int eventId)
		{
			// 提取数据

			var team = await (from i in DbContext.Teams.Include(p => p.Skipper)
							  where i.Id == teamId
							  select i).FirstOrDefaultAsync();

			var eventItem = await (from i in DbContext.Events
								   where i.Id == eventId
								   select i).FirstOrDefaultAsync();

			// 数据有问题

			if (team == null || eventItem == null)
			{
				return HttpBadRequest("提供的参数不正确。");
			}


			// 管理权限

			if (!CanManage(team))
			{
				throw new ActionResultException(403, "你没有权限管理该队伍。");
			}

			// 报名状态

			if (eventItem.State != EventState.Registring || !eventItem.AllowTeamRegistrations)
			{
				throw new ActionResultException(400, "赛事目前不接受队伍报名。");
			}

			// 重复报名

			var existItem = await (from i in DbContext.EventTeamRegistrations
								   where i.TeamId == teamId && i.EventId == eventId
								   select i).FirstOrDefaultAsync();

			if (existItem != null)
			{
				throw new ActionResultException(400, "该球队已经报名参与了该赛事。");
			}

			// 创建项目
			DbContext.EventTeamRegistrations.Add(new EventTeamRegistration
			{
				EventId = eventId,
				TeamId = teamId,
				AuditState = AuditState.NotCommitted,
				AuditCommitTime = null
			});

			// 日志
			DbContext.Logs.Add(new Log
			{
				ActionType = ActionType.TeamCreateEventApply,
				RelatedTeam = team,
				RelatedEvent = eventItem,
				CC98Id = User.GetUserName(),
				Time = DateTime.Now
			});

			try
			{
				await DbContext.SaveChangesAsync();
				Messages.Add(OperationMessageLevel.Success, "操作成功。", "你已经创建了参赛申请。请转到个人资料管理页面设定参赛名单。");
				return Ok();

			}
			catch (DbUpdateException ex)
			{
				return HttpBadRequest(ex.Message);
			}

		}
	}

}
