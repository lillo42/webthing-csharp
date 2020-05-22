using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.Factories
{
    /// <inheritdoc /> 
    public class ThingContextFactory : IThingContextFactory
    {
        private readonly IThingResponseBuilder _response;
        private readonly IActionBuilder _action;
        private readonly IEventBuilder _event;
        private readonly IPropertyBuilder _property;

        /// <summary>
        /// Initialize a new instance of <see cref="IEventBuilder"/>.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="property"></param>
        /// <param name="response"></param>
        /// <param name="action"></param>
        public ThingContextFactory(IEventBuilder @event, IPropertyBuilder property,
            IThingResponseBuilder response, IActionBuilder action)
        {
            _event = @event;
            _property = property;
            _response = response;
            _action = action;
        }

        /// <inheritdoc /> 
        public ThingContext Create(Thing thing, ThingOption option)
        {
            var thingType = thing.GetType();

            _response
                .SetThing(thing)
                .SetThingOption(option);

            _event
                .SetThing(thing)
                .SetThingOption(option)
                .SetThingType(thingType);

            _property
                .SetThing(thing)
                .SetThingOption(option);

            _action
                .SetThing(thing)
                .SetThingOption(option)
                .SetThingType(thingType);

            VisitEvent(thingType);
            VisitProperty(thingType);
            VisitAction(thingType);

            return new ThingContext(
                _response.Build(),
                _event.Build(),
                _action.Build(),
                _property.Build());
        }

        private static readonly Type s_eventHandler = typeof(EventHandler);
        private static readonly Type s_eventHandlerGeneric = typeof(EventHandler<>);

        private void VisitEvent(Type thingType)
        {
            var events = thingType.GetEvents(BindingFlags.Public | BindingFlags.Instance);

            foreach (var @event in events)
            {
                var args = @event.EventHandlerType!.GetGenericArguments();
                if (args.Length > 1)
                {
                    continue;
                }

                if ((args.Length == 0 && @event.EventHandlerType != s_eventHandler)
                    || (args.Length == 1 && @event.EventHandlerType != s_eventHandlerGeneric.MakeGenericType(args[0])))
                {
                    continue;
                }

                var information = @event.GetCustomAttribute<ThingEventAttribute>();

                if (information != null && information.Ignore)
                {
                    continue;
                }

                _event.Add(@event, information);
                _response.Add(@event, information);
            }
        }

        private void VisitProperty(Type thingType)
        {
            var properties = thingType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => !IsThingProperty(x.Name));

            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                var jsonType = propertyType.ToJsonType();
                if (jsonType == JsonType.Object)
                {
                    continue;
                }

                var attribute = property.GetCustomAttribute<ThingPropertyAttribute>();

                if (attribute != null && attribute.Ignore)
                {
                    continue;
                }

               
                var information = attribute!.ToJsonSchema(property);

                _property.Add(property, information);
                _response.Add(property, attribute, information);
            }
            
            static bool IsThingProperty(string name)
                => name == nameof(Thing.Context)
                   || name == nameof(Thing.Name)
                   || name == nameof(Thing.Description)
                   || name == nameof(Thing.Title)
                   || name == nameof(Thing.Type)
                   || name == nameof(Thing.ThingContext);
        }

        private void VisitAction(Type thingType)
        {
            var methods = thingType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => !x.IsSpecialName
                            && x.Name != nameof(Equals) && x.Name != nameof(GetType)
                            && x.Name != nameof(GetHashCode) && x.Name != nameof(ToString));

            foreach (var method in methods)
            {
                var methodAttribute = method.GetCustomAttribute<ThingActionAttribute>();
                if (methodAttribute != null && methodAttribute.Ignore)
                {
                    continue;
                }

                _response.Add(method, methodAttribute);
                _action.Add(method, methodAttribute);

                foreach (var parameter in method.GetParameters())
                {
                    if (parameter.ParameterType == typeof(CancellationToken)
                        || parameter.GetCustomAttribute<FromServicesAttribute>() != null)
                    {
                        continue;
                    }
                    
                    var attribute = parameter.GetCustomAttribute<ThingParameterAttribute>();
                    var information = attribute!.ToJsonSchema(parameter);

                    _action.Add(parameter, information);
                    _response.Add(parameter, attribute, information);
                }
            }
        }
    }
}
