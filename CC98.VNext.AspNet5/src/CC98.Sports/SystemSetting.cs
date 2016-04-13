using System.ComponentModel.DataAnnotations;

namespace CC98.Sports
{
	/// <summary>
	/// 表示系统设置。
	/// </summary>
	public class SystemSetting
	{
		/// <summary>
		/// 获取或设置数据分页的尺寸。
		/// </summary>
		[Range(10, 50)]
		public int PageSize { get; set; } = 20;

		/// <summary>
		/// 获取或设置允许的最大附件大小。
		/// </summary>
		[Range(0, long.MaxValue)]
		public long MaxAttachementSize { get; set; } = 1024 * 1024L;

		/// <summary>
		/// 获取或设置允许的附件的数量上限。
		/// </summary>
		[Range(0, int.MaxValue)]
		public int MaxAttachementCount { get; set; } = 10;

		/// <summary>
		/// 成员 ID 的显示格式。
		/// </summary>
		public string MemberIdFormat { get; set; } = "";

		/// <summary>
		/// 赛事 ID 的显示格式。
		/// </summary>
		public string EventIdFormat { get; set; } = "";

		/// <summary>
		/// 队伍 ID 的显示格式。
		/// </summary>
		public string TeamIdFormat { get; set; } = "";

		/// <summary>
		/// 候选部门列表。
		/// </summary>
		public string DepartmentList { get; set; } = "";

		/// <summary>
		/// 位置列表。
		/// </summary>
		public string LocationList { get; set; } = "";

		/// <summary>
		/// 用于判断外籍运动员的标准国籍。
		/// </summary>
		public string DefaultNation { get; set; } = "中国";

		/// <summary>
		/// 公告消息。
		/// </summary>
		public string Announcement { get; set; } = "";

		/// <summary>
		/// 是否开放审核申请。
		/// </summary>
		public bool OpenUserReviewRequest { get; set; } = true;

		/// <summary>
		/// 是否开放球队注册。
		/// </summary>
		public bool OpenTeamReviewRequest { get; set; } = true;

		/// <summary>
		/// 默认设置值。
		/// </summary>
		public static SystemSetting Default { get; } = new SystemSetting();
	}
}
