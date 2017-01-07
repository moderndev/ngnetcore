using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Dashboard.Api;
using CommonLib.Dashboard;
using Dashboard.Api.Security;

namespace Dashboard.mvc.controllers
{
    [AuthorizeRoles(Roles.User, Roles.UserFromToken)]
    public class DashboardController : BaseController
    {
        public DashboardController(Func<ICurrentIdentity> identityFactory) : base(identityFactory)
        {
        }

        public IActionResult Index()
        {
            return View();

        }
    }
}
