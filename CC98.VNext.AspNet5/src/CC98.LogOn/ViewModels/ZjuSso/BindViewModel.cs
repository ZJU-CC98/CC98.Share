using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CC98.LogOn.ViewModels.ZjuSso
{
	/// <summary>
	/// 绑定操作的数据模型。
	/// </summary>
	public class BindViewModel
	{
		/// <summary>
		/// 获取或设置要绑定的 CC98 账户名。
		/// </summary>
		[Required]
		[DataType(DataType.Text)]
		[Display(Name = "CC98 账户名", ShortName = "账户名")]
		public string CC98UserName { get; set; }

		/// <summary>
		/// 获取或设置要绑定的 CC98 账户密码。
		/// </summary>
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "CC98 账户密码", ShortName = "账户密码")]
		public string CC98Password { get; set; }
	}
}
