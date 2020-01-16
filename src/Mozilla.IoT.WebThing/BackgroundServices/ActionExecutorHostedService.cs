using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.BackgroundServices
{
    public class ActionExecutorHostedService : BackgroundService
    {
        private readonly ChannelReader<Action> _reader;
        private readonly ILogger<ActionExecutorHostedService> _logger;

        public ActionExecutorHostedService(ChannelReader<Action> reader, 
            ILogger<ActionExecutorHostedService> logger)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting ActionExecutor...");
            await foreach (var action in _reader.ReadAllAsync(stoppingToken))
            {
                _logger.LogTrace("Executing Action. [Id: {id}][Name: {name}]", action.Id, action.Name);
#pragma warning disable 4014
                var task = action.StartAsync(stoppingToken)
                    .ConfigureAwait(false);
#pragma warning restore 4014
            }
            
            _logger.LogInformation("Finish ActionExecutor");
        }
    }
}
