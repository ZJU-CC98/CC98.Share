using System;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CC98
{
	/// <summary>
	/// 提供序列化的辅助方法。
	/// </summary>
	[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class DataSerializationService : IDataSerializationService
	{
		/// <summary>
		/// 获取字符串的编码方式。
		/// </summary>
		[PublicAPI]
		protected virtual Encoding StringEncoding { get; } = Encoding.UTF8;

		/// <summary>
		/// 将字符串值编码为二进制序列。
		/// </summary>
		/// <param name="value">要编码的值。</param>
		/// <returns>编码后的二进制序列。</returns>
		[PublicAPI]
		protected virtual byte[] EncodeString(string value) => StringEncoding.GetBytes(value);

		/// <summary>
		/// 将二进制序列解码为字符串值。
		/// </summary>
		/// <param name="value">要解码的值。</param>
		/// <returns>解码后的字符串。</returns>
		[PublicAPI]
		protected virtual string DecodeString(byte[] value) => StringEncoding.GetString(value, 0, value.Length);

		/// <summary>
		/// 用给定的格式解码数据。
		/// </summary>
		/// <typeparam name="T">数据的类型。</typeparam>
		/// <param name="data">保存为二进制格式的数据。</param>
		/// <param name="format">数据的格式。</param>
		/// <returns>解码后的对象。</returns>
		public T Deserialize<T>(byte[] data, string format)
	    {
		    switch (format)
		    {
				case AppSettingFormats.Json:
				    return JsonConvert.DeserializeObject<T>(DecodeString(data));
				default:
					throw new NotSupportedException($"序列化程序不支持 {format} 格式的数据。");
		    }
	    }

		/// <summary>
		/// 用给定的格式编码数据。
		/// </summary>
		/// <typeparam name="T">数据的类型。</typeparam>
		/// <param name="data">要编码的数据。</param>
		/// <param name="format">编码的格式。</param>
		/// <returns>编码后的二进制对象。</returns>
	    public byte[] Serialize<T>(T data, string format)
	    {
		    switch (format)
		    {
				case AppSettingFormats.Json:
					return EncodeString(JsonConvert.SerializeObject(data));
				default:
					throw new NotSupportedException($"序列化程序不支持 {format} 格式的数据。");

			}
		}
    }
}
