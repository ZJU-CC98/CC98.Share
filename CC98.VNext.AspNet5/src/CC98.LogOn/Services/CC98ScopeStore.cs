using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CC98.Identity;
using IdentityServer4.Core.Models;
using IdentityServer4.Core.Resources;
using IdentityServer4.Core.Services;
using JetBrains.Annotations;

namespace CC98.LogOn.Services
{
	[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.WithMembers)]
	public class CC98ScopeStore : IScopeStore
	{
		/// <summary>Gets all scopes.</summary>
		/// <returns>List of scopes</returns>
		public async Task<IEnumerable<Scope>> FindScopesAsync(IEnumerable<string> scopeNames)
		{
			Debug.WriteLine("正在检索可用领域...");

			var allScopes = await GetScopesAsync(true);

			var items = from i in allScopes
						where scopeNames.Contains(i.Name, StringComparer.OrdinalIgnoreCase)
						select i;

			return items;

		}

		/// <summary>Gets all defined scopes.</summary>
		/// <param name="publicOnly">if set to <c>true</c> only public scopes are returned.</param>
		/// <returns></returns>
		public Task<IEnumerable<Scope>> GetScopesAsync(bool publicOnly = true)
		{
			Debug.WriteLine("正在检索领域表..., publicOnly = {0}", publicOnly);

			var allNames = new[] { OAuthScopes.All, "openid" };

			var result = from i in allNames
						 select new Scope { Name = i,  };
			return Task.FromResult(result);
		}
	}
}
