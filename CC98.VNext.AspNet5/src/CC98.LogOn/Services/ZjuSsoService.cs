using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CC98.Identity;
using CC98.LogOn.Services;
using Microsoft.Extensions.OptionsModel;

namespace CC98.LogOn
{
	/// <summary>
	/// 提供浙大通行证登录服务。
	/// </summary>
	public class ZjuSsoService
	{
		/// <summary>
		/// 获取浙大通行证服务的相关选项。
		/// </summary>
		private ZjuSsoOptions Options { get; }

		public ZjuSsoService(IOptions<ZjuSsoOptions> options)
		{
			Options = options.Value;
		}

		/// <summary>
		/// 定义浙大通行证的授权令牌声明类型。该字段为常量。
		/// </summary>
		public const string TokenClaimType = "https://schema.cc98.org/v2/2015/10/Identity/Claims/ZjuSsoToken";

		/// <summary>
		/// 定义浙大通行证的登录类型。该字段为常量。
		/// </summary>
		public const string AuthenticationType = "ZJUSSO";

		/// <summary>
		/// 表示执行登录操作时使用的 URL 地址。该字段为常量。
		/// </summary>
		private const string LogOnUri = "http://zuinfo.zju.edu.cn:8080/AMWebService/Session";

		/// <summary>
		/// 使用给定的用户名和密码，进行浙大统一身份认证登录，并获得登录令牌。
		/// </summary>
		/// <param name="userName">要登录的浙大统一身份认证用户名。</param>
		/// <param name="password">要登录的浙大统一身份认证密码。</param>
		/// <returns>如果登录成功，返回可用于访问用户信息的授权令牌；否则返回 <c>null</c>。</returns>
		public async Task<string> LogOnAsync(string userName, string password)
		{
			var client = new HttpClient();

			var paramDic = new Dictionary<string, string>
			{
				["name"] = userName,
				["password"] = password,
				["module"] = "DataStore",
				["type"] = "1",
				["appUid"] = Options.AppId,
				["appPwd"] = Options.AppPassword
			};

			var message = new HttpRequestMessage(HttpMethod.Post, LogOnUri);

			foreach (var item in paramDic)
			{
				message.Headers.Add(item.Key, item.Value);
			}

			var response = await client.SendAsync(message);

			if (response.IsSuccessStatusCode)
			{
				return response.Headers.GetValue("iPlanetDirectoryPro").ReplaceEmptyWithNull();
			}

			return null;

			//string name = Request.Params["name"];
			//string pwd = Request.Params["password"];
			//HttpWebRequest webRequest2 = (HttpWebRequest)WebRequest.Create(SESSIONURL);
			//webRequest2.Headers.Add("name", name);
			//webRequest2.Headers.Add("password", pwd);
			//webRequest2.Headers.Add("module", "DataStore");
			//webRequest2.Headers.Add("type", "1");
			//webRequest2.Headers.Add("appUid", appUid);
			//webRequest2.Headers.Add("appPwd", appPwd);
			//webRequest2.Method = "POST";
			//webRequest2.ContentType = "application/x-www-form-urlencoded";
			//HttpWebResponse webResponse2 = (HttpWebResponse)webRequest2.GetResponse();
			//ssoid = webResponse2.Headers["iPlanetDirectoryPro"];
			////Label_message.Text += "ssoid-1:" + ssoid + "<br />";
			//ssoid = System.Web.HttpUtility.UrlEncode(ssoid);
			////Label_message.Text += "ssoid-2:" + ssoid + "<br />";
			//webResponse2.Close();
			//HttpCookie sso_cookie = new HttpCookie("iPlanetDirectoryPro", ssoid);
			//sso_cookie.Path = "/";
			//sso_cookie.Domain = ".zju.edu.cn";
			//Response.AppendCookie(sso_cookie);
		}

		/// <summary>
		/// 通过访问令牌获取对应的用户标识。
		/// </summary>
		/// <param name="token">访问令牌字符串。</param>
		/// <returns>访问令牌代表的用户名。如果访问令牌无效，则返回 <c>null</c>。</returns>
		public async Task<string> GetUserIdAsync(string token)
		{
			if (token == null)
			{
				throw new ArgumentNullException(nameof(token));
			}

			var client = new HttpClient();

			var message = new HttpRequestMessage(HttpMethod.Get, LogOnUri);

			var paramDic = new Dictionary<string, string>
			{
				["iPlanetDirectoryPro"] = token,
				["appUid"] = Options.AppId,
				["appPwd"] = Options.AppPassword
			};

			foreach (var item in paramDic)
			{
				message.Headers.Add(item.Key, item.Value);
			}

			var response = await client.SendAsync(message);

			if (response.IsSuccessStatusCode)
			{
				return response.Headers.GetValue("ZJU_SSO_UID").ReplaceEmptyWithNull();
			}

			return null;

			//string uid = null;
			//if (token == null || token.Length <= 0)
			//{
			//	return null;
			//}
			//try
			//{
			//	HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(SESSIONURL);
			//	webRequest.Method = "GET";
			//	webRequest.Headers.Add("iPlanetDirectoryPro", token);
			//	webRequest.Headers.Add("appUid", appUid);
			//	webRequest.Headers.Add("appPwd", appPwd);
			//	HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
			//	uid = webResponse.Headers["ZJU_SSO_UID"];
			//	webResponse.Close();
			//	//Label_message.Text += "uid:" + uid + "<br />";
			//}
			//catch (Exception e)
			//{
			//	Console.WriteLine(e);
			//	//Label_message.Text += "登陆异常:" + e.Message + "<br />";
			//}
			//return uid;
		}

		/// <summary>
		/// 使用给定的用户名和密码，进行浙大统一身份认证登录，并获得用户标识。这是一个快捷方法。
		/// </summary>
		/// <param name="userName">要登录的浙大统一身份认证用户名。</param>
		/// <param name="password">要登录的浙大统一身份认证密码。</param>
		/// <returns>如果登录成功，返回用户标识字符串；否则返回 <c>null</c>。</returns>
		public async Task<string> LogOnGetIdAsync(string userName, string password)
		{
			var token = await LogOnAsync(userName, password);

			if (token == null)
			{
				return null;
			}

			return await GetUserIdAsync(token);
		}

		/// <summary>
		/// 使用给定的浙大通行证用户名和密码登录，并返回用户标识对象。
		/// </summary>
		/// <param name="userName">要登录的浙大统一身份认证用户名。</param>
		/// <param name="password">要登录的浙大统一身份认证密码。</param>
		/// <returns>如果登录成功，返回用户标识对象；否则返回 <c>null</c>。</returns>
		public async Task<ClaimsIdentity> LogOnGetIdentityAsync(string userName, string password)
		{
			var token = await LogOnAsync(userName, password);

			if (token == null)
			{
				return null;
			}

			var userId = await GetUserIdAsync(token);

			if (userId == null)
			{
				return null;
			}

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, userName, ClaimValueTypes.String),
				new Claim(ClaimTypes.NameIdentifier, userId, ClaimValueTypes.String),
				new Claim(TokenClaimType, token, ClaimValueTypes.String),
				new Claim(IdentityHelper.IdentityProviderClaimType, AuthenticationType,ClaimValueTypes.String),
			};

			return new ClaimsIdentity(claims, AuthenticationType);
		}
	}
}
