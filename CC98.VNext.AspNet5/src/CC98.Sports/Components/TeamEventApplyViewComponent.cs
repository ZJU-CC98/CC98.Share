using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;

namespace CC98.Sports.Components
{
	/// <summary>
	/// 提供队伍报名参赛的视图组件。
	/// </summary>
	public class TeamEventApplyViewComponent : ViewComponent
	{
		private SportDataModel DbContext { get; }


		public TeamEventApplyViewComponent(SportDataModel dbContext)
		{
			DbContext = dbContext;
		}

		public async Task<IViewComponentResult> InvokeAsync(int eventId)
		{
			var eventItem = await (from i in DbContext.Events
								   where i.Id == eventId
								   select i).FirstOrDefaultAsync();

			var userName = HttpContext.User.GetUserName();

			var teams = await (from i in DbContext.Teams
							   where i.Skipper != null && i.Skipper.CC98Id == userName
							   select i).ToArrayAsync();

			ViewBag.Event = eventItem;
			ViewBag.Teams = teams;

			return View();
		}

	}
}
