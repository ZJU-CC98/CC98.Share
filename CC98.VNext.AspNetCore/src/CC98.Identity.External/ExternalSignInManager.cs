using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.Options;

namespace CC98.Identity
{
    /// <summary>
    ///     提供使用联合账户进行外部登录的相关功能。
    /// </summary>
    public class ExternalSignInManager
    {
        /// <summary>
        ///     初始化一个对象的新实例。
        /// </summary>
        /// <param name="httpContextAccessor">HTTP 上下文访问器对象。</param>
        /// <param name="identityOptions">标识配置选项。</param>
        public ExternalSignInManager(IHttpContextAccessor httpContextAccessor, IOptions<IdentityOptions> identityOptions)
        {
            AuthenticationManager = httpContextAccessor.HttpContext.Authentication;
            IdentityOptions = identityOptions.Value;
        }

        /// <summary>
        ///     获取验证管理器服务。
        /// </summary>
        private AuthenticationManager AuthenticationManager { get; }

        /// <summary>
        ///     获取身份验证选项。
        /// </summary>
        private IdentityOptions IdentityOptions { get; }

        /// <summary>
        ///     如果用户当前通过外部登录凭据进行了登录。则将登录凭据添加到当前应用。
        /// </summary>
        /// <returns>如果登录成功，返回成功登录的 <see cref="ClaimsPrincipal" /> 对象。如果登录失败，返回 <c>null</c>。</returns>
        public async Task<ClaimsPrincipal> SignInFromExternalCookieAsync()
        {
            var externalLoginInfo =
                await
                    AuthenticationManager.AuthenticateAsync(IdentityOptions.Cookies.ExternalCookieAuthenticationScheme);

            if (externalLoginInfo == null)
            {
                return null;
            }

            // 新的身份验证类型，用于替换外部登录产生的身份验证类型
            var newScheme = IdentityOptions.Cookies.ApplicationCookieAuthenticationScheme;

            await AuthenticationManager.SignInAsync(newScheme, externalLoginInfo.CloneAs(newScheme));
            return externalLoginInfo;
        }

        /// <summary>
        ///     注销当前用户登录的用户主体。
        /// </summary>
        /// <returns>表示异步操作的任务。</returns>
        public async Task SignOutAsync()
        {
            await AuthenticationManager.SignOutAsync(IdentityOptions.Cookies.ApplicationCookieAuthenticationScheme);
            await AuthenticationManager.SignOutAsync(IdentityOptions.Cookies.ExternalCookieAuthenticationScheme);
        }

        /// <summary>
        ///     获取给定用户标识的用户名。如果用户名信息不存在，返回 <c>null</c>。
        /// </summary>
        /// <param name="identity">用户标识对象。</param>
        /// <exception cref="ArgumentNullException"><paramref name="identity" /> 为 <c>null</c>。</exception>
        /// <returns><paramref name="identity" /> 中包含的用户名信息。</returns>
        [CanBeNull]
        public string GetUserName([NotNull] ClaimsIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            return identity.FindFirst(IdentityOptions.ClaimsIdentity.UserNameClaimType)?.Value;
        }

        /// <summary>
        ///     获取给定用户主体的用户名。如果用户名信息不存在，返回 <c>null</c>。
        /// </summary>
        /// <param name="principal">用户主体对象。</param>
        /// <exception cref="ArgumentNullException"><paramref name="principal" /> 为 <c>null</c>。</exception>
        /// <returns><paramref name="principal" /> 中包含的用户名信息。</returns>
        [CanBeNull]
        public string GetUserName([NotNull] ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            return principal.FindFirst(IdentityOptions.ClaimsIdentity.UserNameClaimType)?.Value;
        }


        /// <summary>
        ///     获取给定用户标识的标识。如果用户标识信息不存在，返回 <c>null</c>。
        /// </summary>
        /// <param name="identity">用户标识对象。</param>
        /// <exception cref="ArgumentNullException"><paramref name="identity" /> 为 <c>null</c>。</exception>
        /// <returns><paramref name="identity" /> 中包含的标识信息。</returns>
        [CanBeNull]
        public string GetUserId([NotNull] ClaimsIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            return identity.FindFirst(IdentityOptions.ClaimsIdentity.UserIdClaimType)?.Value;
        }


        /// <summary>
        ///     获取给定用户主体的标识。如果用户标识信息不存在，返回 <c>null</c>。
        /// </summary>
        /// <param name="principal">用户主体对象。</param>
        /// <exception cref="ArgumentNullException"><paramref name="principal" /> 为 <c>null</c>。</exception>
        /// <returns><paramref name="principal" /> 中包含的标识信息。</returns>
        [CanBeNull]
        public string GetUserId([NotNull] ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            return principal.FindFirst(IdentityOptions.ClaimsIdentity.UserIdClaimType)?.Value;
        }

        /// <summary>
        ///     获取给定用户主体的用户角色列表。
        /// </summary>
        /// <param name="principal">用户主体对象。</param>
        /// <exception cref="ArgumentNullException"><paramref name="principal" /> 为 <c>null</c>。</exception>
        /// <returns><paramref name="principal" /> 中包含的用户角色的集合。</returns>
        public IEnumerable<string> GetUserRoles([NotNull] ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            return principal.FindAll(IdentityOptions.ClaimsIdentity.RoleClaimType).Select(i => i.Value);
        }

        /// <summary>
        ///     获取给定用户标识的用户角色列表。
        /// </summary>
        /// <param name="identity">用户主体对象。</param>
        /// <exception cref="ArgumentNullException"><paramref name="identity" /> 为 <c>null</c>。</exception>
        /// <returns><paramref name="identity" /> 中包含的用户角色的集合。</returns>
        public IEnumerable<string> GetUserRoles([NotNull] ClaimsIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            return identity.FindAll(IdentityOptions.ClaimsIdentity.RoleClaimType).Select(i => i.Value);
        }

        /// <summary>
        ///     获取一个值，指示当前用户主体是否已经登录。
        /// </summary>
        /// <param name="principal">要判断的用户主体对象。</param>
        /// <exception cref="ArgumentNullException"><paramref name="principal" /> 为 <c>null</c>。</exception>
        /// <returns>如果当前用户主体已经登录，返回 true；否则返回 false。</returns>
        public bool IsSignedIn([NotNull] ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            return principal.Identities.Any(
                i => i.AuthenticationType == IdentityOptions.Cookies.ApplicationCookieAuthenticationScheme);
        }
    }
}