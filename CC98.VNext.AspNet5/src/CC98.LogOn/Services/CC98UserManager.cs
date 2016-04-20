using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.Models;
using Microsoft.Data.Entity;

namespace CC98.LogOn.Services
{
	/// <summary>
	/// 对 CC98 用户账户信息提供支持。
	/// </summary>
	public class CC98UserManager
	{
		/// <summary>
		/// 获取 CC98 V2 数据库连接。
		/// </summary>
		private CC98V2DatabaseModel V2Model { get; }

		/// <summary>
		/// 获取 CC98 V1 数据库连接。
		/// </summary>
		private CC98V1DatabaseModel V1Model { get; }

		private PasswordEncryptionService PasswordEncryptionService { get; }

		public CC98UserManager(CC98V1DatabaseModel v1Model, CC98V2DatabaseModel v2Model, PasswordEncryptionService passwordEncryptionService)
		{
			V1Model = v1Model;
			V2Model = v2Model;

			PasswordEncryptionService = passwordEncryptionService;
		}

		/// <summary>
		/// 尝试从数据库中获取具有指定用户名的
		/// </summary>
		/// <param name="userName">要搜索的用户名。</param>
		/// <returns>如果找到了 <paramref name="userName"/> 对应的用户，返回包含该用户的 <see cref="IdentityUser"/> 对象。否则返回 <c>null</c>。</returns>
		public Task<IdentityUser> FindUserByName(string userName)
		{
			if (userName == null)
			{
				throw new ArgumentNullException(nameof(userName));
			}

			return (from i in V2Model.Users
					where i.UserName == userName
					select i).SingleOrDefaultAsync();
		}

		public Task<CC98V1User> LogOnAsync(string userName, string password)
		{
			if (userName == null)
			{
				throw new ArgumentNullException(nameof(userName));
			}
			if (password == null)
			{
				throw new ArgumentNullException(nameof(password));
			}

			var passwordHash = PasswordEncryptionService.EncryptV1Password(password);

			return (from i in V1Model.Users
					where i.Name == userName && i.PasswordHash == passwordHash
					select i).SingleOrDefaultAsync();
		}
	}
}
