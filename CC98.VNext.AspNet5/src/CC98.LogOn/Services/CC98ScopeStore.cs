using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.Identity;
using IdentityServer4.Core.Models;
using IdentityServer4.Core.Resources;
using IdentityServer4.Core.Services;

namespace CC98.LogOn.Services
{
	public class CC98ScopeStore : IScopeStore
	{
		/// <summary>Gets all scopes.</summary>
		/// <returns>List of scopes</returns>
		public async Task<IEnumerable<Scope>> FindScopesAsync(IEnumerable<string> scopeNames)
		{
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
			var allNames = new[] { OAuthScopes.All };

			var result = from i in allNames
						 select new Scope { Name = i };

			return Task.FromResult(result);
		}
	}
}
