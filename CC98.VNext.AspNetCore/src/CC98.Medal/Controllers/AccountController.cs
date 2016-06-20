using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.Authentication;
using CC98.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace CC98.Medal.Controllers
{
	/// <summary>
	/// 提供账户操作。
	/// </summary>
	public class AccountController : Controller
	{
		public AccountController(ExternalSignInManager externalSignInManager)
		{
			ExternalSignInManager = externalSignInManager;
		}

		private ExternalSignInManager ExternalSignInManager { get; }

		/// <summary>
		/// 请求登录。
		/// </summary>
		/// <param name="returnUrl">返回的 URL 地址。</param>
		/// <returns>操作结果。</returns>
		[AllowAnonymous]
		public IActionResult LogOn(string returnUrl)
		{
			var callbackUrl = Url.Action("LogOnCallback", "Account", new { returnUrl });

			var prop = new AuthenticationProperties
			{
				RedirectUri = callbackUrl
			};

			return new ChallengeResult(CC98AuthenticationDefaults.AuthentcationScheme, prop);
		}

		/// <summary>
		/// 执行登录后回调操作。
		/// </summary>
		/// <param name="returnUrl">登录成功的返回 URL。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> LogonCallback(string returnUrl)
		{
			var identity = await ExternalSignInManager.SignInFromExternalCookieAsync();

			if (identity != null)
			{
				// 没有返回地址则回到首页
				if (string.IsNullOrEmpty(returnUrl))
				{
					returnUrl = Url.Action("Index", "Home");
				}

				return Redirect(returnUrl);
			}

			// 登录失败则回到首页
			return RedirectToAction("Index", "Home");
		}

		/// <summary>
		/// 注销当前用户。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public async Task<IActionResult> LogOff()
		{
			await ExternalSignInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
	}
}
