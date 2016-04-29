using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CC98.LogOn.Models;
using CC98.LogOn.Services;
using IdentityServer4.Core.Services;
using IdentityServer4.Core.Validation;
using JetBrains.Annotations;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Framework.DependencyInjection;
using Constants = IdentityServer4.Core.Constants;
using IdentityServer4.Core.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CC98.LogOn
{
	[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			// Set up configuration sources.
			var builder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

			if (env.IsDevelopment())
			{
				// For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
				builder.AddUserSecrets();
			}

			builder.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		private IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		[UsedImplicitly]
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddEntityFramework()
				.AddSqlServer()
				.AddDbContext<CC98V2DatabaseModel>(options => options.UseSqlServer(Configuration["Data:CC98V2:ConnectionString"]))
				.AddDbContext<CC98V1DatabaseModel>(options => options.UseSqlServer(Configuration["Data:CC98V1:ConnectionString"]));

			services.AddMvc()
				.AddViewLocalization()
				.AddDataAnnotationsLocalization();

			// Add application services.
			services.AddTransient<IEmailSender, AuthMessageSender>();
			services.AddTransient<ISmsSender, AuthMessageSender>();

			services.AddSingleton<PasswordEncryptionService>();
			services.AddSingleton<CC98UserManager>();

			services.AddCaching();
			services.AddSession();

			services.AddEnhancedTempData();
			services.AddOperationMessages();

			services.AddSingleton<ZjuSsoService>();
			// 浙大通行证登录配置
			services.Configure<ZjuSsoOptions>(Configuration.GetSection("Authentication:ZjuSso"));

			services.AddAuthorization(options =>
			{
				options.AddPolicy(Policies.RequireZjuSso, Policies.GetPolicyForAuthenticationScheme(ZjuSsoService.AuthenticationType));
				options.AddPolicy(Policies.RequireCC98, Policies.GetPolicyForAuthenticationScheme(IdentityCookieOptions.ApplicationCookieAuthenticationType));
			});

			// IdentityServer 和必要服务

			services.AddTransient<IClientStore, CC98AppManager>();
			services.AddSingleton<IScopeStore, CC98ScopeStore>();
			services.AddTransient<IProfileService, CC98ProfileService>();
			services.AddSingleton<IRedirectUriValidator, RedirectUriValidator>();


			services.AddIdentityServer(options =>
			{
				// IdentityServer 默认身份验证类型
				options.AuthenticationOptions.PrimaryAuthenticationScheme =
					IdentityCookieOptions.ApplicationCookieAuthenticationType;
			});

			// 应用管理器
			services.AddTransient<CC98AppManager>();
		}

		private class RedirectUriValidator : IRedirectUriValidator
		{
			public Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
			{
				return Task.FromResult(true);
			}

			public Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
			{
				return client.RedirectUris.Count == 0
					? Task.FromResult(true)
					: Task.FromResult(client.RedirectUris.Contains(requestedUri, StringComparer.OrdinalIgnoreCase));
			}
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		[UsedImplicitly]
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			if (env.IsDevelopment())
			{
				app.UseBrowserLink();
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

			app.UseStaticFiles();

			app.UseSession();

			app.UseCookieAuthentication(options =>
			{
				options.AuthenticationScheme = ZjuSsoService.AuthenticationType;
				options.LoginPath = new PathString("/ZjuSso/LogOn");
				options.LogoutPath = new PathString("/ZjuSso/LogOff");
				options.CookieHttpOnly = true;
				options.CookieSecure = CookieSecureOption.Never;
				options.AutomaticAuthenticate = true;
				options.AutomaticChallenge = true;
			});

			app.UseCookieAuthentication(options =>
			{
				options.AuthenticationScheme = IdentityCookieOptions.ApplicationCookieAuthenticationType;
				options.LoginPath = new PathString("/Account/LogOn");
				options.LogoutPath = new PathString("/Account/LogOff");
				options.CookieHttpOnly = true;
				options.CookieSecure = CookieSecureOption.Never;
				options.AutomaticChallenge = true;
				options.AutomaticAuthenticate = true;
			});

			app.UseIdentityServer();

			// To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}

		// Entry point for the application.
		[UsedImplicitly]
		public static void Main(string[] args) => WebApplication.Run<Startup>(args);
	}
}
