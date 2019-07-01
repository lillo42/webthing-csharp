using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Mozilla.IoT.WebThing.Test")]
namespace Mozilla.IoT.WebThing
{
    /// <summary>
    /// An Action represents an individual action on a thing.
    /// </summary>
    public abstract class Action
    {
        private const string HREF = "href";
        private const string TIME_REQUESTED = "timeRequested";
        private const string STATUS = "status";
        private const string INPUT = "input";
        private const string TIME_COMPLETED = "timeCompleted";

        /// <summary>
        /// Action's ID.
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// The thing associated with this action.
        /// </summary>
        public Thing Thing { get; }

        /// <summary>
        /// Action's name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The prefix of any hrefs associated with this action.
        /// </summary>
        public string HrefPrefix { get; set; }

        /// <summary>
        /// Action's href.
        /// </summary>
        public string Href { get; }

        /// <summary>
        /// Action's status.
        /// </summary>
        public Status Status { get; private set; }

        /// <summary>
        /// The time the action was requested.
        /// </summary>
        public DateTime TimeRequested { get; }

        /// <summary>
        /// The time the action was completed.
        /// </summary>
        public DateTime? TimeCompleted { get; private set; }

        /// <summary>
        /// The inputs for this action.
        /// </summary>
        public IDictionary<string, object> Input { get; }


        
        internal Action()
            : this(null, null)
        {
        }

        protected Action(Thing thing, IDictionary<string, object> input)
        {
            Thing = thing;
            Input = input;
            HrefPrefix = string.Empty;
            Href = $"/actions/{Name}/{Id}";
            TimeRequested = DateTime.UtcNow;
            Status = Status.Created;
        }

        /// <summary>
        /// Get the action description.
        /// </summary>
        /// <returns>Description of the action as a JSONObject.</returns>
        public virtual IDictionary<string, object> AsActionDescription()
        {
            var inner = new Dictionary<string, object>
            {
                [HREF] = HrefPrefix.JoinUrl(Href),
                [TIME_REQUESTED] = TimeRequested,
                [STATUS] = Status.ToString().ToLower()
            };

            if (Input != null && Input.Any()) 
            {
                inner.Add(INPUT, Input);
            }

            if (TimeCompleted.HasValue)
            {
                inner.Add(TIME_COMPLETED, TimeCompleted);
            }

            return inner;
        }

        /// <summary>
        /// Start performing the action
        /// </summary>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellation)
        {
            Status = Status.Pending;
            await PerformActionAsync(cancellation)
                .ConfigureAwait(false);

            Finish();
        }

        protected abstract Task PerformActionAsync(CancellationToken cancellation);


        /// <summary>
        /// Finish performing the action.
        /// </summary>
        public void Finish()
        {
            Status = Status.Completed;
            TimeCompleted = DateTime.UtcNow;
        }
    }
}
