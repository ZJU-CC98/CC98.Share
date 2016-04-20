using System;
using Microsoft.AspNet.Mvc.Filters;

namespace CC98.Sports
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
	public class ActionResultExceptionFilterAttribute : ExceptionFilterAttribute
	{
		public override void OnException(ExceptionContext context)
		{
			var exp = context.Exception as ActionResultException;

			if (exp != null)
			{
				context.Result = exp.Result;
				return;
			}

			base.OnException(context);
		}
	}
}
