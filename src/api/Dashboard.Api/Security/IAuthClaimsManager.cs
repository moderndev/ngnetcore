using CommonLib.Dashboard.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dashboard.Api.Security
{
    public interface IAuthClaimsManager
    {
        Task<ClaimsPrincipal> SignInFromIdp(ClaimsPrincipal externalClaimsPrincipal);
        Task SignInFromToken(Person person, string token);
        Task Logout();
        Task RefreshClaims();
        Task LogoutLobby();
    }
}
