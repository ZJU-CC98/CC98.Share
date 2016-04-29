namespace CC98.LogOn.Models
{
	/// <summary>
	/// 表示应用的状态。
	/// </summary>
	public enum AppAuditState
	{
		/// <summary>
		/// 应用未经审核。
		/// </summary>
		Unverified = 0,
		/// <summary>
		/// 应用已经被审核。
		/// </summary>
		Verified,
		/// <summary>
		/// 应用审核已过期。
		/// </summary>
		Expired,
		/// <summary>
		/// 应用已被吊销。
		/// </summary>
		Revoked
	}
}