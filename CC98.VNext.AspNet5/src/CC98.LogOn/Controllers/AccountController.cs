using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using CC98.LogOn.Services;
using CC98.LogOn.ViewModels.Account;
using IdentityModel;
using IdentityServer4.Core.Models;
using IdentityServer4.Core.Services;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.Extensions.Configuration;
using Sakura.AspNet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Constants = IdentityServer4.Core.Constants;

namespace CC98.LogOn.Controllers
{
	public class AccountController : Controller
	{
		/// <summary>
		/// 表示特定操作请求用户登录后的后续执行操作。
		/// </summary>
		private class SignInResult : IActionResult
		{
			/// <summary>
			/// 构造一个对象的新实例。
			/// </summary>
			/// <param name="requestId">登录操作关联的消息标识。</param>
			public SignInResult(string requestId)
			{
				RequestId = requestId;
			}

			/// <summary>
			/// 获取本次登录相关的标识。
			/// </summary>
			private string RequestId { get; }

			public Task ExecuteResultAsync(ActionContext context)
			{
				var signInInteraction =
					context.HttpContext.RequestServices.GetRequiredService<SignInInteraction>();

				return signInInteraction.ProcessResponseAsync(RequestId, new SignInResponse());
			}
		}

		/// <summary>
		/// 获取 CC98 用户管理器对象。
		/// </summary>
		private CC98UserManager CC98UserManager { get; }

		/// <summary>
		/// 获取操作消息集合。
		/// </summary>
		private ICollection<OperationMessage> OperationMessages { get; }

		/// <summary>
		/// 获取登录交互操作对象。
		/// </summary>
		private SignInInteraction SignInInteraction { get; }

		/// <summary>
		/// 获取授权交互操作对象。
		/// </summary>
		private ConsentInteraction ConsentInteraction { get; }

		/// <summary>
		/// 获取客户端存储区。
		/// </summary>
		private IClientStore ClientStore { get; }

		/// <summary>
		/// 获取应用管理器。
		/// </summary>
		private CC98AppManager AppManager { get; }

		/// <summary>
		/// 获取领域存储区。
		/// </summary>
		private IScopeStore ScopeStore { get; }

		private ILogger<AccountController> Logger { get; }

		public AccountController(CC98UserManager cc98UserManager, IOperationMessageAccessor operationMessageAccessor, SignInInteraction signInInteraction, ConsentInteraction consentInteraction, IClientStore clientStore, IScopeStore scopeStore, IProfileService profileService, ILogger<AccountController> logger, CC98AppManager appManager)
		{
			CC98UserManager = cc98UserManager;
			SignInInteraction = signInInteraction;
			ConsentInteraction = consentInteraction;
			ClientStore = clientStore;
			ScopeStore = scopeStore;
			Logger = logger;
			AppManager = appManager;
			OperationMessages = operationMessageAccessor.Messages;
		}

		/// <summary>
		/// 显示登录界面。
		/// </summary>
		/// <param name="id">登录相关的可选 ID。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[AllowAnonymous]
		[Route(Constants.RoutePaths.Login)]
		public IActionResult LogOn(string id)
		{
			var model = new LogOnViewModel
			{
				MessageId = id
			};

			return View(model);
		}

		/// <summary>
		/// 显示授权页面。
		/// </summary>
		/// <param name="id">授权请求相关的标识。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(Policies.RequireCC98)]
		[Route(Constants.RoutePaths.Consent)]
		public async Task<IActionResult> Authorize(string id)
		{
			var requestMessage = await ConsentInteraction.GetRequestAsync(id);

			if (requestMessage == null)
			{
				Logger.LogError("请求的标识 {0} 不存在。", id);
				return HttpBadRequest();
			}

			var appInfo = await AppManager.FindAppByIdAsync(requestMessage.ClientId);
			var scopes = await ScopeStore.FindScopesAsync(requestMessage.ScopesRequested);

			var model = new AuthorizeViewModel
			{
				AppInfo = appInfo,
				Scopes = new[] {new ScopeDisplayInfo {Name= "all",  IsRequired = true, IsChecked = true, Title = "All", Description = "All description"}, }
			};

			return View(model);
		}

		/// <summary>
		/// 执行授权操作。
		/// </summary>
		/// <param name="model">操作模型。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize(Policies.RequireCC98)]
		[Route(Constants.RoutePaths.Consent)]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Authorize(AuthorizeViewModel model)
		{
			if (ModelState.IsValid)
			{
				var request = await ConsentInteraction.GetRequestAsync(model.RequestId);

				var response = new ConsentResponse
				{
					ScopesConsented = model.ConsentScopes,
					RememberConsent = model.RememberCensent
				};

				await ConsentInteraction.ProcessResponseAsync(model.RequestId, response);
			}


			return View(model);
		}

		/// <summary>
		/// 执行登录操作。
		/// </summary>
		/// <param name="model">数据模型。</param>
		/// <returns>登录结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route(Constants.RoutePaths.Login)]
		public async Task<IActionResult> LogOn(LogOnViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await CC98UserManager.LogOnAsync(model.UserName, model.Password);

				if (user != null)
				{
					var now = DateTime.UtcNow;

					var claims = new[]
					{
						new Claim(ClaimTypes.Name, user.Name),
						new Claim(ClaimTypes.NameIdentifier, user.Id.ToString("D")),

						new Claim(JwtClaimTypes.Subject, user.Name),
						new Claim(JwtClaimTypes.Id, user.Id.ToString("D")),
						new Claim(JwtClaimTypes.Name, user.Name),
						new Claim(JwtClaimTypes.AuthenticationTime, now.ToString("O"), ClaimValueTypes.DateTime),
						new Claim(JwtClaimTypes.IdentityProvider, "idsvr", ClaimValueTypes.String),
						new Claim(JwtClaimTypes.IssuedAt, now.ToString("O"), ClaimValueTypes.DateTime),
					};

					var identity = new ClaimsIdentity(claims, "password", JwtClaimTypes.Name, JwtClaimTypes.Role);
					var principal = new ClaimsPrincipal(identity);

					// 登录辅助属性，包括有效时间和是否长期有效
					var properties = new AuthenticationProperties
					{
						IssuedUtc = DateTimeOffset.Now
					};

					if (model.ValidTime != null)
					{
						properties.ExpiresUtc = DateTimeOffset.Now + model.ValidTime.Value;
						properties.IsPersistent = true;
					}
					else
					{
						properties.IsPersistent = false;
					}

					// 在 HTTP上下文中写入登录信息。
					await HttpContext.Authentication.SignInAsync(IdentityCookieOptions.ApplicationCookieAuthenticationType, principal, properties);

					// 如果登录是由某个请求发起的，则执行后续操作，否则返回首页。
					if (!string.IsNullOrEmpty(model.MessageId))
					{
						return new SignInResult(model.MessageId);
					}
					else
					{
						return RedirectToAction("Index", "Home");
					}
				}
				else
				{
					ModelState.AddModelError("", "登录失败，用户名或密码不正确。");
				}
			}

			return View(model);
		}

		/// <summary>
		/// 显示一个用户的详细信息。
		/// </summary>
		/// <param name="userName">要显示的用户名。</param>
		/// <returns>操作结果。</returns>
		[AllowAnonymous]
		public IActionResult Detail(string userName)
		{
			if (userName == null)
			{
				throw new ArgumentNullException(nameof(userName));
			}

			var url = string.Format(CultureInfo.InvariantCulture, "http://www.cc98.org/dispuser.asp?name={0}",
				Uri.EscapeDataString(userName));
			return Redirect(url);
		}

		/// <summary>
		/// 显示注册页面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}
	}
}