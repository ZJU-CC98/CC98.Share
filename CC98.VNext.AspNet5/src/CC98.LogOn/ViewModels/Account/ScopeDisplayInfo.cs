using JetBrains.Annotations;

namespace CC98.LogOn.ViewModels.Account
{
	/// <summary>
	/// 表示一个领域的显示信息。
	/// </summary>
	public class ScopeDisplayInfo
    {
		/// <summary>
		/// 获取或设置该领域的实际名称。
		/// </summary>
		[LocalizationRequired(false)]
		public string Name { get; set; }

		/// <summary>
		/// 获取或设置该领域的标题。
		/// </summary>
		[LocalizationRequired]
		public string Title { get; set; }

		/// <summary>
		/// 获取或设置该领域的描述。
		/// </summary>
		[LocalizationRequired]
		public string Description { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示该领域是否默认被选中。
		/// </summary>
		public bool IsChecked { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示该领域是否是必须的。
		/// </summary>
		public bool IsRequired { get; set; }
    }
}
