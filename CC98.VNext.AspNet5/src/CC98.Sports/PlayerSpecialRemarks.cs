using System;

namespace CC98.Sports
{
	/// <summary>
	/// 表示运动员的特殊标记状态。
	/// </summary>
	[Flags]
    public enum PlayerSpecialRemarks
    {
		/// <summary>
		/// 无特殊标记。
		/// </summary>
		None = 0,
		/// <summary>
		/// 专业运动员。
		/// </summary>
		Professional = 0x1,
		/// <summary>
		/// 外籍运动员。
		/// </summary>
		Foreign = 0x2,
		/// <summary>
		/// 外援。
		/// </summary>
		External = 0x4
    }
}
