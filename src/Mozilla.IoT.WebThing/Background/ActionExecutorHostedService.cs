using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Background
{
    public class ActionExecutorHostedService : BackgroundService
    {
        private readonly ISourceBlock<Action> _actions;
        private readonly LinkedList<ConfiguredTaskAwaitable> _tasks = new LinkedList<ConfiguredTaskAwaitable>();
        private readonly ILoggerFactory _loggerFactory;

        public ActionExecutorHostedService(ISourceBlock<Action> actions, ILoggerFactory loggerFactory)
        {
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var action = await _actions.ReceiveAsync(stoppingToken)
                    .ConfigureAwait(false);

                ConfiguredTaskAwaitable task = action.StartAsync(_loggerFactory.CreateLogger(typeof(Action)), stoppingToken)
                    .ConfigureAwait(false);

                _tasks.AddLast(task);
                
                task.GetAwaiter()
                    .OnCompleted(() => _tasks.Remove(task));
            }

            foreach (ConfiguredTaskAwaitable task in _tasks.ToImmutableArray())
            {
                await task;
            }
            
            _tasks.Clear();
        }
    }
}
