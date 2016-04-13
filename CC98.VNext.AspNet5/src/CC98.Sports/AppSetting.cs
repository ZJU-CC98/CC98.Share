namespace CC98.Sports
{

	/// <summary>
	/// 表示应用程序设置。无法继承该类型。
	/// </summary>
	public sealed class AppSetting
	{
		/// <summary>
		/// 获取或设置网站的标题。
		/// </summary>
		public string SiteTitle { get; set; }

		/// <summary>
		/// 获取或设置上传的根目录。
		/// </summary>
		public string UploadRootFolder { get; set; }

		/// <summary>
		/// 或取或设置上传的物理目录。
		/// </summary>
		public string UploadPhysicalPath { get; set; }
	}
}
