using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CC98.Medal
{
	/// <summary>
	/// 为应用程序策略提供辅助信息。该类型为静态类型。
	/// </summary>
	public static class Policies
	{
		/// <summary>
		/// 系统管理权限。
		/// </summary>
		public const string Administrate = nameof(Administrate);

		/// <summary>
		/// 通用操作权限。
		/// </summary>
		public const string Operate = nameof(Operate);

		/// <summary>
		/// 勋章编辑权限。
		/// </summary>
		public const string Edit = nameof(Edit);
	}

	/// <summary>
	/// 定义角色组名称。该类型为静态类型。
	/// </summary>
	public static class Roles
	{
		/// <summary>
		/// 通用管理员组。
		/// </summary>
		public const string Adminisrators = "Administrators";

		/// <summary>
		/// 勋章管理员组。
		/// </summary>
		public const string MedalAdministrators = "Medal Administrators";

		/// <summary>
		/// 勋章操作员组。
		/// </summary>
		public const string MedalOperators = "Medal Operators";

		/// <summary>
		/// 勋章操作员组。
		/// </summary>
		public const string MedalEditor = "Medal Editors";
	}
}
