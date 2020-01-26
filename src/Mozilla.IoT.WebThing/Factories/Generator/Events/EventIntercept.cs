using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator.Events
{
    public class EventIntercept : IEventIntercept
    {
        private readonly Dictionary<string, ThingEventCollection> _events;
        private readonly Queue<EventInfo> _eventToBind = new Queue<EventInfo>();

        private readonly ConstructorInfo _createThing = typeof(ThingEvent).GetConstructors(BindingFlags.Public | BindingFlags.Instance)[0];
        private readonly MethodInfo _getContext = typeof(Thing).GetProperty(nameof(Thing.ThingContext)).GetMethod;
        private readonly MethodInfo _getEvent = typeof(ThingContext).GetProperty(nameof(ThingContext.Events)).GetMethod;
        private readonly MethodInfo _getItem = typeof(Dictionary<string, ThingEventCollection>).GetMethod("get_Item");
        private readonly MethodInfo _addItem = typeof(ThingEventCollection).GetMethod(nameof(ThingEventCollection.Add));
        private readonly JsonSerializerOptions _options;
        private readonly TypeBuilder _builder;

        public EventIntercept(JsonSerializerOptions options, TypeBuilder builder, Dictionary<string, ThingEventCollection> events)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
            _events = events ?? throw new ArgumentNullException(nameof(events));
        }

        public void Before(Thing thing)
        {
            
        }

        public void After(Thing thing)
        {
            var type = _builder.CreateType();
            while (_eventToBind.TryDequeue(out var @event))
            {
                var @delegate = Delegate.CreateDelegate(@event.EventHandlerType, type, $"{@event.Name}Handler");
                @event.AddEventHandler(thing, @delegate );
            }
        }

        public void Visit(Thing thing, EventInfo @event, ThingEventAttribute? eventInfo)
        {
            _eventToBind.Enqueue(@event);
            var name = _options.GetPropertyName(eventInfo?.Name ?? @event.Name);
            _events.Add(name, new ThingEventCollection(10));

            var type = @event.EventHandlerType.GetGenericArguments()[0];
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
            il.EmitCall(OpCodes.Callvirt, _addItem, null);
            il.Emit(OpCodes.Ret);
        }
    }
}
