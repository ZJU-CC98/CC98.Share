using CC98.Authentication;
using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc.Razor;
using Microsoft.Data.Entity;
using Microsoft.Framework.DependencyInjection;
using Sakura.AspNet.Mvc.PagedList;
using System.Threading.Tasks;
using Microsoft.AspNet.FileProviders;
using Microsoft.AspNet.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CC98.Sports
{
	/// <summary>
	/// 应用程序的启动类型。
	/// </summary>
	public class Startup
	{
		/// <summary>
		/// 初始化应用程序。
		/// </summary>
		/// <param name="env">宿主环境信息。</param>
		public Startup(IHostingEnvironment env)
		{
			// Setup configuration sources.

			var builder = new ConfigurationBuilder()
				.AddJsonFile("config.json")
				.AddJsonFile($"config.{env.EnvironmentName}.json", optional: true);

			if (env.IsDevelopment())
			{
				// This reads the configuration keys from the secret store.
				// For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
				builder.AddUserSecrets();
			}
			builder.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		/// <summary>
		/// 获取或设置应用程序的配置文件。
		/// </summary>
		private IConfigurationRoot Configuration { get; }

		/// <summary>
		/// 实现 <see cref="ISecurityStampValidator" /> 以处理 Cookie 过期问题。
		/// </summary>
		private class SimpleSecurityStampValidator : ISecurityStampValidator
		{
			/// <summary>
			/// Validates a security stamp of an identity as an asynchronous operation, and rebuilds the identity if the validation succeeds, otherwise rejects
			///             the identity.
			/// </summary>
			/// <param name="context">The context containing the <see cref="ClaimsPrincipal"/>and <see cref="AuthenticationProperties"/> to validate.</param>
			/// <returns>
			/// The <see cref="Task"/> that represents the asynchronous validation operation.
			/// </returns>
			public Task ValidateAsync(CookieValidatePrincipalContext context)
			{
				context.RejectPrincipal();
				return Task.FromResult(0);
			}
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// 应用程序设置
			services.Configure<AppSetting>(Configuration.GetSection("AppSettings"));

			// 配置数据库

			// 添加 EF 支持
			services.AddEntityFramework()
				// 添加 SQL 支持
				.AddSqlServer()
				// 添加程序中使用的数据库上下文
				.AddDbContext<SportDataModel>(options =>
				{
					// 配置 SQL 数据库连接
					options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]);
				})
				.AddDbContext<CC98V2DatabaseModel>(options =>
				{
					options.UseSqlServer(Configuration["Data:CC98V2:ConnectionString"]);
				});


			// 配置应用程序设置服务
			services.AddAppSetting<SystemSetting>(options =>
			{
				options.DefaultSetting = SystemSetting.Default;
				options.LoadMode = AppSettingLoadMode.Auto;
				options.SaveMode = AppSettingSaveMode.Auto;

			}).AddAccess(options =>
			{
				options.AppName = "CC98.Sports";
				options.DataFormat = AppSettingFormats.Json;
			});

			services.Configure<SharedAuthenticationOptions>(options =>
			{
				options.SignInScheme = new IdentityOptions().Cookies.ExternalCookieAuthenticationScheme;
			});

			// 配置应用程序的 Cookie 身份验证
			services.Configure<IdentityOptions>(options =>
			{
				options.Cookies.ApplicationCookie.AutomaticChallenge = true;
				options.Cookies.ApplicationCookie.AutomaticAuthenticate = true;
				options.Cookies.ApplicationCookie.CookieHttpOnly = true;
				options.Cookies.ApplicationCookie.LoginPath = new PathString("/Account/LogOn");
				options.Cookies.ApplicationCookie.LogoutPath = new PathString("/Account/LogOff");
				options.Cookies.ApplicationCookie.CookieSecure = CookieSecureOption.Never;

			});


			// Add MVC services to the services container.
			services.AddMvc(options =>
			{
				options.ModelBinders.InsertFlagsEnumModelBinder();
				options.Filters.Add(typeof(ActionResultExceptionFilterAttribute));
			}).AddViewLocalization(LanguageViewLocationExpanderFormat.SubFolder);

			// Uncomment the following line to add Web API services which makes it easier to port Web API 2 controllers.
			// You will also need to add the Microsoft.AspNet.Mvc.WebApiCompatShim package to the 'dependencies' section of project.json.
			// services.AddWebApiConventions();

			services.AddExternalSignInManager();

			services.AddTransient<ISecurityStampValidator, SimpleSecurityStampValidator>();

			services.AddAuthorization(options =>
			{
				var schema = new IdentityOptions().Cookies.ApplicationCookieAuthenticationScheme;

				options.AddPolicy(UserUtility.SystemAdminPolicy, UserUtility.GeneratePolicy(schema, UserUtility.SystemAdminRoles));
				options.AddPolicy(UserUtility.AdminPolicy, UserUtility.GeneratePolicy(schema, UserUtility.AdminRoles));
				options.AddPolicy(UserUtility.OrganizePolicy, UserUtility.GeneratePolicy(schema, UserUtility.OrganizeRoles));
				options.AddPolicy(UserUtility.OperatePolicy, UserUtility.GeneratePolicy(schema, UserUtility.OperateRoles));
				options.AddPolicy(UserUtility.ReviewPolicy, UserUtility.GeneratePolicy(schema, UserUtility.ReviewRoles));

			});

			// 分页器生成器
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

			services.AddCaching();
			services.AddSession();

			// 消息服务
			services.AddEnhancedTempData();
			services.AddOperationMessages();
		}

		// Configure is called after ConfigureServices is called.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			// Add the following to the request pipeline only in development environment.
			if (env.IsDevelopment())
			{
				app.UseBrowserLink();
				app.UseDeveloperExceptionPage(new ErrorPageOptions());
				app.UseDatabaseErrorPage(options =>
				{
					options.EnableAll();
				});
			}
			else
			{
				// Add Error handling middleware which catches all application specific errors and
				// sends the request to the following path or controller action.

				// 配置通用错误页面
				app.UseExceptionHandler("/Home/Error");

				try
				{
					using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
						.CreateScope())
					{
						serviceScope.ServiceProvider.GetService<SportDataModel>()
							 .Database.Migrate();
					}
				}
				catch
				{
				}
			}

			// 启用 IIS 平台处理程序（必须）
			app.UseIISPlatformHandler(options =>
			{
				options.AuthenticationDescriptions.Clear();
			});

			// 启用会话机制
			app.UseSession();

			// 允许提取网站内的静态资源
			app.UseStaticFiles();

			// Upload 文件夹路径映射
			app.UseStaticFiles(new StaticFileOptions
			{
				RequestPath = new PathString(Configuration["AppSettings:UploadRootFolder"]),
				FileProvider = new PhysicalFileProvider(Configuration["AppSettings:UploadPhysicalPath"])
			});

			// 所有 Cookie
			app.UseAllCookies();

			// 为应用程序启用 CC98 身份验证服务
			app.UseCC98Authentication(options =>
			{
				options.ClientId = Configuration["Authentication:CC98:ClientId"];
				options.ClientSecret = Configuration["Authentication:CC98:ClientSecret"];

			});


			// 配置 MVC 路由规则
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");

				// Uncomment the following line to add a route for porting Web API 2 controllers.
				// routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");
			});
		}

		/// <summary>
		/// 应用程序的入口点。
		/// </summary>
		/// <param name="args">应用程序启动参数。</param>
		public static void Main(string[] args) => WebApplication.Run<Startup>(args);
	}
}
