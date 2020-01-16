using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.EventArgs;

namespace Mozilla.IoT.WebThing
{
    public abstract class Action
    {
        #region Constructor

        protected Action(string id, string name, ILogger logger)
        {
            Name = name;
            Id = id;
            Logger = logger;
        }
        
        protected Action(string name, ILogger logger)
            : this(Guid.NewGuid().ToString(), name, logger)
        {
            
        }

        #endregion

        #region Properties
        protected virtual ILogger Logger { get; }
        public virtual string Id { get; }
        public string Name { get; }

        public DateTime TimeRequested { get; } = DateTime.UtcNow;
        
        public DateTime TimeCompleted { get; private set; }

        private Status _status = Status.Pending;

        public Status Status
        {
            get => _status;
            private set
            {
                _status = value;
                var @event = ActionStatusChanged;
                @event?.Invoke(this, new ActionStatusChangedEventArgs(this));
            } 
        }
        
        #endregion

        public event EventHandler<ActionStatusChangedEventArgs> ActionStatusChanged; 

        protected abstract ValueTask ExecuteAsync(CancellationToken cancellation);

        public async ValueTask StartAsync(CancellationToken cancellationToken = default)
        {
            Status = Status.Executing;

            try
            {
                Logger.LogInformation("Executing [Action Id: {id}][Action Name: {name}]", Id, Name);
                await ExecuteAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error to execute [Action Id: {id}][Action Name: {name}]", Id, Name);
            }
            finally
            {
                TimeCompleted = DateTime.UtcNow;
                Status = Status.Completed;
                Logger.LogInformation("Action finish [Action Id: {id}][Action Name: {name}]", Id, Name);
            }
        }
    }

    public abstract class Action<T> : Action
    {
        public T Data { get; }

        protected Action(string id, string name, T data, ILogger logger) 
            : base(id, name, logger)
        {
            Data = data;
        }

        protected Action(string name, T data, ILogger logger) 
            : base(name, logger)
        {
            Data = data;
        }
    }
}
