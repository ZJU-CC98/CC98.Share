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
	public class CC98AppManager : IClientStore
	{
		/// <summary>
		/// 获取数据库模型对象。
		/// </summary>
		private CC98V2DatabaseModel V2Model { get; }

		public CC98AppManager(CC98V2DatabaseModel v2Model)
		{
			V2Model = v2Model;
		}

		///  <summary>
		///  从数据库中检索具有指定标识的应用信息。
		///  </summary>
		///  <param name="id">应用的标识字符串。</param>
		/// <exception cref="ArgumentNullException"><paramref name="id"/> 为 <c>null</c>。</exception>
		/// <returns>如果检索到了对应的应用，返回该应用的信息。否则返回 <c>null</c>。</returns>
		public async Task<App> FindAppByIdAsync([NotNull] string id)
		{
			if (id == null)
			{
				throw new ArgumentNullException(nameof(id));
			}

			Guid realId;
			return Guid.TryParse(id, out realId) ? await FindAppByIdAsync(realId) : null;
		}

		/// <summary>
		/// 从数据库中检索具有指定标识的应用信息。
		/// </summary>
		/// <param name="id">应用的标识。</param>
		/// <returns>如果检索到了对应的应用，返回该应用的信息。否则返回 <c>null</c>。</returns>
		public Task<App> FindAppByIdAsync(Guid id)
		{
			return (from i in V2Model.Apps
					where i.Id == id
					select i).SingleOrDefaultAsync();
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
				Flow = Flows.AuthorizationCode,
				AllowAccessToAllScopes = true,
				AccessTokenLifetime = item.AccessTokenLifetime,
				AbsoluteRefreshTokenLifetime = item.RefreshTokenLifetime,
				SlidingRefreshTokenLifetime = item.RefreshTokenLifetime,
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
