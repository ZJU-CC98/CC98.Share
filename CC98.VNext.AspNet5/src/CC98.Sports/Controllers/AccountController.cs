using System.Collections.Generic;
using System.Threading.Tasks;
using CC98.Authentication;
using CC98.Identity;
using CC98.Identity.External;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Sakura.AspNet;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CC98.Sports.Controllers
{
	/// <summary>
	/// 登录控制器。
	/// </summary>
	public class AccountController : Controller
	{
		/// <summary>
		/// 获取当前上下文的授权控制器。
		/// </summary>
		private AuthenticationManager Authentication => HttpContext.Authentication;

		private ExternalSignInManager SignInManager { get; }

		/// <summary>
		/// 消息列表。
		/// </summary>
		private ICollection<OperationMessage> Messages { get; }

		/// <summary>
		/// 应用程序的主授权类型。该字段为常量。
		/// </summary>
		private const string PrimaryAuthenticatonScheme = CC98AuthenticationDefaults.AuthentcationScheme;

		/// <summary>
		/// 初始化一个控制器的新实例。
		/// </summary>
		/// <param name="signInManager">外部 Cookie 登录服务。</param>
		/// <param name="messageAccessor">消息列表访问服务。</param>
		public AccountController(ExternalSignInManager signInManager, IOperationMessageAccessor messageAccessor)
		{
			SignInManager = signInManager;
			Messages = messageAccessor.Messages;
		}


		/// <summary>
		/// 执行登录过程。
		/// </summary>
		/// <param name="returnUrl">登录后的返回地址。</param>
		/// <returns>操作结果。</returns>
		[AllowAnonymous]
		public IActionResult LogOn(string returnUrl = null)
		{
			var properties = new AuthenticationProperties
			{
				RedirectUri = Url.Action("LogOnCallback", "Account", new { returnUrl })
			};

			return new ChallengeResult(PrimaryAuthenticatonScheme, properties);
		}

		/// <summary>
		/// 执行登录后的回调。
		/// </summary>
		/// <param name="returnUrl">登录后的返回地址。</param>
		/// <param name="error">登录的错误信息。</param>
		/// <returns>登录后的回调地址。</returns>
		public async Task<IActionResult> LogOnCallback(string returnUrl, string error)
		{
			// 登录出现错误
			if(!string.IsNullOrEmpty(error))
			{
				Messages.Add(new OperationMessage(OperationMessageLevel.Error, "登录失败。", "这可能是登录系统故障或者用户拒绝授权引起的。请稍后再试一次。"));
				return RedirectToAction("Index", "Home");
			}

			if (await SignInManager.SignInFromExternalCookieAsync() == null)
			{
				Messages.Add(OperationMessageLevel.Error, "登录失败。", "登录过程中发生错误，请稍后再试一次。");
			}

			// 如果未定义返回地址，则跳转到主页
			if (string.IsNullOrEmpty(returnUrl))
			{
				returnUrl = Url.Action("Index", "Home");
			}

			return Redirect(returnUrl);
		}

		/// <summary>
		/// 注销用户。
		/// </summary>
		/// <returns>操作结果。</returns>
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LogOff()
		{
			await SignInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
	}
}
