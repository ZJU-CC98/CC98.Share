using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Data.Entity;
using System.Collections.ObjectModel;

namespace CC98.Sports
{
	/// <summary>
	/// 数据库模型。
	/// </summary>
	public class SportDataModel : DbContext
	{
		/// <summary>
		/// 获取或设置该系统中包含的球员的集合。
		/// </summary>
		public virtual DbSet<Member> Members { get; set; }

		/// <summary>
		/// 获取或设置系统中包含的赛事的集合。
		/// </summary>
		public virtual DbSet<Event> Events { get; set; }

		/// <summary>
		/// 获取或设置系统中包含的队伍的集合。
		/// </summary>
		public virtual DbSet<Team> Teams { get; set; }

		/// <summary>
		/// 获取或设置系统中包含的日志的集合。
		/// </summary>
		public virtual DbSet<Log> Logs { get; set; }

		/// <summary>
		/// 获取或设置系统中包含的消息的集合。
		/// </summary>
		public virtual DbSet<Message> Messages { get; set; }

		/// <summary>
		/// 获取或设置系统中包含的比赛的集合。
		/// </summary>
		public virtual DbSet<Game> Games { get; set; }

		/// <summary>
		/// 获取或设置系统中包含的比赛申请的集合。
		/// </summary>
		public virtual DbSet<OfficerGameApplication> OfficerGameApplications { get; set; }


		/// <summary>
		/// 获取或设置队伍在赛事中的注册情况。
		/// </summary>
		public virtual DbSet<EventTeamRegistration> EventTeamRegistrations { get; set; }

		/// <summary>
		/// 获取或设置成员在队伍中的注册情况。
		/// </summary>
		public virtual DbSet<TeamMemberRegistration> TeamMemberRegistrations { get; set; }

		/// <summary>
		/// 获取或设置成员在赛事中的注册情况。
		/// </summary>
		public virtual DbSet<EventMemberRegistration> EventMemberRegistrations { get; set; }

		/// <summary>
		/// Override this method to further configure the model that was discovered by convention from the entity types
		///                 exposed in <see cref="Microsoft.Data.Entity.DbSet{TEntity}"/> properties on your derived context. The resulting model may be cached
		///                 and re-used for subsequent instances of your derived context.
		/// </summary>
		/// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically
		///                 define extension methods on this object that allow you to configure aspects of the model that are specific
		///                 to a given database.
		///             </param>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<EventTeamRegistration>().HasKey(i => new { i.EventId, i.TeamId });

			// 索引
			modelBuilder.Entity<Member>().HasIndex(m => m.AuditState);

			// 消息用户索引
			modelBuilder.Entity<Message>().HasIndex(m => m.IsRead);

			modelBuilder.Entity<OfficerGameApplication>().HasKey(i => new { i.GameId, i.MemberId });

			modelBuilder.Entity<TeamMemberRegistration>().HasKey(i => new { i.TeamId, i.MemberId });

			// 成员索引
			modelBuilder.Entity<EventMemberRegistration>().HasKey(i => new { i.EventId, i.TeamId, i.MemberId });
		}
	}

	/// <summary>
	/// 表示成员类型。
	/// </summary>
	public enum MemberType
	{
		None = 0,
		/// <summary>
		/// 官员。
		/// </summary>
		[Display(Name = "官员")]
		Officer,
		/// <summary>
		/// 领队。
		/// </summary>
		[Display(Name = "领队")]
		Skipper,
		/// <summary>
		/// 教练
		/// </summary>
		[Display(Name = "教练")]
		Coach,
		/// <summary>
		/// 球员
		/// </summary>
		[Display(Name = "运动员")]
		Player,
	}

	/// <summary>
	/// 表示性别。
	/// </summary>
	public enum Gender
	{
		/// <summary>
		/// 男性。
		/// </summary>
		[Display(Name = "男")]
		Male,
		/// <summary>
		/// 女性。
		/// </summary>
		[Display(Name = "女")]
		Female,
	}

	/// <summary>
	/// 定义附件的类型。
	/// </summary>
	public enum AttachmentType
	{
		/// <summary>
		/// 自定义类型。
		/// </summary>
		Custom = 0,
		/// <summary>
		/// 个人身份证明。
		/// </summary>
		PersonalId,
		/// <summary>
		/// 学校身份证明。
		/// </summary>
		SchoolId,
		/// <summary>
		/// 在校证明。
		/// </summary>
		SchoolCertificate,
		/// <summary>
		/// 健康证明。
		/// </summary>
		HealthCertificate,

	}

	/// <summary>
	/// 表示注册球员系统的一个成员。
	/// </summary>
	public class Member
	{
		/// <summary>
		/// 获取或设置该成员的标识。
		/// </summary>
		[Key]
		[Display(Name = "标识")]
		public int Id { get; set; }

		/// <summary>
		/// 获取或设置该成员的 CC98 ID。
		/// </summary>
		[Display(Name = "CC98 账号")]
		public string CC98Id { get; set; }

		/// <summary>
		/// 获取或设置该成员的姓名。
		/// </summary>
		[Required]
		[Display(Name = "姓名")]
		public string Name { get; set; }

		[Display(Name = "描述")]
		public string Description { get; set; }

		/// <summary>
		/// 获取或设置该成员的性别。
		/// </summary>
		[Display(Name = "性别")]
		public Gender Gender { get; set; }

		/// <summary>
		/// 获取或设置该球员的国籍。
		/// </summary>
		[Display(Name = "国籍")]
		public string Nationality { get; set; }

		/// <summary>
		/// 获取或设置该成员的个人身份标识。
		/// </summary>
		[Display(Name = "个人证件号码")]
		public string PersonalId { get; set; }

		/// <summary>
		/// 获取或设置该成员的学校身份标识。
		/// </summary>
		[Display(Name = "校园证件号码")]
		public string SchoolId { get; set; }

		/// <summary>
		/// 获取或设置个人身份标识对应的正面图片的地址。
		/// </summary>
		public string UploadImagePaths { get; set; }

		/// <summary>
		/// 获取或设置该成员所在的部门。
		/// </summary>
		[Display(Name = "所属部门", ShortName = "部门")]
		public string Department { get; set; }

		/// <summary>
		/// 获取或设置该成员的联系方式。
		/// </summary>
		[Display(Name = "联系方式")]
		public string ContactInfo { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示当前成员是否为专业运动员。
		/// </summary>
		public bool IsProfessional { get; set; }

		/// <summary>
		/// 获取或设置成员的官员类型。
		/// </summary>
		public OfficerTypes OfficerTypes { get; set; }

		/// <summary>
		/// 获取或设置当前注册信息的审核状态。
		/// </summary>
		public AuditState AuditState { get; set; }

		/// <summary>
		/// 获取或设置审核的提交时间。
		/// </summary>
		public DateTime? AuditCommitTime { get; set; }

		/// <summary>
		/// 获取或设置成员的注册类型。
		/// </summary>
		[Display(Name = "注册类型", ShortName = "类型")]
		public MemberType Type { get; set; }

		/// <summary>
		/// 获取或设置成员的位置。
		/// </summary>
		[Display(Name = "位置")]
		public string Location { get; set; }


		/// <summary>
		/// 获取该成员包含的日志的集合。
		/// </summary>
		[InverseProperty(nameof(Log.RelatedMember))]
		public virtual ICollection<Log> Logs { get; set; } = new Collection<Log>();

		/// <summary>
		/// 获取该成员的注册球队的集合。
		/// </summary>
		[InverseProperty(nameof(TeamMemberRegistration.Member))]
		public virtual ICollection<TeamMemberRegistration> TeamRegistrations { get; set; } = new Collection<TeamMemberRegistration>();

		/// <summary>
		/// 获取该成员作为领队的球员的集合。
		/// </summary>
		[InverseProperty(nameof(Team.Skipper))]
		public virtual ICollection<Team> SkipperTeams { get; set; } = new Collection<Team>();

		/// <summary>
		/// 获取该成员作为队长的球队的集合。
		/// </summary>
		[InverseProperty(nameof(Team.Captain))]
		public virtual ICollection<Team> CaptainTeams { get; set; } = new Collection<Team>();

		/// <summary>
		/// 获取该成员作为教练的球队的集合。
		/// </summary>
		[InverseProperty(nameof(Team.Coach))]
		public virtual ICollection<Team> CoachTeams { get; set; } = new Collection<Team>();

		/// <summary>
		/// 获取该成员参赛的集合。
		/// </summary>
		[InverseProperty(nameof(EventMemberRegistration.Member))]
		public virtual ICollection<EventMemberRegistration> EventRegistrations { get; set; } = new Collection<EventMemberRegistration>();

		/// <summary>
		/// 在派生类中重写时，用于获取该成员所属的类型。
		/// </summary>
		[NotMapped]
		public virtual MemberType MemberType => MemberType.None;


	}

	/// <summary>
	/// 表示一个教练。
	/// </summary>
	public class Coach : Member
	{
		/// <summary>
		/// 在派生类中重写时，用于获取该成员所属的类型。
		/// </summary>
		public override MemberType MemberType => MemberType.Coach;
	}

	/// <summary>
	/// 表示一个比赛官员。
	/// </summary>
	public class Officer : Member
	{
		/// <summary>
		/// 在派生类中重写时，用于获取该成员所属的类型。
		/// </summary>
		public override MemberType MemberType => MemberType.Officer;
	}

	/// <summary>
	/// 表示需要验证详细身份的成员类型。该类型作为 <see cref="Player"/> 和 <see cref="Leader"/> 的基类。
	/// </summary>
	public abstract class VerifiedMember : Member
	{
	}

	/// <summary>
	/// 标识一个领队。
	/// </summary>
	public class Leader : VerifiedMember
	{

		/// <summary>
		/// 在派生类中重写时，用于获取该成员所属的类型。
		/// </summary>
		public override MemberType MemberType => MemberType.Skipper;
	}

	/// <summary>
	/// 标识一个运动员。
	/// </summary>
	public class Player : Member
	{
		/// <summary>
		/// 在派生类中重写时，用于获取该成员所属的类型。
		/// </summary>
		public override MemberType MemberType => MemberType.Player;
	}

	/// <summary>
	/// 表示审核的状态。
	/// </summary>
	public enum AuditState
	{
		/// <summary>
		/// 尚未提交审核。
		/// </summary>
		[Display(Name = "未提交")]
		NotCommitted = 0,
		/// <summary>
		/// 正在等待结果。
		/// </summary>
		[Display(Name = "审核中")]
		Pending,
		/// <summary>
		/// 已接受。
		/// </summary>
		[Display(Name = "已通过")]
		Accepted,
		/// <summary>
		/// 已拒绝。
		/// </summary>
		[Display(Name = "已拒绝")]
		Rejected,
	}

	/// <summary>
	/// 表示官员的类型。
	/// </summary>
	[Flags]
	public enum OfficerTypes
	{
		None = 0x0,
		/// <summary>
		/// 裁判。
		/// </summary>
		[Display(Name = "裁判")]
		Referee = 0x1,
		[Display(Name = "助理裁判")]
		AssistantReferee = 0x2,
		[Display(Name = "第四官员")]
		FourthOfficial = 0x4,
		[Display(Name = "监督")]
		Commissioner = 0x8
	}

	/// <summary>
	/// 定义球员的备注选项。
	/// </summary>
	[Flags]
	public enum PlayerRemarks
	{
		/// <summary>
		/// 无备注。
		/// </summary>
		None = 0x0,
		/// <summary>
		/// 专业运动员。
		/// </summary>
		Professional = 0x1,
		/// <summary>
		/// 留学生。
		/// </summary>
		Foreign = 0x2
	}

	/// <summary>
	/// 表示一个成员的注册信息。
	/// </summary>
	public class MemberRegistration
	{
		/// <summary>
		/// 获取或设置注册关联的成员标识。
		/// </summary>
		public int MemberId { get; set; }

		/// <summary>
		/// 获取或设置注册关联的成员。
		/// </summary>
		[Required]
		[ForeignKey(nameof(MemberId))]
		public Member Member { get; set; }

		/// <summary>
		/// 获取或设置注册的类型。
		/// </summary>
		public MemberType Type { get; set; }

		/// <summary>
		/// 获取或设置注册的状态。
		/// </summary>
		public AuditState AuditState { get; set; }

		/// <summary>
		/// 获取该成员的球队注册状态的信息。
		/// </summary>
		[InverseProperty(nameof(TeamMemberRegistration.Member))]
		public virtual ICollection<TeamMemberRegistration> TeamRegistrations { get; set; } = new Collection<TeamMemberRegistration>();
	}

	/// <summary>
	/// 表示一场比赛的队伍报名情况。
	/// </summary>
	public class GameTeamRegistration
	{
		/// <summary>
		/// 获取或设置对应的队伍标识。
		/// </summary>
		public int TeamId { get; set; }

		/// <summary>
		/// 获取或设置对应的队伍。
		/// </summary>
		[ForeignKey(nameof(TeamId))]
		public Team Team { get; set; }

		/// <summary>
		/// 获取或设置对应的赛事的标识。
		/// </summary>
		public int GameId { get; set; }

		/// <summary>
		/// 获取或设置对应的赛事。
		/// </summary>
		[ForeignKey(nameof(GameId))]
		public Game Game { get; set; }

		/// <summary>
		/// 获取或设置该名单中包含的成员的集合。
		/// </summary>
		[InverseProperty(nameof(EventMemberRegistration.TeamRegistration))]
		public virtual IList<EventMemberRegistration> Members { get; set; } = new Collection<EventMemberRegistration>();

		/// <summary>
		/// 获取或设置名单中的领队的标识。
		/// </summary>
		public int? SkipperId { get; set; }

		/// <summary>
		/// 获取或设置名单中的领队。
		/// </summary>
		[ForeignKey(nameof(SkipperId))]
		public Member Skipper { get; set; }

		/// <summary>
		/// 获取或设置名单中的队长的标识。
		/// </summary>
		public int? CaptainId { get; set; }

		/// <summary>
		/// 获取或设置名单中的队长。
		/// </summary>
		[ForeignKey(nameof(CaptainId))]
		public Member Captain { get; set; }

		/// <summary>
		/// 获取或设置名单中的教练的标识。
		/// </summary>
		public int? CoachId { get; set; }

		/// <summary>
		/// 获取或设置名单中的教练。
		/// </summary>
		[ForeignKey(nameof(CoachId))]
		public Member Coach { get; set; }

		/// <summary>
		/// 获取或设置报名的其他描述信息。
		/// </summary>
		public string Description { get; set; }
	}

	/// <summary>
	/// 表示赛事中队伍的注册情况。
	/// </summary>
	public class EventTeamRegistration
	{
		/// <summary>
		/// 获取或设置对应的队伍标识。
		/// </summary>
		public int TeamId { get; set; }

		/// <summary>
		/// 获取或设置对应的队伍。
		/// </summary>
		[ForeignKey(nameof(TeamId))]
		public Team Team { get; set; }

		/// <summary>
		/// 获取或设置对应的赛事的标识。
		/// </summary>
		public int EventId { get; set; }

		/// <summary>
		/// 获取或设置对应的赛事。
		/// </summary>
		[ForeignKey(nameof(EventId))]
		public Event Event { get; set; }

		/// <summary>
		/// 获取或设置注册的状态。
		/// </summary>
		public AuditState AuditState { get; set; }

		/// <summary>
		/// 获取或设置注册的提交时间。
		/// </summary>
		public DateTime? AuditCommitTime { get; set; }

		/// <summary>
		/// 获取或设置报名参赛的状态。
		/// </summary>
		public TeamEventState EventState { get; set; }

		/// <summary>
		/// 获取或设置该名单中包含的成员的集合。
		/// </summary>
		[InverseProperty(nameof(EventMemberRegistration.TeamRegistration))]
		public virtual IList<EventMemberRegistration> Members { get; set; } = new Collection<EventMemberRegistration>();

		public int? SkipperId { get; set; }

		/// <summary>
		/// 获取或设置名单中的领队。
		/// </summary>
		[ForeignKey(nameof(SkipperId))]
		public Member Skipper { get; set; }

		public int? CaptainId { get; set; }

		/// <summary>
		/// 获取或设置名单中的队长。
		/// </summary>
		[ForeignKey(nameof(CaptainId))]
		public Member Captain { get; set; }

		/// <summary>
		/// 获取或设置名单中的教练。
		/// </summary>
		public int? CoachId { get; set; }

		[ForeignKey(nameof(CoachId))]
		public Member Coach { get; set; }

		/// <summary>
		/// 获取或设置该报名团队的分组名称。
		/// </summary>
		public string Group { get; set; }

		/// <summary>
		/// 获取或设置该报名团队的分组内编号。
		/// </summary>
		public string GroupNumber { get; set; }

		/// <summary>
		/// 获取或设置报名的其他描述信息。
		/// </summary>
		public string Description { get; set; }
	}

	/// <summary>
	/// 定义一个赛事中参与的成员信息。
	/// </summary>
	public class EventMemberRegistration
	{
		/// <summary>
		/// 获取或设置成员的标识。
		/// </summary>
		public int MemberId { get; set; }

		/// <summary>
		/// 获取或设置赛事的标识。
		/// </summary>
		public int EventId { get; set; }

		/// <summary>
		/// 获取或设置队伍的标识。
		/// </summary>
		public int TeamId { get; set; }

		/// <summary>
		/// 获取或设置对应的赛事。
		/// </summary>
		[ForeignKey(nameof(EventId))]
		public Event Event { get; set; }

		/// <summary>
		/// 获取或设置对应的队伍。
		/// </summary>
		[ForeignKey(nameof(TeamId))]
		public Team Team { get; set; }

		/// <summary>
		/// 该成员所在的球队注册名单。
		/// </summary>
		[ForeignKey("EventId,TeamId")]
		public EventTeamRegistration TeamRegistration { get; set; }

		[ForeignKey(nameof(MemberId))]
		public Member Member { get; set; }

		/// <summary>
		/// 参赛编号。仅对球员有效。
		/// </summary>
		public string Number { get; set; }

		/// <summary>
		/// 参赛的其他辅助信息。
		/// </summary>
		public string Remark { get; set; }
	}

	/// <summary>
	/// 表示一场比赛的状态。
	/// </summary>
	public enum GameState
	{
		/// <summary>
		/// 尚未开始。
		/// </summary>
		[Display(Name = "尚未开始")]
		NotStarted,
		/// <summary>
		/// 正在准备。
		/// </summary>
		[Display(Name = "正在准备")]
		Preparing,
		/// <summary>
		/// 正在比赛。
		/// </summary>
		[Display(Name = "正在进行")]
		Running,
		/// <summary>
		/// 已结束。
		/// </summary>
		[Display(Name = "比赛结束")]
		Ended
	}

	/// <summary>
	/// 定义球队参加一次比赛的状态。
	/// </summary>
	public enum TeamEventState
	{
		/// <summary>
		/// 已经加入赛事但尚未开始比赛。
		/// </summary>
		[Display(Name = "报名参赛")]
		NotStarted,
		/// <summary>
		/// 正在比赛。
		/// </summary>
		[Display(Name = "正在参赛")]
		Playing,
		/// <summary>
		/// 比赛已结束。
		/// </summary>
		[Display(Name = "赛事结束")]
		Ended
	}

	/// <summary>
	/// 表示球员和球队的注册情况。
	/// </summary>
	public class TeamMemberRegistration
	{
		/// <summary>
		/// 获取或设置注册的球队的标识。
		/// </summary>
		public int TeamId { get; set; }

		/// <summary>
		/// 获取或设置注册的球队。
		/// </summary>
		[ForeignKey(nameof(TeamId))]
		[Required]
		public Team Team { get; set; }

		/// <summary>
		/// 获取或设置注册的成员的标识。
		/// </summary>
		public int MemberId { get; set; }

		/// <summary>
		/// 获取或设置注册的成员。
		/// </summary>
		[ForeignKey(nameof(MemberId))]
		[Required]
		public Member Member { get; set; }

		/// <summary>
		/// 获取或设置注册的时间。
		/// </summary>
		public DateTime Time { get; set; }

		/// <summary>
		/// 获取或设置球队的审核状态。
		/// </summary>
		public AuditState TeamAuditState { get; set; }

		/// <summary>
		/// 获取或设置球员的审核状态。
		/// </summary>
		public AuditState MemberAuditState { get; set; }
	}

	/// <summary>
	/// 表示一个日志项目。
	/// </summary>
	public class Log
	{
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// 当前项目的创建者。
		/// </summary>
		public string CC98Id { get; set; }

		public int? RelatedTeamId { get; set; }

		/// <summary>
		/// 当前项目关联到的球队。
		/// </summary>
		[ForeignKey(nameof(RelatedTeamId))]
		public Team RelatedTeam { get; set; }

		/// <summary>
		/// 当前项目关联到的球员。
		/// </summary>
		public Member RelatedMember { get; set; }

		public int? RelatedEventId { get; set; }

		/// <summary>
		/// 当前项目关联到的赛事。
		/// </summary>
		[ForeignKey(nameof(RelatedEventId))]
		public Event RelatedEvent { get; set; }

		/// <summary>
		/// 当前项目关联到的时间。
		/// </summary>
		public DateTime Time { get; set; }

		/// <summary>
		/// 当前项目的备注。
		/// </summary>
		public string Remark { get; set; }

		/// <summary>
		/// 当前项目的操作类型。
		/// </summary>
		public ActionType ActionType { get; set; }
	}

	/// <summary>
	/// 定义日志的行为类型。
	/// </summary>
	public enum ActionType
	{
		[Display(Name = "创建赛事")]
		CreateEvent,
		[Display(Name = "编辑赛事")]
		EditEvent,
		[Display(Name = "删除赛事")]
		DeleteEvent,
		[Display(Name = "创建成员")]
		CreateMember,
		[Display(Name = "编辑成员")]
		EditMember,
		[Display(Name = "删除成员")]
		DeleteMember,
		[Display(Name = "创建队伍")]
		CreateTeam,
		[Display(Name = "编辑队伍")]
		EditTeam,
		[Display(Name = "删除队伍")]
		DeleteTeam,
		[Display(Name = "清除系统日志")]
		ClearLog,
		[Display(Name = "更改系统设置")]
		ChangeSystemSetting,
		[Display(Name = "通过成员审核申请")]
		MemberReviewAccpet,
		[Display(Name = "拒绝成员审核申请")]
		MemberReviewReject,
		[Display(Name = "重置成员审核申请")]
		MemberReviewReset,
		/// <summary>
		/// 球员接受球队邀请。
		/// </summary>
		[Display(Name = "成员接受球队邀请")]
		MemberAcceptTeam,
		/// <summary>
		/// 球员拒绝球队邀请。
		/// </summary>
		[Display(Name = "成员拒绝球队邀请")]
		MemberRejectTeam,
		/// <summary>
		/// 球员创建入队申请。
		/// </summary>
		[Display(Name = "成员创建入队申请")]
		MemberCreateApply,
		/// <summary>
		/// 球员删除入队申请。
		/// </summary>
		[Display(Name = "成员删除入队申请")]
		MemberDeleteApply,
		/// <summary>
		/// 球员离队。
		/// </summary>
		[Display(Name = "成员离开球队")]
		MemberLeaveTeam,
		/// <summary>
		/// 球队接受球员邀请。
		/// </summary>
		[Display(Name = "球队接受成员申请")]
		TeamAcceptMember,
		/// <summary>
		/// 球队拒绝球员邀请。
		/// </summary>
		[Display(Name = "球队拒绝成员申请")]
		TeamRejectMember,
		/// <summary>
		/// 球队创建入队邀请。
		/// </summary>
		[Display(Name = "球队创建入队邀请")]
		TeamCreateInvite,
		/// <summary>
		/// 球队删除入队邀请。
		/// </summary>
		[Display(Name = "球队删除入队邀请")]
		TeamDeleteInvite,
		/// <summary>
		/// 球队删除球员。
		/// </summary>
		[Display(Name = "球队删除成员")]
		TeamDeleteMember,
		[Display(Name = "球队申请参赛")]
		TeamCreateEventApply,
		[Display(Name = "球队修改参赛信息")]
		TeamEditEventApply,
		[Display(Name = "球队取消参赛")]
		TeamDeleteEventApply,
		[Display(Name = "通过队伍参赛申请")]
		TeamEventApplyReviewAccept,
		[Display(Name = "拒绝队伍参赛申请")]
		TeamEventApplyReviewReject,
		[Display(Name = "重置队伍参赛申请")]
		TeamEventApplyReviewReset,
		[Display(Name = "创建赛程")]
		CreateGame,
		[Display(Name = "编辑赛程")]
		EditGame,
		[Display(Name = "删除赛程")]
		DeleteGame,
		/// <summary>
		/// 清空赛程。
		/// </summary>
		[Display(Name = "清空赛程")]
		ClearEventGames,
		/// <summary>
		/// 批量编辑赛事赛程。
		/// </summary>
		[Display(Name = "批量编辑赛事赛程")]
		EventGameBatchEdit,

		[Display(Name = "启动赛事")]
		RunEvent,
		[Display(Name = "结束赛事")]
		CloseEvent,
	}

	/// <summary>
	/// 表示一个系统消息。
	/// </summary>
	public class Message
	{
		/// <summary>
		/// 获取或设置该消息的标识。
		/// </summary>
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// 获取或设置消息的接收者。
		/// </summary>
		[Required]
		public string Receiver { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示该消息是否已经被读取。
		/// </summary>
		public bool IsRead { get; set; }

		/// <summary>
		/// 获取或设置消息的内容。
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// 获取或设置消息的时间。
		/// </summary>
		public DateTime Time { get; set; }
	}

	/// <summary>
	/// 表示一场赛事。
	/// </summary>
	public class Event
	{
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// 获取或设置赛事的名称。
		/// </summary>
		[Required]
		[Display(Name = "赛事名称", ShortName = "名称")]
		public string Name { get; set; }

		/// <summary>
		/// 获取或设置赛事的状态。
		/// </summary>
		[Display(Name = "赛事状态", ShortName = "状态")]
		public EventState State { get; set; }

		/// <summary>
		/// 获取或设置该赛事的类型。
		/// </summary>
		[Display(Name = "赛事类型", ShortName = "类型")]
		public EventType Type { get; set; }


		/// <summary>
		/// 获取或设置赛事允许的最多队伍数目。留空表示不作限制。
		/// </summary>
		[Display(Name = "队伍数量上限", ShortName = "队伍上限")]
		[Range(2, int.MaxValue)]
		public int? MaxTeamCount { get; set; }

		/// <summary>
		/// 获取或设置赛事的说明信息。
		/// </summary>
		[Display(Name = "赛事说明", ShortName = "说明")]
		public string Description { get; set; }

		/// <summary>
		/// 获取或设置赛事的链接。
		/// </summary>
		[Display(Name = "赛事链接", ShortName = "链接")]
		public string Link { get; set; }

		/// <summary>
		/// 获取或设置赛事的组队类型。
		/// </summary>
		[Display(Name = "允许球队自由报名", ShortName = "自由报名")]
		public bool AllowTeamRegistrations { get; set; }

		[Display(Name = "运动员数量下限")]
		[Range(0, int.MaxValue)]
		public int? PlayerMin { get; set; }

		[Display(Name = "运动员数量上限")]
		[Range(0, int.MaxValue)]
		public int? PlayerMax { get; set; }

		[Display(Name = "专业运动员数量上限")]
		[Range(0, int.MaxValue)]
		public int? ProfessionalMax { get; set; }

		[Display(Name = "外籍运动员数量上限")]
		[Range(0, int.MaxValue)]
		public int? ForeignMax { get; set; }

		[Display(Name = "外援数量上限")]
		[Range(0, int.MaxValue)]
		public int? ExternalMax { get; set; }

		/// <summary>
		/// 获取或设置赛事中包含的队伍的集合。
		/// </summary>
		[InverseProperty(nameof(EventTeamRegistration.Event))]
		public virtual IList<EventTeamRegistration> TeamRegistrations { get; set; } = new Collection<EventTeamRegistration>();

		/// <summary>
		/// 获取赛事中注册的成员的集合。
		/// </summary>
		[InverseProperty(nameof(EventMemberRegistration.Event))]
		public virtual IList<EventMemberRegistration> MemberRegistrations { get; set; } = new Collection<EventMemberRegistration>();

		[InverseProperty(nameof(Log.RelatedEvent))]
		public virtual IList<Log> Logs { get; set; } = new Collection<Log>();

		/// <summary>
		/// 获取或设置该赛事中包含的比赛的集合。
		/// </summary>
		[InverseProperty(nameof(Game.Event))]
		public virtual IList<Game> Games { get; set; } = new Collection<Game>();
	}

	/// <summary>
	/// 表示比赛官员对于比赛的申请状态。
	/// </summary>
	public class OfficerGameApplication
	{
		/// <summary>
		/// 获取或设置申请对应的成员的标识。
		/// </summary>
		public int MemberId { get; set; }

		/// <summary>
		/// 获取或设置申请对应的成员。
		/// </summary>
		[ForeignKey(nameof(MemberId))]
		[Required]
		public Member Member { get; set; }

		/// <summary>
		/// 获取或设置申请对应的比赛的标识。
		/// </summary>
		public int GameId { get; set; }

		/// <summary>
		/// 获取或设置申请对应的比赛。
		/// </summary>
		[ForeignKey(nameof(GameId))]
		[Required]
		public Game Game { get; set; }

		/// <summary>
		/// 获取或设置官员的设定状态。
		/// </summary>
		public AuditState AuditState { get; set; }
	}

	/// <summary>
	/// 表示赛事中的一场比赛。
	/// </summary>
	public class Game
	{
		/// <summary>
		/// 获取或设置比赛的标识。
		/// </summary>
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// 获取或设置该比赛关联到的赛事的标识。
		/// </summary>
		public int EventId { get; set; }

		/// <summary>
		/// 获取或设置该比赛关联到的赛事。
		/// </summary>
		[ForeignKey(nameof(EventId))]
		[Required]
		[Display(Name = "关联赛事", ShortName = "赛事")]
		public Event Event { get; set; }

		/// <summary>
		/// 获取或设置比赛的名称。
		/// </summary>
		[Required]
		[Display(Name = "名称")]
		public string Name { get; set; }

		/// <summary>
		/// 获取或设置比赛的描述。
		/// </summary>
		public string Descrption { get; set; }

		/// <summary>
		/// 获取或设置比赛的位置。
		/// </summary>
		public string Location { get; set; }

		/// <summary>
		/// 获取或设置比赛的开始时间。
		/// </summary>
		public DateTime? StartTime { get; set; }
		/// <summary>
		/// 获取或设置比赛的结束时间。
		/// </summary>
		public DateTime? EndTime { get; set; }

		/// <summary>
		/// 获取或设置比赛关联到的队伍 1 的标识。
		/// </summary>
		public int? Team1Id { get; set; }

		/// <summary>
		/// 获取或设置比赛关联到的队伍 1。
		/// </summary>
		[ForeignKey(nameof(Team1Id))]
		public Team Team1 { get; set; }

		/// <summary>
		/// 获取或设置比赛关联到的队伍 2 的标识。
		/// </summary>
		public int? Team2Id { get; set; }

		/// <summary>
		/// 获取或设置比赛关联到的队伍 2。
		/// </summary>
		[ForeignKey(nameof(Team2Id))]
		public Team Team2 { get; set; }

		/// <summary>
		/// 获取或设置比赛的状态。
		/// </summary>
		public GameState State { get; set; }

		/// <summary>
		/// 赛事的轮次编号。
		/// </summary>
		public int Round { get; set; }

		/// <summary>
		/// 获取或设置成员的申请情况。
		/// </summary>
		public virtual ICollection<OfficerGameApplication> OfficerApplications { get; set; }
	}

	/// <summary>
	/// 表示赛事组队的类型。
	/// </summary>
	public enum EventTeamType
	{
		/// <summary>
		/// 自由组队。
		/// </summary>
		[Display(Name = "自由组队")]
		Free,
		/// <summary>
		/// 限制组队。
		/// </summary>
		[Display(Name = "限制组队")]
		Restricted,
	}

	/// <summary>
	/// 表示赛事的类型。
	/// </summary>
	public enum EventType
	{
		/// <summary>
		/// 杯赛。
		/// </summary>
		[Display(Name = "杯赛")]
		Cup,
		/// <summary>
		/// 联赛。
		/// </summary>
		[Display(Name = "联赛")]
		League,
	}

	/// <summary>
	/// 表示赛事的状态。
	/// </summary>
	public enum EventState
	{
		/// <summary>
		/// 赛事尚未开放。
		/// </summary>
		[Display(Name = "尚未开放")]
		NotOpen,
		/// <summary>
		/// 赛事正在进行准备工作。
		/// </summary>
		[Display(Name = "正在准备")]
		Preparing,
		/// <summary>
		/// 赛事处于开放状态。
		/// </summary>
		[Display(Name = "注册报名")]
		Registring,
		/// <summary>
		/// 赛事处于进行状态。
		/// </summary>
		[Display(Name = "正在进行")]
		Running,
		/// <summary>
		/// 赛事已关闭。
		/// </summary>
		[Display(Name = "赛事结束")]
		Closed
	}


	/// <summary>
	/// 表示一个队伍。
	/// </summary>
	public class Team
	{

		/// <summary>
		/// 获取或设置队伍的标识。
		/// </summary>
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// 获取或设置该队伍的领队的标识。
		/// </summary>
		public int? SkipperId { get; set; }

		/// <summary>
		/// 获取或设置该队伍的领队。
		/// </summary>
		[ForeignKey(nameof(SkipperId))]
		public Member Skipper { get; set; }

		/// <summary>
		/// 获取或设置该队伍的队长的标识。
		/// </summary>
		public int? CaptainId { get; set; }

		/// <summary>
		/// 获取或设置该队伍的队长。
		/// </summary>
		[ForeignKey(nameof(CaptainId))]
		public Member Captain { get; set; }

		/// <summary>
		/// 获取或设置该队伍的教练的标识。
		/// </summary>
		public int? CoachId { get; set; }

		/// <summary>
		/// 获取或设置该队伍的教练。
		/// </summary>
		[ForeignKey(nameof(CoachId))]
		public Member Coach { get; set; }

		/// <summary>
		/// 获取或设置队伍的名称。
		/// </summary>
		[Required]
		public string Name { get; set; }

		/// <summary>
		/// 获取或设置队伍的描述。
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// 获取或设置队伍的链接。
		/// </summary>
		public string Link { get; set; }

		/// <summary>
		/// 获取或设置队伍关联的部门。
		/// </summary>
		public string Department { get; set; }

		/// <summary>
		/// 获取或设置队伍的位置。
		/// </summary>
		public string Location { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示是否锁定了球队。
		/// </summary>
		public bool IsLocked { get; set; }

		/// <summary>
		/// 主场球衣颜色。
		/// </summary>
		public string ClotheColor1 { get; set; }

		/// <summary>
		/// 客场球衣颜色。
		/// </summary>
		public string ClotheColor2 { get; set; }

		/// <summary>
		/// 获取该球队的成员注册的集合。
		/// </summary>
		[InverseProperty(nameof(TeamMemberRegistration.Team))]
		public virtual ICollection<TeamMemberRegistration> MemberRegistrations { get; set; } = new Collection<TeamMemberRegistration>();

		/// <summary>
		/// 获取该球队参与的赛事的集合。
		/// </summary>
		[InverseProperty(nameof(EventTeamRegistration.Team))]
		public virtual ICollection<EventTeamRegistration> EventTeamRegistrations { get; set; } = new Collection<EventTeamRegistration>();

		/// <summary>
		/// 获取该球队关联到的日志的集合。
		/// </summary>
		[InverseProperty(nameof(Log.RelatedTeam))]
		public virtual ICollection<Log> Logs { get; set; } = new Collection<Log>();

		/// <summary>
		/// 获取球队作为队伍 1 参加的比赛的集合。
		/// </summary>
		[InverseProperty(nameof(Game.Team1))]
		public virtual ICollection<Game> Game1s { get; set; } = new Collection<Game>();

		/// <summary>
		/// 获取球队作为队伍 2 参加的比赛的集合。
		/// </summary>
		[InverseProperty(nameof(Game.Team2))]
		public virtual ICollection<Game> Game2s { get; set; } = new Collection<Game>();
	}

}
