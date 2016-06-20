namespace CC98
{
	/// <summary>
	/// 表示应用程序设置工具的选项。
	/// </summary>
	public class AppSettingAccessOptions
	{

		/// <summary>
		/// 获取或设置应用的名称。
		/// </summary>
		public string AppName { get; set; }

		/// <summary>
		/// 获取或设置数据的格式。
		/// </summary>
		public string DataFormat { get; set; }

		/// <summary>
		/// 初始化一个对象的新实例。
		/// </summary>
		public AppSettingAccessOptions()
		{

		}

		/// <summary>
		/// 用指定的参数初始化一个应用的新实例。
		/// </summary>
		/// <param name="appName">应用的名称。</param>
		/// <param name="dataFormat">应用的数据格式。</param>
		public AppSettingAccessOptions(string appName, string dataFormat)
		{
			AppName = appName;
			DataFormat = dataFormat;
		}
	}
}
