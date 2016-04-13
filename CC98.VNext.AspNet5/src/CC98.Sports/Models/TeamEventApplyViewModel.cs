namespace CC98.Sports.Models
{
	/// <summary>
	/// 表示球队进行赛事报名的数据视图。
	/// </summary>
	public class TeamEventApplyViewModel
    {
		/// <summary>
		/// 获取或设置要报名的球队的标识。
		/// </summary>
		public int TeamId { get; set; }

		/// <summary>
		/// 获取或设置要报名的赛事的标识。
		/// </summary>
		public int EventId { get; set; }
    }
}
