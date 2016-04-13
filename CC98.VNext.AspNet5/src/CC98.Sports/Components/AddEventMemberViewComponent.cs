using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;

namespace CC98.Sports.Components
{
	/// <summary>
	/// 添加页面成员的视图组件。
	/// </summary>
	public class AddEventMemberViewComponent : ViewComponent
	{
		private SportDataModel DbContext { get; }

		public AddEventMemberViewComponent(SportDataModel dbContext)
		{
			DbContext = dbContext;
		}

		public async Task<IViewComponentResult> InvokeAsync(int teamId, int eventId)
		{
			var addedMembers = await (from i in DbContext.EventMemberRegistrations
									  where i.TeamId == teamId && i.EventId == eventId
									  select i.MemberId).ToArrayAsync();

			var members = await (from i in DbContext.TeamMemberRegistrations
								 where
								   i.TeamId == teamId
								   && i.TeamAuditState == AuditState.Accepted
								   && i.MemberAuditState == AuditState.Accepted
								   && i.Member.AuditState == AuditState.Accepted
								   && !addedMembers.Contains(i.MemberId)
								 select i.Member).ToArrayAsync();

			ViewBag.TeamId = teamId;
			ViewBag.EventId = eventId;

			return View(members);
		}
	}
}
