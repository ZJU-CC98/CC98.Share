namespace CC98
{
	/// <summary>
	/// 表示应用程序的选项相关功能的设置。
	/// </summary>
	/// <typeparam name="T">保存应用程序设置的数据对象类型。</typeparam>
	public class AppSettingOptions<T>
	{
		/// <summary>
		/// 获取或设置当应用程序设置对象不存在时使用的默认设置值。
		/// </summary>
		public T DefaultSetting { get; set; }

		/// <summary>
		/// 获取或设置应用设置的保存模式。
		/// </summary>
		public AppSettingSaveMode SaveMode { get; set; } = AppSettingSaveMode.Auto;

		/// <summary>
		/// 获取或设置应用设置的加载模式。
		/// </summary>
		public AppSettingLoadMode LoadMode { get; set; } = AppSettingLoadMode.Auto;
	}

	/// <summary>
	/// 定义 <see cref="AppSettingService{T}.Current"/> 的保存模式。
	/// </summary>
	public enum AppSettingSaveMode
	{
		/// <summary>
		/// 当修改 <see cref="AppSettingService{T}.Current"/> 属性时自动保存设置。
		/// </summary>
		Auto,
		/// <summary>
		/// 用户必须使用 <see cref="AppSettingService{T}.SaveAsync"/> 方法手动保存设置。
		/// </summary>
		Manual,
	}

	/// <summary>
	/// 定义 <see cref="AppSettingService{T}.Current"/> 的加载模式。
	/// </summary>
	public enum AppSettingLoadMode
	{
		/// <summary>
		/// 当服务对象初始化时自动加载设置。
		/// </summary>
		Auto,
		/// <summary>
		/// 当第一次访问 <see cref="AppSettingService{T}.Current"/> 时加载设置。
		/// </summary>
		AutoDelayed,
		/// <summary>
		/// 不使用缓存，每次访问设置都重新从数据加载数据。这可能严重影响性能。
		/// </summary>
		Always,
		/// <summary>
		/// 除非使用 <see cref="AppSettingService{T}.LoadAsync"/> 方法加载数据，否则绝不自动加载设置。
		/// </summary>
		Manual
	}
}
