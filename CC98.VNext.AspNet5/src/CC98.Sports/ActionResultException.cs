using System;
using System.Net;
using Microsoft.AspNet.Mvc;

namespace CC98.Sports
{
	/// <summary>
	/// 使用异常模型定义 MVC 控制器的操作结果。
	/// </summary>
	public class ActionResultException : Exception
	{
		public IActionResult Result { get; }

		/// <summary>
		/// 用指定的 <see cref="IActionResult"/> 对象创建一个 <see cref="ActionResultException"/> 对象的新实例。
		/// </summary>
		/// <param name="result">作为 MVC 控制器结果的 <see cref="IActionResult"/> 对象。</param>
		public ActionResultException(IActionResult result)
		{
			Result = result;
		}

		/// <summary>
		/// 用给定的 HTTP 响应结果代码创建一个 <see cref="ActionResultException"/> 对象的新实例。
		/// </summary>
		/// <param name="statusCode">作为 HTTP 响应代码的数值。</param>
		/// <seealso cref="HttpStatusCodeResult"/>
		public ActionResultException(int statusCode)
			: this(new HttpStatusCodeResult(statusCode))
		{
		}

		/// <summary>
		/// 用给定的 HTTP 响应结果代码创建一个 <see cref="ActionResultException"/> 对象的新实例。
		/// </summary>
		/// <param name="statusCode">HTTP 响应代码。</param>
		/// <seealso cref="HttpStatusCodeResult"/>
		public ActionResultException(HttpStatusCode statusCode)
			: this((int)statusCode)
		{
		}

		/// <summary>
		/// 用给定的 HTTP 响应结果代码和响应内容创建一个 <see cref="ActionResultException"/> 对象的新实例。
		/// </summary>
		/// <param name="statusCode">作为 HTTP 响应代码的数值。</param>
		/// <param name="result">作为响应内容的对象。</param>
		/// <seealso cref="ObjectResult"/>
		public ActionResultException(int statusCode, object result)
		{
			var actionResult = new ObjectResult(result)
			{
				StatusCode = statusCode
			};

			Result = actionResult;
		}


		/// <summary>
		/// 用给定的 HTTP 响应结果代码和响应内容创建一个 <see cref="ActionResultException"/> 对象的新实例。
		/// </summary>
		/// <param name="statusCode">HTTP 响应代码。</param>
		/// <param name="result">作为响应内容的对象。</param>
		/// <seealso cref="ObjectResult"/>
		public ActionResultException(HttpStatusCode statusCode, object result)
			: this((int)statusCode, result)
		{

		}


		/// <summary>
		/// 用给定的响应结果对象创建一个 <see cref="ActionResultException"/> 对象的新实例。
		/// </summary>
		/// <param name="result">作为响应内容的对象。</param>
		/// <seealso cref="ObjectResult"/>
		public ActionResultException(object result)
			: this(new ObjectResult(result))
		{
		}
	}
}
