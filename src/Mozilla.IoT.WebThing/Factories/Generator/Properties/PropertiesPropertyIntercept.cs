using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;
using System.Threading.Tasks;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Exceptions;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;
using Mozilla.IoT.WebThing.Mapper;

namespace Mozilla.IoT.WebThing.Factories.Generator.Properties
{
    internal class PropertiesPropertyIntercept : IPropertyIntercept
    {
        //https://sharplab.io/#v2:C4LglgNgPgAgTARgLACgYAYAEMEBYDcqG2CArISkQMzZyYAyAhgLYAOACgE4D2rApp2Bg+AZ0whMASS68BQ0agDeqTKsytOYAG6NgfTJz6MAJtwB2EAJ4MWrTBFsU1mFWpg0AImADGQ8405LAB4cdAAaTG4AIwArPl8APkwAcT5gGX5BYREACgBKV1VlFGdnGAB2TDM+AHdML18wf0CQhHDI2PjgBMLS4tKBzABtACIdCABXPhGAXUwAXntbADoANUZJvjDeweGRkVmF1Qc2ZYBlFxLBgF8nNWve3vcOuN8UtIy5SxzQ9R5M4CWAByLD4BSuRR2ajAADMcmMNlMRssAKIARwmG1yGlkgmBoLy4N2/V2bkqJ1Ya0RfDuAweENKwAAFjw6tU6iiAB7ePisPxmHI4gH45hg2mYenOJ40KLcbgQTAAYXMwEYYDMIk+eJ+bT+uMBINFRLUJIGsPh4yRqIxWMF/y+hrBxoGptJFUwwE4U3FzklgyhqndMKxNN69MeKHDaBo6r0nGDPKkF0UUee8BsbCU0swscw602mEUmAA5ml8JgRGWJdnJBdk6XgPhK436amY2Y4wn9NJ7VkFChXbL5UqVWqNVrATqsEKHQTxb1oq9gO90r3J78Z3jHXlxQ1+QFgqEIouuklUqv9dl8hRrkA
        
        private readonly TypeBuilder _builder;
        private readonly LinkedList<(PropertyInfo propertyInfo, ThingPropertyAttribute? information)> _properties;
        private readonly MethodInfo _strEquals;
        private readonly ConstructorInfo _notFound;
        private readonly FieldBuilder _field;
        public PropertiesPropertyIntercept(TypeBuilder builder)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
            _properties = new LinkedList<(PropertyInfo propertyInfo, ThingPropertyAttribute? information)>();
            _strEquals = typeof(string).GetMethod(nameof(string.Equals), new[] {typeof(string)});
            _notFound = typeof(PropertyNotFoundException).GetConstructors()[0];
        }

        public void Before(Thing thing)
        {
        }
        
        public void After(Thing thing)
        {
            CreateContainsProperty();
            CreateGetProperty();
            CreateGetProperties();
            CreateSetProperty();
        }

        public void Intercept(Thing thing, PropertyInfo propertyInfo, ThingPropertyAttribute? thingPropertyAttribute)
        {
            _properties.AddLast((propertyInfo, thingPropertyAttribute));
        }

        private void CreateContainsProperty()
        {
            var method = _builder.DefineMethod(nameof(IProperty.ContainsProperty),
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                typeof(bool), new[] {typeof(string)});

            var il = method.GetILGenerator();
            Label? next = null;
            foreach (var (propertyInfo, information) in _properties)
            {
                var propertyName = information?.Name ?? propertyInfo.Name;

                if (next != null)
                {
                    il.MarkLabel(next.Value);
                }

                next = il.DefineLabel();
                
                il.Emit(OpCodes.Ldstr, propertyName);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, _strEquals);
                il.Emit(OpCodes.Brfalse_S, next.Value);
                
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Ret);
                
            }

            if (next.HasValue)
            {
                il.MarkLabel(next.Value);
            }
            
            
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Ret);
        }
        
        private void CreateGetProperty()
        {
            var method = _builder.DefineMethod(nameof(IProperty.GetProperty),
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                typeof(object), new[] {typeof(string)});

            var il = method.GetILGenerator();
            Label? next = null;
            foreach (var (propertyInfo, information) in _properties)
            {
                var propertyName = information?.Name ?? propertyInfo.Name;

                if (next != null)
                {
                    il.MarkLabel(next.Value);
                }

                next = il.DefineLabel();
                
                il.Emit(OpCodes.Ldstr, propertyName);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, _strEquals);
                il.Emit(OpCodes.Brfalse_S, next.Value);
                
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, _field);
                il.EmitCall(OpCodes.Callvirt, propertyInfo.GetMethod, null);
                
                if (propertyInfo.PropertyType.IsValueType)
                {
                    il.Emit(OpCodes.Box, propertyInfo.PropertyType);
                }
                
                il.Emit(OpCodes.Ret);
                
            }

            if (next.HasValue)
            {
                il.MarkLabel(next.Value);
            }
            
            
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Newobj, _notFound);
            il.Emit(OpCodes.Throw);
        }
        
        private void CreateGetProperties()
        {
            var method = _builder.DefineMethod(nameof(IProperty.GetProperties),
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                typeof(Dictionary<string, object>), Type.EmptyTypes);

            var dic = typeof(Dictionary<string, object>).GetConstructors()[0];
            var set = typeof(Dictionary<string, object>).GetMethod("set_Item",
                BindingFlags.Public | BindingFlags.Instance);
            var il = method.GetILGenerator();
            
            il.Emit(OpCodes.Newobj, dic);
            
            foreach (var (propertyInfo, information) in _properties)
            {
                var propertyName = information?.Name ?? propertyInfo.Name;

                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldstr, propertyName);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, _field);
                il.EmitCall(OpCodes.Callvirt, propertyInfo.GetMethod, null);
                
                if (propertyInfo.PropertyType.IsValueType)
                {
                    il.Emit(OpCodes.Box, propertyInfo.PropertyType);
                }
            }
            
            il.Emit(OpCodes.Ret);
        }
        
        private void CreateSetProperty()
        {
            var method = _builder.DefineMethod(nameof(IProperty.SetProperty),
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                typeof(SetPropertyResult), new []{ typeof(string), typeof(object) });

            var il = method.GetILGenerator();
            var fields = new Dictionary<Type, LocalBuilder>();
            foreach (var type in _properties
                .Where(x => x.information != null 
                            && !x.information.IsReadOnly
                            && (x.information.MinimumValue.HasValue 
                                || x.information.MaximumValue.HasValue
                                || x.information.MultipleOfValue.HasValue
                                || x.information.Enum != null))
                .Select(x => x.propertyInfo.PropertyType)
                .Distinct())
            {
                fields.Add(type, il.DeclareLocal(type));
            }

            Label? next = null;
            foreach (var (propertyInfo, information) in _properties)
            {
                var propertyName = information?.Name ?? propertyInfo.Name;

                if (information != null && information.IsReadOnly)
                {
                    continue;
                }

                if (next != null)
                {
                    il.MarkLabel(next.Value);
                }

                next = il.DefineLabel();
                il.Emit(OpCodes.Ldstr, propertyName);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, _strEquals);
                il.Emit(OpCodes.Brfalse_S, next.Value);

                if (information == null
                    || !information.MinimumValue.HasValue
                    || !information.MaximumValue.HasValue
                    || !information.MultipleOfValue.HasValue
                    || information.Enum != null)
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, _field);
                    il.Emit(OpCodes.Ldarg_2);

                    if (propertyInfo.PropertyType.IsValueType)
                    {
                        il.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                    }
                    else
                    {
                        il.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
                    }
                }
                else
                {
                    var field = fields[propertyInfo.PropertyType];
                    il.Emit(OpCodes.Ldloc_S, field.LocalIndex);
                    
                    if (propertyInfo.PropertyType.IsValueType)
                    {
                        il.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                    }
                    else
                    {
                        il.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
                    }
                    
                    il.Emit(OpCodes.Stloc_S, field.LocalIndex);
                }

                il.Emit(OpCodes.Ret);
                
            }

            if (next.HasValue)
            {
                il.MarkLabel(next.Value);
            }
            
            
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Newobj, _notFound);
            il.Emit(OpCodes.Throw);
        }
        
    }
}
