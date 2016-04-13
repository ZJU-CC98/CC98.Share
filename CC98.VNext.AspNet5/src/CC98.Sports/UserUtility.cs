using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Claims;
using CC98.Identity;
using Microsoft.AspNet.Authorization;

namespace CC98.Sports
{
	/// <summary>
	/// 为用户身份提供相关辅助功能。该类型为静态类型。
	/// </summary>
	public static class UserUtility
	{
		#region 组名称

		/// <summary>
		/// 表示系统管理员的组名称。
		/// </summary>
		public const string Administrators = "Administrators";

		/// <summary>
		/// 表示赛事系统管理员的组名称。
		/// </summary>
		public const string SportAdministrators = "Sport Administrators";

		/// <summary>
		/// 表示赛事系统审核员的组名称。
		/// </summary>
		public const string SportReviewers = "Sport Reviewers";

		/// <summary>
		/// 表示赛事系统操作员的名称。
		/// </summary>
		public const string SportOperators = "Sport Operators";

		/// <summary>
		/// 表示赛事系统组织员的名称。
		/// </summary>
		public const string SportOrganizer = "Sport Organizers";

		#endregion

		#region 策略名称


		/// <summary>
		/// 系统管理员权限的策略名称。
		/// </summary>
		public const string SystemAdminPolicy = nameof(SystemAdminPolicy);

		/// <summary>
		/// 管理权限的策略名称。
		/// </summary>
		public const string AdminPolicy = "Admin";

		/// <summary>
		/// 审核权限的策略名称。
		/// </summary>
		public const string ReviewPolicy = "Review";

		/// <summary>
		/// 执行一般管理操作权限的策略名称。
		/// </summary>
		public const string OperatePolicy = "Operate";

		/// <summary>
		/// 执行赛事组织操作的策略名称。
		/// </summary>
		public const string OrganizePolicy = "Organize";

		#endregion

		#region 策略组

		/// <summary>
		/// 生成只读的权限组。
		/// </summary>
		/// <param name="roles">权限组列表。</param>
		/// <returns>权限组集合。</returns>
		private static IReadOnlyCollection<string> GenerateRoles(params string[] roles)
		{
			return new ReadOnlyCollection<string>(roles);
		}

		/// <summary>
		/// 生成只读的权限组。
		/// </summary>
		/// <param name="baseRoles">基础权限。</param>
		/// <param name="roles">权限组列表。</param>
		/// <returns>权限组集合。</returns>
		private static IReadOnlyCollection<string> GenerateRoles(IEnumerable<string> baseRoles, params string[] roles)
		{
			var list = new List<string>(baseRoles);
			list.AddRange(roles);

			return new ReadOnlyCollection<string>(list);
		}

		/// <summary>
		/// 系统管理权限对应的用户角色组。
		/// </summary>
		public static IReadOnlyCollection<string> SystemAdminRoles { get; } = GenerateRoles(Administrators);


		/// <summary>
		/// 管理权限对应的用户角色组。
		/// </summary>
		public static IReadOnlyCollection<string> AdminRoles { get; } = GenerateRoles(Administrators, SportAdministrators);

		/// <summary>
		/// 操作权限对应的用户角色组。
		/// </summary>
		public static IReadOnlyCollection<string> OperateRoles { get; } = GenerateRoles(AdminRoles, SportOperators);

		/// <summary>
		/// 审核权限对应的用户角色组。
		/// </summary>
		public static IReadOnlyCollection<string> ReviewRoles { get; } = GenerateRoles(OperateRoles, SportReviewers);

		/// <summary>
		/// 组织赛事对应的用户角色组。
		/// </summary>
		public static IReadOnlyCollection<string> OrganizeRoles { get; } = GenerateRoles(OperateRoles, SportOrganizer);

		#endregion

		/// <summary>
		/// 构建当前应用程序可用的授权策略。
		/// </summary>
		/// <param name="roles">授权策略对应的角色的集合。</param>
		/// <returns>授权策略对象。</returns>
		public static AuthorizationPolicy GeneratePolicy(string authenticationScheme, IEnumerable<string> roles)
		{
			var builder = new AuthorizationPolicyBuilder(authenticationScheme);
			builder.RequireRole(roles);

			return builder.Build();
		}

		/// <summary>
		/// 获取一个值，指示该用户是否是全局系统管理员。
		/// </summary>
		/// <param name="principal">用户主体对象。</param>
		/// <returns>如果当前用户具有全局管理权限，返回 true；否则返回 false。</returns>
		public static bool IsSystemAdmin(this ClaimsPrincipal principal)
		{
			return principal.IsInAnyRole(SystemAdminRoles);
		}

		/// <summary>
		/// 获取一个值，指示该用户是否对于注册球员系统具有管理权限。
		/// </summary>
		/// <param name="principal">用户主体对象。</param>
		/// <returns>如果当前用户具有管理权限，返回 true；否则返回 false。</returns>
		public static bool CanAdmin(this ClaimsPrincipal principal)
		{
			return principal.IsInAnyRole(AdminRoles);
		}

		/// <summary>
		/// 获取一个值，指示该用户是否对于赛事系统具有一般操作权限。
		/// </summary>
		/// <param name="principal">用户主体对象。</param>
		/// <returns>如果当前用户具有审核权限，返回 true；否则返回 false。</returns>
		public static bool CanOperate(this ClaimsPrincipal principal)
		{
			return principal.IsInAnyRole(OperateRoles);
		}

		/// <summary>
		/// 获取一个值，指示该用户是否对于赛事系统具有审核权限。
		/// </summary>
		/// <param name="principal">用户主体对象。</param>
		/// <returns>如果当前用户具有审核权限，返回 true；否则返回 false。</returns>
		public static bool CanReview(this ClaimsPrincipal principal)
		{
			return principal.IsInAnyRole(ReviewRoles);
		}

		/// <summary>
		/// 获取一个值，指示该用户是否对于赛事系统具有组织权限。
		/// </summary>
		/// <param name="principal">用户主体对象。</param>
		/// <returns>如果当前用户具有组织权限，返回 true；否则返回 false。</returns>
		public static bool CanOrganize(this ClaimsPrincipal principal)
		{
			return principal.IsInAnyRole(OrganizeRoles);
		}
	}
}
