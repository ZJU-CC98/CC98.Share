using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace CC98.Share.Data
{
	/// <summary>
	///     表示 CC98 Share 数据库模型对象。
	/// </summary>
	public class CC98ShareModel : DbContext
	{
		/// <summary>
		///     创建一个新的数据库模型对象。
		/// </summary>
		/// <param name="options">数据库上下文选项。</param>
		public CC98ShareModel(DbContextOptions options)
			: base(options)
		{
		}

		/// <summary>
		///     Override this method to further configure the model that was discovered by convention from the entity types
		///     exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
		///     and re-used for subsequent instances of your derived context.
		/// </summary>
		/// <remarks>
		///     If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
		///     then this method will not be run.
		/// </remarks>
		/// <param name="modelBuilder">
		///     The builder being used to construct the model for this context. Databases (and other extensions) typically
		///     define extension methods on this object that allow you to configure aspects of the model that are specific
		///     to a given database.
		/// </param>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<ShareItem>().HasIndex(i => new { i.UserName, i.Size });
		}

		/// <summary>
		///     获取或设置数据库中所有上传项目的集合。
		/// </summary>
		[UsedImplicitly(ImplicitUseKindFlags.Assign)]
		public virtual DbSet<ShareItem> Items { get; set; }
	}
}