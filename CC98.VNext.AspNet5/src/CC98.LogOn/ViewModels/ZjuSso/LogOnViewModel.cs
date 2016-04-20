using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CC98.LogOn.ViewModels.ZjuSso
{
	/// <summary>
	/// 表示浙大通行证的登录信息。
	/// </summary>
	public class LogOnViewModel
	{
		/// <summary>
		/// 获取或设置登录使用的用户名。
		/// </summary>
		[Required]
		[DataType(DataType.Text)]
		public string UserName { get; set; }

		/// <summary>
		/// 获取或设置登录使用的密码。
		/// </summary>
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
