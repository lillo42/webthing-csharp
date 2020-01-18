using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;
using Mozilla.IoT.WebThing.Converts;
using Mozilla.IoT.WebThing.Factories.Generator;

namespace Mozilla.IoT.WebThing.Factories
{
    public partial class ThingConverterFactory : IThingConverterFactory
    {
        public IThingConverter Create(Thing thing, JsonSerializerOptions options)
        {
            var thingType = thing.GetType();
            var typeName = $"{thingType.Name}Converter";
            var assemblyBuilder = GenerateAssemblyBuilder(typeName);

            var moduleBuilder = assemblyBuilder.DefineDynamicModule($"{typeName}Module");
            const TypeAttributes typeAttributes = TypeAttributes.AutoClass | TypeAttributes.Class | TypeAttributes.Public;
            var interfaces = new[] {typeof(IThingConverter)};

            var type = moduleBuilder.DefineType(typeName, typeAttributes, null, interfaces);
            var methodBuilder = type.DefineMethod(nameof(IThingConverter.Write), MethodAttributes.Public  | MethodAttributes.Final | MethodAttributes.Virtual, typeof(void), 
                new[] { typeof(Utf8JsonWriter), typeof(Thing), typeof(JsonSerializerOptions) });

            var il = methodBuilder.GetILGenerator();
            
            var generated = new ThingConverterGenerator(il, options);
            
            generated.Generated(thing);
            
            il.Emit(OpCodes.Ret);

            return (IThingConverter) Activator.CreateInstance(type.CreateType());
        }

        private static AssemblyBuilder GenerateAssemblyBuilder(string typeName)
        {
            var assemblyName = new AssemblyName($"{typeName}Assembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            return assemblyBuilder;
        }
    }
}
