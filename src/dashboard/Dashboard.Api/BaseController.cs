using CommonLib;
using CommonLib.Dashboard;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dashboard.Api
{
    public abstract class BaseController : Controller
    {
        private readonly Func<ICurrentIdentity> _identityFactory;

        protected BaseController(Func<ICurrentIdentity> identityFactory)
        {
            Args.NotNull(identityFactory, nameof(identityFactory));
            _identityFactory = identityFactory;
        }
        protected bool IsLoggedIn()
        {
            var identity = GetIdentity();
            return identity.Claims.Any();
        }
        protected ICurrentIdentity CurrentIdentity => _identityFactory();

        private ClaimsIdentity GetIdentity()
        {
            return (ClaimsIdentity)HttpContext.User.Identity;
        }
    }
}
