using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.Factories.Generator
{
    internal class PropertiesGenerator
    {
        private readonly TypeBuilder _typeBuilder;

        public PropertiesGenerator(TypeBuilder typeBuilder)
        {
            _typeBuilder = typeBuilder ?? throw new ArgumentNullException(nameof(typeBuilder));
        }

        public void Generate(Thing thing)
        {
            var thingType = thing.GetType();
            var properties = thingType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => !IsThingProperty(x.Name)).ToArray();

            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                var jsonType = GetJsonType(propertyType);
                if (jsonType == null)
                {
                    continue;
                }
                
                var information = propertyType.GetCustomAttribute<ThingPropertyAttribute>();

                if (information != null && information.Ignore)
                {
                    continue;
                }
                
                var propertyName =  information?.Name ?? property.Name;
                var builder = _typeBuilder.DefineProperty(propertyName, PropertyAttributes.None,
                    property.PropertyType, null);
                
                const MethodAttributes getAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
                // Define the "get" accessor method for CustomerName.
                var getMethod = _typeBuilder.DefineMethod($"get_{propertyName}", getAttr, property.PropertyType, Type.EmptyTypes);
                var getMethodBuild = getMethod.GetILGenerator();
                
                getMethodBuild.Emit(OpCodes.Ldarg_0);
                //getMethodBuild.Emit(OpCodes.Ldfld, );
                getMethodBuild.EmitCall(OpCodes.Callvirt, property.GetMethod, null);
                getMethodBuild.Emit(OpCodes.Ret);
                
                builder.SetGetMethod(getMethod);
            }
        }
        
        private static string? GetJsonType(Type? type)
        {
            if (type == null)
            {
                return null;
            }
            
            if (type == typeof(string))
            {
                return "string";
            }

            if (type == typeof(bool))
            {
                return "boolean";
            }
            
            if (type == typeof(int)
                || type == typeof(byte)
                || type == typeof(short)
                || type == typeof(long)
                || type == typeof(uint)
                || type == typeof(ulong)
                || type == typeof(ushort))
            {
                return "integer";
            }
            
            if (type == typeof(double)
                || type == typeof(float))
            {
                return "number";
            }

            return null;
        }
        
        private static bool IsThingProperty(string name)
            => name == nameof(Thing.Context)
               || name == nameof(Thing.Name)
               || name == nameof(Thing.Description)
               || name == nameof(Thing.Title)
               || name == nameof(Thing.Type);
    }
}
