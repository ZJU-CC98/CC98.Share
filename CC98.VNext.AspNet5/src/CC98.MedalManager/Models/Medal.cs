using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.MedalManager.Models
{
	/// <summary>
	/// 表示一个勋章。
	/// </summary>
	[Table("MedalSet")]
	public class Medal
	{
		/// <summary>
		/// 获取或设置该勋章的标识。
		/// </summary>
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// 获取或设置该勋章的名称。
		/// </summary>
		[Required]
		public string Name { get; set; }

		/// <summary>
		/// 获取或设置该勋章的描述。
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// 获取或设置该勋章对应的图片的地址。
		/// </summary>
		[Required]
		[DataType(DataType.ImageUrl)]
		public string ImagePath { get; set; }

		/// <summary>
		/// 获取或设置购买该勋章需要支付的 98 财富值。
		/// </summary>
		[Range(0, int.MaxValue)]
		public int Price { get; set; }

		/// <summary>
		/// 获取或设置勋章的购买类型。
		/// </summary>
		public MedalOrderType OrderType { get; set; }

		/// <summary>
		/// 获取或设置勋章的过期时间，以天为单位。设置为 <c>null</c> 表示勋章不会过期。
		/// </summary>
		[Range(0, int.MaxValue)]
		public int? ExpireDays { get; set; }

		/// <summary>
		/// 获取或设置勋章相关的链接地址。
		/// </summary>
		[DataType(DataType.Url)]
		public string LinkUri { get; set; }

		/// <summary>
		/// 获取或设置勋章所属分类的标识。
		/// </summary>
		public int? CategoryId { get; }

		/// <summary>
		/// 获取或设置勋章所属的分类。
		/// </summary>
		[ForeignKey(nameof(CategoryId))]
		public MedalCategory Category { get; set; }

		/// <summary>
		/// 获取或设置勋章在当前分类下的默认排序权重。
		/// </summary>
		public int SortWeight { get; set; }

		/// <summary>
		/// 获取该勋章相关的颁发记录。
		/// </summary>
		[InverseProperty(nameof(UserMedalIssue.Medal))]
		public virtual ICollection<UserMedalIssue> Issues { get; set; } = new Collection<UserMedalIssue>();

		/// <summary>
		/// 获取该勋章相关的显示设置。
		/// </summary>
		[InverseProperty(nameof(UserMedalDisplaySetting.Medal))]
		public virtual ICollection<UserMedalDisplaySetting> DisplaySettings { get; set; } = new Collection<UserMedalDisplaySetting>();
	}
}