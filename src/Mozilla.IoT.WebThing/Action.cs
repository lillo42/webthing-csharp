using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing
{
    public abstract class Action
    {
        /// <summary>
        /// Action's ID.
        /// </summary>
        public virtual string Id { get; } = Guid.NewGuid().ToString();
        
        /// <summary>
        /// Action's name.
        /// </summary>
        public virtual string Name { get; set; }
        
        /// <summary>
        /// Action's href.
        /// </summary>
        public virtual string Href { get; set; }

        /// <summary>
        /// The prefix of any hrefs associated with this action.
        /// </summary>
        public virtual string HrefPrefix { get; set; } = string.Empty;
        
        /// <summary>
        /// Action's status.
        /// </summary>
        public virtual Status Status { get; private set; }
        
        /// <summary>
        /// The time the action was requested.
        /// </summary>
        public virtual DateTime TimeRequested { get; }

        /// <summary>
        /// The time the action was completed.
        /// </summary>
        public virtual DateTime? TimeCompleted { get; private set; }
        
        /// <summary>
        /// The inputs for this action.
        /// </summary>
        public virtual IDictionary<string, object> Input { get; internal set; }
        
        
        public virtual IDictionary<string, object> Metadata { get; set; }
        
        /// <summary>
        /// The thing associated with this action.
        /// </summary>
        public Thing Thing { get; set; }

        protected Action()
        {
            TimeRequested = DateTime.UtcNow;
            Status = Status.Created;
        }
        
        protected abstract Task ExecuteAsync(CancellationToken cancellation);

        public async Task StartAsync(IServiceProvider service, CancellationToken cancellation)
        {
            Status = Status.Pending;

            try
            {
                await ExecuteAsync(cancellation)
                    .ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                var logger = service.GetService<ILogger>();
                logger.LogError(exception, $"Error to executor action: {ToString()}");
            }

            Status = Status.Completed;
            TimeCompleted = DateTime.UtcNow;
        }

        public override string ToString()
            => $"[{nameof(Id)}: {Id}]" +
            $"[{nameof(Name)}: {Name}]" +
            $"[{nameof(Href)}: {Href}]" +
            $"[{nameof(HrefPrefix)}: {HrefPrefix}]" +
            $"[{nameof(TimeRequested)}: {TimeRequested}]";
    }
}
