using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Dashboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string environmentName = "Development";

            var hostingConfig = new ConfigurationBuilder()
                .AddJsonFile("hosting.json", optional: true)
                .AddCommandLine(args)
                .AddEnvironmentVariables()
                .Build();

            var host = new WebHostBuilder()
                .UseConfiguration(hostingConfig)
                .UseEnvironment(environmentName)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}


// http://benfoster.io/blog/how-to-configure-kestrel-urls-in-aspnet-core-rc2