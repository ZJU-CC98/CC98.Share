using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;
using Sakura.AspNet.Mvc.TagHelpers;

namespace CC98.Sports
{
	/// <summary>
	/// 提供辅助方法。该类型为静态类型。
	/// </summary>
	public static class Utility
	{
		/// <summary>
		/// 获取 <see cref="IFormFile"/> 信息中包含的文件名。
		/// </summary>
		/// <param name="formFile">要分析的 <see cref="IFormFile"/> 对象。</param>
		/// <returns>从 <paramref name="formFile"/> 中提取的文件名字符串。如果文件名信息不存在，则该方法返回 <c>null</c>。</returns>
		/// <remarks>
		/// 由于浏览器和系统安全设置的不同，获取的文件名字符串可能包含完整路径，也可能只包含文件名。
		/// </remarks>
		public static string GetFileName(this IFormFile formFile)
		{
			if (formFile == null)
			{
				throw new ArgumentNullException(nameof(formFile));
			}

			var match = Regex.Match(formFile.ContentDisposition, @"filename=""(.*?)""", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

			return match.Success ? match.Groups[1].Value : null;
		}

		/// <summary>
		/// 获取给定枚举项目的文字。
		/// </summary>
		/// <typeparam name="T">枚举对象的类型。</typeparam>
		/// <param name="item">枚举项目对象。</param>
		/// <returns>该项目上定义的文字。</returns>
		public static string GetDisplayName<T>(this T item)
		{
			if (item.GetType().GetTypeInfo().BaseType != typeof(Enum))
			{
				throw new ArgumentException("该方法只能用于枚举类型。", nameof(item));
			}

			var valueName = Enum.GetName(item.GetType(), item);
			var member = item.GetType().GetTypeInfo().GetDeclaredField(valueName);
			return member.GetTextForMember(EnumOptionTextSource.Name);
		}

		/// <summary>
		/// 将 <see cref="bool"/> 类型转换成适用于 JavaScript 语言的等效字符串。
		/// </summary>
		/// <param name="value">要转换的值。</param>
		/// <returns>转换后的字符串。</returns>
		public static string ToJavaScriptString(this bool value)
		{
			return value ? "true" : "false";
		}

		/// <summary>
		/// 尝试获取字典中的具有给定键的值。如果值不存在，则返回默认值。
		/// </summary>
		/// <typeparam name="TKey">字典的键的类型。</typeparam>
		/// <typeparam name="TValue">要获取的值的类型。</typeparam>
		/// <param name="dictionary">保存数据的字典。</param>
		/// <param name="key">要获取的值对应的键。</param>
		/// <param name="defaultValue">当键不存在时返回的默认值。</param>
		/// <returns>如果字典中存在对应的值，则返回该值；否则，返回 <paramref name="defaultValue"/> 的值。</returns>
		public static TValue GetValueOfDefault<TKey, TValue>(this IDictionary<TKey, object> dictionary, TKey key, TValue defaultValue)
		{
			object result;
			return dictionary.TryGetValue(key, out result) ? (TValue)result : defaultValue;
		}

		/// <summary>
		/// 获取集合中具有指定类型的第一个对象的索引。
		/// </summary>
		/// <typeparam name="T">要检索的对象的类型。</typeparam>
		/// <param name="collection">要检索的集合。</param>
		/// <returns>如果 <paramref name="collection"/> 中包含给定类型的对象，返回第一个对象从零开始的索引；否则返回 -1。</returns>
		public static int IndexOf<T>(this IEnumerable collection)
		{
			var index = 0;
			foreach (var item in collection)
			{
				if (item is T)
				{
					return index;
				}

				index++;
			}

			return -1;
		}

		/// <summary>
		/// 将字符串以换行符号分割成多个项目。
		/// </summary>
		/// <param name="value">要分割的字符串。</param>
		/// <returns>分割后包含的所有子字符串。如果字符串对象为 <c>null</c> 或 <see cref="string.Empty"/>，则返回长度为零的数组。</returns>
		public static string[] SplitWithNewLine(this string value)
		{
			// NULL 替换
			value = value ?? string.Empty;
			return value.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		}

		/// <summary>
		/// 从数据库中分离给定的项目。
		/// </summary>
		/// <typeparam name="TEntity">数据实体的类型。</typeparam>
		/// <param name="dbContext">数据库上下文对象。</param>
		/// <param name="entity">要分离的对象。</param>
		public static void Detach<TEntity>(this DbContext dbContext, TEntity entity) where TEntity : class
		{
			dbContext.Entry(entity).State = EntityState.Detached;
		}

		/// <summary>
		/// 将数据库中的实体更换为新的对象。
		/// </summary>
		/// <typeparam name="TEntity">数据实体的类型。</typeparam>
		/// <param name="dbContext">数据库上下文对象。</param>
		/// <param name="originalObject">要更换的原始对象。</param>
		/// <param name="newObject">更换后的新对象。</param>
		public static void Replace<TEntity>(this DbContext dbContext, TEntity originalObject, TEntity newObject) where TEntity : class
		{
			dbContext.Detach(originalObject);
			dbContext.Update(newObject);
		}


		/// <summary>
		/// 获取一个值，指示当前成员是否被锁定。
		/// </summary>
		/// <param name="member">要检测的成员。</param>
		/// <returns>如果成员被锁定，返回 true；否则返回 false。</returns>
		public static bool IsLocked(this Member member)
		{
			return member.EventRegistrations.Any(i => i.TeamRegistration.EventState != TeamEventState.Ended);
		}

		/// <summary>
		/// 获取一个值，指示当前成员是否被某队伍锁定。
		/// </summary>
		/// <param name="member">要检测的成员。</param>
		/// <param name="teamId">要检测的队伍。</param>
		/// <returns>如果成员被锁定，返回 true；否则返回 false。</returns>
		public static bool IsLockedByTeam(this Member member, int teamId)
		{
			return member.EventRegistrations.Any(i => i.TeamId == teamId && i.TeamRegistration.EventState != TeamEventState.Ended);
		}

		/// <summary>
		/// 计算某个成员的特殊标记。
		/// </summary>
		/// <param name="member">成员对象。</param>
		/// <param name="standardNation">标准国籍。</param>
		/// <param name="teamDepartment">团队部门。</param>
		/// <returns>特殊标记的组合。</returns>
		public static PlayerSpecialRemarks GetSpecialRemark(this Member member, string standardNation, string teamDepartment)
		{
			var result = PlayerSpecialRemarks.None;

			if (member.IsProfessional)
			{
				result |= PlayerSpecialRemarks.Professional;
			}

			if (member.Nationality != standardNation)
			{
				result |= PlayerSpecialRemarks.Foreign;
			}

			if (!string.IsNullOrEmpty(teamDepartment) && member.Department != teamDepartment)
			{
				result |= PlayerSpecialRemarks.External;
			}

			return result;
		}

		/// <summary>
		/// 获取一个值，指示报名名单是否被锁定。
		/// </summary>
		/// <param name="item">要判断的项目。</param>
		/// <returns>如果名单被锁定，返回 true；否则返回 false。</returns>
		public static bool IsLocked(this EventTeamRegistration item)
		{
			return item.EventState != TeamEventState.NotStarted
					|| (item.AuditState == AuditState.Accepted && item.Event.State != EventState.Registring);
		}

		public static bool IsLockedByEvent(this Team team)
		{
			return team.EventTeamRegistrations.Any(i => i.EventState != TeamEventState.Ended && i.IsLocked());
		}

		/// <summary>
		/// 获取小组报名的分组和编号的联合结果。
		/// </summary>
		/// <param name="info">小组报名信息。</param>
		/// <returns>分组和编号的联合结果。</returns>
		public static string GetTeamGroupAndNumber(this EventTeamRegistration info)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}{1}", info.Group, info.GroupNumber);
		}

		/// <summary>
		/// 计算整数的整数次幂。
		/// </summary>
		/// <param name="x">X。</param>
		/// <param name="y">Y。</param>
		/// <returns>X 的 Y 次幂的值。</returns>
		public static int PowN(int x, int y)
		{
			if (y < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(y), y, "幂次不能为负数。");
			}

			var result = 1;

			while (y > 0)
			{
				result *= x;
				y--;
			}

			return result;
		}
	}
}
