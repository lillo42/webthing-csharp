using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Hosting;

namespace Mozilla.IoT.WebThing.HostedServices
{
    public class ActionExecutorHostedService : IHostedService
    {
        private readonly ISourceBlock<Action> _actions;
        private readonly LinkedList<ConfiguredTaskAwaitable> _tasks = new LinkedList<ConfiguredTaskAwaitable>();

        public ActionExecutorHostedService(ISourceBlock<Action> actions)
        {
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var action = await _actions.ReceiveAsync(cancellationToken)
                    .ConfigureAwait(false);

                ConfiguredTaskAwaitable task = action.StartAsync(cancellationToken)
                    .ConfigureAwait(false);

                _tasks.AddLast(task);
                
                task.GetAwaiter()
                    .OnCompleted(() => _tasks.Remove(task));
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
