using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Actions
{
    /// <summary>
    /// Action information to return in Web Socket and Web API.
    /// </summary>
    public abstract class ThingActionInformation
    {
        private readonly Guid _id = Guid.NewGuid();
        /// <summary>
        /// The <see cref="CancellationTokenSource"/> to cancel action when ask by <see cref="CancellationToken"/>. 
        /// </summary>
        protected CancellationTokenSource Source { get; } = new CancellationTokenSource();

        internal Thing? Thing { get; set; }

        /// <summary>
        /// The href of action.
        /// </summary>
        public string Href { get; set; } = string.Empty;

        /// <summary>
        /// The time when action was requested.
        /// </summary>
        public DateTime TimeRequested { get; } = DateTime.UtcNow;
        
        /// <summary>
        /// The time when action was completed
        /// </summary>
        public DateTime? TimeCompleted { get; private set; } = null;
        
        private ActionStatus _status = ActionStatus.Created;

        /// <summary>
        /// The <see cref="Status"/> of action.
        /// </summary>
        public ActionStatus Status
        {
            get => _status;
            private set
            {
                _status = value;
                StatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        
        /// <summary>
        /// To performance action executing.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/> associated with action to be executed.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> of scope to execute action.</param>
        /// <returns>The action executed or executing.</returns>
        protected abstract ValueTask InternalExecuteAsync(Thing thing, IServiceProvider provider);

        /// <summary>
        /// To Execute action.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/> associated with action to be executed.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> of scope to execute action.</param>
        /// <returns>Execute task async.</returns>
        public async Task ExecuteAsync(Thing thing, IServiceProvider provider)
        {
            Status = ActionStatus.Created;
            var logger = provider.GetRequiredService<ILogger<ThingActionInformation>>();
            logger.LogInformation("Going to execute {actionName}. [Thing: {thingName}]", GetActionName(), thing.Name);
            Status = ActionStatus.Pending;

            try
            {
                await InternalExecuteAsync(thing, provider)
                    .ConfigureAwait(false);
                
                logger.LogInformation("{actionName} to executed. [Thing: {thingName}", GetActionName(), thing.Name);
            }
            catch (Exception e)
            {
                logger.LogError(e,"Error to execute {actionName}. [Thing: {thingName}", GetActionName(), thing.Name);
            }
            
            TimeCompleted = DateTime.UtcNow;
            Status = ActionStatus.Completed;
        }
        
        /// <summary>
        /// The action name.
        /// </summary>
        /// <returns></returns>
        public abstract string GetActionName();

        /// <summary>
        /// The action Id.
        /// </summary>
        /// <returns></returns>
        public Guid GetId()
            => _id;
        
        /// <summary>
        /// To cancel action executing.
        /// </summary>
        public void Cancel()
            => Source.Cancel();

        /// <summary>
        /// The Status changed event. 
        /// </summary>
        public virtual event EventHandler? StatusChanged;
    }
}
