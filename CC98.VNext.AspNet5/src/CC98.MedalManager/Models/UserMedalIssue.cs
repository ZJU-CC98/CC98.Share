using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.MedalManager.Models
{
	/// <summary>
	/// ��ʾ�û���ѫ�µİ䷢��ϵ��
	/// </summary>
	[Table("UserMedalIssueSet")]
	public class UserMedalIssue
	{
		/// <summary>
		/// ��ȡ�����ù������û��ı�ʶ��
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// ��ȡ�����ñ��䷢��ѫ�µı�ʶ��
		/// </summary>
		public int MedalId { get; set; }

		/// <summary>
		/// ��ȡ�����ñ��䷢��ѫ�¡�
		/// </summary>
		[ForeignKey(nameof(MedalId))]
		public Medal Medal { get; set; }

		/// <summary>
		/// ��ȡ������һ��ֵ��ָʾѫ�°䷢���������״̬��
		/// </summary>
		public bool IsUnderReview { get; set; }

		/// <summary>
		/// ��ȡ�����ð䷢��ʱ�䡣
		/// </summary>
		public DateTime Time { get; set; }

		/// <summary>
		/// ��ȡ�����ñ��ΰ䷢�Ĺ���ʱ�䣬����Ϊ��λ������Ϊ <c>null</c> ��ʾ������ڡ�
		/// </summary>
		[Range(0, int.MaxValue)]
		public int? ExpireDays { get; set; }
	}
}