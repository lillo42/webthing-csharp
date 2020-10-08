using System;
using System.Threading.Tasks;
using FluentAssertions;
using Mozilla.IoT.WebThing.Integration.Test.Web;
using Xunit;

namespace Mozilla.IoT.WebThing.Integration.Test.mDNS
{
    public class MDnsTest : IClassFixture<TestHost>
    {
        private readonly TestHost _host;

        public MDnsTest(TestHost testHost)
        {
            _host = testHost;
        }

        [Fact]
        public async Task DiscoveryService()
        {
            await Task.Delay(TimeSpan.FromSeconds(30));
            _host.ServiceName.Should().NotBeNull();
            _host.ServiceName.Labels.Should().Contain("test-thing");
        }
    }
}
