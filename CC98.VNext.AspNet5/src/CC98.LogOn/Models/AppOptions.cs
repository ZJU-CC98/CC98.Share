using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.LogOn.Models
{
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
}