using System;
using System.Globalization;
using System.Security.Claims;

namespace CC98.Identity
{
    /// <summary>
    ///     为 CC98 声明标识提供辅助方法。该类型为静态类型。
    /// </summary>
    public static class CC98IdentityHelper
    {
        /// <summary>
        ///     获取指定用户凭据包含的用户 ID 值。
        /// </summary>
        /// <param name="identity">用户凭据。</param>
        /// <returns>用户的 ID 值。</returns>
        public static int GetId(this ClaimsIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            return int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     获取指定用户凭据包含的用户 V1 版数据库 ID 值。
        /// </summary>
        /// <param name="identity">用户凭据。</param>
        /// <returns>用户的 V1 版数据库 ID 值。</returns>
        public static int GetOldId(this ClaimsIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            return int.Parse(identity.FindFirst(CC98UserClaimTypes.OldId).Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     获取指定用户凭据包含的用户头像地址。
        /// </summary>
        /// <param name="identity">用户凭据。</param>
        /// <returns>用户的头像地址。</returns>
        public static string GetPortraitUri(this ClaimsIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            return identity.FindFirst(CC98UserClaimTypes.PortraitUri).Value;
        }

        /// <summary>
        ///     获取指定用户凭据包含的用户 ID 值。
        /// </summary>
        /// <param name="principal">用户凭据。</param>
        /// <returns>用户的 ID 值。</returns>
        public static int GetId(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            return int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier).Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     获取指定用户凭据包含的用户 V1 版数据库 ID 值。
        /// </summary>
        /// <param name="principal">用户凭据。</param>
        /// <returns>用户的 V1 版数据库 ID 值。</returns>
        public static int GetOldId(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            return int.Parse(principal.FindFirst(CC98UserClaimTypes.OldId).Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     获取指定用户凭据包含的用户头像地址。
        /// </summary>
        /// <param name="principal">用户凭据。</param>
        /// <returns>用户的头像地址。</returns>
        public static string GetPortraitUri(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            return principal.FindFirst(CC98UserClaimTypes.PortraitUri).Value;
        }

        /// <summary>
        ///     获取指定用户主体包含的访问令牌值。
        /// </summary>
        /// <param name="principal">用户主体。</param>
        /// <returns>用户的访问令牌值。</returns>
        public static string GetAccessToken(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            return principal.FindFirst(CC98UserClaimTypes.AccessToken).Value;
        }

        /// <summary>
        ///     获取指定用户标识包含的访问令牌值。
        /// </summary>
        /// <param name="identity">用户标识。</param>
        /// <returns>用户的访问令牌值。</returns>
        public static string GetAccessToken(this ClaimsIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            return identity.FindFirst(CC98UserClaimTypes.AccessToken).Value;
        }
    }
}