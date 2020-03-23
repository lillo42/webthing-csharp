using System;
using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;

namespace Mozilla.IoT.WebThing.Builders
{
    /// <summary>
    /// Create <see cref="ThingResponse"/>.
    /// </summary>
    public interface IThingResponseBuilder
    {
        /// <summary>
        /// Set <see cref="Thing"/>.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/> to be set.</param>
        /// <returns></returns>
        IThingResponseBuilder SetThing(Thing thing);
        
        /// <summary>
        /// Set <see cref="Thing"/> type.
        /// </summary>
        /// <param name="thingType">The <see cref="Thing"/> typeto be set.</param>
        /// <returns></returns>
        IThingResponseBuilder SetThingType(Type thingType);

        /// <summary>
        /// Set <see cref="ThingOption"/>
        /// </summary>
        /// <param name="option">The <see cref="ThingOption"/> to be set.</param>
        /// <returns></returns>
        IThingResponseBuilder SetThingOption(ThingOption option);
        
        /// <summary>
        /// Add event.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <param name="eventInfo">Extra information about event</param>
        void Add(EventInfo @event, ThingEventAttribute? eventInfo);

        /// <summary>
        /// Build the <see cref="ThingResponse"/>.
        /// </summary>
        /// <returns>New <see cref="ThingResponse"/>.</returns>
        ThingResponse Build();
    }
}
