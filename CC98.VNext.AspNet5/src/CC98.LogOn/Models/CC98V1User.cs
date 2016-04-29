using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.LogOn.Models
{
	/// <summary>
	/// ��ʾ CC98 V1 �汾�û���
	/// </summary>
	[Table("user")]
	public class CC98V1User
	{
		/// <summary>
		/// ��ȡ�����ø��û��ı�ʶ��
		/// </summary>
		[Key]
		[Column("UserId")]
		public int Id { get; set; }

		/// <summary>
		/// ��ȡ�����ø��û������ơ�
		/// </summary>
		[Required]
		[Column("UserName")]
		[StringLength(50)]
		public string Name { get; set; }

		/// <summary>
		/// ��ȡ�����ø��û��������ɢ�С�
		/// </summary>
		[Required]
		[Column("UserPassword")]
		[StringLength(20)]
		public string PasswordHash { get; set; }

		/// <summary>
		/// ��ȡ�����ø��û��󶨵����ͨ��֤�˻���
		/// </summary>
		[Column("regmail")]
		[StringLength(30)]
		public string ZjuSsoId { get; set; }

		/// <summary>
		/// ��ȡ������һ��ֵ��ָʾ��ǰ�û��Ƿ����ߡ�
		/// </summary>
		[Column("online")]
		public bool IsOnline { get; set; }

		/// <summary>
		/// ��ȡ������һ��ֵ��ָʾ��ǰ�û��Ƿ��Ѿ�ͨ����֤��
		/// </summary>
		[Column("verified")]
		public bool IsVerified { get; set; }

		/// <summary>
		/// ��ȡ�����ø��û���ע��ʱ�䡣
		/// </summary>
		[Column("addDate")]
		public DateTime RegisterTime { get; set; }

		/// <summary>
		/// ��ȡ�������û����ϴε�¼ʱ�䡣
		/// </summary>
		[Column("lastlogin")]
		public DateTime LastLogOnTime { get; set; }

		/// <summary>
		/// ��ȡ�������û��ϴε�¼�� IP ��ַ��
		/// </summary>
		[Column("UserLastIP")]
		[StringLength(15)]
		public string LastLogOnIP { get; set; }

		/// <summary>
		/// ��ȡ������һ��ֵ��ָʾ�û��Ƿ��Ǳ����û���
		/// </summary>
		[Column("reserved")]
		public bool IsReserved { get; set; }

		/// <summary>
		/// ��ȡ�������û���״̬��
		/// </summary>
		[Column("lockuser")]
		public UserState State { get; set; }

		/// <summary>
		/// ��ȡ�������û��ĵȼ���
		/// </summary>
		[Column("userclass")]
		[StringLength(20)]
		public string Level { get; set; }

		/// <summary>
		/// ��ȡ�������û��ķ���������
		/// </summary>
		[Column("Article")]
		public int TotalPosts { get; set; }

		/// <summary>
		/// ��ȡ�������û��ĵ�¼������
		/// </summary>
		[Column("logins")]
		public int LogOnTimes { get; set; }

		/// <summary>
		/// ��ȡ�������û����Ա�
		/// </summary>
		[Column("sex")]
		public Gender Gender { get; set; }
	}
}