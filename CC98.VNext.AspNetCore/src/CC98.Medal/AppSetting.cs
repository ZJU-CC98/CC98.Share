using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CC98.Medal
{
	/// <summary>
	/// 表示应用程序设置。
	/// </summary>
	public class AppSetting
	{
		/// <summary>
		/// 获取或设置勋章图片的存储文件夹。
		/// </summary>
		public string MedalImageStoreFolder { get; set; }
		/// <summary>
		/// 获取或设置勋章图片的访问路径。
		/// </summary>
		public string MedalImagePath { get; set; }

	}
}
