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
        public DateTime? TimeCompleted { get; protected set; } = null;
        public Status Status { get; protected set; } = Status.Pending;


        public abstract bool IsValid();
        protected abstract ValueTask ExecuteAsync(Thing thing);
        public virtual async Task ExecuteAsync(Thing thing, ILogger<ActionInfo> logger)
        {
            logger.LogInformation("Going to execute {actionName}", ActionName);
            Status = Status.Executing;
            
            try
            {
                await ExecuteAsync(thing)
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
