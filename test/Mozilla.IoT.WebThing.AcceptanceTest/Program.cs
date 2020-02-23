using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Extensions;

namespace Mozilla.IoT.WebThing.AcceptanceTest
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args, Action<ThingOption>? option = null) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logger =>
                {
                    logger.ClearProviders()
                        .AddConsole()
                        .AddFilter("*", LogLevel.Debug);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    Startup.Option = option;
                    webBuilder
                        .UseTestServer()
                        .UseStartup<Startup>();
                });

        private static IHost s_defaultHost;

        public static ValueTask<IHost> GetHost()
        {
            if (s_defaultHost != null)
            {
                return new ValueTask<IHost>(s_defaultHost);
            }

            return new ValueTask<IHost>(CreateHostBuilderAndStartAsync(null));
        }
        
        private static async Task<IHost> CreateHostBuilderAndStartAsync(string[] args)
        {
            return  s_defaultHost = await Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .UseStartup<Startup>();
                }).StartAsync();
        }
    }
}
