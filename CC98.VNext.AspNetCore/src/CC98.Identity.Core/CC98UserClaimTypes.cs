namespace CC98.Identity
{
    /// <summary>
    ///     定义 CC98 用户相关的用户标识属性。该类型为静态类型。
    /// </summary>
    public static class CC98UserClaimTypes
    {
        /// <summary>
        ///     CC98 V1 版本用户标识。
        /// </summary>
        public const string OldId = "http://schemas.cc98.org/ws/2014/06/identity/claims/oldid";

        /// <summary>
        ///     访问令牌标识。
        /// </summary>
        public const string AccessToken = "http://schemas.cc98.org/ws/2015/01/identity/claims/accesstoken";

        /// <summary>
        /// 用户头像标识。
        /// </summary>
        public const string PortraitUri = "http://schemas.cc98.org/ws/2014/06/identity/claims/portraitUri";
    }
}