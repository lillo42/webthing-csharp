using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;
using Mozilla.IoT.WebThing.Converts;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator.Converter
{
    internal class ConverterInterceptorFactory : IInterceptorFactory
    {
        private readonly JsonSerializerOptions _options;
        private readonly TypeBuilder _builder;
        private readonly Utf8JsonWriterILGenerator _jsonWriterIlGenerator;
        private readonly ILGenerator _il;

        public ConverterInterceptorFactory(Thing thing, JsonSerializerOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            
            var thingType = thing.GetType();
            _builder = Factory.CreateTypeBuilder($"{thingType.Name}Converter", thingType.Name, 
                typeof(IThingConverter), TypeAttributes.AutoClass | TypeAttributes.Class | TypeAttributes.Public);
            
            var methodBuilder = _builder.DefineMethod(nameof(IThingConverter.Write), 
                MethodAttributes.Public  | MethodAttributes.Final | MethodAttributes.Virtual, 
                typeof(void), 
                new[] { typeof(Utf8JsonWriter), typeof(Thing), typeof(JsonSerializerOptions) });

            _il = methodBuilder.GetILGenerator();
            _jsonWriterIlGenerator = new Utf8JsonWriterILGenerator(_il, _options);
        }

        public IThingIntercept CreateThingIntercept() 
            => new ConverterThingIntercept(_jsonWriterIlGenerator);

        public IPropertyIntercept CreatePropertyIntercept() 
            => new ConverterPropertyIntercept(_jsonWriterIlGenerator, _options);

        public IActionIntercept CreatActionIntercept() 
            => new ConvertActionIntercept(_jsonWriterIlGenerator, _options);

        public IEventIntercept CreatEventIntercept() 
            => new ConvertEventIntercept(_jsonWriterIlGenerator, _options);

        public IThingConverter Create()
        {
            _il.Emit(OpCodes.Ret);
            return (IThingConverter)Activator.CreateInstance(_builder.CreateType());
        }
    }
}
