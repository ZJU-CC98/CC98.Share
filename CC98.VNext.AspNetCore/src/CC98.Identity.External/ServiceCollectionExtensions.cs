using System;
using CC98.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     为应用程序添加 CC98 登录身份验证服务。
        /// </summary>
        /// <param name="services">服务集合对象。</param>
        /// <param name="setupOptions">应用程序标识相关的选项的配置方法。</param>
        public static void AddExternalSignInManager(this IServiceCollection services,
            Action<IdentityOptions> setupOptions = null)
        {
            services.TryAddSingleton<ISecurityStampValidator, ExternalSecurityStampValidator>();
            services.TryAddScoped<ExternalSignInManager>();

            if (setupOptions != null)
            {
                services.Configure(setupOptions);
            }
        }
    }
}