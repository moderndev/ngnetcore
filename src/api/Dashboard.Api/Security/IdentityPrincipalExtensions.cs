using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dashboard.Api.Security
{
    public static class IdentityPrincipalExtensions
    {
        //public static Guid GetPersonId(this ClaimsIdentity identity)
        //{
        //    return Guid.Parse(identity.FindFirst(ClaimKeys.UserPersonId).Value);
        //}

        //public static Guid GetPersonId(this ClaimsPrincipal principal)
        //{
        //    var identity = principal.Identity as ClaimsIdentity;
        //    if (identity != null)
        //        return Guid.Parse(identity.FindFirst(ClaimKeys.UserPersonId).Value);
        //    return Guid.Empty;
        //}

        public static Guid GetIdentityId(this ClaimsPrincipal externalClaimsPrincipal)
        {
            var identityId = Guid.Parse(externalClaimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value);
            return identityId;
        }

        //public static bool HasAuthenticatedWithin(this ClaimsPrincipal user, double minutes)
        //{
        //    var authTime = user.FindFirst(ClaimKeys.AuthTime);

        //    if (authTime == null) return false;

        //    var authenticatedAt = DateTimeOffset.FromUnixTimeSeconds(Int64.Parse(authTime.Value)).DateTime;

        //    var diff = DateTime.UtcNow - authenticatedAt;
        //    return Math.Abs(diff.TotalMinutes) < minutes;
        //}
    }
}
