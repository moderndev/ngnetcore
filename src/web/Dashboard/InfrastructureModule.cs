using Autofac;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var httpContextAccesor = c.Resolve<IHttpContextAccessor>();
                var httpContext = httpContextAccesor.HttpContext;
                return httpContext.Request;
            });

        }
    }
}
