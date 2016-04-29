using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.MedalManager.Models
{
	/// <summary>
	/// 表示勋章所分类。
	/// </summary>
	[Table("MedalCategorySet")]
	public class MedalCategory
	{
		/// <summary>
		/// 获取或设置分类的标识。
		/// </summary>
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// 获取或设置分类的名称。
		/// </summary>
		[Required]
		public string Name { get; set; }

		/// <summary>
		/// 获取或设置分类相对于同级分类的排序权重。
		/// </summary>
		public int SortWeight { get; set; }

		/// <summary>
		/// 获取或设置父级分类的标识。
		/// </summary>
		public int? ParentId { get; set; }

		/// <summary>
		/// 获取该分类的上级分类。
		/// </summary>
		[ForeignKey(nameof(ParentId))]
		public MedalCategory Parent { get; set; }

		/// <summary>
		/// 获取或设置该分类包含的所有子分类的集合。
		/// </summary>
		[InverseProperty(nameof(Parent))]
		public virtual ICollection<MedalCategory> Children { get; set; } = new Collection<MedalCategory>();

		/// <summary>
		/// 后驱或设置该分类包含的所有勋章的集合。
		/// </summary>
		[InverseProperty(nameof(Medal.Category))]
		public virtual ICollection<Medal> Medals { get; set; } = new Collection<Medal>();
	}
}