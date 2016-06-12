using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.Medal.Data
{
	/// <summary>
	/// 表示勋章的分类。
	/// </summary>
	public class MedalCategory
	{
		/// <summary>
		/// 获取或设置该分类的标识。
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// 获取或设置该分类的名称。
		/// </summary>
		[Required]
		[StringLength(50)]
		public string Name { get; set; }

		/// <summary>
		/// 获取或设置该分类下的勋章的集合。
		/// </summary>
		[InverseProperty(nameof(Medal.Category))]
		public virtual ICollection<Medal> Medals { get; set; } = new Collection<Medal>();
	}
}