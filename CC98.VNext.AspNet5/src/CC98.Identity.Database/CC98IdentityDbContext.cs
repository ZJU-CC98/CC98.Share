using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity.EntityFramework;

namespace CC98.Identity
{
	/// <summary>
	/// 表示 CC98 用户数据库上下文对象。
	/// </summary>
	public class CC98IdentityDbContext : IdentityDbContext<CC98IdentityUser, CC98IdentityRole, int>
	{
	}

	/// <summary>
	/// 表示 CC98 数据库用户。
	/// </summary>
	public class CC98IdentityUser : IdentityUser<int>
	{
	}

	/// <summary>
	/// 表示 CC98 用户角色。
	/// </summary>
	public class CC98IdentityRole : IdentityRole<int>
	{
		/// <summary>
		/// 获取用于在 <see cref="AdminRoles"/> 中分割多个项目的分隔符。该字段为常量。
		/// </summary>
		[NotMapped]
		public const string AdminRolesSeperator = ",";

		/// <summary>
		/// 获取或设置能够管理该角色的角色列表的原始字符串。
		/// </summary>
		public string AdminRolesString { get; set; }

		/// <summary>
		/// 获取或设置能够管理该角色的角色列表。
		/// </summary>
		[NotMapped]
		public string[] AdminRoles
		{
			get
			{
				return (AdminRolesString ?? string.Empty).Split(new[] { AdminRolesSeperator }, StringSplitOptions.RemoveEmptyEntries);
			}
			set
			{
				if (value == null)
				{
					AdminRolesString = null;
				}
				else
				{
					AdminRolesString = string.Join(AdminRolesSeperator, value);
				}
			}

		}
	}
}
