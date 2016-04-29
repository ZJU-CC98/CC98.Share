using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.LogOn.Models
{
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
		[DataType(DataType.Url)]
		public string RedirectUri { get; set; }

		/// <summary>
		/// 获取该应用的审核状态。
		/// </summary>
		public AppAuditState AuditState { get; set; }

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

		/// <summary>
		/// 获取或设置访问令牌的有效期，以秒为单位。
		/// </summary>
		public int AccessTokenLifetime { get; set; }

		/// <summary>
		/// 获取或设置刷新令牌的有效期，以秒为单位。
		/// </summary>
		public int RefreshTokenLifetime { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示该应用是否已经被启用。
		/// </summary>
		public bool IsEnabled { get; set; }
	}
}