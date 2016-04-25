using CC98.Authentication;
using CC98.Share.Models;
using JetBrains.Annotations;
using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.FileProviders;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.StaticFiles;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Framework.DependencyInjection;
using Sakura.AspNet.Mvc;
using Sakura.AspNet.Mvc.PagedList;

namespace CC98.Share
{
    public class Setting
    {
        public string StoreFolder
        {
            get;
            set;
        }
    }
    /// <summary>
    /// 应用程序的启动类型。
    /// </summary>
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
	public class Startup
	{
		/// <summary>
		/// 创建启动环境。
		/// </summary>
		/// <param name="env">ASP.NET 宿主环境信息。</param>
		[UsedImplicitly]
		public Startup(IHostingEnvironment env)
		{
            // 导入应用程序配置
            var builder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

			// 如果处于开发模式，则添加用户个人机密数据（如服务器密码）
			if (env.IsDevelopment())
			{
				// For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
				builder.AddUserSecrets();
			}

			// 导入系统环境变量
			builder.AddEnvironmentVariables();

			// 生成配置对象
			Configuration = builder.Build();
		}

		/// <summary>
		/// 获取应用程序的配置。
		/// </summary>
		private IConfigurationRoot Configuration { get; }

		/// <summary>
		/// 配置应用程序的服务。
		/// </summary>
		/// <param name="services">应用程序中所有已注册服务的集合。</param>
		[UsedImplicitly]
		public void ConfigureServices(IServiceCollection services)
		{
            services.Configure<Setting>(Configuration.GetSection("FileSetting"));
            // 为应用程序添加数据访问支持
            services.AddEntityFramework()
				// 添加 SQL 数据库链接功能
				.AddSqlServer()
				// 添加 CC98ShareModel 数据存储
				.AddDbContext<CC98ShareModel>(options =>
				{
					// 从配置文件中读取连接到数据库使用的连接字符串
					options.UseSqlServer(Configuration["Data:ShareDatabase:ConnectionString"]);
				});

            services.UseBootstrapPagerGenerator(options =>
            {
                options.ExpandPageLinksForCurrentPage = 2;
                options.PageLinksForEndings = 3;

                options.Layout = PagedListPagerLayouts.Default;

                options.Items = new PagerItemOptions
                {
                    EncodeText = true,
                    LinkParameterName = "page",
                    TextFormat = "{0:d}",
                };

                options.Omitted = new PagerItemOptions
                {
                    Text = "...",
                    Link = string.Empty
                };

                options.FirstButton = new FirstAndLastPagerItemOptions
                {
                    Text = "&laquo;",
                    EncodeText = false,
                    ActiveMode = FirstAndLastPagerItemActiveMode.Always,
                    InactiveBehavior = SpecialPagerItemInactiveBehavior.Disable,
                    LinkParameterName = "page",
                };

                options.LastButton = new FirstAndLastPagerItemOptions
                {
                    Text = "&raquo;",
                    EncodeText = false,
                    ActiveMode = FirstAndLastPagerItemActiveMode.Always,
                    InactiveBehavior = SpecialPagerItemInactiveBehavior.Disable,
                    LinkParameterName = "page",
                };

                options.PreviousButton = new SpecialPagerItemOptions
                {
                    Text = "&lsaquo;",
                    EncodeText = false,
                    InactiveBehavior = SpecialPagerItemInactiveBehavior.Disable,
                    LinkParameterName = "page",
                };

                options.NextButton = new SpecialPagerItemOptions
                {
                    Text = "&rsaquo;",
                    EncodeText = false,
                    InactiveBehavior = SpecialPagerItemInactiveBehavior.Disable,
                    LinkParameterName = "page",
                };
            });

            // 为应用程序添加 MVC 功能
            services.AddMvc();

			// 配置应用程序的身份验证设置
			services.Configure<IdentityOptions>(identityOptions =>
			{
				// 应用程序的主 Cookie 设置
				identityOptions.Cookies.ApplicationCookie.CookieHttpOnly = true;
				identityOptions.Cookies.ApplicationCookie.CookieSecure = CookieSecureOption.Never;
				identityOptions.Cookies.ApplicationCookie.LoginPath = new PathString("/Account/LogOn");
				identityOptions.Cookies.ApplicationCookie.LogoutPath = new PathString("/Account/LogOff");
				identityOptions.Cookies.ApplicationCookie.AutomaticAuthenticate = true;
				identityOptions.Cookies.ApplicationCookie.AutomaticChallenge = true;

				// 外部 Cookie（和其他身份验证交互使用的临时票证）设置
				identityOptions.Cookies.ExternalCookie.CookieHttpOnly = true;
				identityOptions.Cookies.ExternalCookie.CookieSecure = CookieSecureOption.Never;
				identityOptions.Cookies.ExternalCookie.AutomaticAuthenticate = false;
				identityOptions.Cookies.ExternalCookie.AutomaticChallenge = false;
			});

			// 配置第三方身份验证相关的设置
			services.Configure<SharedAuthenticationOptions>(options =>
			{
				// 将第三方身份验证信息暂存入网站的外部 Cookie 中
				options.SignInScheme = new IdentityOptions().Cookies.ExternalCookieAuthenticationScheme;
			});


			// 添加外部身份验证管理器功能
			services.AddExternalSignInManager();

			// 添加缓存和会话数据服务（TempData 必须）
			services.AddCaching();
			services.AddSession();

			// 使用增强的 TempData 服务（Message 服务必须）
			services.AddEnhancedTempData();
			// 通过 TempData 在网页间传输标准化提示消息的功能
			services.AddOperationMessages();
		}

		/// <summary>
		/// 配置应用程序设置。
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

				try
				{
					using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
						.CreateScope())
					{
						// 启用数据库自动迁移
						serviceScope.ServiceProvider.GetService<CC98ShareModel>().Database.Migrate();
					}
				}
				catch { }
			}

			// 使用 IIS 平台处理程序承载网站（必须语句，不可去除！）
			app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

			// 启用会话数据服务支持（TempData 必须）
			app.UseSession();

			// 启用所有 Cookie 相关的身份验证功能
			app.UseAllCookies();

			// 配置应用程序使用 CC98 账户登录
			app.UseCC98Authentication(options =>
			{
				options.AllowInsecureHttp = true;
				options.ClientId = Configuration["Authentication:CC98:ClientId"];
				options.ClientSecret = Configuration["Authentication:CC98:ClientSecret"];
			});

            // 允许网站直接返回静态文件（样式表，脚本等）的内容
            var option = new StaticFileOptions();
            option.RequestPath = new PathString(Configuration["WebFolder"]);
            option.FileProvider = new PhysicalFileProvider(Configuration["StoreFolder"]);
            option.ServeUnknownFileTypes = true;
            app.UseStaticFiles();

			// 配置 MVC 的路径映射表
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}

		/// <summary>
		/// 应用程序的启动方法。
		/// </summary>
		/// <param name="args">应用程序的启动参数。</param>
		[UsedImplicitly]
		public static void Main(string[] args) => WebApplication.Run<Startup>(args);
	}
}
