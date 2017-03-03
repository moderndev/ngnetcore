using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Dashboard.Api;
using CommonLib.Dashboard;
using Dashboard.Api.Security;

namespace Dashboard.mvc.controllers
{
    [AuthorizeRoles(Roles.Admin, Roles.UserFromToken)]
    public class AdminController : BaseController
    {
        public AdminController(Func<ICurrentIdentity> identityFactory) : base(identityFactory)
        {
        }

        [Route("/Admin")]
        public IActionResult Index()
        {
            return View();

        }

    }
}
