using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace CC98.Share
{
	public static class UrlUtility
	{
		public static string AbsoluteAction(this IUrlHelper urlHelper, [AspMvcAction] string action, [AspMvcController] string controller, object values = null)
		{
			return urlHelper.Action(action, controller, values, urlHelper.ActionContext.HttpContext.Request.Scheme,
				urlHelper.ActionContext.HttpContext.Request.Host.ToString());

		}
	}
}
