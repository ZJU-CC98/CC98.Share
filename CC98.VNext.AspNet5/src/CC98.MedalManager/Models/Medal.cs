using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.MedalManager.Models
{
	/// <summary>
	/// ��ʾһ��ѫ�¡�
	/// </summary>
	[Table("MedalSet")]
	public class Medal
	{
		/// <summary>
		/// ��ȡ�����ø�ѫ�µı�ʶ��
		/// </summary>
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// ��ȡ�����ø�ѫ�µ����ơ�
		/// </summary>
		[Required]
		public string Name { get; set; }

		/// <summary>
		/// ��ȡ�����ø�ѫ�µ�������
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// ��ȡ�����ø�ѫ�¶�Ӧ��ͼƬ�ĵ�ַ��
		/// </summary>
		[Required]
		[DataType(DataType.ImageUrl)]
		public string ImagePath { get; set; }

		/// <summary>
		/// ��ȡ�����ù����ѫ����Ҫ֧���� 98 �Ƹ�ֵ��
		/// </summary>
		[Range(0, int.MaxValue)]
		public int Price { get; set; }

		/// <summary>
		/// ��ȡ������ѫ�µĹ������͡�
		/// </summary>
		public MedalOrderType OrderType { get; set; }

		/// <summary>
		/// ��ȡ������ѫ�µĹ���ʱ�䣬����Ϊ��λ������Ϊ <c>null</c> ��ʾѫ�²�����ڡ�
		/// </summary>
		[Range(0, int.MaxValue)]
		public int? ExpireDays { get; set; }

		/// <summary>
		/// ��ȡ������ѫ����ص����ӵ�ַ��
		/// </summary>
		[DataType(DataType.Url)]
		public string LinkUri { get; set; }

		/// <summary>
		/// ��ȡ������ѫ����������ı�ʶ��
		/// </summary>
		public int? CategoryId { get; }

		/// <summary>
		/// ��ȡ������ѫ�������ķ��ࡣ
		/// </summary>
		[ForeignKey(nameof(CategoryId))]
		public MedalCategory Category { get; set; }

		/// <summary>
		/// ��ȡ������ѫ���ڵ�ǰ�����µ�Ĭ������Ȩ�ء�
		/// </summary>
		public int SortWeight { get; set; }

		/// <summary>
		/// ��ȡ��ѫ����صİ䷢��¼��
		/// </summary>
		[InverseProperty(nameof(UserMedalIssue.Medal))]
		public virtual ICollection<UserMedalIssue> Issues { get; set; } = new Collection<UserMedalIssue>();

		/// <summary>
		/// ��ȡ��ѫ����ص���ʾ���á�
		/// </summary>
		[InverseProperty(nameof(UserMedalDisplaySetting.Medal))]
		public virtual ICollection<UserMedalDisplaySetting> DisplaySettings { get; set; } = new Collection<UserMedalDisplaySetting>();
	}
}