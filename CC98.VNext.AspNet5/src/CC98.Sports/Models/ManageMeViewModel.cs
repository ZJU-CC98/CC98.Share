using System.Collections.Generic;

namespace CC98.Sports.Models
{
	/// <summary>
	/// 为 Manage/Me 页面提供数据。
	/// </summary>
	public class ManageMeViewModel
    {
		/// <summary>
		/// 获取或设置成员的集合。
		/// </summary>
		public IEnumerable<Member> Members { get; set; }

		/// <summary>
		/// 获取或设置球队的集合。
		/// </summary>
		public IEnumerable<Team> Teams { get; set; } 

		/// <summary>
		/// 获取或设置注册球队赛事的集合。
		/// </summary>
		public IEnumerable<EventTeamRegistration> EventTeamRegistrations { get; set; } 
    }
}
