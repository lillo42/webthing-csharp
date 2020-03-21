using System.Collections.Generic;
using System.Reflection;
using Mozilla.IoT.WebThing.Events;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator.Events
{
    /// <inheritdoc/> 
    public class EventInterceptFactory : IInterceptorFactory
    {
        private readonly EventIntercept _intercept;
        private readonly EmptyIntercept _empty;

        /// <summary>
        /// Initialize a new instance of <see cref="EventInterceptFactory"/>.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/>.</param>
        /// <param name="options">The <see cref="ThingOption"/>.</param>
        public EventInterceptFactory(Thing thing, ThingOption options)
        {
            var thingType = thing.GetType();
            var builder = Factory.CreateTypeBuilder($"{thingType.Name}EventBinder", thingType.Name, 
                null, TypeAttributes.AutoClass | TypeAttributes.Class | TypeAttributes.Public);
            
            _intercept = new EventIntercept(builder, options);
            _empty = new EmptyIntercept();
        }

        /// <summary>
        /// The <see cref="EventCollection"/> created, map by action name.
        /// </summary>
        public Dictionary<string, EventCollection> Events => _intercept.Events;

        /// <summary>
        /// Return the <see cref="EmptyIntercept"/>.
        /// </summary>
        /// <returns>The <see cref="EmptyIntercept"/>.</returns>
        public IThingIntercept CreateThingIntercept()
            => _empty;

        
        /// <summary>
        /// Return the <see cref="EmptyIntercept"/>.
        /// </summary>
        /// <returns>The <see cref="EmptyIntercept"/>.</returns>
        public IPropertyIntercept CreatePropertyIntercept()
            => _empty;

        /// <summary>
        /// Return the <see cref="EmptyIntercept"/>.
        /// </summary>
        /// <returns>The <see cref="EmptyIntercept"/>.</returns>
        public IActionIntercept CreatActionIntercept()
            => _empty;

        /// <inheritdoc /> 
        public IEventIntercept CreatEventIntercept()
            => _intercept;
    }
}
