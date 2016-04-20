using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Data.Entity;

namespace CC98.LogOn.Models
{
	/// <summary>
	/// CC98 V2 数据库模型。
	/// </summary>
	public class CC98V2DatabaseModel : DbContext
	{
		public CC98V2DatabaseModel()
		{
		}

		//为您要在模型中包含的每种实体类型都添加 DbSet。有关配置和使用 Code First  模型
		//的详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=390109。

		// public virtual DbSet<MyEntity> MyEntities { get; set; }

		/// <summary>
		/// 数据库中包含的所有应用的集合。
		/// </summary>
		public virtual DbSet<App> Apps { get; set; }

		/// <summary>
		/// 用户表。
		/// </summary>
		/// <returns></returns>
		public virtual DbSet<IdentityUser> Users { get; set; }

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
			modelBuilder.Entity<App>().HasAlternateKey(i => i.Name);
		}
	}

	/// <summary>
	/// 表示一个应用
	/// </summary>
	[Table("Apps")]
	public class App
	{
		/// <summary>
		/// 项目的 ID。
		/// </summary>
		/// <returns></returns>
		[Required]
		[Key]
		public Guid Id { get; set; }

		/// <summary>
		/// 项目的 Secret。
		/// </summary>
		/// <returns></returns>
		[Required]
		public Guid Secret { get; set; }

		/// <summary>
		/// 应用的名称。
		/// </summary>
		/// <returns></returns>
		[Required]
		[Display(Name = "应用名称", ShortName = "名称")]
		public string Name { get; set; }

		/// <summary>
		/// 应用的描述。
		/// </summary>
		/// <returns></returns>
		public string Description { get; set; }


		/// <summary>
		/// 应用的回调 URL。
		/// </summary>
		/// <returns></returns>
		[DataType(DataType.Url)]
		public string RedirectUri { get; set; }

		/// <summary>
		/// 应用的状态。
		/// </summary>
		/// <returns></returns>
		[Required]
		public AppState State { get; set; }

		/// <summary>
		/// 项目的主页 URL。
		/// </summary>
		/// <returns></returns>
		[DataType(DataType.Url)]
		public string HomePageUri { get; set; }

		/// <summary>
		/// 项目的 LOGO URL。
		/// </summary>
		/// <returns></returns>
		[DataType(DataType.ImageUrl)]
		public string LogoUri { get; set; }

		/// <summary>
		/// 指示是否是受信任客户端。
		/// </summary>
		/// <returns></returns>
		public bool IsTrusted { get; set; }

		/// <summary>
		/// 是否是内置核心客户端。
		/// </summary>
		/// <returns></returns>
		public bool IsBuiltIn { get; set; }

		/// <summary>
		/// 创建时间。
		/// </summary>
		/// <returns></returns>
		public DateTime CreateTime { get; set; }

		/// <summary>
		/// 过期时间。
		/// </summary>
		/// <returns></returns>
		public DateTime? ExpireTime { get; set; }

		/// <summary>
		/// 应用相关的选项。
		/// </summary>
		/// <returns></returns>
		//public AppOptions Options { get; set; } = new AppOptions();

		/// <summary>
		/// 管理人员的电子邮件地址。
		/// </summary>
		/// <returns></returns>
		[EmailAddress]
		[DataType(DataType.EmailAddress)]
		public string ManageEmailAddress { get; set; }

		/// <summary>
		/// 是否要显示在应用列表上。
		/// </summary>
		/// <returns></returns>
		public bool ShowInAppList { get; set; }

		/// <summary>
		/// 应用的所有者的标识。
		/// </summary>
		/// <returns></returns>
		public int? OwnerId { get; set; }

		/// <summary>
		/// 该应用的所有者。
		/// </summary>
		/// <returns></returns>
		[ForeignKey("OwnerId")]
		public IdentityUser Owner { get; set; }
	}

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


	/// <summary>
	/// 表示应用的选项。
	/// </summary>
	[ComplexType]
	public class AppOptions
	{
		/// <summary>
		/// 是否启用授权码验证。
		/// </summary>
		/// <returns></returns>
		public bool EnableAuthorizationCode { get; set; }

		/// <summary>
		/// 是否启用隐式授权验证。
		/// </summary>
		/// <returns></returns>
		public bool EnableImplicitGrant { get; set; }

		/// <summary>
		/// 是否启用用户名密码验证。
		/// </summary>
		/// <returns></returns>
		public bool EnableResourceOwnerPassword { get; set; }
	}

	/// <summary>
	/// 表示应用的状态。
	/// </summary>
	public enum AppState
	{
		/// <summary>
		/// 正常状态。
		/// </summary>
		Normal = 0,
		/// <summary>
		/// 未激活。
		/// </summary>
		Inactive,
		/// <summary>
		/// 已过期。
		/// </summary>
		Expired,
		/// <summary>
		/// 已被吊销。
		/// </summary>
		Revoked
	}
}