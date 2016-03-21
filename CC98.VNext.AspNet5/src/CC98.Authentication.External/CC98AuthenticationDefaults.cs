namespace CC98.Authentication
{
	/// <summary>
	/// CC98 身份验证相关的默认值。该类型为静态类型。
	/// </summary>
	public static class CC98AuthenticationDefaults
	{
		/// <summary>
		/// 获取身份验证方案的名称。该字段为常量。
		/// </summary>
		public const string AuthentcationScheme = "CC98";

		/// <summary>
		/// 获取验证方法的显示名称。该字段为常量。
		/// </summary>
		public const string DisplayName = "CC98";

		/// <summary>
		/// 获取 OAuth Authorize 终结点的地址。该字段为常量。
		/// </summary>
		public const string AuthorizationEndPoint = "https://login.cc98.org/OAuth/Authorize";

		/// <summary>
		/// 获取 OAuth Token 终结点的地址。该字段为常量。
		/// </summary>
		public const string TokenEndPoint = "https://login.cc98.org/OAuth/Token";

		/// <summary>
		/// 获取用于获得用户个人信息的 API 地址。该字段为常量。
		/// </summary>
		public const string UserInformationEndPoint = "https://api.cc98.org/Me/Basic";

		/// <summary>
		/// 中间件的回调路径。该字段为常量。
		/// </summary>
		public const string CallbackPath = "/~signin-cc98";
	}
}
