using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Makaretu.Dns;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.HostServices;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.HostServices
{
    public class MDnsRegisterTest
    {
        private readonly Fixture _fixture;
        private readonly IServer _server;
        private readonly ThingOption _option;
        private readonly ICollection<Thing> _things;
        private readonly ILogger<MDnsRegisterHostedService> _logger;

        public MDnsRegisterTest()
        {
            _fixture = new Fixture();
            _server = Substitute.For<IServer>();
            _option = new ThingOption();
            _logger = Substitute.For<ILogger<MDnsRegisterHostedService>>();
            
            _things = new List<Thing>();
        }

        [Fact]
        public async Task Start_Should_NotRegister_When_RegisterMDNSsIsFalse()
        {
            var serverName = _fixture.Create<string>();
            _option.RegistermDNS = false;
            _option.ServerName = serverName;
            
            var host = new MDnsRegisterHostedService(_server, _option, _things, _logger);
            await host.StartAsync(CancellationToken.None);
            
            using var discovery = new ServiceDiscovery();
            discovery.ServiceDiscovered += (sender, name) =>
            {
                name.Labels.Contains(serverName).Should().BeFalse();
            };
            
            discovery.ServiceInstanceDiscovered += (sender, args) => 
            {
                args.ServiceInstanceName.Labels.Contains(serverName).Should().BeFalse();
            };

            await Task.Delay(TimeSpan.FromSeconds(5));
            await host.StopAsync(CancellationToken.None);
        }

        [Fact]
        public async Task Start_Should_Register_When_RegisterMDNSsIsTrue()
        {
            _option.RegistermDNS = true;
            _option.ServerName = string.Empty;
            
            var addresses = new ServerAddressesFeature();
            addresses.Addresses.Add("local:8888");
            _server.Features.Get<IServerAddressesFeature>().Returns(addresses);
            
            _things.Add(new FakeThing());
            
            var find = false;
            using var discovery = new ServiceDiscovery();
            discovery.ServiceDiscovered += (sender, name) =>
            {
                if (name.Labels.Contains("Fake"))
                {
                    find = true;
                }
            };
            
            discovery.ServiceInstanceDiscovered += (sender, args) => 
            {
                if (args.ServiceInstanceName.Labels.Contains("Fake"))
                {
                    find = true;
                }
            };

            var host = new MDnsRegisterHostedService(_server, _option, _things, _logger);
            await host.StartAsync(CancellationToken.None);
            
            await Task.Delay(TimeSpan.FromSeconds(10));
            find.Should().BeTrue();
            
            await host.StopAsync(CancellationToken.None);
        }
        
        [Fact]
        public async Task Start_Should_Register_When_RegisterMDNSsIsTrueAndHaveTls()
        {
            _option.RegistermDNS = true;
            _option.ServerName = string.Empty;
            
            var addresses = new ServerAddressesFeature();
            addresses.Addresses.Add("local:8888");
            _server.Features.Get<IServerAddressesFeature>().Returns(addresses);

            var tls = Substitute.For<ITlsHandshakeFeature>();
            tls.Protocol.Returns(SslProtocols.Tls);
            _server.Features.Get<ITlsHandshakeFeature>().Returns(tls);
            
            _things.Add(new FakeThing());

            var find = false;
            using var discovery = new ServiceDiscovery();
            
            discovery.ServiceDiscovered += (sender, name) =>
            {
                if (name.Labels.Contains("Fake"))
                {
                    find = true;
                }
            };
            
            discovery.ServiceInstanceDiscovered += (sender, args) => 
            {
                if (args.ServiceInstanceName.Labels.Contains("Fake"))
                {
                    find = true;
                }
            };

            var host = new MDnsRegisterHostedService(_server, _option, _things, _logger);
            await host.StartAsync(CancellationToken.None);
            
            await Task.Delay(TimeSpan.FromSeconds(10));
            find.Should().BeTrue();
            
            await host.StopAsync(CancellationToken.None);
        }

        private class FakeThing : Thing
        {
            public override string Name => "Fake";
        }
    }
}
