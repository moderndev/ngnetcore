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
        [Route("/Account/Login")]
        public IActionResult Login(string returnUrl = null)
        {
            if (returnUrl == null) returnUrl = string.Empty;
            return ExternalIdpLogin(string.Empty, returnUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/Account/ExternalIdpLogin")]
        public IActionResult ExternalIdpLogin(string provider, string returnUrl = null)
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = $"/Account/ExternalIdpLoginCallback?returnUrl={returnUrl}"
                // RedirectUri = $"/signin-oidc"
            };

            return new ChallengeResult(AuthenticationSchemeNames.MDOpenIdConnect, props);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/Account/ExternalIdpLoginCallback")]
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
        [Route("/Account/Logout")]
        public async Task<IActionResult> Logout()
        {
            await _authClaimsManager.Logout();

            //return Login();
            return Redirect("/");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/Account/AccessDenied")]
        public IActionResult AccessDenied()
        {
            if (HttpContext.User.IsInRole(Roles.NonVerifiedUser))
            {
                return Redirect("/Public/#/RegisterUser/EmailNotValidated");
            }

            if (HttpContext.User.IsInRole(Roles.UserFromToken))
            {
                return RedirectToAction("Login");
            }

            return View("AccessDenied");
        }
    }
}
