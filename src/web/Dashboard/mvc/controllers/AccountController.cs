using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Dashboard.Api;
using CommonLib.Dashboard;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Authentication;
using Dashboard.Api.Security;
using CommonLib;

namespace Dashboard.mvc.controllers
{
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        private readonly IAuthClaimsManager _authClaimsManager;

        public AccountController(Func<ICurrentIdentity> identityFactory, IAuthClaimsManager authClaimsManager) : base(identityFactory)
        {
            Args.NotNull(authClaimsManager, nameof(authClaimsManager));

            _authClaimsManager = authClaimsManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            if (returnUrl == null) returnUrl = string.Empty;
            return ExternalIdpLogin(string.Empty, returnUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ExternalIdpLogin(string provider, string returnUrl = null)
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = $"/Account/ExternalIdpLoginCallback?returnUrl={returnUrl}"
            };

            return new ChallengeResult(AuthenticationSchemeNames.MDOpenIdConnect, props);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalIdpLoginCallback(string returnUrl = "/")
        {
            var externalClaimsPrincipal = await HttpContext.Authentication.AuthenticateAsync(AuthenticationSchemeNames.ClientCookieTemp);
            if (externalClaimsPrincipal == null)
            {
                return Challenge();
            }
            await _authClaimsManager.SignInFromIdp(externalClaimsPrincipal);
            await HttpContext.Authentication.SignOutAsync(AuthenticationSchemeNames.ClientCookieTemp);

            returnUrl = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
            return Redirect(returnUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Logout()
        {
            //await _authClaimsManager.Logout();

            return Login();
        }
    }
}
