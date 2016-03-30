using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.Identity.External;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
		/// <summary>
		/// 为应用程序添加 CC98 登录身份验证服务。
		/// </summary>
		/// <param name="services">服务集合对象。</param>
	    public static void AddExternalSignInManager(this IServiceCollection services)
	    {
			services.TryAddScoped<ExternalSignInManager>();
	    }
    }
}
