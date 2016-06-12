using System.Collections;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace CC98.Medal.Data
{
	/// <summary>
	/// 为勋章系统提供数据库模型。
	/// </summary>
	public class MedalDataModel : DbContext
	{
		#region 构造方法

		/// <summary>
		/// 初始化一个数据连接的新实例。
		/// </summary>
		[UsedImplicitly(ImplicitUseKindFlags.Access)]
		public MedalDataModel()
		{

		}

		/// <summary>
		/// 用指定的选项初始化一个数据连接的新实例。
		/// </summary>
		/// <param name="options">数据库链接选项。</param>
		[UsedImplicitly(ImplicitUseKindFlags.Access)]
		public MedalDataModel(DbContextOptions<MedalDataModel> options) : base(options)
		{

		}

		#endregion

		#region 配置

		/// <summary>
		///     Override this method to further configure the model that was discovered by convention from the entity types
		///     exposed in <see cref="Microsoft.EntityFrameworkCore.DbSet{TEntity}" /> properties on your derived context. The resulting model may be cached
		///     and re-used for subsequent instances of your derived context.
		/// </summary>
		/// <remarks>
		///     If a model is explicitly set on the options for this context (via <see cref="DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
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
			modelBuilder.Entity<UserMedalIssue>().HasKey(i => new { i.UserId, i.MedalId });
		}

		#endregion

		#region 数据表

		/// <summary>
		/// 获取或设置数据库中包含的勋章的集合。
		/// </summary>
		public virtual DbSet<Medal> Medals { get; set; }

		/// <summary>
		/// 获取或设置数据库中包含的用户勋章颁发记录的集合。
		/// </summary>
		public virtual DbSet<UserMedalIssue> UserMedalIssues { get; set; }

		/// <summary>
		/// 获取或设置数据库中包含的勋章分类的集合。
		/// </summary>
		public virtual DbSet<MedalCategory> MedalCategories { get; set; }

		#endregion
	}
}
