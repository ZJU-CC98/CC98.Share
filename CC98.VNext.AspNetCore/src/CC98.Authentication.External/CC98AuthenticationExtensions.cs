using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace CC98.Authentication
{
    /// <summary>
    ///     为 CC98 身份验证模块提供辅助方法。
    /// </summary>
    public static class CC98AuthenticationExtensions
    {
        /// <summary>
        ///     默认配置方法。
        /// </summary>
        /// <param name="options">配置参数。</param>
        private static void DefaultConfigureOptions(CC98AuthenticationOptions options)
        {
        }

        /// <summary>
        ///     配置应用程序使用 CC98 账户登录。
        /// </summary>
        /// <param name="app">ASP.NET 应用程序对象。</param>
        /// <param name="options">配置选项。</param>
        public static void UseCC98Authentication(this IApplicationBuilder app, CC98AuthenticationOptions options)
        {
            app.UseMiddleware<CC98AuthenticationMiddleware>(Options.Create(options));
        }
    }
}