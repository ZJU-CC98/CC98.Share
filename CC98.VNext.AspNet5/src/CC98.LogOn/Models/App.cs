using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.LogOn.Models
{
	/// <summary>
	/// ��ʾһ��Ӧ��
	/// </summary>
	[Table("Apps")]
	public class App
	{
		/// <summary>
		/// ��Ŀ�� ID��
		/// </summary>
		/// <returns></returns>
		[Required]
		[Key]
		public Guid Id { get; set; }

		/// <summary>
		/// ��Ŀ�� Secret��
		/// </summary>
		/// <returns></returns>
		[Required]
		public Guid Secret { get; set; }

		/// <summary>
		/// Ӧ�õ����ơ�
		/// </summary>
		/// <returns></returns>
		[Required]
		[Display(Name = "Ӧ������", ShortName = "����")]
		public string Name { get; set; }

		/// <summary>
		/// Ӧ�õ�������
		/// </summary>
		/// <returns></returns>
		public string Description { get; set; }


		/// <summary>
		/// Ӧ�õĻص� URL��
		/// </summary>
		[DataType(DataType.Url)]
		public string RedirectUri { get; set; }

		/// <summary>
		/// ��ȡ��Ӧ�õ����״̬��
		/// </summary>
		public AppAuditState AuditState { get; set; }

		/// <summary>
		/// ��Ŀ����ҳ URL��
		/// </summary>
		/// <returns></returns>
		[DataType(DataType.Url)]
		public string HomePageUri { get; set; }

		/// <summary>
		/// ��Ŀ�� LOGO URL��
		/// </summary>
		/// <returns></returns>
		[DataType(DataType.ImageUrl)]
		public string LogoUri { get; set; }

		/// <summary>
		/// ָʾ�Ƿ��������οͻ��ˡ�
		/// </summary>
		/// <returns></returns>
		public bool IsTrusted { get; set; }

		/// <summary>
		/// �Ƿ������ú��Ŀͻ��ˡ�
		/// </summary>
		/// <returns></returns>
		public bool IsBuiltIn { get; set; }

		/// <summary>
		/// ����ʱ�䡣
		/// </summary>
		/// <returns></returns>
		public DateTime CreateTime { get; set; }

		/// <summary>
		/// ����ʱ�䡣
		/// </summary>
		/// <returns></returns>
		public DateTime? ExpireTime { get; set; }

		/// <summary>
		/// Ӧ����ص�ѡ�
		/// </summary>
		/// <returns></returns>
		//public AppOptions Options { get; set; } = new AppOptions();

		/// <summary>
		/// ������Ա�ĵ����ʼ���ַ��
		/// </summary>
		/// <returns></returns>
		[EmailAddress]
		[DataType(DataType.EmailAddress)]
		public string ManageEmailAddress { get; set; }

		/// <summary>
		/// �Ƿ�Ҫ��ʾ��Ӧ���б��ϡ�
		/// </summary>
		/// <returns></returns>
		public bool ShowInAppList { get; set; }

		/// <summary>
		/// Ӧ�õ������ߵı�ʶ��
		/// </summary>
		/// <returns></returns>
		public int? OwnerId { get; set; }

		/// <summary>
		/// ��Ӧ�õ������ߡ�
		/// </summary>
		/// <returns></returns>
		[ForeignKey("OwnerId")]
		public IdentityUser Owner { get; set; }

		/// <summary>
		/// ��ȡ�����÷������Ƶ���Ч�ڣ�����Ϊ��λ��
		/// </summary>
		public int AccessTokenLifetime { get; set; }

		/// <summary>
		/// ��ȡ������ˢ�����Ƶ���Ч�ڣ�����Ϊ��λ��
		/// </summary>
		public int RefreshTokenLifetime { get; set; }

		/// <summary>
		/// ��ȡ������һ��ֵ��ָʾ��Ӧ���Ƿ��Ѿ������á�
		/// </summary>
		public bool IsEnabled { get; set; }
	}
}