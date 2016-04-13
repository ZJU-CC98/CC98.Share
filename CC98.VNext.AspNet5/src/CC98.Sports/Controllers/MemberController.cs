using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Sakura.AspNet;
using Sakura.AspNet.Mvc.Messages;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CC98.Sports.Controllers
{
	using System.Collections.Generic;
	using Models;

	/// <summary>
	/// 提供成员相关操作界面。
	/// </summary>
	public class MemberController : Controller
	{

		/// <summary>
		/// 数据库上下文对象。
		/// </summary>
		private SportDataModel DbContext { get; }

		/// <summary>
		/// 应用程序设置服务。
		/// </summary>
		private AppSettingService<SystemSetting> SettingService { get; }

		/// <summary>
		/// 消息集合。
		/// </summary>
		private ICollection<OperationMessage> Messages { get; }

		/// <summary>
		/// 初始化一个控制器的新实例。
		/// </summary>
		/// <param name="dbContext">数据库上下文对象。</param>
		/// <param name="settingService">设置服务对象。</param>
		/// <param name="messageAccessor">消息访问器。</param>
		public MemberController(SportDataModel dbContext, AppSettingService<SystemSetting> settingService, IOperationMessageAccessor messageAccessor)
		{
			DbContext = dbContext;
			SettingService = settingService;
			Messages = messageAccessor.Messages;
		}

		/// <summary>
		/// 释放该对象占用的所有资源。
		/// </summary>
		~MemberController()
		{
			DbContext.Dispose();
		}

		/// <summary>
		/// 显示主页。
		/// </summary>
		/// <param name="page">页码。</param>
		/// <returns>操作结果。</returns>
		public IActionResult Index(int page = 1)
		{
			ViewBag.ShowAdmin = User.CanOperate();
			return View(DbContext.Members.ToPagedList(SettingService.Current.PageSize, page));
		}

		/// <summary>
		/// 显示搜索页面。
		/// </summary>
		/// <param name="model">搜索信息。</param>
		/// <param name="page">页码。</param>
		/// <returns>操作结果。</returns>
		public IActionResult Search(MemberSearchViewModel model, int page = 1)
		{
			IQueryable<Member> result = DbContext.Members;

			// 名字搜索
			if (!string.IsNullOrEmpty(model.Name))
			{
				result = result.Where(i => i.Name.Contains(model.Name));
			}

			// 类型搜索
			switch (model.Type)
			{
				case MemberSearchType.Officer:
					result = result.Where(i => i.Type == MemberType.Officer);
					break;
				case MemberSearchType.Leader:
					result = result.Where(i => i.Type == MemberType.Skipper);
					break;
				case MemberSearchType.Player:
					result = result.Where(i => i.Type == MemberType.Player);
					break;
			}

			ViewBag.SearchInfo = model;
			ViewBag.ShowAdmin = User.CanAdmin();
			return View(result.ToPagedList(SettingService.Current.PageSize, page));
		}

		/// <summary>
		/// 创建成员界面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize]
		public IActionResult Create()
		{
			return View();
		}

		/// <summary>
		/// 执行成员创作操作。
		/// </summary>
		/// <param name="model">数据对象。</param>
		/// <param name="files">上传的文件列表。</param>
		/// <param name="commitAudit">是否提交审核。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Member model, string[] files, bool commitAudit)
		{
			// 对模型进行额外检查
			HandleMemberModel(model, files, commitAudit);

			if (ModelState.IsValid)
			{
				// 更新核心数据
				model.CC98Id = User.GetUserName();

				DbContext.Members.Add(model);

				DbContext.Logs.Add(new Log
				{
					ActionType = ActionType.CreateMember,
					RelatedMember = model,
					CC98Id = User.GetUserName(),
					Time = DateTime.Now
				});

				try
				{
					await DbContext.SaveChangesAsync();
					Messages.Add(new OperationMessage(OperationMessageLevel.Success, "操作成功。", "已经成功地注册了成员的信息。"));
					return RedirectToAction("Index");
				}
				// 发生错误
				catch (Exception ex)
				{
					ModelState.AddModelError("", ex.Message);
				}
			}

			return View(model);
		}

		/// <summary>
		/// 根据用户是否审核申请的设定，修改数据的相关信息。
		/// </summary>
		/// <param name="model">数据对象。</param>
		/// <param name="commitAudit">是否提交审核。</param>
		private void HandleAuditCommit(Member model, bool commitAudit)
		{
			// 操作权限
			if (User.CanOperate())
			{
				if (model.AuditState != AuditState.NotCommitted && model.AuditCommitTime == null)
				{
					ModelState.AddModelError("", "审核状态不是未申请时，必须设定审核提交时间。");
				}
			}
			else
			{
				if (commitAudit)
				{
					// 提交申请，但是现在禁止申请
					if (!SettingService.Current.OpenUserReviewRequest)
					{
						ModelState.AddModelError("", "目前管理员禁止提交审核申请。");
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
			}
		}

		/// <summary>
		/// 编辑给定的球员信息。
		/// </summary>
		/// <param name="id">要编辑的成员的 ID。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Edit(int id)
		{
			// 查找项目
			var item = await GetItemAndCheckManage(id);

			CheckMemberLock(item);

			// 可以执行额外管理操作
			ViewBag.CanAdmin = User.CanAdmin();


			return View(item);

		}

		/// <summary>
		/// 检查用户是否被锁定。如果被锁定则拒绝操作。
		/// </summary>
		/// <param name="item">要检查的对象。</param>
		private void CheckMemberLock(Member item)
		{
			if (item.IsLocked() && !User.CanOperate())
			{
				throw new ActionResultException(400, "当前用户正在参与一场赛事，无法修改个人信息。");
			}
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> Edit(Member model, string[] files, bool commitAudit)
		{
			// 对模型进行额外检查
			HandleMemberModel(model, files, commitAudit);

			// 原始项目
			var item = await GetItemAndCheckManage(model.Id);

			if (item.IsLocked() && !User.CanOperate())
			{
				ModelState.AddModelError("", "当前用户正在参与一场赛事，无法修改个人信息。");
			}

			if (ModelState.IsValid)
			{
				// 取消关联原有对象并关联新对象
				DbContext.Replace(item, model);

				DbContext.Logs.Add(new Log
				{
					ActionType = ActionType.EditMember,
					RelatedMember = model,
					CC98Id = User.GetUserName(),
					Time = DateTime.Now
				});

				// 保存
				try
				{
					await DbContext.SaveChangesAsync();
					Messages.Add(new OperationMessage(OperationMessageLevel.Success, "操作成功。", "成员信息已经成功更新。"));
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
		/// 对成员数据模型进行额外检查和处理。
		/// </summary>
		/// <param name="model">要检查的模型。</param>
		/// <param name="files">上传的文件列表。</param>
		/// <param name="commitAudit">是否提交审核申请。</param>
		private void HandleMemberModel(Member model, string[] files, bool commitAudit)
		{
			// 报名类型检查
			if (model.Type == MemberType.None)
			{
				ModelState.AddModelError(nameof(model.Type), "你必须选择一个报名类型。");
			}

			// 官员检查
			if (model.Type == MemberType.Officer && model.OfficerTypes == OfficerTypes.None)
			{
				ModelState.AddModelError("", "注册为官员时必须至少选择一个官员类型。");
			}

			// 附件检查和处理
			if (files.Length > SettingService.Current.MaxAttachementCount)
			{
				ModelState.AddModelError("", "附件数量超过允许上限。");
			}
			else
			{
				// 附件列表
				model.UploadImagePaths = string.Join("\n", files);
			}

			// 审核检查和处理
			HandleAuditCommit(model, commitAudit);
		}

		/// <summary>
		/// 查看注册人员的详细信息。
		/// </summary>
		/// <param name="id">用户标识。</param>
		/// <param name="returnUrl">返回 URL。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		public async Task<IActionResult> Detail(int id, string returnUrl)
		{
			// 查找项目
			var item = await (from i in DbContext.Members
								.Include(p => p.EventRegistrations).ThenInclude(p => p.TeamRegistration)
								.Include(p => p.TeamRegistrations).ThenInclude(p => p.Team)
							  where i.Id == id
							  select i).FirstOrDefaultAsync();

			// 找不到
			if (item == null)
			{
				return HttpNotFound();
			}

			ViewBag.ReturnUrl = returnUrl;
			ViewBag.ShowReview = User.CanReview();
			// 详细信息权限：用户是自己，或可以审核，或可以组织
			ViewBag.ShowDetail = string.Equals(item.CC98Id, User.GetUserName(), StringComparison.OrdinalIgnoreCase) ||
								 User.CanReview() || User.CanOrganize();

			// 管理权限：用户是自己或可以操作
			ViewBag.ShowAdmin = string.Equals(item.CC98Id, User.GetUserName(), StringComparison.OrdinalIgnoreCase) ||
								User.CanOperate();

			return View(item);
		}

		/// <summary>
		/// 删除人员注册信息。
		/// </summary>
		/// <param name="id">用户标识。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public async Task<IActionResult> Delete(int id)
		{
			var item = await GetItemAndCheckManage(id);
			CheckMemberLock(item);

			try
			{
				DbContext.Members.Remove(item);

				DbContext.Logs.Add(new Log
				{
					ActionType = ActionType.DeleteMember,
					CC98Id = User.GetUserName(),
					Time = DateTime.Now,
					RelatedMember = item
				});


				// 修改日志
				foreach (var i in item.Logs)
				{
					i.RelatedMember = null;
					i.Remark += string.Format(CultureInfo.InvariantCulture, "{0} (#{1})", item.Name, item.Id);
				}


				await DbContext.SaveChangesAsync();
				Messages.Add(new OperationMessage(OperationMessageLevel.Success, "操作成功。", "已经删除了选定的成员信息。"));
				return Ok();
			}
			catch (DbUpdateException ex)
			{
				return HttpBadRequest(ex.Message);
			}
		}

		/// <summary>
		/// 获取具有指定标识的成员，并确定用户具有管理权限。
		/// </summary>
		/// <param name="id">要获取的成员的标识。</param>
		/// <returns>具有给定标识的成员。</returns>
		private async Task<Member> GetItemAndCheckManage(int id)
		{
			var item = await (from i in DbContext.Members
								.Include(p => p.Logs)
								.Include(p => p.EventRegistrations).ThenInclude(p => p.TeamRegistration)
							  where i.Id == id
							  select i).FirstOrDefaultAsync();

			// 找不到项目
			if (item == null)
			{
				throw new ActionResultException(404);
			}

			// 用户不是管理员，也不是所有者
			if (!CanManageMember(item))
			{
				throw new ActionResultException(403);
			}
			return item;
		}

		/// <summary>
		/// 执行通过审核操作。
		/// </summary>
		/// <param name="id">要审核的成员标识。</param>
		/// <param name="reviewState">要设置的审核状态。</param>
		/// <param name="reason">审核原因。</param>
		/// <param name="returnUrl">审核后返回的地址。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(UserUtility.ReviewPolicy)]
		public async Task<IActionResult> Review(int id, AuditState reviewState, string reason, string returnUrl)
		{
			var item = await (from i in DbContext.Members.Include(p => p.Logs)
							  where i.Id == id
							  select i).FirstOrDefaultAsync();

			// 找不到项目
			if (item == null)
			{
				return HttpNotFound();
			}

			// 未提交审核
			if (item.AuditState == AuditState.NotCommitted)
			{
				Messages.Add(OperationMessageLevel.Error, "操作失败。", "当前用户尚未提交审核申请，你无法进行审核。");
				return RedirectToAction("Detail", "Member", new { id });
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
						messageTextFormat = "通过了成员 {0} 的资格申请。";
						userMessageTextFormat = "你提交的审核申请（姓名：{0}，身份：{1}）已被管理员通过。";
						logActionType = ActionType.MemberReviewAccpet;
						break;
					case AuditState.Rejected:
						messageTextFormat = "拒绝了成员 {0} 的资格申请。";
						userMessageTextFormat = "你提交的审核申请（姓名：{0}，身份：{1}）已被管理员驳回。";
						logActionType = ActionType.MemberReviewReject;
						break;
					case AuditState.Pending:
						messageTextFormat = "将成员 {0} 的资格申请恢复到未审核状态。";
						userMessageTextFormat = "你提交的审核申请（姓名：{0}，身份：{1}）已被管理员重置，需要重新审核。";
						logActionType = ActionType.MemberReviewReset;
						break;
					default:
						throw new ArgumentException("参数不是枚举的有效选项之一。", nameof(reviewState));
				}

				// 追加日志
				var logItem = new Log
				{
					CC98Id = User.GetUserName(),
					ActionType = logActionType,
					RelatedMember = item,
					Time = DateTime.Now
				};


				DbContext.Logs.Add(logItem);

				// 用户提示消息
				var message = new Message
				{
					Receiver = item.CC98Id,
					Content = string.Format(CultureInfo.CurrentCulture, userMessageTextFormat, item.Name, item.Type.GetDisplayName()),
					IsRead = false,
					Time = DateTime.Now
				};

				// 如果设定了原因，则追加提示
				if (!string.IsNullOrEmpty(reason))
				{
					message.Content += string.Format(CultureInfo.CurrentCulture, "原因：{0}。", reason);
				}

				DbContext.Messages.Add(message);


				// 执行数据库更改
				await DbContext.SaveChangesAsync();

				// 消息提示
				Messages.Add(OperationMessageLevel.Success, "审核操作成功。", string.Format(CultureInfo.CurrentCulture, messageTextFormat, item.Name));
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
			return RedirectToAction("Detail", "Member", new { id });
		}

		/// <summary>
		/// 获取一个值，指示当前用户是否对于给定的成员有管理权限。
		/// </summary>
		/// <param name="item">要判断的对象。</param>
		/// <returns>如果当前用户可以管理该成员，返回 true；否则返回 false。</returns>
		private bool CanManageMember(Member item)
		{
			return User.CanOperate() || IsOwner(item);
		}

		/// <summary>
		/// 获取一个值，指示当前用户是否是成员的所有者。
		/// </summary>
		/// <param name="item">要判断的对象。</param>
		/// <returns>如果当前用户是 <paramref name="item"/> 的所有者，返回 true；否则返回 false。</returns>
		private bool IsOwner(Member item)
		{
			return User.Identity.IsAuthenticated && string.Equals(item.CC98Id, User.GetUserName(), StringComparison.OrdinalIgnoreCase);
		}


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
		public async Task<IActionResult> ApplyTeam(int memberId, int teamId, string reason)
		{
			var member = await (from i in DbContext.Members
								where i.Id == memberId
								select i).FirstOrDefaultAsync();

			var team = await (from i in DbContext.Teams
							  where i.Id == teamId
							  select i).FirstOrDefaultAsync();

			if (member == null || team == null)
			{
				throw new ActionResultException(400, "提供的球员或者球队信息有误。");
			}

			// 管理权限不足
			if (!CanManageMember(member))
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
				MemberAuditState = AuditState.Accepted,
				TeamAuditState = AuditState.Pending,
				Time = DateTime.Now,
			};

			DbContext.TeamMemberRegistrations.Add(newItem);

			await AddMessageAndLogCore(newItem, ActionType.MemberCreateApply, reason,
				string.Format(CultureInfo.CurrentCulture, "成员 {0} 向队伍 {1} 发出了入队申请。", member.Name, team.Name),
				string.Format(CultureInfo.CurrentCulture, "成员 {0} 已经向队伍 {1} 发出了入队申请。", member.Name, team.Name));

			await DbContext.SaveChangesAsync();
			return Ok();
		}

		#region 申请相关

		/// <summary>
		/// 球员执行通过申请操作。
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
			return AuditTeamInviteCore(memberId, teamId, reason, true);
		}

		/// <summary>
		/// 球员执行拒绝申请操作。
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
			return AuditTeamInviteCore(memberId, teamId, reason, false);
		}

		/// <summary>
		/// 球员执行取消申请操作。
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

			// 球队发起，球员未通过
			if (item.MemberAuditState != AuditState.Accepted || item.TeamAuditState == AuditState.Accepted)
			{
				throw new ActionResultException(400, "必须是球员发起的申请才能执行这个操作。");
			}

			// 删除对象
			DbContext.TeamMemberRegistrations.Remove(item);

			// 创建日志并保存结果
			return await AddMessageAndLogCore(
				item,
				ActionType.MemberDeleteApply,
				reason,
				string.Format(CultureInfo.CurrentCulture, "成员 {0} 取消了向球队 {1} 发出的加入申请。", item.Member.Name, item.Team.Name),
				string.Format(CultureInfo.InvariantCulture, "成员 {0} 已经取消了向球队 {1} 发出的加入申请。", item.Member.Name, item.Team.Name));
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
		/// 球员执行离队操作。
		/// </summary>
		/// <param name="memberId">球员标识。</param>
		/// <param name="teamId">球队标识。</param>
		/// <param name="reason">操作原因。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LeaveTeam(int memberId, int teamId, string reason)
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
				throw new ActionResultException(400, "参与赛事期间无法离队。");

			}

			// 删除对象
			DbContext.TeamMemberRegistrations.Remove(item);

			FixMemberLeave(item.Team, memberId);

			// 创建日志并保存结果
			return await AddMessageAndLogCore(
				item,
				ActionType.MemberLeaveTeam,
				reason,
				string.Format(CultureInfo.CurrentCulture, "成员 {0} 离开了球队 {1}。", item.Member.Name, item.Team.Name),
				string.Format(CultureInfo.InvariantCulture, "成员 {0} 离开了球队 {1}。", item.Member.Name, item.Team.Name));
		}

		/// <summary>
		/// 执行通过或者拒绝球队申请的核心方法。
		/// </summary>
		/// <param name="memberId">球员标识。</param>
		/// <param name="teamId">球队标识。</param>
		/// <param name="reason">操作原因。</param>
		/// <param name="isAccept">true 表示通过申请，false 表示拒绝申请。</param>
		/// <returns>操作结果。</returns>
		private async Task<IActionResult> AuditTeamInviteCore(int memberId, int teamId, string reason, bool isAccept)
		{
			var item = await GetRegistrationItem(memberId, teamId);

			if (item == null)
			{
				throw new ActionResultException(400, "当前不存在对应的申请。");
			}

			if (item.TeamAuditState != AuditState.Accepted)
			{
				throw new ActionResultException(400, "必须是球队发起的申请才能执行这个操作。");
			}

			// 修改状态
			item.MemberAuditState = isAccept ? AuditState.Accepted : AuditState.Rejected;

			return isAccept
				? await AddMessageAndLogCore(
					item,
					ActionType.MemberAcceptTeam,
					reason,
					string.Format(CultureInfo.CurrentCulture, "成员 {0} 接受了球队 {1} 的加入邀请。", item.Member.Name, item.Team.Name),
					string.Format(CultureInfo.InvariantCulture, "成员 {0} 已经通过了队伍 {1} 的加入邀请。", item.Member.Name, item.Team.Name))
				: await AddMessageAndLogCore(
					item,
					ActionType.MemberRejectTeam,
					reason,
					string.Format(CultureInfo.CurrentCulture, "成员 {0} 拒绝了球队 {1} 的加入邀请。", item.Member.Name, item.Team.Name),
					string.Format(CultureInfo.InvariantCulture, "成员 {0} 已经拒绝了队伍 {1} 的加入邀请。", item.Member.Name, item.Team.Name));
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

			// 如果队伍有领队则追加领队消息
			if (item.Team.Skipper != null)
			{
				// 消息
				var message = new Message
				{
					Receiver = item.Team.Skipper.CC98Id,
					Content = messageContent,
					Time = DateTime.Now,
				};

				// 追加原因
				if (!string.IsNullOrEmpty(reason))
				{
					message.Content += string.Format(CultureInfo.CurrentCulture, "原因：{0}。", reason);
				}

				DbContext.Messages.Add(message);
			}

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
							  where i.Id == teamId
							  select i).FirstOrDefaultAsync();

			if (member == null || team == null)
			{
				throw new ActionResultException(400, "提供的球员或者球队信息有误。");
			}

			// 管理权限不足
			if (!CanManageMember(member))
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


	}
}
