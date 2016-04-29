using System;

namespace CC98.LogOn.Models
{
	[Flags]
	public enum UserState
	{
		/// <summary>
		/// 用户处于正常状态。
		/// </summary>
		Normal = 0x0,
		/// <summary>
		/// 用户被锁定。
		/// </summary>
		Locked = 0x1,
		/// <summary>
		/// 用户被屏蔽。
		/// </summary>
		Forbidden = 0x2
	}
}