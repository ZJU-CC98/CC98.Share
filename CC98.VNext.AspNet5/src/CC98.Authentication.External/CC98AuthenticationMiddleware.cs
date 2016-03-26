using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Authentication.OAuth;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.WebEncoders;

namespace CC98.Authentication
{
	/// <summary>
	/// 表示 CC98 身份验证中间件。
	/// </summary>
	public class CC98AuthenticationMiddleware : OAuthMiddleware<CC98AuthenticationOptions>
	{
		public CC98AuthenticationMiddleware(RequestDelegate next, IDataProtectionProvider dataProtectionProvider, ILoggerFactory loggerFactory, IUrlEncoder encoder, IOptions<SharedAuthenticationOptions> sharedOptions, CC98AuthenticationOptions options)
			: base(next, dataProtectionProvider, loggerFactory, encoder, sharedOptions, options)
		{
		}

		/// <summary>
		/// 创建身份验证中间件对应的验证处理程序。
		/// </summary>
		/// <returns>该身份验证中间件对应的处理程序。</returns>
		protected override AuthenticationHandler<CC98AuthenticationOptions> CreateHandler()
		{
			return new CC98AuthenticationHandler(Backchannel);
		}
	}
}
