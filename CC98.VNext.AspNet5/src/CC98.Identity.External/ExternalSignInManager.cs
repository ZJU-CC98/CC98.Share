using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.OptionsModel;

namespace CC98.Identity.External
{
	/// <summary>
	/// 提供使用联合账户进行外部登录的相关功能。
	/// </summary>
	public class ExternalSignInManager
	{
		/// <summary>
		/// 获取验证管理器服务。
		/// </summary>
		private AuthenticationManager AuthenticationManager { get; }

		/// <summary>
		/// 获取身份验证选项。
		/// </summary>
		private IdentityOptions IdentityOptions { get; }

		/// <summary>
		/// 初始化一个对象的新实例。
		/// </summary>
		/// <param name="httpContextAccessor">HTTP 上下文访问器对象。</param>
		/// <param name="identityOptions">标识配置选项。</param>
		public ExternalSignInManager(IHttpContextAccessor httpContextAccessor, IOptions<IdentityOptions> identityOptions)
		{
			AuthenticationManager = httpContextAccessor.HttpContext.Authentication;
			IdentityOptions = identityOptions.Value;
		}

		/// <summary>
		/// 如果用户当前通过外部登录凭据进行了登录。则将登录凭据添加到当前应用。
		/// </summary>
		/// <returns>如果登录成功，返回成功登录的 <see cref="ClaimsPrincipal"/> 对象。如果登录失败，返回 <c>null</c>。</returns>
		public async Task<ClaimsPrincipal> SignInFromExternalCookieAsync()
		{
			var externalLoginInfo =
				await AuthenticationManager.AuthenticateAsync(IdentityOptions.Cookies.ExternalCookieAuthenticationScheme);

			if (externalLoginInfo == null)
			{
				return null;
			}

			await AuthenticationManager.SignInAsync(IdentityOptions.Cookies.ApplicationCookieAuthenticationScheme, externalLoginInfo);
			return externalLoginInfo;

		}

		/// <summary>
		/// 注销当前用户登录的用户主体。
		/// </summary>
		/// <returns>表示异步操作的任务。</returns>
		public async Task SignOutAsync()
		{
			await AuthenticationManager.SignOutAsync(IdentityOptions.Cookies.ApplicationCookieAuthenticationScheme);
			await AuthenticationManager.SignOutAsync(IdentityOptions.Cookies.ExternalCookieAuthenticationScheme);
		}
	}
}
