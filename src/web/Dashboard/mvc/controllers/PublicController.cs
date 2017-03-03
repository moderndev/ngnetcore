using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Dashboard.Api;
using CommonLib.Dashboard;

namespace Dashboard.mvc.controllers
{
    [AllowAnonymous]
    public class PublicController : BaseController
    {
        public PublicController(Func<ICurrentIdentity> identityFactory) : base(identityFactory)
        {
        }

        [Route("/")]
        [Route("/Public")]
        public IActionResult Index()
        {
            return View();

        }
        

        #if DEBUG
        public IActionResult Error()
        {
            throw new ArgumentNullException();
        }
        #endif

    }
}
