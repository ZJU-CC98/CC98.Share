using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace CC98.Share
{
	using System;
	using System.Data.Entity;
	using System.Linq;

	/// <summary>
	/// 表示 CC98 Share 数据库模型对象。
	/// </summary>
	public class CC98ShareModel : DbContext
	{
		/// <summary>
		/// 用默认的连接字符串链接到数据库引擎。
		/// </summary>
		public CC98ShareModel()
			: base("name=CC98ShareModel")
		{
		}

		/// <summary>
		/// 获取或设置数据库中所有上传项目的集合。
		/// </summary>
		[UsedImplicitly(ImplicitUseKindFlags.Assign)]
		public virtual DbSet<ShareItem> Items { get; set; }
	}

	/// <summary>
	/// 表示用户上传的一个项目。
	/// </summary>
	public class ShareItem
	{
		/// <summary>
		/// 获取或设置该项目的标识。
		/// </summary>
		[Key]
		[UsedImplicitly(ImplicitUseKindFlags.Assign)]
		public int Id { get; set; }

		/// <summary>
		/// 获取或设置该项目的名称。
		/// </summary>
		[Required]
		[StringLength(50)]
		public string Name { get; set; }

		/// <summary>
		/// 获取或设置该项目的描述。
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// 获取或设置项目的实际下载地址。
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// 获取或设置该项目的拥有者的用户名。
		/// </summary>
		[Required]
		public string UserName { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示该项目是否被所有者共享。
		/// </summary>
		public bool IsShared { get; set; }

		/// <summary>
		/// 获取或设置该项目的下载次数。
		/// </summary>
		[Range(0, int.MaxValue)]
		public int DownloadCount { get; set; }
	}
}