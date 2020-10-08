using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Makaretu.Dns;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Hosting;
using Mozilla.IoT.WebThing.Extensions;

namespace Mozilla.IoT.WebThing.HostServices
{
    internal class MDnsRegister : IHostedService
    {
        private readonly ServiceDiscovery _discovery;
        private readonly string _name;
        private readonly ThingOption _option;
        private readonly IServer _server;
        private readonly ICollection<ServiceProfile> _profiles;

        public MDnsRegister(IServer addressesFeature, ThingOption options, IEnumerable<Thing> things)
        {
            _discovery = new ServiceDiscovery();
            _profiles = new List<ServiceProfile>();
            _server = addressesFeature ?? throw new ArgumentNullException(nameof(addressesFeature));
            _option = options;
            _name = options.ServerName;

            if (string.IsNullOrEmpty(_name))
            {
                _name = things.First().Name;
            }
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_option.UseMultiDNS)
            {
                Task.Run(() =>
                {
                    Task.Delay(_option.MDnsDelay, cancellationToken).GetAwaiter().GetResult();

                    var tls = _server.Features.Get<ITlsHandshakeFeature>();
                    var hasTls = tls != null && tls.Protocol != SslProtocols.None;

                    var addresses = _server.Features.Get<IServerAddressesFeature>()?.Addresses ?? new List<string>();
                    
                    foreach (var address in addresses)
                    {
                        var port = ushort.Parse(address.AsSpan().Slice(address.LastIndexOf(':')));

                        var profile = new ServiceProfile("_webthing._tcp.local", _name, port);
                        profile.AddProperty("path", "/");

                        if (hasTls)
                        {
                            profile.AddProperty("tls", "1");
                        }

                        _profiles.Add(profile);
                        _discovery.Advertise(profile);
                        _discovery.Announce(profile);
                    }
                }, cancellationToken);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var profile in _profiles)
            {
                _discovery.Unadvertise(profile);
            }
            
            return Task.CompletedTask;
        }
    }
}
