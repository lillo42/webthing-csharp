using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator.Properties
{
    internal class PropertiesInterceptFactory : IInterceptorFactory
    {
        private readonly JsonSerializerOptions _options;
        private readonly TypeBuilder _builder;
        private readonly FieldBuilder _thingFiled;
        private readonly ILGenerator _ilGenerator;
        
        public PropertiesInterceptFactory(Thing thing, JsonSerializerOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            
            var thingType = thing.GetType();
            _builder = Factory.CreateTypeBuilder($"{thingType.Name}Converter", thingType.Name, 
                typeof(IProperties));
            
            var methodBuilder = _builder.DefineMethod(nameof(IProperties.GetProperties), 
                MethodAttributes.Public  | MethodAttributes.Final | MethodAttributes.Virtual, 
                typeof(Dictionary<string, object>), null);

            _thingFiled = _builder.DefineField("_thing", thingType, FieldAttributes.Private | FieldAttributes.InitOnly);


            var constructor =
                _builder.DefineConstructor(MethodAttributes.Public, CallingConventions.Any, new[] {thingType});

            var il = constructor.GetILGenerator();
            
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldfld, _thingFiled);
            il.Emit(OpCodes.Ret);
            
            _ilGenerator = methodBuilder.GetILGenerator();
        }


        public IThingIntercept CreateThingIntercept() => new EmptyIntercept();

        public IPropertyIntercept CreatePropertyIntercept() 
            => new PropertiesPropertyIntercept(_ilGenerator, _options, _thingFiled);

        public IActionIntercept CreatActionIntercept() 
            => new EmptyIntercept();

        public IEventIntercept CreatEventIntercept()
            => new EmptyIntercept();

        public Type CreateType() 
            => _builder.CreateType();

        private class EmptyIntercept : IThingIntercept, IActionIntercept, IEventIntercept
        {
            public void Before(Thing thing)
            {
                
            }

            public void After(Thing thing)
            {
                
            }

            public void Visit(Thing thing, EventInfo @event, ThingEventAttribute? eventInfo)
            {
                
            }

            public void Intercept(Thing thing, MethodInfo action, ThingActionAttribute? actionInfo)
            {
                
            }
        }
    }
}
