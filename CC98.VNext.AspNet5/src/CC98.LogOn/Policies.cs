using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;

namespace CC98.LogOn
{
	/// <summary>
	/// 为应用程序策略提供辅助方法和字段。该类型为静态类型。
	/// </summary>
	internal static class Policies
	{
		/// <summary>
		/// 表示需要浙大通行证登录的策略名称。该字段为常量。
		/// </summary>
		public const string RequireZjuSso = nameof(RequireZjuSso);

		/// <summary>
		/// 表示需要 CC98 账号登录的策略名称。该字段为常量。
		/// </summary>
		public const string RequireCC98 = nameof(RequireCC98);

		/// <summary>
		/// 获取一个策略，该策略要求当前用户主体使用给定的验证方案进行验证。
		/// </summary>
		/// <param name="authenticationScheme">要验证的验证方案。</param>
		/// <returns>表示授权策略的 <see cref="AuthorizationPolicy"/> 对象。</returns>
		public static AuthorizationPolicy GetPolicyForAuthenticationScheme(string authenticationScheme)
		{
			var builder = new AuthorizationPolicyBuilder(authenticationScheme);
			builder.RequireAuthenticatedUser();
			return builder.Build();
		}
	}
}
