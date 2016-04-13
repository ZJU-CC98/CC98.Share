using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CC98.LogOn.Services
{
	/// <summary>
	/// 表示浙大通行证登录相关的设置。
	/// </summary>
    public class ZjuSsoOptions
    {
		/// <summary>
		/// 浙大通行证应用标识。
		/// </summary>
		public string AppId { get; set; }

		/// <summary>
		/// 浙大通行证应用密码。
		/// </summary>
		public string AppPassword { get; set; }
    }
}
