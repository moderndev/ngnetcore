using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace Dashboard
{
    public class Startup
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IHostingEnvironment _env;
        private readonly ILogger<Startup> _logger;

        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _env = env;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<Startup>();




        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddLogging();

            // only allow authenticated users
            var defaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            // Add application services.

            // OpenID Connect Authentication Requires Cookie Auth
            //services.Configure<SharedAuthenticationOptions>(
            //    options => { options.SignInScheme = AuthenticationSchemeNames.ClientCookie; });

            services.AddAuthentication();

            services.AddDataProtection().SetApplicationName("ModernDev");

            // Add Autofac
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<WebModule>();
            containerBuilder.Populate(services);

            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
