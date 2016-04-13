using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;
using CC98.LogOn.Models;
using CC98.LogOn.Services;
using CC98.LogOn.ViewModels.Account;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Http.Authentication;
using Sakura.AspNet;

namespace CC98.LogOn.Controllers
{
	public class AccountController : Controller
	{
		private CC98UserManager CC98UserManager { get; }

		private ICollection<OperationMessage> OperationMessages { get; }

		public AccountController(CC98UserManager cc98UserManager, IOperationMessageAccessor operationMessageAccessor)
		{
			CC98UserManager = cc98UserManager;
			OperationMessages = operationMessageAccessor.Messages;
		}

		/// <summary>
		/// 显示登录界面。
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[AllowAnonymous]
		public IActionResult LogOn()
		{
			return View();
		}

		/// <summary>
		/// 执行登录操作。
		/// </summary>
		/// <param name="model">数据模型。</param>
		/// <returns>登录结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LogOn(LogOnViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await CC98UserManager.LogOnAsync(model.UserName, model.Password);

				if (user != null)
				{
					var claims = new[]
					{
						new Claim(ClaimTypes.Name, user.Name),
						new Claim(ClaimTypes.NameIdentifier, user.Id.ToString("D"))
					};

					var identity = new ClaimsIdentity(claims);
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

					await HttpContext.Authentication.SignInAsync(IdentityCookieOptions.ApplicationCookieAuthenticationType, principal, properties);

					return RedirectToAction("Index", "Home");
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
