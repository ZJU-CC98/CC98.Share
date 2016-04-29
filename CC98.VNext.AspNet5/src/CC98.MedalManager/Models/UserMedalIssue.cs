using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.MedalManager.Models
{
	/// <summary>
	/// 表示用户和勋章的颁发关系。
	/// </summary>
	[Table("UserMedalIssueSet")]
	public class UserMedalIssue
	{
		/// <summary>
		/// 获取或设置关联的用户的标识。
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// 获取或设置被颁发的勋章的标识。
		/// </summary>
		public int MedalId { get; set; }

		/// <summary>
		/// 获取或设置被颁发的勋章。
		/// </summary>
		[ForeignKey(nameof(MedalId))]
		public Medal Medal { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示勋章颁发还处于审核状态。
		/// </summary>
		public bool IsUnderReview { get; set; }

		/// <summary>
		/// 获取或设置颁发的时间。
		/// </summary>
		public DateTime Time { get; set; }

		/// <summary>
		/// 获取或设置本次颁发的过期时间，以天为单位。设置为 <c>null</c> 表示不会过期。
		/// </summary>
		[Range(0, int.MaxValue)]
		public int? ExpireDays { get; set; }
	}
}