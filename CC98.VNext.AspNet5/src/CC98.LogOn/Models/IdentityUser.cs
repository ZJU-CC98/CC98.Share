using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.LogOn.Models
{
	/// <summary>
	/// ��ʾ CC98V2 �汾�û���
	/// </summary>
	[Table("Users")]
	public class IdentityUser
	{
		/// <summary>
		/// �û��ı�ʶ��
		/// </summary>
		/// <returns></returns>
		[Key]
		[Required]
		public int Id { get; set; }

		/// <summary>
		/// �û��� V1 �汾��ʶ��
		/// </summary>
		/// <returns></returns>
		public int OldId { get; set; }

		/// <summary>
		/// �û�����
		/// </summary>
		/// <returns></returns>
		[Required]
		public string UserName { get; set; }
	}
}