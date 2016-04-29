using Microsoft.Data.Entity;

namespace CC98.LogOn.Models
{
	/// <summary>
	/// CC98 V2 数据库模型。
	/// </summary>
	public class CC98V2DatabaseModel : DbContext
	{
		public CC98V2DatabaseModel()
		{
		}

		//为您要在模型中包含的每种实体类型都添加 DbSet。有关配置和使用 Code First  模型
		//的详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=390109。

		// public virtual DbSet<MyEntity> MyEntities { get; set; }

		/// <summary>
		/// 数据库中包含的所有应用的集合。
		/// </summary>
		public virtual DbSet<App> Apps { get; set; }

		/// <summary>
		/// 用户表。
		/// </summary>
		/// <returns></returns>
		public virtual DbSet<IdentityUser> Users { get; set; }

		/// <summary>
		///     Override this method to further configure the model that was discovered by convention from the entity types
		///     exposed in <see cref="T:Microsoft.Data.Entity.DbSet`1" /> properties on your derived context. The resulting model may be cached
		///     and re-used for subsequent instances of your derived context.
		/// </summary>
		/// <remarks>
		///     If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.Data.Entity.DbContextOptionsBuilder.UseModel(Microsoft.Data.Entity.Metadata.IModel)" />)
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
			modelBuilder.Entity<App>().HasAlternateKey(i => i.Name);
		}
	}
}