using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.LogOn.Models
{
	/// <summary>
	/// 表示 CC98V2 版本用户。
	/// </summary>
	[Table("Users")]
	public class IdentityUser
	{
		/// <summary>
		/// 用户的标识。
		/// </summary>
		/// <returns></returns>
		[Key]
		[Required]
		public int Id { get; set; }

		/// <summary>
		/// 用户的 V1 版本标识。
		/// </summary>
		/// <returns></returns>
		public int OldId { get; set; }

		/// <summary>
		/// 用户名。
		/// </summary>
		/// <returns></returns>
		[Required]
		public string UserName { get; set; }
	}
}