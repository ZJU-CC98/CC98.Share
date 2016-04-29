using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.LogOn.Models
{
	/// <summary>
	/// ��ʾӦ�õ�ѡ�
	/// </summary>
	[ComplexType]
	public class AppOptions
	{
		/// <summary>
		/// �Ƿ�������Ȩ����֤��
		/// </summary>
		/// <returns></returns>
		public bool EnableAuthorizationCode { get; set; }

		/// <summary>
		/// �Ƿ�������ʽ��Ȩ��֤��
		/// </summary>
		/// <returns></returns>
		public bool EnableImplicitGrant { get; set; }

		/// <summary>
		/// �Ƿ������û���������֤��
		/// </summary>
		/// <returns></returns>
		public bool EnableResourceOwnerPassword { get; set; }
	}
}