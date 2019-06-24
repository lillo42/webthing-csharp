using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Mozilla.IoT.WebThing.Extensions;
using Newtonsoft.Json.Linq;

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
        public JObject Input { get; }


        
        internal Action()
            : this(null, null)
        {
        }

        protected Action(Thing thing, JObject input)
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
        public JObject AsActionDescription()
        {
            var inner = new JObject(
                new JProperty(HREF, HrefPrefix.JoinUrl(Href)),
                new JProperty(TIME_REQUESTED, TimeRequested),
                new JProperty(STATUS, Status.ToString().ToLower()));

            if (Input != null && Input.HasValues) 
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
