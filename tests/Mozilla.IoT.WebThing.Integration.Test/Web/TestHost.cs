using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace Mozilla.IoT.WebThing.Integration.Test.Web
{
    public class TestHost : IDisposable
    {
        public IHost Host { get; }
        
        public TestHost()
        { 
            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(null)
                .ConfigureWebHostDefaults(builder =>
                {
                    builder
                        .UseTestServer()
                        .UseStartup<Startup>();
                })
                .Start();
        }
        
        
        public void Dispose()
        {
            Host.Dispose();
        }
    }
}
