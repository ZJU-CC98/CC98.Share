using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace CC98.LogOn.ViewModels.ZjuSso
{
	/// <summary>
	/// 表示重置密码页面的数据模型。
	/// </summary>
	[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.Members)]
	public class ResetPasswordViewModel
	{
		/// <summary>
		/// 要重置的 CC98 用户名。
		/// </summary>
		[Required]
		[Display(Name = "用户名")]
		public string CC98UserName { get; set; }

		/// <summary>
		/// 要重置的 CC98 新密码。
		/// </summary>
		[Required]
		[Display(Name = "新密码")]
		public string CC98NewPassword { get; set; }

		/// <summary>
		/// 要重置的 CC98 确认密码。
		/// </summary>
		[Required]
		[Display(Name = "确认密码")]
		[Compare(nameof(CC98NewPassword))]
		[UsedImplicitly(ImplicitUseKindFlags.Access)]
		public string CC98ConfirmPassword { get; set; }
	}
}
