using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.MedalManager.Models
{
	/// <summary>
	/// ��ʾѫ�������ࡣ
	/// </summary>
	[Table("MedalCategorySet")]
	public class MedalCategory
	{
		/// <summary>
		/// ��ȡ�����÷���ı�ʶ��
		/// </summary>
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// ��ȡ�����÷�������ơ�
		/// </summary>
		[Required]
		public string Name { get; set; }

		/// <summary>
		/// ��ȡ�����÷��������ͬ�����������Ȩ�ء�
		/// </summary>
		public int SortWeight { get; set; }

		/// <summary>
		/// ��ȡ�����ø�������ı�ʶ��
		/// </summary>
		public int? ParentId { get; set; }

		/// <summary>
		/// ��ȡ�÷�����ϼ����ࡣ
		/// </summary>
		[ForeignKey(nameof(ParentId))]
		public MedalCategory Parent { get; set; }

		/// <summary>
		/// ��ȡ�����ø÷�������������ӷ���ļ��ϡ�
		/// </summary>
		[InverseProperty(nameof(Parent))]
		public virtual ICollection<MedalCategory> Children { get; set; } = new Collection<MedalCategory>();

		/// <summary>
		/// ���������ø÷������������ѫ�µļ��ϡ�
		/// </summary>
		[InverseProperty(nameof(Medal.Category))]
		public virtual ICollection<Medal> Medals { get; set; } = new Collection<Medal>();
	}
}