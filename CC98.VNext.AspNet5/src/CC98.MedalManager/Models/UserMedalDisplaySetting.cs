using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.MedalManager.Models
{
	/// <summary>
	/// ��ʾ�û����ض�ѫ�µ���ʾ���á�
	/// </summary>
	[Table("UserMedalDisplaySettingSet")]
	public class UserMedalDisplaySetting
	{
		/// <summary>
		/// ��ȡ�����ù������û��ı�ʶ��
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// ��ȡ�����ù�����ѫ�µı�ʶ��
		/// </summary>
		public int MedalId { get; set; }

		/// <summary>
		/// ��ȡ�����ù�����ѫ�¡�
		/// </summary>
		[ForeignKey(nameof(MedalId))]
		public Medal Medal { get; set; }

		/// <summary>
		/// ��ȡ������һ��ֵ��ָʾ�û��Ƿ����ظ�ѫ�¡�
		/// </summary>
		public bool IsHide { get; set; }

		/// <summary>
		/// ��ȡ�������û���ѫ���趨��Ȩ�ء�
		/// </summary>
		public int SortWeight { get; set; }

	}
}