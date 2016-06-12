using System;
using CC98.Authentication;
using CC98.Medal.Data;
using JetBrains.Annotations;
using Microsoft.AspNet.Builder;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Framework.DependencyInjection;
using Sakura.AspNetCore.Mvc;

namespace CC98.Medal
{
	/// <summary>
	/// 应用程序的启动类型。
	/// </summary>
	public class Startup
	{
		/// <summary>
		/// 初始化应用程序的启动配置。
		/// </summary>
		/// <param name="env">宿主环境信息。</param>
		[UsedImplicitly(ImplicitUseKindFlags.Access)]
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", true, true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
				.AddEnvironmentVariables();

			// 用户机密配置
			if (env.IsDevelopment())
			{
				builder.AddUserSecrets();
			}

			Configuration = builder.Build();
		}

		/// <summary>
		/// 应用程序的配置信息。
		/// </summary>
		private IConfigurationRoot Configuration { get; }

		/// <summary>
		/// 配置应用程序服务。
		/// </summary>
		/// <param name="services">应用程序服务容器。</param>
		[UsedImplicitly(ImplicitUseKindFlags.Access)]
		public void ConfigureServices(IServiceCollection services)
		{
			// 配置应用程序设置
			services.Configure<AppSetting>(Configuration.GetSection("AppSetting"));

			// Add framework services.
			services.AddMvc();

			// 数据库支持
			services.AddDbContext<MedalDataModel>(options =>
			{
				options.UseSqlServer(Configuration["ConnectionStrings:MedalDatabase"]);
			});

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services.AddAuthentication(options =>
			{
				options.SignInScheme = new IdentityOptions().Cookies.ExternalCookieAuthenticationScheme;
			});

			services.AddExternalSignInManager(identityOptions =>
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

			// 授权配置
			services.AddAuthorization(options =>
			{
				options.AddPolicy(Policies.Administrate, builder =>
				{
					builder.RequireRole(Roles.Adminisrators, Roles.MedalAdministrators);
				});

				options.AddPolicy(Policies.Operate, builder =>
				{
					builder.RequireRole(Roles.Adminisrators, Roles.MedalAdministrators, Roles.MedalOperators);
				});

				options.AddPolicy(Policies.Edit, builder =>
				{
					builder.RequireRole(Roles.Adminisrators, Roles.MedalAdministrators, Roles.MedalOperators, Roles.MedalEditor);
				});
			});

			services.AddSession();
			services.AddMemoryCache();
			services.AddEnhancedTempData();
			services.AddOperationMessages();

			// 分页器
			services.AddBootstrapPagerGenerator(options =>
			{
				options.ConfigureDefault();
			});
		}

		[UsedImplicitly(ImplicitUseKindFlags.Access)]
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseSession();

			// Cookie 登录
			app.UseAllCookies();

			app.UseCC98Authentication(new CC98AuthenticationOptions
			{
				ClientId = Configuration["Authentication:CC98:ClientId"],
				ClientSecret = Configuration["Authentication:CC98:ClientSecret"],
			});



			// 默认文件
			app.UseStaticFiles();

			// 勋章图片存储
			app.UseStaticFiles(new StaticFileOptions
			{
				RequestPath = new PathString(Configuration["AppSetting:MedalImagePath"]),
				FileProvider = new PhysicalFileProvider(Configuration["AppSetting:MedalImageStoreFolder"])
			});

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					"default",
					"{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
