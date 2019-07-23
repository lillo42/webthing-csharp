using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Hosting;

namespace Mozilla.IoT.WebThing.Background
{
    public class ActionExecutorHostedService : BackgroundService
    {
        private readonly ISourceBlock<Action> _actions;
        private readonly LinkedList<ConfiguredTaskAwaitable> _tasks = new LinkedList<ConfiguredTaskAwaitable>();
        private readonly IServiceProvider _provider;

        public ActionExecutorHostedService(ISourceBlock<Action> actions, IServiceProvider provider)
        {
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var action = await _actions.ReceiveAsync(stoppingToken)
                    .ConfigureAwait(false);

                ConfiguredTaskAwaitable task = action.StartAsync(_provider, stoppingToken)
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
