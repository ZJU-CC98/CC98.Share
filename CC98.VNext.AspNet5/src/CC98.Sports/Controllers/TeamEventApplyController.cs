using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Sakura.AspNet;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CC98.Sports.Controllers
{
	/// <summary>
	/// 提供队伍对赛事的申请操作。
	/// </summary>
	public class TeamEventApplyController : Controller
	{
		private SportDataModel DbContext { get; }
		private ICollection<OperationMessage> Messages { get; }

		public TeamEventApplyController(SportDataModel dbContext, IOperationMessageAccessor messageAccessor)
		{
			DbContext = dbContext;
			Messages = messageAccessor.Messages;
		}

		#region 成员操作

		/// <summary>
		/// 添加赛事成员。
		/// </summary>
		/// <param name="eventId">赛事标识。</param>
		/// <param name="teamId">队伍标识。</param>
		/// <param name="memberId">要添加的一个或多个成员的标识。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddMember(int eventId, int teamId, int[] memberId)
		{
			var item = await GetItemAndCheckManageAsync(eventId, teamId);



			var members = await (from i in DbContext.Members
									.Include(p => p.TeamRegistrations)
									.Include(p => p.EventRegistrations)
								 where memberId.Contains(i.Id)
								 select i).ToArrayAsync();

			// 逐个添加用户
			foreach (var m in members)
			{
				// 个人身份验证要求
				if (m.AuditState != AuditState.Accepted)
				{
					return HttpBadRequest($"成员 {m.Name} 未通过个人身份审核认证。");
				}

				// 入队要求
				if (!m.TeamRegistrations.Any(
						i => i.TeamId == teamId && i.TeamAuditState == AuditState.Accepted && i.MemberAuditState == AuditState.Accepted))
				{
					return HttpBadRequest($"成员 {m.Name} 不式该队伍的正式成员。");
				}

				// 球员和领队不能重复参加
				if ((m.Type == MemberType.Player || m.Type == MemberType.Skipper) && m.EventRegistrations.Any(i => i.EventId == eventId))
				{
					return HttpBadRequest($"成员 {m.Name} 已经报名参加了该赛事。");
				}

				// 日志
				DbContext.EventMemberRegistrations.Add(new EventMemberRegistration
				{
					EventId = eventId,
					TeamId = teamId,
					MemberId = m.Id
				});
			}

			// 如果用户没有权限，则重置审核状态
			if (!User.CanOperate())
			{
				item.AuditState = AuditState.NotCommitted;
				item.AuditCommitTime = null;
			}

			await DbContext.SaveChangesAsync();
			Messages.Add(OperationMessageLevel.Success, "操作成功。", "选定的用户已经被加入参赛列表。");
			return Ok();
		}

		/// <summary>
		/// 删除赛事成员。
		/// </summary>
		/// <param name="eventId">赛事标识。</param>
		/// <param name="teamId">队伍标识。</param>
		/// <param name="memberId">要删除的成员标识。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteMember(int eventId, int teamId, int memberId)
		{

			var item = await GetItemAndCheckManageAsync(eventId, teamId);

			var memberItem = (from i in item.Members
							  where i.MemberId == memberId
							  select i).FirstOrDefault();

			if (memberItem == null)
			{
				Messages.Add(OperationMessageLevel.Error, "操作失败。", "要删除的成员不在报名表内。");
			}
			else
			{
				item.Members.Remove(memberItem);
				DbContext.EventMemberRegistrations.Remove(memberItem);

				// 修正特殊影响
				if (item.CaptainId == memberId)
				{
					item.CaptainId = null;
				}
				if (item.CoachId == memberId)
				{
					item.CoachId = null;
				}
				if (item.SkipperId == memberId)
				{
					item.SkipperId = null;
				}

				// 如果用户没有操作权限，执行删除将重置审核状态
				if (!User.CanOperate())
				{
					item.AuditState = AuditState.NotCommitted;
					item.AuditCommitTime = null;
				}

				try
				{
					await DbContext.SaveChangesAsync();
				}
				catch (DbUpdateException ex)
				{
					Messages.Add(OperationMessageLevel.Error, "操作失败。", $"操作中发生错误：{ex.Message}");
				}
			}

			// 回到修改主页
			return RedirectToAction("Edit", "TeamEventApply", new { eventId, teamId });
		}


		#endregion

		/// <summary>
		/// 显示编辑界面。
		/// </summary>
		/// <param name="eventId">赛事标识。</param>
		/// <param name="teamId">队伍标识。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Edit(int eventId, int teamId)
		{
			var item = await GetItemAndCheckManageAsync(eventId, teamId);
			return View(item);
		}

		/// <summary>
		/// 执行赛事名单的编辑操作。
		/// </summary>
		/// <param name="model">新数据。</param>
		/// <param name="commitAudit">是否提交审核。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(EventTeamRegistration model, bool commitAudit)
		{
			var item = await GetItemAndCheckManageAsync(model.EventId, model.TeamId);

			// 移除关联属性检测
			ModelState.Remove(nameof(model.Skipper));
			ModelState.Remove(nameof(model.Captain));
			ModelState.Remove(nameof(model.Coach));

			// 审核相关
			if (User.CanOperate())
			{
				// 管理员有直接编辑权限，则检查审核时间

				if (model.AuditState != AuditState.NotCommitted && model.AuditCommitTime == null)
				{
					ModelState.AddModelError("", "审核状态不是未申请时，必须设定审核提交时间。");
				}
			}
			else
			{
				// 用户没有编辑权限，检查赛事状态是否允许提交
				if (commitAudit)
				{
					// 提交申请，但是现在禁止申请
					if (item.Event.State != EventState.Registring)
					{
						ModelState.AddModelError("", "目前赛事不处于注册报名阶段，无法提交审核申请。");
					}
					else
					{
						model.AuditState = AuditState.Pending;
						model.AuditCommitTime = DateTime.Now;
					}
				}
				else
				{
					model.AuditState = AuditState.NotCommitted;
					model.AuditCommitTime = null;
				}

				model.EventState = TeamEventState.NotStarted;
			}


			if (ModelState.IsValid)
			{
				// 释放
				foreach (var member in item.Members)
				{
					DbContext.Detach(member);
				}

				// 替换数据
				DbContext.Replace(item, model);

				// 日志
				DbContext.Logs.Add(new Log
				{
					CC98Id = User.GetUserName(),
					ActionType = ActionType.TeamEditEventApply,
					RelatedTeamId = model.TeamId,
					RelatedEventId = model.EventId,
					Time = DateTime.Now,
				});

				try
				{
					await DbContext.SaveChangesAsync();
					Messages.Add(OperationMessageLevel.Success, "操作成功。", "参赛名单已经更改。");

					return RedirectToAction("Edit", "TeamEventApply", new { teamId = model.TeamId, eventId = model.EventId });
				}
				catch (DbUpdateException ex)
				{
					ModelState.AddModelError("", ex.Message);
				}

			}

			return View(model);
		}

		/// <summary>
		/// 删除参赛记录。
		/// </summary>
		/// <param name="eventId">赛事标识。</param>
		/// <param name="teamId">队伍标识。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int eventId, int teamId)
		{
			var item = await GetItemAndCheckManageAsync(eventId, teamId);

			// 归档状态
			if (item.EventState != TeamEventState.NotStarted)
			{
				throw new ActionResultException(400, "该参赛队伍记录已经归档，无法删除。");
			}

			// 锁定而用户没有管理权
			if (item.AuditState == AuditState.Accepted && item.Event.State != EventState.Registring && !User.CanOperate())
			{
				throw new ActionResultException(400, "该参赛队伍记录已经锁定，无法删除。");
			}

			// 删除对象和所有关联的成员注册
			DbContext.EventTeamRegistrations.Remove(item);
			DbContext.EventMemberRegistrations.RemoveRange(item.Members);

			// 日志
			DbContext.Logs.Add(new Log
			{
				ActionType = ActionType.TeamDeleteEventApply,
				CC98Id = User.GetUserName(),
				RelatedTeamId = teamId,
				RelatedEventId = eventId,
				Time = DateTime.Now,
			});

			try
			{
				await DbContext.SaveChangesAsync();
				Messages.Add(OperationMessageLevel.Success, "操作成功。", "选定的参赛记录已经被删除。");
				return Ok();
			}
			catch (DbUpdateException ex)
			{
				return HttpBadRequest(ex.Message);
			}
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
		/// 尝试获取一个给定的项目，并且检查当前用户是否可以编辑。
		/// </summary>
		/// <param name="eventId">赛事标识。</param>
		/// <param name="teamId">队伍标识。</param>
		/// <returns>赛事的队伍注册信息。</returns>
		private async Task<EventTeamRegistration> GetItemAndCheckManageAsync(int eventId, int teamId)
		{
			var item = await GetItemAsync(eventId, teamId);

			if (item == null)
			{
				throw new ActionResultException(404);
			}

			if (!CanManage(item.Team))
			{
				throw new ActionResultException(403);
			}

			// 名单被锁定后只有管理员可以编辑
			if (item.IsLocked() && !User.CanOperate())
			{
				throw new ActionResultException(400, "开始比赛后参赛名单无法更改。");
			}

			return item;
		}

		/// <summary>
		/// 尝试获取具有给定标识的注册信息。
		/// </summary>
		/// <param name="eventId">赛事标识。</param>
		/// <param name="teamId">队伍标识。</param>
		/// <returns>注册信息。</returns>
		private Task<EventTeamRegistration> GetItemAsync(int eventId, int teamId)
		{
			return (from i in DbContext.EventTeamRegistrations
						.Include(p => p.Event)
						.Include(p => p.Team).ThenInclude(p => p.MemberRegistrations).ThenInclude(p => p.Member)
						.Include(p => p.Members).ThenInclude(p => p.Member)
					where i.TeamId == teamId && i.EventId == eventId
					select i).FirstOrDefaultAsync();
		}

		/// <summary>
		/// 查看对象的详细信息。
		/// </summary>
		/// <param name="eventId">赛事标识。</param>
		/// <param name="teamId">队伍标识。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		public async Task<IActionResult> Detail(int eventId, int teamId)
		{
			var item = await GetItemAsync(eventId, teamId);

			// 找不到申请对象
			if (item == null)
			{
				throw new ActionResultException(404);
			}

			return View(item);
		}

		/// <summary>
		/// 执行审核操作。
		/// </summary>
		/// <param name="id">要审核的成员标识。</param>
		/// <param name="reviewState">要设置的审核状态。</param>
		/// <param name="reason">审核原因。</param>
		/// <param name="returnUrl">审核后返回的地址。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(UserUtility.ReviewPolicy)]
		public async Task<IActionResult> Review(int eventId, int teamId, AuditState reviewState, string reason, string returnUrl)
		{
			var item = await GetItemAsync(eventId, teamId);

			// 找不到项目
			if (item == null)
			{
				return HttpNotFound();
			}

			// 未提交审核
			if (item.AuditState == AuditState.NotCommitted)
			{
				Messages.Add(OperationMessageLevel.Error, "操作失败。", "当前参赛名单尚未提交审核申请，你无法进行审核。");
				return RedirectToAction("Detail", "TeamEventApply", new { eventId, teamId, returnUrl });
			}

			// 修改审核状态
			item.AuditState = reviewState;

			try
			{
				// 根据审核状态设定提示消息和日志记录类型
				string messageTextFormat;
				string userMessageTextFormat;
				ActionType logActionType;

				switch (reviewState)
				{
					case AuditState.Accepted:
						messageTextFormat = "通过了队伍 {0} 参加赛事 {1} 的资格申请。";
						userMessageTextFormat = "你提交的参赛资格审核申请（队伍：{0}，赛事：{1}）已被管理员通过。";
						logActionType = ActionType.TeamEventApplyReviewAccept;
						break;
					case AuditState.Rejected:
						messageTextFormat = "拒绝了队伍 {0} 参加赛事 {1} 的资格申请。";
						userMessageTextFormat = "你提交的参赛资格审核申请（队伍：{0}，赛事：{1}）已被管理员驳回。";
						logActionType = ActionType.TeamEventApplyReviewReject;
						break;
					case AuditState.Pending:
						messageTextFormat = "将队伍 {0} 参加赛事 {1} 的资格申请恢复到未审核状态。";
						userMessageTextFormat = "你提交的参赛资格审核申请（队伍：{0}，赛事：{1}）已被管理员重置，需要重新审核。";
						logActionType = ActionType.TeamEventApplyReviewReset;
						break;
					default:
						throw new ArgumentException("参数不是枚举的有效选项之一。", nameof(reviewState));
				}

				// 追加日志
				var logItem = new Log
				{
					CC98Id = User.GetUserName(),
					ActionType = logActionType,
					RelatedEventId = eventId,
					RelatedTeamId = teamId,
					Time = DateTime.Now
				};


				DbContext.Logs.Add(logItem);

				// 如果队伍由领队，则给领队发消息。
				if (item.Skipper != null)
				{
					// 用户提示消息
					var message = new Message
					{
						Receiver = item.Skipper.CC98Id,
						Content = string.Format(CultureInfo.CurrentCulture, userMessageTextFormat, item.Team.Name, item.Event.Name),
						IsRead = false,
						Time = DateTime.Now
					};

					// 如果设定了原因，则追加提示
					if (!string.IsNullOrEmpty(reason))
					{
						message.Content += string.Format(CultureInfo.CurrentCulture, "原因：{0}。", reason);
					}

					DbContext.Messages.Add(message);
				}

				// 执行数据库更改
				await DbContext.SaveChangesAsync();

				// 消息提示
				Messages.Add(OperationMessageLevel.Success, "审核操作成功。", string.Format(CultureInfo.CurrentCulture, messageTextFormat, item.Team.Name, item.Event.Name));
			}
			catch (DbUpdateException)
			{
				// 执行错误
				Messages.Add(OperationMessageLevel.Error, "操作失败。", "执行审核操作时发生错误，请稍后再试一次。");

			}

			// 如果有跳转地址，则返回到跳转地址。
			if (!string.IsNullOrEmpty(returnUrl))
			{
				return Redirect(returnUrl);
			}

			// 返回详细信息页面。
			return RedirectToAction("Detail", "TeamEventApply", new { teamId, eventId });
		}
	}
}
