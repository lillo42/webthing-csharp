using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.Factories.Generator.Intercepts
{
    /// <summary>
    /// Intercept <see cref="Thing"/> Event.
    /// </summary>
    public interface IEventIntercept : IIntercept
    {
        /// <summary>
        /// Intercept <see cref="Thing"/> Event.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/>.</param>
        /// <param name="event">The method intercept.</param>
        /// <param name="eventInfo">The <see cref="ThingEventAttribute"/>.</param>
        void Visit(Thing thing, EventInfo @event, ThingEventAttribute? eventInfo);
    }
}
