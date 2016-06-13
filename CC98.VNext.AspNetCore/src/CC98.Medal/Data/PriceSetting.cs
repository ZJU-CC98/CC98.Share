using System.ComponentModel.DataAnnotations;

namespace CC98.Medal.Data
{
	/// <summary>
	/// 表示单个定价的设定。
	/// </summary>
	public class PriceSetting
	{
		/// <summary>
		/// 获取或设置要设定的价格。
		/// </summary>
		[Range(0, int.MaxValue)]
		public int Price { get; set; }

		/// <summary>
		/// 获取或设置要设定的有效期（以天为单位）。如果该属性为 null，则表示有效期为无限。
		/// </summary>
		[Range(0, int.MaxValue)]
		public int? ExpiryDate { get; set; }
	}
}