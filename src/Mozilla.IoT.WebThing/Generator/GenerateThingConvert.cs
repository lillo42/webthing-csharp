using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Converts;

namespace Mozilla.IoT.WebThing.Generator
{
    public class GenerateThingConvert : IGenerateThingConvert
    {
        private static readonly MethodInfo s_propertyName = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WritePropertyName), BindingFlags.Public);
        private static readonly MethodInfo s_startObject = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteStartObject), BindingFlags.Public);
        private static readonly MethodInfo s_endObject = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteEndObject), BindingFlags.Public);
        private static readonly MethodInfo s_strValue = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteStringValue), BindingFlags.Public);
        private static readonly MethodInfo s_nullValue = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteNullValue), BindingFlags.Public);

        private static readonly Type s_strType = typeof(string);
        public IThingConverter Generate<T>(T thing)
            where  T : Thing
        {
            var thingType = thing.GetType();
            var typeName = $"{thingType.Name}Proxy";
            var assemblyBuilder = GenerateAssemblyBuilder(typeName);

            var moduleBuilder = assemblyBuilder.DefineDynamicModule($"{typeName}Module");
            const TypeAttributes typeAttributes = TypeAttributes.AutoClass | TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.BeforeFieldInit;
            var interfaces = new[] {typeof(IThingConverter<>)};

            var type = moduleBuilder.DefineType(typeName, typeAttributes, null, interfaces);
            var methodBuilder = type.DefineMethod(nameof(IThingConverter.Write), MethodAttributes.Public, typeof(void), new[] { typeof(Utf8JsonWriter), thingType, typeof(JsonSerializerOptions) });
            var il = methodBuilder.GetILGenerator();

            var c = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteStartObject), BindingFlags.Public);

            WritePropertyName(il, "Properties");

            il.Emit(OpCodes.Ldarg_1);
            il.EmitCall(OpCodes.Callvirt, s_startObject, null);

            foreach (var propertyInfo in thingType.GetProperties(BindingFlags.Public).Where(x => !IsThingProperty(x.Name)))
            {
                if (propertyInfo.DeclaringType == null || propertyInfo.DeclaringType.IsByRef)
                {
                    continue;
                }

                var propertyNameAttribute = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>();
                var propertyName = propertyNameAttribute?.Name ?? propertyInfo.Name;

                WritePropertyName(il, propertyName);

                il.Emit(OpCodes.Ldarg_1);
                il.EmitCall(OpCodes.Callvirt, s_startObject, null);

                var propertyInformation = propertyInfo.GetCustomAttribute<JsonPropertyInformationAttribute>();

                if (propertyInformation != null)
                {
                    WritePropertyNameWithValue(il, nameof(JsonPropertyInformationAttribute.Title), propertyInformation.Title);
                    WritePropertyNameWithValue(il, nameof(JsonPropertyInformationAttribute.Description), propertyInformation.Description);
                    WritePropertyNameWithype(il, "@type", propertyInformation.Type);
                }


                il.Emit(OpCodes.Ldarg_1);
                il.EmitCall(OpCodes.Callvirt, s_endObject, null);
            }


            il.Emit(OpCodes.Ldarg_1);
            il.EmitCall(OpCodes.Callvirt, s_endObject, null);

            return (IThingConverter) Activator.CreateInstance(type.CreateType());
        }

        private static void WritePropertyNameWithValue(ILGenerator il, string propertyName, string? value)
        {
            WritePropertyName(il, propertyName);

            il.Emit(OpCodes.Ldarg_1);
            if (value != null)
            {
                il.Emit(OpCodes.Ldstr, value);
                il.EmitCall(OpCodes.Callvirt, s_strValue, new[] { s_strType });
            }
            else
            {
                il.EmitCall(OpCodes.Callvirt, s_nullValue, null);
            }
        }


        private static void WritePropertyNameWithype(ILGenerator il, string propertyName, string[]? value)
        {
            WritePropertyName(il, propertyName);

            il.Emit(OpCodes.Ldarg_1);
            if (value == null)
            {
                il.EmitCall(OpCodes.Callvirt, s_nullValue, null);
            }
            else if (value.Length == 1)
            {
                il.Emit(OpCodes.Ldstr, value[0]);
                il.EmitCall(OpCodes.Callvirt, s_strValue, new[] { s_strType });
            }
            else
            {
                il.EmitCall(OpCodes.Callvirt, s_nullValue, null);
            }
        }

        private static void WritePropertyName(ILGenerator il, string propertyName)
        {
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldstr, propertyName);
            il.EmitCall(OpCodes.Callvirt, s_propertyName, new[] {s_strType});
        }

        private static bool IsThingProperty(string name)
            => name == nameof(Thing.Context)
               || name == nameof(Thing.Name)
               || name == nameof(Thing.Description)
               || name == nameof(Thing.Title)
               || name == nameof(Thing.Title);

        private static AssemblyBuilder GenerateAssemblyBuilder(string typeName)
        {
            var assemblyName = new AssemblyName($"{typeName}Assembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            return assemblyBuilder;
        }
    }
}
