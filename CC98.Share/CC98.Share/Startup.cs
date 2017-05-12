using CC98.Authentication;
using CC98.Share.Data;
using JetBrains.Annotations;
using Microsoft.AspNet.Builder;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Framework.DependencyInjection;
using Sakura.AspNetCore.Mvc;

namespace CC98.Share
{
	/// <summary>
	///     应用程序的启动类型。
	/// </summary>
	[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
	public class Startup
	{
		/// <summary>
		///     创建启动环境。
		/// </summary>
		/// <param name="env">ASP.NET 宿主环境信息。</param>
		[UsedImplicitly]
		public Startup(IHostingEnvironment env)
		{
			// 导入应用程序配置
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json")
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

			// 开发模式配置
			if (env.IsDevelopment())
			{
				// 如果处于开发模式，则添加用户个人机密数据（如服务器密码）
				builder.AddUserSecrets();

				// 添加 ApplicationInsights 开发模式设置
				builder.AddApplicationInsightsSettings(true);
			}

			// 导入系统环境变量
			builder.AddEnvironmentVariables();

			// 生成配置对象
			Configuration = builder.Build();
		}

		/// <summary>
		///     获取应用程序的配置。
		/// </summary>
		private IConfigurationRoot Configuration { get; }

		/// <summary>
		///     配置应用程序的服务。
		/// </summary>
		/// <param name="services">应用程序中所有已注册服务的集合。</param>
		[UsedImplicitly]
		public void ConfigureServices(IServiceCollection services)
		{
			// 添加 ApplicationInsights 遥测服务
			services.AddApplicationInsightsTelemetry(Configuration);

			services.Configure<Setting>(Configuration.GetSection("FileSetting"));

			// 添加 CC98ShareModel 数据存储
			services
				.AddDbContext<CC98ShareModel>(options =>
				{
					// 从配置文件中读取连接到数据库使用的连接字符串
					options.UseSqlServer(Configuration["ConnectionStrings:ShareDatabase"]);
				});

			// 添加分页支持
			services.AddBootstrapPagerGenerator(options => { options.ConfigureDefault(); });

			// 为应用程序添加 MVC 功能
			services.AddMvc();

			// 配置第三方身份验证相关的设置
			services.Configure<SharedAuthenticationOptions>(options =>
			{
				// 将第三方身份验证信息暂存入网站的外部 Cookie 中
				options.SignInScheme = new IdentityOptions().Cookies.ExternalCookieAuthenticationScheme;
			});


			// 添加外部身份验证管理器功能
			services.AddExternalSignInManager(identityOptions =>
			{
				// 应用程序的主 Cookie 设置
				identityOptions.Cookies.ApplicationCookie.CookieHttpOnly = true;
				identityOptions.Cookies.ApplicationCookie.CookieSecure = CookieSecurePolicy.None;
				identityOptions.Cookies.ApplicationCookie.LoginPath = new PathString("/Account/LogOn");
				identityOptions.Cookies.ApplicationCookie.LogoutPath = new PathString("/Account/LogOff");
				identityOptions.Cookies.ApplicationCookie.AutomaticAuthenticate = true;
				identityOptions.Cookies.ApplicationCookie.AutomaticChallenge = true;

				// 外部 Cookie（和其他身份验证交互使用的临时票证）设置
				identityOptions.Cookies.ExternalCookie.CookieHttpOnly = true;
				identityOptions.Cookies.ExternalCookie.CookieSecure = CookieSecurePolicy.None;
				identityOptions.Cookies.ExternalCookie.AutomaticAuthenticate = false;
				identityOptions.Cookies.ExternalCookie.AutomaticChallenge = false;
			});

			// 添加缓存和会话数据服务（TempData 必须）
			services.AddMemoryCache();
			services.AddSession();

			// 使用增强的 TempData 服务（Message 服务必须）
			services.AddEnhancedTempData();
			// 通过 TempData 在网页间传输标准化提示消息的功能
			services.AddOperationMessages();
		}

		/// <summary>
		///     配置应用程序设置。
		/// </summary>
		/// <param name="app">应用程序对象。</param>
		/// <param name="env">环境设置信息。</param>
		/// <param name="loggerFactory">用于创建日志记录工具的辅助对象。</param>
		[UsedImplicitly]
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			// 如果项目通过控制台承载，在控制台输出所有网站运行的日志信息
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));

			// 在开发环境的调试窗口输出所有日志信息
			loggerFactory.AddDebug();

			// 启用 ApplicationInsights 请求遥测服务
			app.UseApplicationInsightsRequestTelemetry();


			// 开发环境配置
			if (env.IsDevelopment())
			{
				// 启用 BrowseLink 功能，在网页上添加和调试器互动的工具栏
				app.UseBrowserLink();
				// 启用开发者错误页面，当网页出错时显示所有详细的错误说明信息
				app.UseDeveloperExceptionPage();
				// 启用数据库错误页面
				app.UseDatabaseErrorPage();
			}
			// 生产环境配置
			else
			{
				// 使用通用配置错误页面，当网站发生错误时跳转到 Home/Error 页
				app.UseExceptionHandler("/Home/Error");
			}

			// 启用 ApplicationInsights 异常遥测服务
			app.UseApplicationInsightsExceptionTelemetry();

			// 启用会话数据服务支持（TempData 必须）
			app.UseSession();

			app.UseAllCookies();

			// 配置应用程序使用 CC98 账户登录
			app.UseCC98Authentication(new CC98AuthenticationOptions
			{
				AllowInsecureHttp = true,
				ClientId = Configuration["Authentication:CC98:ClientId"],
				ClientSecret = Configuration["Authentication:CC98:ClientSecret"]
			});

			// 允许网站直接返回网站内静态文件（样式表，脚本等）的内容      
			app.UseStaticFiles();

			// 配置上传文件的物理存储区
			var option = new StaticFileOptions
			{
				RequestPath = new PathString(Configuration["FileSetting:WebFolder"]),
				FileProvider = new PhysicalFileProvider(Configuration["FileSetting:StoreFolder"]),
				ServeUnknownFileTypes = true
			};
			app.UseStaticFiles(option);

			// 配置 MVC 的路径映射表
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					"default",
					"{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}