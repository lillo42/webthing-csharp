using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Actions
{
    public abstract class ActionInfo
    {
        private readonly Guid _id = Guid.NewGuid();
        protected CancellationTokenSource Source { get; } = new CancellationTokenSource();
        internal Thing? Thing { get; set; }
        public string Href { get; set; }

        public DateTime TimeRequested { get; } = DateTime.UtcNow;
        public DateTime? TimeCompleted { get; private set; } = null;
        
        private Status _status = Status.Pending;

        public Status Status
        {
            get => _status;
            private set
            {
                _status = value;
                StatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        
        protected abstract ValueTask InternalExecuteAsync(Thing thing, IServiceProvider provider);

        public async Task ExecuteAsync(Thing thing, IServiceProvider provider)
        {
            Status = Status.Pending;
            var logger = provider.GetRequiredService<ILogger<ActionInfo>>();
            logger.LogInformation("Going to execute {actionName}", GetActionName());
            Status = Status.Executing;

            try
            {
                await InternalExecuteAsync(thing, provider)
                    .ConfigureAwait(false);
                
                logger.LogInformation("{actionName} to executed", GetActionName());
            }
            catch (Exception e)
            {
                logger.LogError(e,"Error to execute {actionName}", GetActionName());
            }
            
            TimeCompleted = DateTime.UtcNow;
            Status = Status.Completed;
        }
        
        public abstract string GetActionName();

        public Guid GetId()
            => _id;
        
        public void Cancel()
            => Source.Cancel();

        public event EventHandler? StatusChanged;
    }
}
