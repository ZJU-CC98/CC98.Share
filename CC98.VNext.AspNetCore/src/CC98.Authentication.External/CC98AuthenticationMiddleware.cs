using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CC98.Authentication
{
    /// <summary>
    ///     表示 CC98 身份验证中间件。
    /// </summary>
    public class CC98AuthenticationMiddleware : OAuthMiddleware<CC98AuthenticationOptions>
    {
        public CC98AuthenticationMiddleware(RequestDelegate next, IDataProtectionProvider dataProtectionProvider,
            ILoggerFactory loggerFactory, UrlEncoder encoder, IOptions<SharedAuthenticationOptions> sharedOptions,
            IOptions<CC98AuthenticationOptions> options)
            : base(next, dataProtectionProvider, loggerFactory, encoder, sharedOptions, options)
        {
        }

        /// <summary>
        ///     创建身份验证中间件对应的验证处理程序。
        /// </summary>
        /// <returns>该身份验证中间件对应的处理程序。</returns>
        protected override AuthenticationHandler<CC98AuthenticationOptions> CreateHandler()
        {
            return new CC98AuthenticationHandler(Backchannel);
        }
    }
}