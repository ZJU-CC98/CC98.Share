using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace CC98
{
	/// <summary>
	/// 提供对强类型应用程序设置的访问。
	/// </summary>
	/// <typeparam name="T">应用程序设置的类型。</typeparam>
	public class AppSettingService<T>
	{
		/// <summary>
		/// 应用程序访问服务。
		/// </summary>
		private AppSettingAccessService AccessService { get; }

		/// <summary>
		/// 设置选项。
		/// </summary>
		private AppSettingOptions<T> Options { get; set; }

		/// <summary>
		/// 用指定的信息初始化一个对象的新实例。
		/// </summary>
		/// <param name="accessService">应用程序设置访问服务。</param>
		/// <param name="options">应用程序设置。</param>
		public AppSettingService(AppSettingAccessService accessService, IOptions<AppSettingOptions<T>> options)
		{
			AccessService = accessService;
			Options = options.Value;

			// 如果自动加载，则执行加载操作
			switch (Options.LoadMode)
			{
				case AppSettingLoadMode.Auto:
					Load();
					break;
			}
		}

		/// <summary>
		/// 获取或设置一个值，指示当前设置是否已经加载。
		/// </summary>
		private bool IsLoaded { get; set; }

		/// <summary>
		/// 当前值的内部值。
		/// </summary>
		protected T CurrentCore { get; set; }

		/// <summary>
		/// 获取或设置应用程序的当前设置。
		/// </summary>
		public T Current
		{
			get
			{
				switch (Options.LoadMode)
				{
					// 始终加载
					case AppSettingLoadMode.Always:
						Load();
						break;

					// 延迟加载
					case AppSettingLoadMode.AutoDelayed:
						if (!IsLoaded)
						{
							Load();
						}
						break;
				}

				return CurrentCore;
			}
			set
			{
				CurrentCore = value;

				// 如果自动保存，则调用保存方法
				switch (Options.SaveMode)
				{
					case AppSettingSaveMode.Auto:
						Save();
						break;
				}
			}
		}


		/// <summary>
		/// 同步加载当前设置。
		/// </summary>
		public void Load()
		{
			CurrentCore = AccessService.LoadSettingOrDefault(Options.DefaultSetting);
			IsLoaded = true;
		}


		/// <summary>
		/// 异步加载当前设置。
		/// </summary>
		/// <returns>表示异步操作的对象。</returns>
		public async Task LoadAsync()
		{
			CurrentCore = await AccessService.LoadSettingOrDefaultAsync(Options.DefaultSetting);
			IsLoaded = true;
		}

		/// <summary>
		/// 异步保存当前设置。
		/// </summary>
		public void Save()
		{
			AccessService.SaveSetting(CurrentCore);
		}

		/// <summary>
		/// 异步保存当前设置。
		/// </summary>
		/// <returns>表示异步操作的对象。</returns>
		public async Task SaveAsync()
		{
			await AccessService.SaveSettingAsync(CurrentCore);
		}
	}
}
