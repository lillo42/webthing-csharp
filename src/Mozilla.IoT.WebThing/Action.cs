using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing
{
    public abstract class Action : IEquatable<Action>
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

        private Status _status = Status.Created;
        /// <summary>
        /// Action's status.
        /// </summary>
        public virtual Status Status
        {
            get => _status;
            protected set
            {
                _status = value;
                var @event = ActionStatusChanged;
                @event?.Invoke(this, new ActionStatusChangedEventArgs(this));
            } 
        }
        
        /// <summary>
        /// The time the action was requested.
        /// </summary>
        public virtual DateTime TimeRequested { get; } = DateTime.UtcNow;

        /// <summary>
        /// The time the action was completed.
        /// </summary>
        public virtual DateTime? TimeCompleted { get; private set; }
        
        /// <summary>
        /// The inputs for this action.
        /// </summary>
        public virtual IDictionary<string, object> Input { get; internal set; }
        
        /// <summary>
        /// The thing associated with this action.
        /// </summary>
        public virtual Thing Thing { get; set; }

        protected abstract ValueTask ExecuteAsync(CancellationToken cancellation);

        public async ValueTask StartAsync(ILogger logger, CancellationToken cancellation)
        {
            Status = Status.Pending;

            try
            {
                await ExecuteAsync(cancellation)
                    .ConfigureAwait(false);
            }
            catch (Exception exception)
            {
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
            $"[{nameof(TimeRequested)}: {TimeRequested}]]";

        public bool Equals(Action other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(Id, other.Id) 
                   && string.Equals(Name, other.Name) 
                   && string.Equals(Href, other.Href)
                   && string.Equals(HrefPrefix, other.HrefPrefix) 
                   && Status == other.Status 
                   && TimeRequested.Equals(other.TimeRequested)
                   && TimeCompleted.Equals(other.TimeCompleted) 
                   && Equals(Input, other.Input)
                   && Equals(Thing, other.Thing);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((Action) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Href != null ? Href.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (HrefPrefix != null ? HrefPrefix.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) Status;
                hashCode = (hashCode * 397) ^ TimeRequested.GetHashCode();
                hashCode = (hashCode * 397) ^ TimeCompleted.GetHashCode();
                hashCode = (hashCode * 397) ^ (Input != null ? Input.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Thing != null ? Thing.GetHashCode() : 0);
                return hashCode;
            }
        }
        public event EventHandler<ActionStatusChangedEventArgs> ActionStatusChanged;
    }

//    public abstract class Action<TInput> : Action
//    {
//        public TInput Input { get; internal set; }
//    }
}
