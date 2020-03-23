using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;

namespace Mozilla.IoT.WebThing.Builders
{
    /// <inheritdoc />
    public class ThingResponseBuilder : IThingResponseBuilder
    {
        private static readonly Type s_string = typeof(string);
        private static readonly ConstructorInfo s_baseConstructor = typeof(ThingResponse).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0];
        private const MethodAttributes s_getSetAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

        private Thing? _thing;
        private ThingOption? _option;
        private Type? _thingType;
        private ModuleBuilder? _module;
        private TypeBuilder? _builder;
        private TypeBuilder? _events;

        private readonly LinkedList<(FieldBuilder, Type)> _eventCreated = new LinkedList<(FieldBuilder, Type)>();
        
        /// <inheritdoc />
        public IThingResponseBuilder SetThing(Thing thing)
        {
            _thing = thing;
            return this;
        }

        /// <inheritdoc />
        public IThingResponseBuilder SetThingType(Type thingType)
        {
            _thingType = thingType;
            
            var baseName = $"{thingType.Name}ThingResponse";
            var assemblyName = new AssemblyName($"{baseName}Assembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
             _module = assemblyBuilder.DefineDynamicModule($"{baseName}Module");
            
            _builder = _module.DefineType(baseName, 
                TypeAttributes.AutoClass | TypeAttributes.Class | TypeAttributes.Public, 
                typeof(ThingResponse), null);
            
            _events = _module.DefineType("Events", 
                TypeAttributes.AutoClass | TypeAttributes.Class | TypeAttributes.Public, 
                null, null);
            
            return this;
        }

        /// <inheritdoc />
        public IThingResponseBuilder SetThingOption(ThingOption option)
        {
            _option = option;
            return this;
        }

        /// <inheritdoc />
        public void Add(EventInfo @event, ThingEventAttribute? eventInfo)
        {
            if (_thing == null)
            {
                throw new InvalidOperationException($"Thing is null, call {nameof(SetThing)} before build");
            }
            
            if (_events == null || _module == null)
            {
                throw new InvalidOperationException($"ThingType is null, call {nameof(SetThingType)} before add");
            }
            
            if (_option == null)
            {
                throw new InvalidOperationException($"ThingOption is null, call {nameof(SetThingOption)} before add");
            }
            
            var eventType = _module.DefineType($"{@event.Name}Event", 
                TypeAttributes.AutoClass | TypeAttributes.Class | TypeAttributes.Public, 
                null, null);

            FieldBuilder? types = null;
            if (eventInfo != null)
            {
                CreateProperty(eventType, nameof(ThingEventAttribute.Title), eventInfo.Title);
                CreateProperty(eventType, nameof(ThingEventAttribute.Description), eventInfo.Description);
                CreateProperty(eventType, nameof(ThingEventAttribute.Unit), eventInfo.Unit);
                if (eventInfo.Type == null)
                {
                    CreateProperty(eventType, nameof(ThingEventAttribute.Type), (string?)null);
                }
                else if (eventInfo.Type.Length == 1)
                {
                    CreateProperty(eventType, nameof(ThingEventAttribute.Type), eventInfo.Type[0]);
                }
                else
                {
                    types = CreateProperty(eventType, nameof(ThingEventAttribute.Type), typeof(string[]));
                }
            }
            
            var link = CreateProperty(eventType, "Link", typeof(Link[]));

            var thingName = _option.PropertyNamingPolicy.ConvertName(_thing.Name);
            var eventName = _option.PropertyNamingPolicy.ConvertName(@event.Name);
            
            var constructor = eventType.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);
            var generator = constructor.GetILGenerator();
            
            generator.NewLinkArray(link, $"/thing/{thingName}/events/{eventName}", "event");

            if (types != null && eventInfo != null)
            {
                generator.NewStringArray(types, eventInfo.Type!);
            }
            
            generator.Emit(OpCodes.Ret);
            
            _eventCreated.AddLast((CreateProperty(_events, @event.Name, eventType.CreateType()!), eventType));
        }

        
        private static void CreateProperty(TypeBuilder builder, string name, string? value)
        {
            var getProperty = builder.DefineMethod($"get_{name}", s_getSetAttributes,
                s_string, null);

            getProperty.GetILGenerator().Return(value);

            var propertyBuilder = builder.DefineProperty(name, 
                PropertyAttributes.HasDefault,
                s_string, null);
            
            propertyBuilder.SetGetMethod(getProperty);
        }

        private static FieldBuilder CreateProperty(TypeBuilder builder, string name, Type type)
        {
            var field = builder.DefineField($"_{name}", type, FieldAttributes.Private);
            var propertyBuilder = builder.DefineProperty(name, 
                PropertyAttributes.HasDefault,
                type, null);

            var getProperty = builder.DefineMethod($"get_{name}", s_getSetAttributes,
                type, Type.EmptyTypes);

            getProperty.GetILGenerator().Return(field);

            // Define the "set" accessor method for CustomerName.
            var setProperty = builder.DefineMethod($"set_{name}", s_getSetAttributes,
                null, new[] {type});

            setProperty.GetILGenerator().Set(field);

            propertyBuilder.SetGetMethod(getProperty);
            propertyBuilder.SetSetMethod(setProperty);

            return field;
        }

        /// <inheritdoc />
        public ThingResponse Build()
        {
            if (_thing == null)
            {
                throw new InvalidOperationException($"Thing is null, call {nameof(SetThing)} before build");
            }

            if (_builder == null || _events == null)
            {
                throw new InvalidOperationException($"ThingType is null, call {nameof(SetThingType)} before build");
            }

            var eventConstructor = Initializer(_events, _eventCreated);

            var @event = CreateProperty(_builder, "Events", _events);

            var constructor = _builder.DefineConstructor(
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                CallingConventions.Standard, 
                new [] {typeof(Thing)});
            var generator = constructor.GetILGenerator();
            
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, s_baseConstructor);
            generator.NewObj(@event, eventConstructor);
            generator.Emit(OpCodes.Ret);
            
            return (ThingResponse)Activator.CreateInstance(_builder.CreateType()!, _thing)!;
        }

        private static ConstructorInfo Initializer(TypeBuilder builder, ICollection<(FieldBuilder, Type)> fieldToInitializer)
        {
            var constructor = builder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, 
                CallingConventions.Standard, 
                null);
            
            var generator = constructor.GetILGenerator();

            foreach (var (filed, type) in fieldToInitializer)
            {
                generator.NewObj(filed, type.GetConstructors()[0]);
            }
            
            generator.Emit(OpCodes.Ret);

            builder.CreateType();
            fieldToInitializer.Clear();
            return constructor;
        }
    }
}
