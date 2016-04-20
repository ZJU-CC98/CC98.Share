using System;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.Models;
using IdentityServer4.Core.Models;
using IdentityServer4.Core.Services;
using JetBrains.Annotations;
using Microsoft.Data.Entity;

namespace CC98.LogOn.Services
{
	[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.WithMembers)]
	public class CC98ClientStore : IClientStore
	{
		private CC98V2DatabaseModel V2Model { get; }

		public CC98ClientStore(CC98V2DatabaseModel v2Model)
		{
			V2Model = v2Model;
		}

		/// <summary>Finds a client by id</summary>
		/// <param name="clientId">The client id</param>
		/// <returns>The client</returns>
		public async Task<Client> FindClientByIdAsync(string clientId)
		{
			Guid id;

			if (!Guid.TryParse(clientId, out id))
			{
				return null;
			}

			var item = await (from i in V2Model.Apps
							  where i.Id == id
							  select i).SingleOrDefaultAsync();

			if (item == null)
			{
				return null;
			}

			var result = new Client
			{
				ClientName = item.Name,
				LogoUri = item.LogoUri,
				ClientId = item.Id.ToString("D"),
				ClientUri = item.HomePageUri,
				Flow = Flows.AuthorizationCode
			};

			result.ClientSecrets.Add(new Secret(item.Secret.ToString("D")));

			if (!string.IsNullOrEmpty(item.RedirectUri))
			{
				result.RedirectUris.Add(item.RedirectUri);
			}

			return result;

		}
	}
}
