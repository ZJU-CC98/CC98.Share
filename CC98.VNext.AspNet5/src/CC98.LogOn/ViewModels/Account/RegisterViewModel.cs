using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.Models;

namespace CC98.LogOn.ViewModels.Account
{
	/// <summary>
	/// 用户注册的视图模型。
	/// </summary>
	public class RegisterViewModel
	{
		/// <summary>
		/// 获取或设置注册的用户名。
		/// </summary>
		[Required]
		[StringLength(10)]
		[DataType(DataType.Text)]
		[Display(Name = "用户名")]
		public string UserName { get; set; }

		/// <summary>
		/// 获取或设置注册的密码。
		/// </summary>
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "密码")]
		public string Password { get; set; }

		/// <summary>
		/// 获取或设置注册的密码。
		/// </summary>
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "确认密码")]
		[Compare(nameof(Password))]
		public string ConfirmPassword { get; set; }

		/// <summary>
		/// 获取或设置个人主页地址。
		/// </summary>
		[DataType(DataType.Url)]
		[Display(Name = "个人主页")]
		public string HomePage { get; set; }

		/// <summary>
		/// 获取或设置电子邮件地址。
		/// </summary>
		[DataType(DataType.EmailAddress)]
		[Display(Name = "电子邮件")]
		public string Email { get; set; }

		/// <summary>
		/// 获取或设置生日。
		/// </summary>
		[Display(Name = "生日")]
		public DateTime Birthday { get; set; }

		/// <summary>
		/// 获取或设置性别。
		/// </summary>
		[Display(Name = "性别")]
		public Gender Gender { get; set; }
	}
}
