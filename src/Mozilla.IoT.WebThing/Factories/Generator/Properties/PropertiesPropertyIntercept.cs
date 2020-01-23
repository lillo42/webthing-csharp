using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator.Properties
{
    internal class PropertiesPropertyIntercept : IPropertyIntercept
    {
        private readonly ILGenerator _ilGenerator;
        private readonly JsonSerializerOptions _options;
        private readonly FieldBuilder _thingFiled;
        private readonly MethodInfo _add = typeof(Dictionary<string, object>).GetMethod(
            nameof(Dictionary<string, object>.Add), new []{ typeof(string), typeof(object)});

        public PropertiesPropertyIntercept(ILGenerator ilGenerator, JsonSerializerOptions options, FieldBuilder thingFiled)
        {
            _ilGenerator = ilGenerator ?? throw new ArgumentNullException(nameof(ilGenerator));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _thingFiled = thingFiled ?? throw new ArgumentNullException(nameof(thingFiled));
        }

        public void BeforeVisit(Thing thing)
        {
            _ilGenerator.DeclareLocal(thing.GetType());
            _ilGenerator.Emit(OpCodes.Ldarg_0);
            _ilGenerator.Emit(OpCodes.Ldfld, _thingFiled);
            _ilGenerator.Emit(OpCodes.Stloc_0);
            
            var constructor = typeof(Dictionary<string, object>).GetConstructor(null);
            _ilGenerator.Emit(OpCodes.Newobj, constructor);
        }

        public void Intercept(Thing thing, PropertyInfo propertyInfo, ThingPropertyAttribute? thingPropertyAttribute)
        {
            var propertyName =  thingPropertyAttribute?.Name ?? propertyInfo.Name;
            if (propertyInfo.PropertyType == typeof(string))
            {
                AddStringValue(propertyName, propertyInfo.GetMethod);
            }
            else
            {
                AddValue(propertyName, propertyInfo.GetMethod, propertyInfo.PropertyType);
            }
        }

        public void AfterVisit(Thing thing)
        {
            _ilGenerator.Emit(OpCodes.Ret);
        }
        
        private void AddStringValue(string propertyName, MethodInfo getProperty)
        {
            _ilGenerator.Emit(OpCodes.Dup);
            _ilGenerator.Emit(OpCodes.Ldstr, _options.GetPropertyName(propertyName));
            _ilGenerator.Emit(OpCodes.Ldloc_0);
            _ilGenerator.EmitCall(OpCodes.Callvirt, getProperty, null);
            _ilGenerator.EmitCall(OpCodes.Callvirt, _add, null);
        }
        private void AddValue(string propertyName, MethodInfo getProperty, Type boxType)
        {
            _ilGenerator.Emit(OpCodes.Dup);
            _ilGenerator.Emit(OpCodes.Ldstr,  _options.GetPropertyName(propertyName));
            _ilGenerator.Emit(OpCodes.Ldloc_0);
            _ilGenerator.EmitCall(OpCodes.Callvirt, getProperty, null);
            _ilGenerator.Emit(OpCodes.Box, boxType);
            _ilGenerator.EmitCall(OpCodes.Callvirt, _add, null);
        }
    }
}
