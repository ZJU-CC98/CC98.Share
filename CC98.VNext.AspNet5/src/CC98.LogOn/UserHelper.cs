using System;
using System.Linq;
using System.Security.Claims;
using CC98.Identity;
using JetBrains.Annotations;
using Microsoft.AspNet.Identity;

namespace CC98.LogOn
{
	/// <summary>
	/// 为用户主体对象提供扩展方法。该类型为静态类型。
	/// </summary>
	public static class UserHelper
	{
		/// <summary>
		/// 获取一个值，指示给定的用户主体是否使用特定方法登录。
		/// </summary>
		/// <param name="principal">要检查的用户主体。</param>
		/// <param name="authenticationType">要检查的身份验证类型。</param>
		/// <returns>如果 <paramref name="principal"/> 使用了 <paramref name="authenticationType"/> 给定的方法登录，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="principal"/> 或 <paramref name="authenticationType"/> 为 <c>null</c>。</exception>
		[PublicAPI]
		public static bool IsSignedInWith([NotNull] this ClaimsPrincipal principal, [NotNull] string authenticationType)
		{
			if (principal == null)
			{
				throw new ArgumentNullException(nameof(principal));
			}

			if (authenticationType == null)
			{
				throw new ArgumentNullException(nameof(authenticationType));
			}

			return principal.Identities.Any(i => i.AuthenticationType == authenticationType);
		}

		/// <summary>
		/// 获取一个值，指示给定的用户主体是否具有 CC98 账户的验证信息。
		/// </summary>
		/// <param name="principal">要检查的用户主体。</param>
		/// <returns>如果 <paramref name="principal"/> 具有 CC98 账户信息，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="principal"/> 为 <c>null</c>。</exception>
		public static bool IsSignedInWithCC98([NotNull] this ClaimsPrincipal principal)
			=> principal.IsSignedInWith(IdentityCookieOptions.ApplicationCookieAuthenticationType);

		/// <summary>
		/// 获取一个值，指示给定的用户主体是否具有浙大通行证账户的验证信息。
		/// </summary>
		/// <param name="principal">要检查的用户主体。</param>
		/// <returns>如果 <paramref name="principal"/> 具有浙大通行证账户信息，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="principal"/> 为 <c>null</c>。</exception>
		public static bool IsSignedInWithZjuSso([NotNull] this ClaimsPrincipal principal)
			=> principal.IsSignedInWith(ZjuSsoService.AuthenticationType);
	}
}