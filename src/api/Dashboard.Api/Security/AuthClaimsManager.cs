using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CommonLib.Dashboard.Model;
using System.Security.Principal;
using Microsoft.AspNetCore.Http.Authentication;
using CommonLib;

namespace Dashboard.Api.Security
{
    public class AuthClaimsManager : IAuthClaimsManager
    {
        private AuthenticationManager AuthenticationManager { get; }

        static AuthClaimsManager()
        {
        }

        public AuthClaimsManager(AuthenticationManager authenticationManager)
        {
            Args.NotNull(authenticationManager, nameof(authenticationManager));

            AuthenticationManager = authenticationManager;
        }

        public async Task Logout()
        {
            await LogoutLobby();
            try {
                await AuthenticationManager.SignOutAsync(AuthenticationSchemeNames.MDOpenIdConnect);
            }
            catch (Exception ex) { }
            
        }

        public async Task LogoutLobby()
        {
            await AuthenticationManager.SignOutAsync(AuthenticationSchemeNames.ClientCookie);
            await AuthenticationManager.SignOutAsync(AuthenticationSchemeNames.ClientCookieTemp);
        }

        public Task RefreshClaims()
        {
            throw new NotImplementedException();
        }

        public async Task<ClaimsPrincipal> SignInFromIdp(ClaimsPrincipal externalClaimsPrincipal)
        {
            GenericPrincipal principal;
            //using (var unitOfWork = _unitOfWorkFactory())
            //{
                var identityId = externalClaimsPrincipal.GetIdentityId();
                var person = new Person(identityId); //_personRepository.GetByIdentityId(identityId);
                var claims = GetClaimsFromExternalPrincipal(externalClaimsPrincipal);
            //person.ProcessSuccesfulLogin(identityId);

            principal = await CreatePrincipal(person, claims);
            
            //unitOfWork.Complete();
            //}
            await AuthenticationManager.SignInAsync(AuthenticationSchemeNames.ClientCookie, principal);
            return principal;
        }

        public Task SignInFromToken(Person person, string token)
        {
            throw new NotImplementedException();
        }


        private List<Claim> GetClaimsFromExternalPrincipal(ClaimsPrincipal externalClaimsPrincipal)
        {
            return externalClaimsPrincipal.Claims.Select(c => new Claim($"{"external_"}{c.Type}", c.Value)).ToList();
        }

        private async Task<GenericPrincipal> CreatePrincipal(Person person, IEnumerable<Claim> claims)
        {
            var roles = new[] { person.IsAccountVerified ? Roles.User : Roles.NonVerifiedUser };
            if (person.IsAdministrator) roles = roles.Append(Roles.Admin).ToArray();
            return await CreatePrincipal(person, claims, roles);
        }

        private async Task<GenericPrincipal> CreatePrincipal(Person person, IEnumerable<Claim> claims, string[] roles)
        {
            var allClaims = new List<Claim>();
            allClaims.AddRange(claims);

            // add custom claims from external principal claims
            AddCustomClaimFromExternalClaimIfItExists(allClaims, ClaimTypes.NameIdentifier, "identityId");
            AddCustomClaimFromExternalClaimIfItExists(allClaims, "iat", "iat");
            AddCustomClaimFromExternalClaimIfItExists(allClaims, "auth_time", "auth_time");

            allClaims.AddRange(GetExternalClaimValues(allClaims, "http://schemas.microsoft.com/claims/authnmethodsreferences")
                    .Select(v => new Claim("amr", v))
                    .ToArray());

            //allClaims.AddRange(GetClaimsFromDatabase(person));
            //allClaims.AddRange(await GetClaimsUsingDefaultIdentityService(person.UserName));

            var newIdentity = CreateIdentity(allClaims);
            return new GenericPrincipal(newIdentity, roles);
        }

        private ClaimsIdentity CreateIdentity(IEnumerable<Claim> claims)
        {
            var newId = new ClaimsIdentity(AuthenticationSchemeNames.MDOpenIdConnect, "name", "role");
            newId.AddClaims(claims);
            return newId;
        }

        private void AddCustomClaimFromExternalClaimIfItExists(List<Claim> claims, string externalClaimName, string customClaimName)
        {
            var claim = claims.FirstOrDefault(x => x.Type.ToString() == $"{"external_"}{externalClaimName}");
            if (claim != null)
            {
                claims.Add(new Claim(customClaimName, claim.Value));
            }
        }

        private IEnumerable<string> GetExternalClaimValues(IEnumerable<Claim> claims, string name)
        {
            return claims
                .Where(c => c.Type.ToString() == $"{"external_"}{name}")
                .Select(c => c.Value)
                .Where(v => !String.IsNullOrEmpty(v))
                .ToArray();
        }
    }
}
