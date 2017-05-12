using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CC98.Share
{
	/// <summary>
	/// 定义系统策略对象。
	/// </summary>
	public static class Policies
	{
		/// <summary>
		/// 表示管理策略的名称。该字段为常量。
		/// </summary>
		public const string Administrate = nameof(Administrate);

		/// <summary>
		/// 表示操作策略的名称。该字段为常量。
		/// </summary>
		public const string Operate = nameof(Operate);

		/// <summary>
		/// 定义系统角色对象。
		/// </summary>
		public static class Roles
		{
			/// <summary>
			/// 表示全局管理员角色。该字段为常量。
			/// </summary>
			public const string GeneralAdministrators = "Administrators";

			/// <summary>
			/// 表示网盘管理员角色。该字段为常量。
			/// </summary>
			public const string Administrators = "Share Administrators";

			/// <summary>
			/// 表示网盘操作员角色。该字段为常量。
			/// </summary>
			public const string Operators = "Share Operators";
		}
	}
}
