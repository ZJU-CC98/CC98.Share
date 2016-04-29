using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.LogOn.Models
{
	/// <summary>
	/// 表示 CC98 V1 版本用户。
	/// </summary>
	[Table("user")]
	public class CC98V1User
	{
		/// <summary>
		/// 获取或设置该用户的标识。
		/// </summary>
		[Key]
		[Column("UserId")]
		public int Id { get; set; }

		/// <summary>
		/// 获取或设置该用户的名称。
		/// </summary>
		[Required]
		[Column("UserName")]
		[StringLength(50)]
		public string Name { get; set; }

		/// <summary>
		/// 获取或设置该用户的密码的散列。
		/// </summary>
		[Required]
		[Column("UserPassword")]
		[StringLength(20)]
		public string PasswordHash { get; set; }

		/// <summary>
		/// 获取或设置该用户绑定的浙大通行证账户。
		/// </summary>
		[Column("regmail")]
		[StringLength(30)]
		public string ZjuSsoId { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示当前用户是否在线。
		/// </summary>
		[Column("online")]
		public bool IsOnline { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示当前用户是否已经通过验证。
		/// </summary>
		[Column("verified")]
		public bool IsVerified { get; set; }

		/// <summary>
		/// 获取或设置该用户的注册时间。
		/// </summary>
		[Column("addDate")]
		public DateTime RegisterTime { get; set; }

		/// <summary>
		/// 获取或设置用户的上次登录时间。
		/// </summary>
		[Column("lastlogin")]
		public DateTime LastLogOnTime { get; set; }

		/// <summary>
		/// 获取或设置用户上次登录的 IP 地址。
		/// </summary>
		[Column("UserLastIP")]
		[StringLength(15)]
		public string LastLogOnIP { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示用户是否是保留用户。
		/// </summary>
		[Column("reserved")]
		public bool IsReserved { get; set; }

		/// <summary>
		/// 获取或设置用户的状态。
		/// </summary>
		[Column("lockuser")]
		public UserState State { get; set; }

		/// <summary>
		/// 获取或设置用户的等级。
		/// </summary>
		[Column("userclass")]
		[StringLength(20)]
		public string Level { get; set; }

		/// <summary>
		/// 获取或设置用户的发言总数。
		/// </summary>
		[Column("Article")]
		public int TotalPosts { get; set; }

		/// <summary>
		/// 获取或设置用户的登录总数。
		/// </summary>
		[Column("logins")]
		public int LogOnTimes { get; set; }

		/// <summary>
		/// 获取或设置用户的性别。
		/// </summary>
		[Column("sex")]
		public Gender Gender { get; set; }
	}
}