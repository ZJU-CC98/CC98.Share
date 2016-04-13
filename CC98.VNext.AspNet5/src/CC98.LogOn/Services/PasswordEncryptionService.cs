using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CC98.LogOn.Services
{
	/// <summary>
	/// 提供 CC98 密码的加密服务。
	/// </summary>
	public class PasswordEncryptionService
	{
		/// <summary>
		/// 获取 SHA1 处理程序。
		/// </summary>
		private SHA1 SHA1Handler { get; } = SHA1.Create();

		/// <summary>
		/// 获取 MD5 处理程序。
		/// </summary>
		private MD5 MD5Handler { get; } = MD5.Create();

		/// <summary>
		/// 将密码字符串转换为 CC98 V1 的数据库密码。
		/// </summary>
		/// <param name="password">原始密码字符串。</param>
		/// <returns>转换后的 V1 数据库密码。</returns>
		public string EncryptV1Password(string password)
		{
			if (password == null)
			{
				throw new ArgumentNullException(nameof(password));
			}

			var passwordBytes = Encoding.UTF8.GetBytes(password);
			var md5Result = MD5Handler.ComputeHash(passwordBytes);

			var result = new StringBuilder();
			for (var i = 4; i < 12; i++)
			{
				result.AppendFormat(CultureInfo.InvariantCulture, "{0:x2}", md5Result[i]);
			}

			return result.ToString();
		}
	}
}
