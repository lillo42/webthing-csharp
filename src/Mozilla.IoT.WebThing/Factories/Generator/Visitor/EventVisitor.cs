using System;
using System.Collections.Generic;
using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator.Visitor
{
    internal static class EventVisitor
    {
        public static void Visit(IEnumerable<IEventIntercept> intercepts, Thing thing)
        {
            var thingType = thing.GetType();
            var events = thingType.GetEvents(BindingFlags.Public | BindingFlags.Instance);

            foreach (var intercept in intercepts)
            {
                intercept.Before(thing);
            }

            foreach (var @event in events)
            {
                var args = @event.EventHandlerType.GetGenericArguments();
                if (args.Length > 1)
                {
                    continue;
                }

                if ((args.Length == 0 && @event.EventHandlerType != typeof(EventHandler))
                    || (args.Length == 1 && @event.EventHandlerType != typeof(EventHandler<>).MakeGenericType(args[0])))
                {
                    continue;
                }
                

                var information = @event.GetCustomAttribute<ThingEventAttribute>();

                if (information != null && information.Ignore)
                {
                    continue;
                }
                
                foreach (var intercept in intercepts)
                {
                    intercept.Visit(thing, @event, information);
                }
            }

            foreach (var intercept in intercepts)
            {
                intercept.After(thing);
            }
        }
    }
}
