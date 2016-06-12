using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.Medal.Data
{
	/// <summary>
	/// 表示用户勋章的颁发记录。
	/// </summary>
	public class UserMedalIssue
	{
		/// <summary>
		/// 获取或设置用户的标识。
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// 获取或设置要颁发的勋章的标识。
		/// </summary>
		public int MedalId { get; set; }

		/// <summary>
		/// 获取或设置要颁发的勋章。
		/// </summary>
		[ForeignKey(nameof(MedalId))]
		public Medal Medal { get; set; }

		/// <summary>
		/// 获取或设置颁发的时间。
		/// </summary>
		public DateTime Time { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示当前颁发是否处于申请审核阶段。
		/// </summary>
		public bool NeedReview { get; set; }
	}
}