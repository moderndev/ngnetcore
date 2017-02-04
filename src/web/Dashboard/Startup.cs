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
using Dashboard.Api.Security;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Authorization;
using Dashboard.Api.Filters;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Dashboard.Bootstrap;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Dashboard
{
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IHostingEnvironment _env;
        private readonly ILogger<Startup> _logger;

        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _env = env;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<Startup>();
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath);

            if (env.IsDevelopment())
            {
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            _configuration = builder.Build();
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
            services.AddMvc(setup =>
            {
                setup.Filters.Add(new AuthorizeFilter(defaultPolicy));
                setup.Filters.Add(new UnhandledExceptionFilterAttribute(_loggerFactory));
            });

            services.AddAuthorization(
               options =>
               {
                  
               });

            // https://github.com/aspnet/Hosting/issues/793
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // OpenID Connect Authentication Requires Cookie Auth
            services.Configure<SharedAuthenticationOptions>(
                options => { options.SignInScheme = AuthenticationSchemeNames.ClientCookie; });

            // use a custom folder structure for views/controllers 
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new MvcFolderLocationExpander());
            });

            services.AddAuthentication();

            services.AddDataProtection().SetApplicationName("ModernDev");

            // Add Autofac
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<WebModule>();
            containerBuilder.RegisterModule<CoreModule>();
            containerBuilder.RegisterModule<InfrastructureModule>();
            
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

            SetupAuthentication(app);

            //app.UseMvcWithDefaultRoute();
            app.UseMvc();

            var fileServerOptions = new FileServerOptions { EnableDefaultFiles = true };
            fileServerOptions.StaticFileOptions.ServeUnknownFileTypes = true;
            fileServerOptions.StaticFileOptions.DefaultContentType = "text/plain";
            app.UseFileServer(fileServerOptions);

            _logger.LogInformation("Process ID {0}", Process.GetCurrentProcess().Id);


            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }


        private void SetupAuthentication(IApplicationBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                AuthenticationScheme = AuthenticationSchemeNames.ClientCookie,
                CookieName = CookieAuthenticationDefaults.CookiePrefix + AuthenticationSchemeNames.ClientCookie,
                LoginPath = new PathString("/Account/Login"),
                AccessDeniedPath = new PathString("/Account/AccessDenied"),
                ExpireTimeSpan = TimeSpan.FromMinutes(240),
                //Events = new CookieAuthenticationEvents
                //{
                //    // Set other options
                //    OnValidatePrincipal = context => OnValidatePrincipal(context, originalEvents.OnValidatePrincipal),
                //    OnSignedIn = context => OnSignedIn(context, app.ApplicationServices, originalEvents.OnSignedIn),
                //    OnSigningIn = context => OnSigningIn(context, originalEvents.OnSigningIn),
                //    OnRedirectToLogin = context => OnRedirectToLogin(context, originalEvents.OnRedirectToLogin),
                //    OnRedirectToAccessDenied = context => OnRedirectToAccessDenied(context, originalEvents.OnRedirectToAccessDenied),
                //    OnRedirectToLogout = context => OnRedirectToLogout(context, originalEvents.OnRedirectToLogout),
                //    OnRedirectToReturnUrl = context => OnRedirectToReturnUrl(context, originalEvents.OnRedirectToReturnUrl),
                //    OnSigningOut = context => OnSigningOut(context, app.ApplicationServices, originalEvents.OnSigningOut)
                //}
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = AuthenticationSchemeNames.ClientCookieTemp,
                AutomaticAuthenticate = false,
                ExpireTimeSpan = TimeSpan.FromSeconds(5)
            });

            //app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            //{
            //    AuthenticationScheme = AuthenticationSchemeNames.MDOpenIdConnect,
            //    RequireHttpsMetadata = false,
            //    SaveTokens = true,
            //    AutomaticChallenge = false,
            //    ClientId = "client",
            //    ClientSecret = "secret",
            //    ResponseType = OpenIdConnectResponseType.Code,
            //    // Authority = "http://idp.localtest.me:8080/",
            //    Authority = "http://localhost:8080/",
            //    SignInScheme = AuthenticationSchemeNames.ClientCookieTemp,
            //    GetClaimsFromUserInfoEndpoint = true,
            //    Events = new OpenIdConnectEvents
            //    {
            //        OnRedirectToIdentityProvider = async redirectContext =>
            //        {
            //            var reAuthCtx =
            //                (ReAuthContext)redirectContext.HttpContext.Items[ReAuthContext.ReAuthContextKey];

            //            redirectContext.ProtocolMessage.Prompt = reAuthCtx?.Prompt;
            //            if (reAuthCtx != null && reAuthCtx.LiteVersion)
            //                redirectContext.ProtocolMessage.SetParameter("lite", "true");
            //            redirectContext.ProtocolMessage.LoginHint = reAuthCtx?.LoginHint;
            //            await Task.FromResult(0);
            //        }
            //    }
            //});


            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                AuthenticationScheme = AuthenticationSchemeNames.MDOpenIdConnect,
                SignInScheme = AuthenticationSchemeNames.ClientCookieTemp, // "Cookies", //

                Authority = "http://localhost:8080/",
                RequireHttpsMetadata = false,

                ClientId = "client.moderndev", //"mvc.hybrid",
                ClientSecret = "secret",

                ResponseType = OpenIdConnectResponseType.Code, //"code id_token",
                Scope = { "openid", "profile", "email", "api1", "offline_access", "custom.profile" },
                GetClaimsFromUserInfoEndpoint = true,
                SaveTokens = true,

                TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    NameClaimType = "name", // JwtClaimTypes.Name,
                    RoleClaimType = "role", // JwtClaimTypes.Role,
                }

                ,
                Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProvider = async redirectContext =>
                    {
                        var reAuthCtx =
                            (ReAuthContext)redirectContext.HttpContext.Items[ReAuthContext.ReAuthContextKey];

                        redirectContext.ProtocolMessage.Prompt = reAuthCtx?.Prompt;
                        if (reAuthCtx != null && reAuthCtx.LiteVersion)
                            redirectContext.ProtocolMessage.SetParameter("lite", "true");
                        redirectContext.ProtocolMessage.LoginHint = reAuthCtx?.LoginHint;
                        await Task.FromResult(0);
                    }
                }
            });

        }
    }
}



