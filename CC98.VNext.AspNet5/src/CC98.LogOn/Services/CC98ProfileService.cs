using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Core.Models;
using IdentityServer4.Core.Services;

namespace CC98.LogOn.Services
{
	public class CC98ProfileService : IProfileService
	{
		/// <summary>
		/// This method is called whenever claims about the user are requested (e.g. during token creation or via the userinfo endpoint)
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public async Task GetProfileDataAsync(ProfileDataRequestContext context)
		{
			return;
		}

		/// <summary>
		/// This method gets called whenever identity server needs to determine if the user is valid or active (e.g. if the user's account has been deactivated since they logged in).
		/// (e.g. during token issuance or validation).
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public async Task IsActiveAsync(IsActiveContext context)
		{
			context.IsActive = true;
		}
	}
}
