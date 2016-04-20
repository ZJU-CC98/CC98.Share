using System.ComponentModel.DataAnnotations;

namespace CC98.Sports.Models
{
	/// <summary>
	/// 成员搜索数据模块。
	/// </summary>
	public class MemberSearchViewModel
    {
		/// <summary>
		/// 搜索的名称。
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 搜索的类型。
		/// </summary>
		public MemberSearchType Type { get; set; }
    }

	/// <summary>
	/// 表示成员的搜索类型。
	/// </summary>
	public enum MemberSearchType
	{
		/// <summary>
		/// 全部成员。
		/// </summary>
		[Display(Name = "全部")]
		All,
		/// <summary>
		/// 官员。
		/// </summary>
		[Display(Name = "官员")]
		Officer,
		/// <summary>
		/// 领队。
		/// </summary>
		[Display(Name = "领队")]
		Leader,
		/// <summary>
		/// 球员。
		/// </summary>
		[Display(Name = "球员")]
		Player
	}
}
