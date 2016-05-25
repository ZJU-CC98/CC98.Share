using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace CC98
{
	/// <summary>
	/// 为应用程序设置功能提供访问接口。
	/// </summary>
	public class AppSettingAccessService
	{

		/// <summary>
		/// 获取数据库对象。
		/// </summary>
		private CC98V2DatabaseModel DbContext { get; }

		/// <summary>
		/// 获取设置对象。
		/// </summary>
		private AppSettingAccessOptions Options { get; }

		/// <summary>
		/// 数据编解码辅助工具。
		/// </summary>
		private IDataSerializationService SerializationService { get; }

		/// <summary>
		/// 初始化一个应用程序设置工具的新实例。
		/// </summary>
		/// <param name="dbContext">数据库连接对象。</param>
		/// <param name="options">应用设置对象。</param>
		/// <param name="serializationService">数据的序列化帮助程序。</param>
		public AppSettingAccessService(CC98V2DatabaseModel dbContext, IOptions<AppSettingAccessOptions> options, IDataSerializationService serializationService)
		{
			DbContext = dbContext;
			Options = options.Value;
			SerializationService = serializationService;
		}

		/// <summary>
		/// 获取应用设置对象。
		/// </summary>
		/// <returns>当前应用的设置对象。如果该对象不存在则返回 null。</returns>
		public AppSetting LoadSettingItem()
		{
			return (from i in DbContext.AppSettings
					where i.AppName == Options.AppName
					select i).SingleOrDefault();
		}

		/// <summary>
		/// 获取应用设置对象。
		/// </summary>
		/// <returns>当前应用的设置对象。如果该对象不存在则返回 null。</returns>
		public Task<AppSetting> LoadSettingItemAsync()
		{
			return (from i in DbContext.AppSettings
					where i.AppName == Options.AppName
					select i).SingleOrDefaultAsync();
		}

		/// <summary>
		/// 写入应用程序设置对象。
		/// </summary>
		/// <param name="appSetting">要写入的设置。</param>
		public void SaveSettingItem(AppSetting appSetting)
		{
			var item = LoadSettingItem();

			if (item == null)
			{
				DbContext.AppSettings.Add(appSetting);
			}
			else
			{
				item.Data = appSetting.Data;
				item.Format = appSetting.Format;
			}

			DbContext.SaveChanges();
		}

		/// <summary>
		/// 写入应用程序设置对象。
		/// </summary>
		/// <param name="appSetting">要写入的设置。</param>
		/// <returns>表示异步操作的对象。</returns>
		public async Task SaveSettingItemAsync(AppSetting appSetting)
		{
			var item = await LoadSettingItemAsync();

			if (item == null)
			{
				DbContext.AppSettings.Add(appSetting);
			}
			else
			{
				item.Data = appSetting.Data;
				item.Format = appSetting.Format;
			}

			await DbContext.SaveChangesAsync();
		}

		/// <summary>
		/// 加载应用程序设置数据。
		/// </summary>
		/// <typeparam name="T">应用程序设置数据的类型。</typeparam>
		/// <returns>加载的应用程序设置。如果该设置不存在，则返回 <typeparamref name="T"/> 类型的默认值。</returns>
		public T LoadSetting<T>()
		{
			// 加载设置
			var settingItem = LoadSettingItem();

			// 如果设置项不存在则返回默认值，否则解码数据并返回
			return settingItem == null ? default(T) : SerializationService.Deserialize<T>(settingItem.Data, Options.DataFormat);

		}

		/// <summary>
		/// 加载应用程序设置数据。
		/// </summary>
		/// <typeparam name="T">应用程序设置数据的类型。</typeparam>
		/// <returns>加载的应用程序设置。如果该设置不存在，则返回 <typeparamref name="T"/> 类型的默认值。</returns>
		public async Task<T> LoadSettingAsync<T>()
		{
			// 加载设置
			var settingItem = await LoadSettingItemAsync();

			// 如果设置项不存在则返回默认值，否则解码数据并返回
			return settingItem == null ? default(T) : SerializationService.Deserialize<T>(settingItem.Data, Options.DataFormat);

		}

		/// <summary>
		/// 写入应用程序设置。
		/// </summary>
		/// <typeparam name="T">应用程序设置数据的类型。</typeparam>
		/// <returns>表示异步操作的对象。</returns>
		public void SaveSetting<T>(T appSetting)
		{
			// 设置数据
			var settingData = SerializationService.Serialize(appSetting, Options.DataFormat);

			// 加载现有设置
			var item = LoadSettingItem();

			// 如果现有设置不存在，则创建新设置
			if (item == null)
			{
				DbContext.AppSettings.Add(new AppSetting
				{
					Format = Options.DataFormat,
					AppName = Options.AppName,
					Data = settingData
				});
			}
			else
			{
				// 否则修改现有数据
				item.Data = settingData;
			}

			DbContext.SaveChanges();
		}

		/// <summary>
		/// 写入应用程序设置。
		/// </summary>
		/// <typeparam name="T">应用程序设置数据的类型。</typeparam>
		/// <param name="appSetting">要写入的应用程序设置。</param>
		/// <returns>表示异步操作的对象。</returns>
		public async Task SaveSettingAsync<T>(T appSetting)
		{
			// 设置数据
			var settingData = SerializationService.Serialize(appSetting, Options.DataFormat);

			// 加载现有设置
			var item = await LoadSettingItemAsync();

			// 如果现有设置不存在，则创建新设置
			if (item == null)
			{
				DbContext.AppSettings.Add(new AppSetting
				{
					Format = Options.DataFormat,
					AppName = Options.AppName,
					Data = settingData
				});
			}
			else
			{
				// 否则修改现有数据
				item.Data = settingData;
			}

			await DbContext.SaveChangesAsync();
		}

		/// <summary>
		/// 读取应用程序设置。如果应用程序设置不存在，则在数据库中写入默认值。
		/// </summary>
		/// <typeparam name="T">应用程序的数据数据类型。</typeparam>
		/// <param name="defaultValue">当数据不存在时使用的默认值。</param>
		/// <returns>当前数据库中的设置。如果设置不存在，则返回 <paramref name="defaultValue"/>。</returns>
		public T LoadSettingOrDefault<T>(T defaultValue)
		{
			// 加载当前设置项目
			var settingItem = LoadSettingItem();

			// 如果设置存在，则读取内容
			if (settingItem != null)
			{
				return SerializationService.Deserialize<T>(settingItem.Data, Options.DataFormat);
			}
			// 否则写入新内容
			else
			{
				SaveSetting(defaultValue);
				return defaultValue;
			}
		}

		/// <summary>
		/// 读取应用程序设置。如果应用程序设置不存在，则在数据库中写入默认值。
		/// </summary>
		/// <typeparam name="T">应用程序的数据数据类型。</typeparam>
		/// <param name="defaultValue">当数据不存在时使用的默认值。</param>
		/// <returns>当前数据库中的设置。如果设置不存在，则返回 <paramref name="defaultValue"/>。</returns>
		public async Task<T> LoadSettingOrDefaultAsync<T>(T defaultValue)
		{
			// 加载当前设置项目
			var settingItem = await LoadSettingItemAsync();

			// 如果设置存在，则读取内容
			if (settingItem != null)
			{
				return SerializationService.Deserialize<T>(settingItem.Data, Options.DataFormat);
			}
			// 否则写入新内容
			else
			{
				await SaveSettingAsync(defaultValue);
				return defaultValue;
			}
		}
	}
}
