using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;

namespace CC98.Sports.Components
{
	/// <summary>
	/// 提供球队队球员的邀请操作。
	/// </summary>
	public class TeamMemberInviteViewComponent : ViewComponent
	{

		private SportDataModel DbContext { get; }

		public TeamMemberInviteViewComponent(SportDataModel dbContext)
		{
			DbContext = dbContext;
		}

		/// <summary>
		/// 执行视图
		/// </summary>
		/// <returns></returns>
		public async Task<IViewComponentResult> InvokeAsync(int memberId)
		{
			var member = await (from i in DbContext.Members
								where i.Id == memberId
								select i).FirstOrDefaultAsync();

			if (member == null)
			{
				throw new InvalidOperationException();
			}

			var userName = HttpContext.User.GetUserName();


			var teams = await (from i in DbContext.Teams
							   join j in DbContext.Members on i.SkipperId equals j.Id
							   where j.CC98Id == userName
							   select i).ToArrayAsync();


			ViewBag.Teams = teams;
			ViewBag.Member = member;

			return View();
		}
	}
}
