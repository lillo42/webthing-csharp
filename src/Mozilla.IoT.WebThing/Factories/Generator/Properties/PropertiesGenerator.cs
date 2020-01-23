using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;
using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.Factories.Generator
{
    internal class PropertiesGenerator
    {
        private readonly ILGenerator _ilGenerator;
        private readonly JsonSerializerOptions _options;

        private readonly MethodInfo _add = typeof(Dictionary<string, object>).GetMethod(
            nameof(Dictionary<string, object>.Add), new []{ typeof(string), typeof(object)});
            
        public PropertiesGenerator(ILGenerator ilGenerator, JsonSerializerOptions options)
        {
            _ilGenerator = ilGenerator ?? throw new ArgumentNullException(nameof(ilGenerator));
            _options = options ?? throw new ArgumentNullException(nameof(options));
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
                if (property.PropertyType == typeof(string))
                {
                    AddStringValue(propertyName, property.GetMethod);
                }
                else if (property.PropertyType == typeof(bool))
                {
                    
                }
                
            }
        }

        private void CreateDictionary()
        {
            var constructor = typeof(Dictionary<string, object>).GetConstructor(null);
            _ilGenerator.Emit(OpCodes.Newobj, constructor);
            
        }

        private void AddStringValue(string propertyName, MethodInfo getProperty)
        {
            _ilGenerator.Emit(OpCodes.Dup);
            _ilGenerator.Emit(OpCodes.Ldstr,  propertyName);
            _ilGenerator.Emit(OpCodes.Ldloc_0);
            _ilGenerator.EmitCall(OpCodes.Callvirt, getProperty, null);
            _ilGenerator.EmitCall(OpCodes.Callvirt, _add, null);
        }
        
        private void AddBoolValue(string propertyName, MethodInfo getProperty)
        {
            _ilGenerator.Emit(OpCodes.Dup);
            _ilGenerator.Emit(OpCodes.Ldstr,  propertyName);
            _ilGenerator.Emit(OpCodes.Ldloc_0);
            _ilGenerator.EmitCall(OpCodes.Callvirt, getProperty, null);
            _ilGenerator.Emit(OpCodes.Box, typeof(bool));
            _ilGenerator.EmitCall(OpCodes.Callvirt, _add, null);
        }

        private string GetPropertyName(string propertyName)
        {
            if (_options.PropertyNamingPolicy != null)
            {
                return _options.PropertyNamingPolicy.ConvertName(propertyName);
            }

            return propertyName;
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
