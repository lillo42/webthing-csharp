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
        private readonly ChannelReader<Action> _actions;
        private readonly ILoggerFactory _loggerFactory;
        public ActionExecutorHostedService(ChannelReader<Action> actions, ILoggerFactory loggerFactory)
        {
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach(var action in _actions.ReadAllAsync(stoppingToken)
                .ConfigureAwait(false))
            {
                action.StartAsync(_loggerFactory.CreateLogger(typeof(Action)), stoppingToken)
                    .ConfigureAwait(false);
            }
        }
    }
}
