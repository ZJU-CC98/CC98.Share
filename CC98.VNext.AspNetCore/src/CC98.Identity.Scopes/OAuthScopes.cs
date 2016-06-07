using JetBrains.Annotations;

namespace CC98.Identity
{
    /// <summary>
    ///     包含 CC98 OAuth 验证中的合法区域。该类型为静态类型。
    /// </summary>
    [PublicAPI]
    public static class OAuthScopes
    {
        /// <summary>
        ///     允许执行所有操作。该字段为常量。
        /// </summary>
        public const string All = "all";

        /// <summary>
        ///     允许访问基本信息。该字段为常量。
        /// </summary>
        public const string Basic = "basic";

        /// <summary>
        ///     允许读取用户资料。该字段为常量。
        /// </summary>
        public const string GetUserInfo = "getuserinfo";

        /// <summary>
        ///     允许设置用户资料。该字段为常量。
        /// </summary>
        public const string SetUserInfo = "setuserinfo";

        /// <summary>
        ///     允许执行密码相关操作。该字段为常量。
        /// </summary>
        public const string Password = "password";

        /// <summary>
        ///     允许读取用户发言。该字段为常量。
        /// </summary>
        public const string GetPost = "getpost";

        /// <summary>
        ///     允许管理用户发言。该字段为常量。
        /// </summary>
        public const string SetPost = "setpost";

        /// <summary>
        ///     允许读取用户短消息。该字段为常量。
        /// </summary>
        public const string GetMessage = "getmessage";

        /// <summary>
        ///     允许管理用户短消息。该字段为常量。
        /// </summary>
        public const string SetMessage = "setmessage";

        /// <summary>
        ///     允许执行管理操作。该字段为常量。
        /// </summary>
        public const string Manage = "manage";
    }
}