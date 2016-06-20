using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace CC98.Authentication
{
    /// <summary>
    ///     表示 CC98 身份验证的选项。
    /// </summary>
    public class CC98AuthenticationOptions : OAuthOptions
    {
        /// <summary>
        ///     使用默认值初始化一个项目的新实例。
        /// </summary>
        public CC98AuthenticationOptions()
        {
            // 验证方案名称
            AuthenticationScheme = CC98AuthenticationDefaults.AuthentcationScheme;

            // 终结点地址
            AuthorizationEndpoint = CC98AuthenticationDefaults.AuthorizationEndPoint;
            TokenEndpoint = CC98AuthenticationDefaults.TokenEndPoint;
            UserInformationEndpoint = CC98AuthenticationDefaults.UserInformationEndPoint;

            // 回调地址
            CallbackPath = new PathString(CC98AuthenticationDefaults.CallbackPath);

            // 默认标题
            DisplayName = CC98AuthenticationDefaults.DisplayName;
        }

        /// <summary>
        ///     获取或设置一个值，指示是否允许使用不安全的 HTTP 协议进行验证。该选项默认值为 false。
        /// </summary>
        public bool AllowInsecureHttp { get; set; } = false;
    }
}