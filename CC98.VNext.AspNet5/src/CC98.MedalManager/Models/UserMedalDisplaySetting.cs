using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.MedalManager.Models
{
	/// <summary>
	/// 表示用户对特定勋章的显示设置。
	/// </summary>
	[Table("UserMedalDisplaySettingSet")]
	public class UserMedalDisplaySetting
	{
		/// <summary>
		/// 获取或设置关联的用户的标识。
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// 获取或设置关联的勋章的标识。
		/// </summary>
		public int MedalId { get; set; }

		/// <summary>
		/// 获取或设置关联的勋章。
		/// </summary>
		[ForeignKey(nameof(MedalId))]
		public Medal Medal { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示用户是否隐藏该勋章。
		/// </summary>
		public bool IsHide { get; set; }

		/// <summary>
		/// 获取或设置用户对勋章设定的权重。
		/// </summary>
		public int SortWeight { get; set; }

	}
}