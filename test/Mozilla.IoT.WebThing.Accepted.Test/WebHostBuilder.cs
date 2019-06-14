using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Mozilla.IoT.WebThing.Accepted.Test
{
    internal static class WebHostBuilder
    {
        internal static IWebHostBuilder Create<T>()
            where T : class
            => WebHost.CreateDefaultBuilder()
                //.UseSetting("https_port", "8080")
                .UseStartup<T>();
    }
}
