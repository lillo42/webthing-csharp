using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Events;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator.Events
{
    /// <inheritdoc /> 
    public class EventIntercept : IEventIntercept
    {
        /// <summary>
        /// The <see cref="EventCollection"/> created, map by action name.
        /// </summary>
        public Dictionary<string, EventCollection> Events { get; }
        
        private readonly Queue<EventInfo> _eventToBind = new Queue<EventInfo>();

        private readonly ConstructorInfo _createThing = typeof(Event).GetConstructors(BindingFlags.Public | BindingFlags.Instance)[0];
        private readonly MethodInfo _getContext = typeof(Thing).GetProperty(nameof(Thing.ThingContext))?.GetMethod!;
        private readonly MethodInfo _getEvent = typeof(ThingContext).GetProperty(nameof(ThingContext.Events))?.GetMethod!;
        private readonly MethodInfo _getItem = typeof(Dictionary<string, EventCollection>).GetMethod("get_Item")!;
        private readonly MethodInfo _addItem = typeof(EventCollection).GetMethod(nameof(EventCollection.Enqueue))!;
        private readonly ThingOption _options;
        private readonly TypeBuilder _builder;

        /// <summary>
        /// Initialize a new instance of <see cref="EventIntercept"/>.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        public EventIntercept(TypeBuilder builder, ThingOption options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
            
            Events = options.IgnoreCase ? new Dictionary<string, EventCollection>(StringComparer.OrdinalIgnoreCase) 
                : new Dictionary<string, EventCollection>();
        }

        /// <inheritdoc />
        public void Before(Thing thing)
        {
            
        }

        /// <inheritdoc />
        public void After(Thing thing)
        {
            var type = _builder.CreateType()!;
            while (_eventToBind.TryDequeue(out var @event))
            {
                var @delegate = Delegate.CreateDelegate(@event.EventHandlerType!, type, $"{@event.Name}Handler");
                @event.AddEventHandler(thing, @delegate );
            }
        }

        /// <inheritdoc />
        public void Visit(Thing thing, EventInfo @event, ThingEventAttribute? eventInfo)
        {
            _eventToBind.Enqueue(@event);
            var name = eventInfo?.Name ?? @event.Name;
            Events.Add(_options.PropertyNamingPolicy.ConvertName(name), new EventCollection(_options.MaxEventSize));

            var type = @event.EventHandlerType?.GetGenericArguments()[0]!;
            var methodBuilder =_builder.DefineMethod($"{@event.Name}Handler",
                MethodAttributes.Public | MethodAttributes.Static);

            methodBuilder.SetParameters(typeof(object), type);
            
            var il = methodBuilder.GetILGenerator();
            
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, thing.GetType());
            il.EmitCall(OpCodes.Callvirt, _getContext, null);
            il.EmitCall(OpCodes.Callvirt, _getEvent, null);
            il.Emit(OpCodes.Ldstr, name);
            il.EmitCall(OpCodes.Callvirt, _getItem, null);
            il.Emit(OpCodes.Ldarg_1);

            if (type.IsValueType)
            {
                il.Emit(OpCodes.Box, type);
            }
            
            il.Emit(OpCodes.Newobj, _createThing);
            il.Emit(OpCodes.Ldstr, _options.PropertyNamingPolicy.ConvertName(name));
            il.EmitCall(OpCodes.Callvirt, _addItem, null);
            il.Emit(OpCodes.Ret);
        }
    }
}
