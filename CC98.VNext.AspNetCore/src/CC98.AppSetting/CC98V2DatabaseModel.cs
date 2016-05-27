using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CC98
{
	/// <summary>
	/// 定义 CC98 V2 数据库结构。
	/// </summary>
	public class CC98V2DatabaseModel : DbContext
	{
		/// <summary>
		/// 获取或设置数据库中包含的应用的信息。
		/// </summary>
		public virtual DbSet<AppSetting> AppSettings { get; set; }
	}

	/// <summary>
	/// 定义一个应用程序设置。
	/// </summary>
	[Table("AppSettingSet")]
	public class AppSetting
	{
		/// <summary>
		/// 获取或设置设置的标识。
		/// </summary>
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// 获取或设置应用的名称。
		/// </summary>
		[StringLength(50)]
		[Required]
		public string AppName { get; set; }

		/// <summary>
		/// 获取或设置数据的格式。
		/// </summary>
		[StringLength(50)]
		public string Format { get; set; }

		/// <summary>
		/// 获取或设置应用的数据。
		/// </summary>
		public byte[] Data { get; set; }
	}
}
