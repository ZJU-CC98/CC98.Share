using System;

namespace CC98
{
	/// <summary>
	/// 为服务注入功能提供扩展方法。该类型为静态类型。
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// 为应用程序添加设置访问服务。
		/// </summary>
		/// <param name="services">服务描述符容器对象。</param>
		/// <param name="setupAction">设置访问服务的相关设置。</param>
		public static void AddAppSettingAccess(this IServiceCollection services, Action<AppSettingAccessOptions> setupAction = null)
		{
			services.TryAddSingleton<IDataSerializationService, DataSerializationService>();
			services.TryAddSingleton<AppSettingAccessService>();

			if (setupAction != null)
			{
				services.Configure(setupAction);
			}
		}

		/// <summary>
		/// 为应用程序添加设置服务。
		/// </summary>
		/// <typeparam name="T">设置的类型。</typeparam>
		/// <param name="services">服务描述符容器对象。</param>
		/// <param name="setupAction">设置的相关设置。</param>
		/// <returns>可用于进一步配置访问服务的服务生成器。</returns>
		public static AppSettingServiceBuilder AddAppSetting<T>(this IServiceCollection services, Action<AppSettingOptions<T>> setupAction = null)
		{
			services.TryAddSingleton<AppSettingService<T>>();

			if (setupAction != null)
			{
				services.Configure(setupAction);
			}

			return new AppSettingServiceBuilder(services);
		}
	}

	/// <summary>
	/// 为 <see cref="AppSettingService{T}"/> 提供访问设置的相关设置。
	/// </summary>
	public class AppSettingServiceBuilder
	{
		private IServiceCollection Services { get; }

		public AppSettingServiceBuilder(IServiceCollection services)
		{
			Services = services;
		}

		/// <summary>
		/// 为设置服务添加基础访问功能。
		/// </summary>
		/// <param name="setupAction">访问服务的相关设置。</param>
		public void AddAccess(Action<AppSettingAccessOptions> setupAction = null)
		{
			Services.AddAppSettingAccess(setupAction);
		}
	}
}
