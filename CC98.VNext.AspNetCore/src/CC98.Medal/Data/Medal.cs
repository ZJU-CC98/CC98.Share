using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace CC98.Medal.Data
{
	/// <summary>
	/// 表示一个勋章系统的勋章。
	/// </summary>
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
		[StringLength(50)]
		public string Name { get; set; }

		/// <summary>
		/// 获取或设置该勋章的描述。
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// 获取或设置该勋章的链接地址。
		/// </summary>
		[DataType(DataType.Url)]
		public string Link { get; set; }

		/// <summary>
		/// 获取或设置该勋章的图片的地址。
		/// </summary>
		[Required]
		public string ImageUri { get; set; }

		/// <summary>
		/// 获取或设置勋章的价格。这只对可购买的勋章有效。
		/// </summary>
		[Range(0, int.MaxValue)]
		public int? Price { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示勋章是否可以向管理员申请颁发。
		/// </summary>
		public bool CanApply { get; set; }

		/// <summary>
		/// 获取或设置该勋章的定价设置。
		/// </summary>
		[NotMapped]
		public PriceSetting[] PriceSettings
		{
			get
			{
				return string.IsNullOrEmpty(PriceSettingsInternal) ? new PriceSetting[0] : JsonConvert.DeserializeObject<PriceSetting[]>(PriceSettingsInternal);
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				PriceSettingsInternal = JsonConvert.SerializeObject(value);
			}
		}

		/// <summary>
		/// 获取或设置存储定价设置的内部字段。
		/// </summary>
		[Column(nameof(PriceSettings))]
		private string PriceSettingsInternal { get; set; }

		/// <summary>
		/// 获取或设置该勋章所属的分类的标识。
		/// </summary>
		public int? CategoryId { get; set; }

		/// <summary>
		/// 获取或设置该勋章所属的分类。
		/// </summary>
		[ForeignKey(nameof(CategoryId))]
		public MedalCategory Category { get; set; }
	}
}