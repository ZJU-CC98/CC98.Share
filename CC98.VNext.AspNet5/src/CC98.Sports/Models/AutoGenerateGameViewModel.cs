using System.ComponentModel.DataAnnotations;

namespace CC98.Sports.Models
{
	/// <summary>
	/// 为自动生成比赛的操作提供视图模型。
	/// </summary>
	public class AutoGenerateGameViewModel
    {
		/// <summary>
		/// 获取或设置赛事的标识。
		/// </summary>
		[Display(Name = "赛事编号")]
		public int EventId { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示是否生成小组赛。
		/// </summary>
		[Display(Name = "生成小组赛")]
		public bool GenerateGroupMatches { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示是否生成淘汰赛。
		/// </summary>
		[Display(Name = "生成淘汰赛")]
		public bool GenerateKnockoutMatches { get; set; }

		/// <summary>
		/// 获取或设置小组赛的标题格式。
		/// </summary>
		[Display(Name = "小组赛标题格式")]
		public string GroupMatchTitleFormat { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示小组赛是否使用双向模式。
		/// </summary>
		[Display(Name = "双向小组赛")]
		public bool UseDuplexGroupMatch { get; set; }

		/// <summary>
		/// 获取或设置淘汰赛的标题格式。
		/// </summary>
		[Display(Name = "淘汰赛标题格式")]
		public string KnockoutMatchTitleFormat { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示淘汰赛是否使用双向模式。
		/// </summary>
		[Display(Name = "双向淘汰赛")]
		public bool UseDuplexKnockoutMatch { get; set; }

		/// <summary>
		/// 获取或设置淘汰赛的轮次。
		/// </summary>
		[Display(Name = "淘汰赛轮次")]
		[Range(1, int.MaxValue)]
		public int? KnockoutRoundCount { get; set; }

		/// <summary>
		/// 获取或设置败者组淘汰赛的轮次。
		/// </summary>
		[Display(Name = "败者组淘汰赛轮次")]
		[Range(0, int.MaxValue)]
		public int? LoserKnockoutRountCount { get; set; }
    }
}
