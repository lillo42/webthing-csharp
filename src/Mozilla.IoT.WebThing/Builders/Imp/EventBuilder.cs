using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Events;
using Mozilla.IoT.WebThing.Extensions;

namespace Mozilla.IoT.WebThing.Builders
{
    /// <inheritdoc /> 
    public class EventBuilder : IEventBuilder
    {
        private readonly Queue<EventInfo> _eventToBind;
        
        private Thing? _thing;
        private ThingOption? _option;
        private Type? _thingType;
        private TypeBuilder? _builder;
        private Dictionary<string, EventCollection>? _events;

        private static readonly MethodInfo s_handler = typeof(InternalEventHandle).GetMethod(nameof(InternalEventHandle.Handler))!;
        
        
        /// <summary>
        /// Initialize a new instance of <see cref="EventBuilder"/>.
        /// </summary>
        public EventBuilder()
        {
            _eventToBind = new Queue<EventInfo>();
        }
        
        /// <inheritdoc /> 
        public IEventBuilder SetThing(Thing thing)
        {
            _thing = thing;
            return this;
        }
        
        /// <inheritdoc /> 
        public IEventBuilder SetThingType(Type thingType)
        {
            _thingType = thingType;
            var baseName = $"{thingType.Name}EventBinder";
            var assemblyName = new AssemblyName($"{baseName}Assembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule($"{baseName}Module");
            
            _builder = moduleBuilder.DefineType(baseName, 
                TypeAttributes.AutoClass | TypeAttributes.Class | TypeAttributes.Public, 
                null, null);
            
            return this;
        }

        /// <inheritdoc /> 
        public IEventBuilder SetThingOption(ThingOption option)
        {
            _option = option;
            _events = new Dictionary<string, EventCollection>(option.IgnoreCase ? StringComparer.OrdinalIgnoreCase : null);
            return this;
        }

        /// <inheritdoc /> 
        public void Add(EventInfo @event, ThingEventAttribute? eventInfo)
        {
            if (_thingType == null || _builder == null)
            {
                throw new InvalidOperationException($"ThingType is null, call {nameof(SetThingType)} before add");
            }

            if (_events == null || _option == null)
            {
                throw new InvalidOperationException($"ThingOption is null, call {nameof(SetThingOption)} before add");
            }
            
            _eventToBind.Enqueue(@event);
            var name = eventInfo?.Name ?? @event.Name;
            _events.Add(_option.PropertyNamingPolicy.ConvertName(name), new EventCollection(_option.MaxEventSize));
            var type = @event.EventHandlerType?.GetGenericArguments()[0]!;
            var methodBuilder = _builder.DefineMethod($"{@event.Name}Handler",
                MethodAttributes.Public | MethodAttributes.Static);

            methodBuilder.SetParameters(typeof(object), type);
            
            var il = methodBuilder.GetILGenerator();
            
            // https://sharplab.io/#v2:C4LglgNgPgAgTARgLACgYGYAE9MGFMDeqmJ2WMCAbNgCyYCyAFAPYBGAVgKYDGwmAzpwB2AE04AnADSZWzZhEwBDcQHN+ASkLFSOgKIA3YcAASi0RE4A6U+c6NBoidOVrpAIjfqA3NpIBfVACUVFQwIWAJIUUFCmocAyMbEQtUIhQdDGwqWkwkixYOHj4HMSlMNi5eJVV+aQoABkwogFtOdQIgvyA===
            // static void <EventName>Handler(object sender, <EventType> args)
            // {
            //    EventHandle.Handle(sender, args, "<eventName>");
            // }
            //
            
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Box, type);
            }
            
            il.Emit(OpCodes.Ldstr, _option.PropertyNamingPolicy.ConvertName(name));
            il.EmitCall(OpCodes.Call, s_handler, null);
            il.Emit(OpCodes.Ret);
        }

        /// <inheritdoc /> 
        public Dictionary<string, EventCollection> Build()
        {
            if (_thing == null)
            {
                throw new InvalidOperationException($"Thing is null, call {nameof(SetThing)} before build");
            }
            
            if (_builder == null)
            {
                throw new InvalidOperationException($"ThingType is null, call {nameof(SetThingType)} before build");
            }
            
            var type = _builder.CreateType()!;
            while (_eventToBind.TryDequeue(out var @event))
            {
                var @delegate = Delegate.CreateDelegate(@event.EventHandlerType!, type, $"{@event.Name}Handler");
                @event.AddEventHandler(_thing, @delegate );
            }
            
            return _events!;
        }
    }
}
