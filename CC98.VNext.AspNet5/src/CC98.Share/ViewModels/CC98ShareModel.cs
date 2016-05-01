using JetBrains.Annotations;
using Microsoft.Data.Entity;

namespace CC98.Share.ViewModels
{
	/// <summary>
	/// 表示 CC98 Share 数据库模型对象。
	/// </summary>
	public class CC98ShareModel : DbContext
	{
		/// <summary>
		/// 用默认的连接字符串链接到数据库引擎。
		/// </summary>
		public CC98ShareModel()
		{
		}

		/// <summary>
		/// 获取或设置数据库中所有上传项目的集合。
		/// </summary>
		[UsedImplicitly(ImplicitUseKindFlags.Assign)]
		public virtual DbSet<ShareItem> Items { get; set; }
	}
}