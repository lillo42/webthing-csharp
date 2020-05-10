using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace Mozilla.IoT.WebThing.Integration.Test.Web
{
    public static class HostFactory
    {
        private static IHost s_host;
        
        public static ValueTask<IHost> CreateHost()
        {
            if (s_host != null)
            {
                return new ValueTask<IHost>(s_host);
            }
            
            return new ValueTask<IHost>(Create(null));
        }

        private static async Task<IHost> Create(string[] args)
        {
            return s_host = await Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder =>
                {
                    builder
                        .UseTestServer()
                        .UseStartup<Startup>();
                })
                .StartAsync();
        }
    }
}
