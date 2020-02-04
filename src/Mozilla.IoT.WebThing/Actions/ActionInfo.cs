using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Actions
{
    public abstract class ActionInfo
    {
        internal Guid Id { get; } = Guid.NewGuid();
        internal Thing Thing { get; set; } = default!;
        protected abstract string ActionName { get; }
        
        public string Href => $"/things/{Thing.Name}/actions/{ActionName}/{Id}";
        
        public DateTime TimeRequested { get; } = DateTime.UtcNow;
        public DateTime? TimeCompleted { get; private set; } = null;
        public Status Status { get; private set; } = Status.Pending;


        public abstract bool IsValid();
        protected abstract ValueTask InternalExecuteAsync(Thing thing, IServiceProvider provider);
        public virtual async Task ExecuteAsync(Thing thing, IServiceProvider provider)
        {
            var logger = provider.GetRequiredService<ILogger<ActionInfo>>();
            logger.LogInformation("Going to execute {actionName}", ActionName);
            Status = Status.Executing;
            
            try
            {
                await InternalExecuteAsync(thing, provider)
                    .ConfigureAwait(false);
                
                logger.LogInformation("{actionName} to executed", ActionName);
            }
            catch (Exception e)
            {
                logger.LogError(e,"Error to execute {actionName}", ActionName);
            }
            
            TimeCompleted = DateTime.UtcNow;
            Status = Status.Completed;
        }
    }
}
