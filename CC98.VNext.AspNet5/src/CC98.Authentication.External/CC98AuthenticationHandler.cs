using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using CC98.Identity;
using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Authentication.OAuth;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CC98.Authentication
{
	/// <summary>
	/// 表示 CC98 身份验证处理程序。
	/// </summary>
	internal class CC98AuthenticationHandler : OAuthHandler<CC98AuthenticationOptions>
	{
		/// <summary>
		/// 初始化一个 CC98 身份验证处理程序的新实例。
		/// </summary>
		/// <param name="backChannel">处理程序使用的后台 HTTP 通讯组件。</param>
		public CC98AuthenticationHandler(HttpClient backChannel)
			: base(backChannel)
		{

		}

		protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
		{
			try
			{
				// 消息内容
				var message = new HttpRequestMessage(HttpMethod.Get, Options.UserInformationEndpoint);

				// Authorization 标头
				message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(tokens.TokenType, tokens.AccessToken);

				// 发送 GET 请求
				var response = await Backchannel.SendAsync(message);

				// 响应状态查询
				if (!response.IsSuccessStatusCode)
				{
					Logger.LogError("CC98 Identity 框架无法加载用户个人信息，HTTPClient 返回代码：{0}", response.StatusCode);
					return null;
				}

				// 获得内容
				var content = await response.Content.ReadAsStringAsync();

				try
				{
					var userInfo = JsonConvert.DeserializeObject<CC98User>(content);

					// 基本信息
					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.NameIdentifier, userInfo.V2Id.ToString("D", CultureInfo.InvariantCulture), ClaimValueTypes.Integer, Options.ClaimsIssuer),
						new Claim(CC98UserClaimTypes.OldId, userInfo.V1Id.ToString("D", CultureInfo.InvariantCulture), ClaimValueTypes.Integer, Options.ClaimsIssuer),
						new Claim(ClaimTypes.Name, userInfo.Name, ClaimValueTypes.String, Options.ClaimsIssuer)
					};
					claims.AddRange(userInfo.Roles.Select(i => new Claim(ClaimTypes.Role, i, ClaimValueTypes.String, Options.ClaimsIssuer)));

					// 权限

					// 访问令牌
					if (Options.SaveTokensAsClaims)
					{
						claims.Add(new Claim(CC98UserClaimTypes.AccessToken, tokens.AccessToken, ClaimValueTypes.String, Options.ClaimsIssuer));
					}

					// IdentityProvider
					claims.Add(new Claim(IdentityHelper.IdentityProviderClaimType, Options.AuthenticationScheme, ClaimValueTypes.String, Options.ClaimsIssuer));

					// 添加所有生命对象
					identity.AddClaims(claims);

					// 主体对象
					var principal = new ClaimsPrincipal(identity);

					// 返回结果
					return new AuthenticationTicket(principal, properties, Options.AuthenticationScheme);

				}
				catch (Exception ex)
				{
					Logger.LogError("CC98 Identity 框架无法加载用户个人信息。HTTPClient 响应内容无法解析为 JSON 对象。错误消息：{0}。实际响应字符串：{1}", ex.Message, content);
				}

			}
			catch (Exception ex)
			{
				Logger.LogError("CC98 Identity 框架无法加载用户个人信息，HTTPClient GET 方法返回错误：{0}", ex.Message);
			}

			return null;
		}

	}
}
