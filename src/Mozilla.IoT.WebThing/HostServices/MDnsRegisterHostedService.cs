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
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Extensions;

namespace Mozilla.IoT.WebThing.HostServices
{
    /// <summary>
    /// The Hosted Service responsible for register this service in mDNS
    /// </summary>
    public class MDnsRegisterHostedService : IHostedService
    {
        private readonly ServiceDiscovery _discovery;
        private readonly string _name;
        private readonly ThingOption _option;
        private readonly IServer _server;
        private readonly ICollection<ServiceProfile> _profiles;
        private readonly ILogger<MDnsRegisterHostedService> _logger;

        /// <summary>
        /// Initialize a new instance of <see cref="MDnsRegisterHostedService"/>.
        /// </summary>
        /// <param name="server">The <see cref="IServer"/> with feature.</param>
        /// <param name="options">The <see cref="ThingOption"/>.</param>
        /// <param name="things">All things for in the application. It's use for determinate the server name if isn't set.</param>
        /// <param name="logger">The <see cref="ILogger{TCategoryName}"/></param>
        public MDnsRegisterHostedService(IServer server, 
            ThingOption options, 
            IEnumerable<Thing> things, 
            ILogger<MDnsRegisterHostedService> logger)
        {
            _discovery = new ServiceDiscovery();
            _profiles = new List<ServiceProfile>();
            _server = server ?? throw new ArgumentNullException(nameof(server));
            _option = options;
            _logger = logger;
            _name = options.ServerName;

            if (string.IsNullOrEmpty(_name))
            {
                _name = things.First().Name;
            }
        }
        
        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("Going to check if is necessary register.");
            if (_option.RegistermDNS)
            {
                _logger.LogTrace("Going to register Service in mDNS.");
                Task.Run(() =>
                {
                    _logger.LogTrace("Going to wait {delay} before register.", _option.MDnsDelay.ToString("G"));
                    Task.Delay(_option.MDnsDelay, cancellationToken).GetAwaiter().GetResult();

                    var tls = _server.Features.Get<ITlsHandshakeFeature>();
                    var hasTls = tls != null && tls.Protocol != SslProtocols.None;

                    var addresses = _server.Features.Get<IServerAddressesFeature>()?.Addresses ?? new List<string>();
                    
                    foreach (var address in addresses)
                    {
                        var index = address.LastIndexOf(':');
                        if (index < 0 || index + 1 == address.Length)
                        {
                            _logger.LogWarning("Going to ignore {address} address, because it has a invalid format", address);
                            continue;
                        }
                        
                        var port = ushort.Parse(address.AsSpan().Slice(index + 1));

                        var profile = new ServiceProfile("_webthing._tcp.local", _name, port);
                        profile.AddProperty("path", "/");

                        if (hasTls)
                        {
                            profile.AddProperty("tls", "1");
                        }

                        _profiles.Add(profile);
                        
                        _logger.LogInformation("Advertising and Announcing the {serviceName} in {port} port", _name, port);
                        _discovery.Advertise(profile);
                        _discovery.Announce(profile);
                    }
                }, cancellationToken);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var profile in _profiles)
            {
                _logger.LogInformation("Unadvertising the {serviceName}", profile.ServiceName);
                _discovery.Unadvertise(profile);
            }
            
            return Task.CompletedTask;
        }
    }
}
