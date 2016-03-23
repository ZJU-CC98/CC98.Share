using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.Share.Models;
using JetBrains.Annotations;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CC98.Share
{
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

			// 为应用程序添加 MVC 功能
			services.AddMvc();
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

			// 允许网站直接返回静态文件（样式表，脚本等）的内容
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
