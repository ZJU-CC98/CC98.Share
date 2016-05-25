namespace CC98
{
	/// <summary>
	/// 定义数据序列化所必需的方法。
	/// </summary>
	public interface IDataSerializationService
	{
		/// <summary>
		/// 用给定的格式解码数据。
		/// </summary>
		/// <typeparam name="T">数据的类型。</typeparam>
		/// <param name="data">保存为二进制格式的数据。</param>
		/// <param name="format">数据的格式。</param>
		/// <returns>解码后的对象。</returns>
		T Deserialize<T>(byte[] data, string format);

		/// <summary>
		/// 用给定的格式编码数据。
		/// </summary>
		/// <typeparam name="T">数据的类型。</typeparam>
		/// <param name="data">要编码的数据。</param>
		/// <param name="format">编码的格式。</param>
		/// <returns>编码后的二进制对象。</returns>
		byte[] Serialize<T>(T data, string format);
	}
}