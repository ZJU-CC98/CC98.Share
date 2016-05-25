using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sakura.AspNetCore;
using CC98.Identity;
using CC98.Authentication;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CC98.Share.Controllers
{
    /// <summary>
    ///     提供用户身份的相关功能。
    /// </summary>
    public class AccountController : Controller
    {
        public AccountController(ExternalSignInManager externalSignInManager, IOperationMessageAccessor messageAccessor)
        {
            ExternalSignInManager = externalSignInManager;
            MessageAccessor = messageAccessor;
        }
        
        /// <summary>
        ///     提供外部身份验证信息检索的相关功能。
        /// </summary>
        private ExternalSignInManager ExternalSignInManager { get; }

        /// <summary>
        ///     提供添加和检索网页消息的功能。
        /// </summary>
        private IOperationMessageAccessor MessageAccessor { get; }

        /// <summary>
        ///     当应用程序需要登录时，将访问该控制器操作。
        /// </summary>
        /// <param name="returnUrl">登录后要跳转回的地址。</param>
        /// <returns>操作结果。</returns>
        [AllowAnonymous]
        public IActionResult LogOn(string returnUrl)
        {
            // 将跳转返回的地址存入验证相关信息中
            var authProperties = new AuthenticationProperties
            {
                // 为了统一处理身份验证操作结果，返回地址必须统一跳转到 LogOnCallback，而登录时希望的跳转地址作为一个参数
                RedirectUri = Url.Action("LogOnCallback", "Account", returnUrl)
            };

            // 向系统发出请求 CC98 身份验证的操作
            return new ChallengeResult(new[] {CC98AuthenticationDefaults.AuthentcationScheme}, authProperties);
        }

        /// <summary>
        ///     授权服务器完成授权并返回网站时将访问该操作。
        /// </summary>
        /// <param name="error">服务器返回的错误信息。</param>
        /// <param name="returnUrl">登录完成后要跳转回的地址。</param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> LogOnCallback(string error, string returnUrl)
        {
            // 登录服务器返回错误
            if (!string.IsNullOrEmpty(error))
            {
                // 添加失败提示并返回主页
                MessageAccessor.Messages.Add(OperationMessageLevel.Error, "登录失败。", $"登录时发生错误，请再试一次。错误消息：{error}");
                return RedirectToAction("Index", "Home");
            }

            // 尝试提取系统中保留的第三方身份信息，并写入应用程序的主 Cookie
            var identity = await ExternalSignInManager.SignInFromExternalCookieAsync();

            // 返回 null 表示登录失败或者用户信息不正确
            if (identity == null)
            {
                // 添加失败提示并返回主页。
                MessageAccessor.Messages.Add(OperationMessageLevel.Error, "登录失败。", "无法检索到有效的用户信息。请稍后再试一次。");
                return RedirectToAction("Index", "Home");
            }

            // 如果用户未指定返回地址，则默认返回到首页。
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = Url.Action("Index", "Home");
            }

            // 返回到登录前页面
            return Redirect(returnUrl);
        }

        /// <summary>
        ///     当用户执行注销操作时引发。
        /// </summary>
        /// <returns>操作结果。</returns>
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            // 注销所有登录凭据
            await ExternalSignInManager.SignOutAsync();

            // 回到主页
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        ///     当用户希望注册账户时引发。
        /// </summary>
        /// <returns>操作结果。</returns>
        [AllowAnonymous]
        public IActionResult Register()
        {
            // 目前跳转到注册页面。
            return Redirect("https://secure.cc98.org/Register");
        }

        /// <summary>
        ///     管理当前用户的信息。
        /// </summary>
        /// <returns>操作结果。</returns>
        [Authorize]
        public IActionResult Manage()
        {
            // 目前直接跳转到当前用户的详细资料页面
            return RedirectToAction("Detail", "User", new {name = ExternalSignInManager.GetUserName(User)});
        }
    }
}