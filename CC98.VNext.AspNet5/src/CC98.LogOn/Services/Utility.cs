using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CC98.LogOn.Services
{
	/// <summary>
	/// 为操作提供辅助方法。该类型为静态类型。
	/// </summary>
	internal static class Utility
    {
		/// <summary>
		/// 尝试获取 <see cref="HttpHeaders"/> 中具有指定键名称的第一个值。
		/// </summary>
		/// <param name="headers">表示 HTTP 标头信息集合的 <see cref="HttpHeaders"/> 对象。</param>
		/// <param name="key">要查找的内容的键。</param>
		/// <returns>如果 <paramref name="key"/> 对应的项目存在， 则返回其第一个值；否则返回 <c>null</c>。</returns>
		public static string GetValue(this HttpHeaders headers, string key)
		{
			IEnumerable<string> values;

			if (headers.TryGetValues(key, out values))
			{
				return values.FirstOrDefault();
			}

			return null;
		}

		/// <summary>
		/// 如果字符串是 <see cref="string.Empty"/>，则使用 <c>null</c> 进行替换。
		/// </summary>
		/// <param name="value">要替换的字符串。</param>
		/// <returns>替换后的结果。</returns>
		public static string ReplaceEmptyWithNull(this string value)
		{
			return string.IsNullOrEmpty(value) ? null : value;
		}
	}
}
