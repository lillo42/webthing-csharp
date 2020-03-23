using System;
using System.Collections.Generic;
using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Events;
using Mozilla.IoT.WebThing.Extensions;

namespace Mozilla.IoT.WebThing.Builders
{
    /// <summary>
    /// Create event bind.
    /// </summary>
    public interface IEventBuilder
    {
        /// <summary>
        /// Set <see cref="Thing"/>.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/> to be set.</param>
        /// <returns></returns>
        IEventBuilder SetThing(Thing thing);
        
        /// <summary>
        /// Set <see cref="Thing"/> type.
        /// </summary>
        /// <param name="thingType">The <see cref="Thing"/> typeto be set.</param>
        /// <returns></returns>
        IEventBuilder SetThingType(Type thingType);

        /// <summary>
        /// Set <see cref="ThingOption"/>
        /// </summary>
        /// <param name="option">The <see cref="ThingOption"/> to be set.</param>
        /// <returns></returns>
        IEventBuilder SetThingOption(ThingOption option);
        
        /// <summary>
        /// Add event.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <param name="eventInfo">Extra information about event</param>
        void Add(EventInfo @event, ThingEventAttribute? eventInfo);

        /// <summary>
        /// Build the <see cref="Dictionary{TKey,TValue}"/>
        /// </summary>
        /// <returns>New <see cref="Dictionary{TKey,TValue}"/> of the <see cref="EventCollection"/></returns>
        Dictionary<string, EventCollection> Build();
    }
}
