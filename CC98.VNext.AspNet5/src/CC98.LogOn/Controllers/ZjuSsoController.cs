using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CC98.LogOn.Models;
using CC98.LogOn.Services;
using CC98.LogOn.ViewModels.ZjuSso;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata.Internal;
using Sakura.AspNet;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CC98.LogOn.Controllers
{
	/// <summary>
	/// 提供浙大通行证相关的操作。
	/// </summary>
	public class ZjuSsoController : Controller
	{
		public ZjuSsoController(ZjuSsoService zjuSsoService, CC98V1DatabaseModel v1Model, PasswordEncryptionService passwordEncryptionService, IOperationMessageAccessor operationMessageAccessor)
		{
			ZjuSsoService = zjuSsoService;
			V1Model = v1Model;
			PasswordEncryptionService = passwordEncryptionService;
			OperationMessages = operationMessageAccessor.Messages;
		}

		/// <summary>
		/// 获取浙大通行证服务对象。
		/// </summary>
		private ZjuSsoService ZjuSsoService { get; }

		private CC98V1DatabaseModel V1Model { get; }

		private PasswordEncryptionService PasswordEncryptionService { get; }

		private ICollection<OperationMessage> OperationMessages { get; }


		/// <summary>
		/// 显示浙大通行证登录页面。
		/// </summary>
		/// <param name="returnUrl">操作的返回结果。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[AllowAnonymous]
		public IActionResult LogOn(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		/// <summary>
		/// 执行浙大通行证登录。
		/// </summary>
		/// <param name="model">数据模型。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[AllowAnonymous]
		public async Task<IActionResult> LogOn(LogOnViewModel model, string returnUrl)
		{
			if (ModelState.IsValid)
			{
				var identity = await ZjuSsoService.LogOnGetIdentityAsync(model.UserName, model.Password);

				if (identity != null)
				{
					var principal = new ClaimsPrincipal(identity);
					await HttpContext.Authentication.SignInAsync(ZjuSsoService.AuthenticationType, principal);

					if (string.IsNullOrEmpty(returnUrl))
					{
						returnUrl = Url.Action("Index", "Home");
					}

					return Redirect(returnUrl);
				}
				else
				{
					ModelState.AddModelError("", "登录失败。请确认你输入的用户名和密码正确，且浙大通行证账户状态正常。");
				}
			}

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LogOff()
		{
			await HttpContext.Authentication.SignOutAsync(ZjuSsoService.AuthenticationType);
			return RedirectToAction("Index", "Home");
		}

		/// <summary>
		/// 显示浙大通行证绑定的所有 CC98 账户。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(Policies.RequireZjuSso)]
		public async Task<IActionResult> Manage()
		{
			var principal = await HttpContext.Authentication.AuthenticateAsync(ZjuSsoService.AuthenticationType);
			var zjuId = principal.GetUserId();

			var items = from i in V1Model.Users
						where i.ZjuSsoId == zjuId
						select i;

			return View(await items.ToArrayAsync());
		}

		/// <summary>
		/// 为浙大通行证绑定新的 CC98 账户。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(Policies.RequireZjuSso)]
		public IActionResult Bind()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policies.RequireZjuSso)]
		public async Task<IActionResult> Bind(BindViewModel model)
		{
			if (ModelState.IsValid)
			{
				var userName = model.CC98UserName;
				var passwordHash = PasswordEncryptionService.EncryptV1Password(model.CC98Password);

				var user = await (from i in V1Model.Users
								  where i.Name == userName && i.PasswordHash == passwordHash
								  select i).SingleOrDefaultAsync();

				if (user == null)
				{
					ModelState.AddModelError("", "用户信息验证失败，输入的用户名或密码不正确。");
					return View(model);
				}

				if (user.IsVerified)
				{
					ModelState.AddModelError("", "该用户已经绑定了浙大通行证账户，无法再次绑定。");
					return View(model);
				}

				user.IsVerified = true;
				user.ZjuSsoId = User.GetUserId();

				try
				{
					await V1Model.SaveChangesAsync();
					OperationMessages.Add(OperationMessageLevel.Success, "操作成功。",
						string.Format(CultureInfo.CurrentCulture, "你已经将账号 {0} 绑定到了当前浙大通行证账户。", userName));
					return RedirectToAction("Manage", "ZjuSso");
				}
				catch (DbUpdateException ex)
				{
					ModelState.AddModelError("", ex.Message);
				}

			}

			return View(model);
		}

		/// <summary>
		/// 执行重置密码操作。
		/// </summary>
		/// <param name="model">数据模型。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policies.RequireZjuSso)]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var userName = model.CC98UserName;

				var user = await (from i in V1Model.Users
								  where i.Name == userName
								  select i).SingleOrDefaultAsync();

				if (user == null)
				{
					return HttpBadRequest("给定的用户名不存在。");
				}

				if (!string.Equals(user.ZjuSsoId, User.GetUserId(), StringComparison.OrdinalIgnoreCase))
				{
					return HttpBadRequest("你没有权限重置该用户的密码。");
				}

				user.PasswordHash = PasswordEncryptionService.EncryptV1Password(model.CC98NewPassword);
				await V1Model.SaveChangesAsync();
				OperationMessages.Add(OperationMessageLevel.Success, "操作成功。", string.Format(CultureInfo.CurrentCulture, "你已经为用户 {0} 重置了密码。", userName));
				return Ok();
			}

			return HttpBadRequest("提交的数据不正确。");
		}
	}
}
