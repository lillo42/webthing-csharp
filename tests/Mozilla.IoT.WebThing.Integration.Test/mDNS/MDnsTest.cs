using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Makaretu.Dns;
using Mozilla.IoT.WebThing.Integration.Test.Web;
using Xunit;

namespace Mozilla.IoT.WebThing.Integration.Test.mDNS
{
    public class MDnsTest : IClassFixture<TestHost>, IDisposable
    {
        private readonly TestHost _host;
        private readonly ServiceDiscovery _serviceDiscovery;
        private DomainName _name;

        public MDnsTest(TestHost testHost)
        {
            _host = testHost;
            _serviceDiscovery = new ServiceDiscovery();
            
            _serviceDiscovery.ServiceInstanceDiscovered += OnServiceInstanceDiscovered;
            _serviceDiscovery.ServiceDiscovered += OnServiceDiscovered;
        }

        [Fact]
        public async Task DiscoveryService()
        {
            await Task.Delay(TimeSpan.FromSeconds(15));
            _name.Should().NotBeNull();
            _name.Labels.Should().Contain("test-thing");
        }

        private void OnServiceDiscovered(object sender, DomainName args)
        {
            if (args.Labels.Any(x => x.Contains("_webthing")))
            {
                _name = args;
            }
        }

        private void OnServiceInstanceDiscovered(object sender, ServiceInstanceDiscoveryEventArgs args)
        {
            if (args.ServiceInstanceName.Labels.Any(x => x.Contains("_webthing")))
            {
                _name = args.ServiceInstanceName;
            }
        }
        
        public void Dispose()
        {
            _serviceDiscovery.ServiceInstanceDiscovered -= OnServiceInstanceDiscovered;
            _serviceDiscovery.ServiceDiscovered -= OnServiceDiscovered;
            _serviceDiscovery.Dispose();
        }
    }
}
