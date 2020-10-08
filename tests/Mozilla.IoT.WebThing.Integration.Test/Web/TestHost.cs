using System;
using System.Linq;
using Makaretu.Dns;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace Mozilla.IoT.WebThing.Integration.Test.Web
{
    public class TestHost : IDisposable
    {
        private readonly ServiceDiscovery _serviceDiscovery;

        public IHost Host { get; }
        public DomainName ServiceName { get; private set; }
        
        public TestHost()
        {
            _serviceDiscovery = new ServiceDiscovery();
            
            _serviceDiscovery.ServiceInstanceDiscovered += OnServiceInstanceDiscovered;
            _serviceDiscovery.ServiceDiscovered += OnServiceDiscovered;
            
            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(null)
                .ConfigureWebHostDefaults(builder =>
                {
                    builder
                        .UseTestServer()
                        .UseStartup<Startup>();
                })
                .Start();
        }

        private void OnServiceDiscovered(object sender, DomainName args)
        {
            if (args.Labels.Any(x => x.Contains("_webthing")))
            {
                ServiceName = args;
            }
        }

        private void OnServiceInstanceDiscovered(object sender, ServiceInstanceDiscoveryEventArgs args)
        {
            if (args.ServiceInstanceName.Labels.Any(x => x.Contains("_webthing")))
            {
                ServiceName = args.ServiceInstanceName;
            }
        }

        public void Dispose()
        {
            Host.Dispose();
            _serviceDiscovery.ServiceInstanceDiscovered -= OnServiceInstanceDiscovered;
            _serviceDiscovery.ServiceDiscovered -= OnServiceDiscovered;
            _serviceDiscovery.Dispose();
        }
    }
}
