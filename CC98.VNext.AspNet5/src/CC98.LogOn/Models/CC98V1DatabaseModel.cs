using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace CC98.LogOn.Models
{
	public class CC98V1DatabaseModel : DbContext
	{
		public CC98V1DatabaseModel()
		{

		}

		/// <summary>
		///     Override this method to further configure the model that was discovered by convention from the entity types
		///     exposed in <see cref="T:Microsoft.Data.Entity.DbSet`1" /> properties on your derived context. The resulting model may be cached
		///     and re-used for subsequent instances of your derived context.
		/// </summary>
		/// <remarks>
		///     If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.Data.Entity.DbContextOptionsBuilder.UseModel(Microsoft.Data.Entity.Metadata.IModel)" />)
		///     then this method will not be run.
		/// </remarks>
		/// <param name="modelBuilder">
		///     The builder being used to construct the model for this context. Databases (and other extensions) typically
		///     define extension methods on this object that allow you to configure aspects of the model that are specific
		///     to a given database.
		/// </param>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<CC98V1User>().HasAlternateKey(i => i.Name);
		}

		public virtual DbSet<CC98V1User> Users { get; set; }
	}

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

	[Flags]
	public enum UserState
	{
		/// <summary>
		/// 用户处于正常状态。
		/// </summary>
		Normal = 0x0,
		/// <summary>
		/// 用户被锁定。
		/// </summary>
		Locked = 0x1,
		/// <summary>
		/// 用户被屏蔽。
		/// </summary>
		Forbidden = 0x2
	}

	/// <summary>
	/// 表示用户的性别。
	/// </summary>
	public enum Gender
	{
		/// <summary>
		/// 女性。
		/// </summary>
		Female = 0,
		/// <summary>
		/// 男性。
		/// </summary>
		Male = 1
	}
}
