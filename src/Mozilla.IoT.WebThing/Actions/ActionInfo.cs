using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Actions
{
    public abstract class ActionInfo
    {
        internal Guid Id { get; } = Guid.NewGuid();
        internal Thing Thing { get; set; }
        protected abstract string ActionName { get; }
        
        public string Href => $"/things/{Thing.Name}/actions/{ActionName}/{Id}";
        
        public DateTime TimeRequested { get; } = DateTime.UtcNow;
        public DateTime? TimeCompleted { get; protected set; } = null;
        public Status Status { get; protected set; } = Status.Pending;


        public abstract bool IsValid();
        protected virtual ValueTask ExecuteAsync() => new ValueTask();
        public virtual async Task ExecuteAsync(ILogger<ActionInfo> logger)
        {
            logger.LogInformation("Going to execute {actionName}", ActionName);
            Status = Status.Executing;
            
            try
            {
                await ExecuteAsync()
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
