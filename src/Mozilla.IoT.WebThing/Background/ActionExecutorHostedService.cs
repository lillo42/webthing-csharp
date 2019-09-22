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
        private static readonly object locker = new object();

        private readonly LinkedList<ConfiguredValueTaskAwaitable> _tasks =
            new LinkedList<ConfiguredValueTaskAwaitable>();

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

                var task = action.StartAsync(_loggerFactory.CreateLogger(typeof(Action)), stoppingToken)
                    .ConfigureAwait(false);

                lock (locker)
                {
                    _tasks.AddLast(task);
                }

                task.GetAwaiter()
                    .OnCompleted(() =>
                    {
                        lock (locker)
                        {
                            _tasks.Remove(task);
                        }
                    });
            }

            ImmutableArray<ConfiguredValueTaskAwaitable> immutable;

            lock (locker)
            {
                immutable = _tasks.ToImmutableArray();
            }

            foreach (var task in immutable)
            {
                await task;
            }

            _tasks.Clear();
        }
    }
}
