using Autofac;
using CommonLib.Dashboard;
using Dashboard.Api.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;

namespace Dashboard.Bootstrap
{
    public class WebModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CurrentIdentity>().As<ICurrentIdentity>();
            builder.Register(c =>
            {
                var httpContextAccessor = c.Resolve<IHttpContextAccessor>();
                return httpContextAccessor.HttpContext.Authentication;

            }).As<AuthenticationManager>().InstancePerLifetimeScope();

            builder.RegisterType<AuthClaimsManager>().As<IAuthClaimsManager>().InstancePerLifetimeScope();
            builder.RegisterType<WebAuditContext>().AsImplementedInterfaces();
        }
    }
}
