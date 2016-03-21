using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CC98.Identity
{
	/// <summary>
	/// 为 <see cref="CC98User"/> 对象提供辅助方法。该类型为静态类型。
	/// </summary>
	public static class CC98UserHelper
	{
		/// <summary>
		/// 将 <see cref="CC98User"/> 对象转换为 <see cref="ClaimsIdentity"/> 对象。
		/// </summary>
		/// <param name="userInfo">要转换的 <see cref="CC98User"/> 对象。</param>
		/// <param name="authenticationType"><see cref="ClaimsIdentity"/> 的授权类型字符串。</param>
		/// <returns>转换后的 <see cref="ClaimsIdentity"/> 对象。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="userInfo"/> 或 <paramref name="authenticationType"/> 为 <c>null</c>。</exception>
		/// <seealso cref="ToPrincipal"/>
		public static ClaimsIdentity ToIdentity(this CC98User userInfo, string authenticationType)
		{
			// 参数检查
			if (userInfo == null)
			{
				throw new ArgumentNullException(nameof(userInfo));
			}

			if (authenticationType == null)
			{
				throw new ArgumentNullException(nameof(authenticationType));
			}

			// 标准声明类型：用户名，用户 ID 和老版本 ID
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, userInfo.Name, ClaimValueTypes.String),
				new Claim(ClaimTypes.NameIdentifier, userInfo.V2Id.ToString("D", CultureInfo.InvariantCulture),
					ClaimValueTypes.Integer),
				new Claim(CC98UserClaimTypes.OldId, userInfo.V1Id.ToString("D", CultureInfo.InvariantCulture),
					ClaimValueTypes.Integer)
			};

			// 角色
			foreach (var role in userInfo.Roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role, ClaimValueTypes.String));
			}

			// 返回结果
			return new ClaimsIdentity(claims, authenticationType, ClaimTypes.Name, ClaimTypes.Role);
		}


		/// <summary>
		/// 将 <see cref="CC98User"/> 对象转换为 <see cref="ClaimsPrincipal"/> 对象。
		/// </summary>
		/// <param name="userInfo">要转换的 <see cref="CC98User"/> 对象。</param>
		/// <param name="authenticationType">创建 <see cref="ClaimsIdentity"/> 标识对象时使用的授权类型字符串。</param>
		/// <returns>转换后的 <see cref="ClaimsPrincipal"/> 对象。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="userInfo"/> 或 <paramref name="authenticationType"/> 为 <c>null</c>。</exception>
		/// <seealso cref="ToIdentity"/>
		public static ClaimsPrincipal ToPrincipal(this CC98User userInfo, string authenticationType)
			=> new ClaimsPrincipal(new[] { userInfo.ToIdentity(authenticationType) });

		/// <summary>
		/// 将 <see cref="ClaimsIdentity"/> 对象转换为 <see cref="CC98User"/> 对象。
		/// </summary>
		/// <param name="identity">要转换的 <see cref="ClaimsIdentity"/> 对象。</param>
		/// <returns>转换后的 <see cref="CC98User"/> 对象。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="identity"/> 为 <c>null</c>。</exception>
		public static CC98User ToCC98User(this ClaimsIdentity identity)
		{
			// 参数检查
			if (identity == null)
			{
				throw new ArgumentNullException(nameof(identity));
			}

			return new CC98User
			{
				Name = identity.Name,
				V1Id = identity.GetOldId(),
				V2Id = identity.GetId(),
				Roles = identity.GetRoles().ToArray()
			};
		}

		/// <summary>
		/// 将 <see cref="ClaimsPrincipal"/> 对象转换为 <see cref="CC98User"/> 对象。
		/// </summary>
		/// <param name="principal">要转换的 <see cref="ClaimsPrincipal"/> 对象。</param>
		/// <returns>转换后的 <see cref="CC98User"/> 对象。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="principal"/> 为 <c>null</c>。</exception>
		public static CC98User ToCC98User(this ClaimsPrincipal principal)
		{
			if (principal == null)
			{
				throw new ArgumentNullException(nameof(principal));
			}

			return new ClaimsIdentity(principal.Identity).ToCC98User();
		}
	}
}
