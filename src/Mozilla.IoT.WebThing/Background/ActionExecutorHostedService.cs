using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Background
{
    public class ActionExecutorHostedService : BackgroundService
    {
        private readonly ChannelReader<Action> _reader;
        private readonly ILoggerFactory _loggerFactory;

        public ActionExecutorHostedService(ChannelReader<Action> reader, ILoggerFactory loggerFactory)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var action in _reader.ReadAllAsync(stoppingToken))
            {
#pragma warning disable 4014
                action.StartAsync(_loggerFactory.CreateLogger(typeof(Action)), stoppingToken)
                    .ConfigureAwait(false);
#pragma warning restore 4014
            }
        }
    }
}
