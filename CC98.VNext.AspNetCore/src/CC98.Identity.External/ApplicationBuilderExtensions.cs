using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace

namespace Microsoft.AspNet.Builder
{
    /// <summary>
    ///     为 <seealso cref="IApplicationBuilder" /> 提供扩展方法。该类型为静态类型。
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        ///     配置应用程序使用应用程序和外部 Cookie 身份验证。
        /// </summary>
        /// <param name="app"></param>
        public static void UseAllCookies(this IApplicationBuilder app)
        {
            // 获得当前的标识身份验证设置。
            var identityOptions = app.ApplicationServices.GetService<IOptions<IdentityOptions>>().Value;

            // 配置 Cookie 身份验证，注意顺序
            app.UseCookieAuthentication(identityOptions.Cookies.ExternalCookie);
            app.UseCookieAuthentication(identityOptions.Cookies.TwoFactorRememberMeCookie);
            app.UseCookieAuthentication(identityOptions.Cookies.TwoFactorUserIdCookie);
            app.UseCookieAuthentication(identityOptions.Cookies.ApplicationCookie);
        }
    }
}