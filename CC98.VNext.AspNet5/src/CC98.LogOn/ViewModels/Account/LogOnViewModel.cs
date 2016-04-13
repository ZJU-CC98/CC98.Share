using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CC98.LogOn.ViewModels.Account
{
	/// <summary>
	/// Account/LogOn 方法的数据模型。
	/// </summary>
	public class LogOnViewModel
	{
		/// <summary>
		/// 获取或设置登录时的用户名。
		/// </summary>
		[Required]
		[DataType(DataType.Text)]
		public string UserName { get; set; }

		/// <summary>
		/// 获取或设置登录时的密码。
		/// </summary>
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		/// <summary>
		/// 获取或设置登录的有效期。
		/// </summary>
		public TimeSpan? ValidTime { get; set; }
	}
}
