using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.Models;
using IdentityServer4.Core.Models;

namespace CC98.LogOn.ViewModels.Account
{
	/// <summary>
	/// 定义授权页面需要的视图数据。
	/// </summary>
	public class AuthorizeViewModel
	{
		/// <summary>
		/// 获取或设置客户端信息。
		/// </summary>
		public App AppInfo { get; set; }

		public IEnumerable<ScopeDisplayInfo> Scopes { get; set; }

		/// <summary>
		/// 获取或设置用户授权的领域。
		/// </summary>
		public string[] ConsentScopes { get; set; }

		/// <summary>
		/// 获取或设置授权的标识。
		/// </summary>
		public string RequestId { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示是否要记住授权。
		/// </summary>
		public bool RememberCensent { get; set; }

		/// <summary>
		/// 获取或设置用户的操作结果。
		/// </summary>
		public AuthorizeOperation Operation { get; set; }
	}
}
