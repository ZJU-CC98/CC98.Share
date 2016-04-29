namespace CC98.MedalManager.Models
{
	/// <summary>
	/// 定义勋章的购买类型。
	/// </summary>
	public enum MedalOrderType
	{
		/// <summary>
		/// 用户可自由购买勋章。
		/// </summary>
		Order = 0,
		/// <summary>
		/// 用户必须申请勋章并且管理员审核。
		/// </summary>
		Apply,
		/// <summary>
		/// 只能由管理员授予。
		/// </summary>
		Grant
	}
}