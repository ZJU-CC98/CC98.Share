using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace CC98.Identity
{
	/// <summary>
	/// 为标识提供辅助方法。该类型为静态类型。
	/// </summary>
	public static class IdentityHelper
	{
		/// <summary>
		/// 表示身份提供程序的声明类型。该字段为常量。
		/// </summary>
		public const string IdentityProviderClaimType = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider";

		/// <summary>
		/// 创建表示标识的身份提供程序的声明。
		/// </summary>
		/// <param name="identityProviderValue">身份提供程序的值。</param>
		/// <returns>表示身份提供程序的声明。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="identityProviderValue"/> 为 <c>null</c>。</exception>
		public static Claim CreateIdentityProviderClaim(string identityProviderValue)
		{
			if (identityProviderValue == null)
			{
				throw new ArgumentNullException(nameof(identityProviderValue));
			}

			return new Claim(IdentityProviderClaimType, identityProviderValue, ClaimValueTypes.String);
		}

		/// <summary>
		/// 为指定的标识凭据添加身份提供程序声明。
		/// </summary>
		/// <param name="identity">要添加声明的标识。</param>
		/// <param name="value">身份提供程序声明的值。</param>
		/// <exception cref="ArgumentNullException"><paramref name="identity"/> 或 <paramref name="value"/> 为 <c>null</c>。</exception>
		public static void AddIdentityProvider(this ClaimsIdentity identity, string value)
		{
			if (identity == null)
			{
				throw new ArgumentNullException(nameof(identity));
			}

			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			identity.AddClaim(new Claim(IdentityProviderClaimType, value, ClaimValueTypes.String));
		}

		/// <summary>
		/// 从指定的标识创建新标识，并将其主要验证类型更换为指定类型。
		/// </summary>
		/// <param name="identity">要复制的标识。</param>
		/// <param name="authenticationType">新标识的验证类型。</param>
		/// <returns>复制的新标识。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="identity"/> 或 <paramref name="authenticationType"/> 为 null。</exception>
		public static ClaimsIdentity CloneAs(this ClaimsIdentity identity, string authenticationType)
		{
			if (identity == null)
			{
				throw new ArgumentNullException(nameof(identity));
			}
			if (authenticationType == null)
			{
				throw new ArgumentNullException(nameof(authenticationType));
			}

			return new ClaimsIdentity(identity.Claims, authenticationType, identity.NameClaimType, identity.RoleClaimType);
		}

		/// <summary>
		/// 从指定的主体创建新主体，并将其主要所有标识的验证类型更换为指定类型。
		/// </summary>
		/// <param name="principal">要复制的主体。</param>
		/// <param name="authenticationType">新主体中所有标识的验证类型。</param>
		/// <returns>复制的新主体。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="principal"/> 或 <paramref name="authenticationType"/> 为 null。</exception>
		public static ClaimsPrincipal CloneAs(this ClaimsPrincipal principal, string authenticationType)
		{
			if (principal == null)
			{
				throw new ArgumentNullException(nameof(principal));
			}
			if (authenticationType == null)
			{
				throw new ArgumentNullException(nameof(authenticationType));
			}

			var newIdentities = principal.Identities.Select(i => i.CloneAs(authenticationType));

			return new ClaimsPrincipal(newIdentities);
		}

		/// <summary>
		/// 获取给定的用户标识包含的所有角色的列表。
		/// </summary>
		/// <param name="identity">要检查的用户标识。</param>
		/// <returns><paramref name="identity"/> 所具有的所有角色。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="identity"/> 为 <c>null</c>。</exception>
		public static IReadOnlyList<string> GetRoles(this ClaimsIdentity identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException(nameof(identity));
			}

			var result = from i in identity.FindAll(ClaimTypes.Role)
						 select i.Value;

			return new ReadOnlyCollection<string>(result.ToArray());
		}

		/// <summary>
		/// 获取给定的用户主体包含的所有角色的列表。
		/// </summary>
		/// <param name="principal">要检查的用户主体。</param>
		/// <returns><paramref name="principal"/> 所具有的所有角色。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="principal"/> 为 <c>null</c>。</exception>
		public static IReadOnlyList<string> GetRoles(this ClaimsPrincipal principal)
		{
			if (principal == null)
			{
				throw new ArgumentNullException(nameof(principal));
			}

			var result = from i in principal.FindAll(ClaimTypes.Role)
						 select i.Value;

			return new ReadOnlyCollection<string>(result.ToArray());
		}

		/// <summary>
		/// 获取一个值，指示当前用户主体是否属于给定用户组中的任何一个。
		/// </summary>
		/// <param name="principal">要判断的用户主体。</param>
		/// <param name="roles">给定的一系列用户组。</param>
		/// <returns>如果 <see cref="principal"/> 属于 <paramref name="roles"/> 中所定义的任意一个用户组，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="principal"/> 或 <paramref name="roles"/> 为 null。</exception>
		public static bool IsInAnyRole(this IPrincipal principal, IEnumerable<string> roles)
		{
			if (principal == null)
			{
				throw new ArgumentNullException(nameof(principal));
			}

			if (roles == null)
			{
				throw new ArgumentNullException(nameof(roles));
			}

			return roles.Any(principal.IsInRole);
		}

		/// <summary>
		/// 获取一个值，指示当前用户主体是否属于给定用户组中的任何一个。
		/// </summary>
		/// <param name="principal">要判断的用户主体。</param>
		/// <param name="roles">给定的一系列用户组。</param>
		/// <returns>如果 <see cref="principal"/> 属于 <paramref name="roles"/> 中所定义的任意一个用户组，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="principal"/> 或 <paramref name="roles"/> 为 null。</exception>
		public static bool IsInAnyRole(this IPrincipal principal, params string[] roles)
		{
			return principal.IsInAnyRole((IEnumerable<string>)roles);
		}

		/// <summary>
		/// 生成只读的权限列表。
		/// </summary>
		/// <param name="roles">角色列表。</param>
		/// <returns><paramref name="roles"/> 对应的只读权限列表。</returns>
		public static IReadOnlyCollection<string> GenerateRoleList(params string[] roles)
			=> GenerateRoleList((IEnumerable<string>)roles);

		/// <summary>
		/// 生成只读的权限列表。
		/// </summary>
		/// <param name="roles">角色列表。</param>
		/// <returns><paramref name="roles"/> 对应的只读权限列表。</returns>
		public static IReadOnlyCollection<string> GenerateRoleList(IEnumerable<string> roles)
		{
			if (roles == null)
			{
				throw new ArgumentNullException(nameof(roles));
			}

			return new ReadOnlyCollection<string>(roles.ToArray());
		}

		/// <summary>
		/// 生成只读的权限列表。
		/// </summary>
		/// <param name="baseRoles">基础权限列表。</param>
		/// <param name="roles">追加权限列表。</param>
		/// <returns>合并后的权限列表。</returns>
		public static IReadOnlyCollection<string> GenerateRoleList(IEnumerable<string> baseRoles, IEnumerable<string> roles)
		{
			if (baseRoles == null)
			{
				throw new ArgumentNullException(nameof(baseRoles));
			}

			if (roles == null)
			{
				throw new ArgumentNullException(nameof(roles));
			}

			var items = new List<string>();
			items.AddRange(baseRoles);
			items.AddRange(roles);

			return new ReadOnlyCollection<string>(items);
		}

		/// <summary>
		/// 生成只读的权限列表。
		/// </summary>
		/// <param name="baseRoles">基础权限列表。</param>
		/// <param name="roles">追加权限列表。</param>
		/// <returns>合并后的权限列表。</returns>
		public static IReadOnlyCollection<string> GenerateRoleList(IEnumerable<string> baseRoles, params string[] roles)
			=> GenerateRoleList(baseRoles, (IEnumerable<string>)roles);

	}
}
