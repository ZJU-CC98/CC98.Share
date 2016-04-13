using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;

namespace CC98.Sports.Components
{
	/// <summary>
	/// 提供球员的球队申请操作。
	/// </summary>
	public class TeamMemberApplyViewComponent : ViewComponent
	{
		private SportDataModel DbContext { get; }

		public TeamMemberApplyViewComponent(SportDataModel dbContext)
		{
			DbContext = dbContext;
		}

		/// <summary>
		/// 执行视图
		/// </summary>
		/// <returns></returns>
		public async Task<IViewComponentResult> InvokeAsync(int teamId)
		{
			var teams = await (from i in DbContext.Teams
							   select i).ToArrayAsync();

			var userName = HttpContext.User.GetUserName();

			var members = await (from i in DbContext.Members
									  where i.Type != MemberType.Officer && i.CC98Id == userName
									  select i).ToArrayAsync();

			ViewBag.TeamId = teamId;
			ViewBag.Teams = teams;
			ViewBag.Members = members;

			return View();
		}
	}
}
