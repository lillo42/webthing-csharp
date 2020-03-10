using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;
using Mozilla.IoT.WebThing.Properties;
using Mozilla.IoT.WebThing.Properties.Boolean;
using Mozilla.IoT.WebThing.Properties.String;

namespace Mozilla.IoT.WebThing.Factories.Generator.Properties
{
    public class PropertiesIntercept : IPropertyIntercept
    {
        private readonly ThingOption _option;
        private readonly ModuleBuilder _moduleBuilder;

        public Dictionary<string, IProperty> Properties { get; }
        
        public PropertiesIntercept(ThingOption option, ModuleBuilder moduleBuilder)
        {
            _option = option ?? throw new ArgumentNullException(nameof(option));
            _moduleBuilder = moduleBuilder ?? throw new ArgumentNullException(nameof(moduleBuilder));
            Properties = option.IgnoreCase ? new Dictionary<string, IProperty>(StringComparer.InvariantCultureIgnoreCase) 
                : new Dictionary<string, IProperty>();
        }
        
        public void Before(Thing thing)
        {
            
        }
        
        public void After(Thing thing)
        {
            
        }
        
        public void Intercept(Thing thing, PropertyInfo propertyInfo, ThingPropertyAttribute? propertyAttribute)
        {
            var thingType = thing.GetType();
            var propertyName = propertyAttribute?.Name ?? propertyInfo.Name;

            var isReadOnly = !propertyInfo.CanWrite || !propertyInfo.GetMethod.IsPublic ||
                             (propertyAttribute != null && propertyAttribute.IsReadOnly);


            var getter = GetGetMethod(propertyInfo);

            if (isReadOnly)
            {
                Properties.Add(propertyName, new PropertyReadOnly(thing, getter));
                return;
            }

            var setter = GetSetMethod(propertyInfo);
            var propertyType = propertyInfo.PropertyType.GetUnderlyingType();
            var isNullable = (propertyType ==  typeof(string) && propertyType.IsNullable()) 
                             && (propertyAttribute == null || propertyAttribute.Enum.Contains(null));

            IProperty property = null;
            
            if(propertyType == typeof(bool))
            {
                property = new PropertyBoolean(thing, getter, setter, isNullable);
            }
            else if (propertyType == typeof(string))
            {
                property = new PropertyString(thing, getter, setter, isNullable,
                    propertyAttribute?.MinimumLengthValue, propertyAttribute?.MaximumLengthValue, propertyAttribute?.Pattern,
                    propertyAttribute?.Enum?.Select(Convert.ToString).ToArray());
            }
            else if (propertyType == typeof(Guid))
            {
                property = new PropertyGuid(thing, getter, setter, isNullable,
                    propertyAttribute?.Enum?.Select(x=> Guid.Parse(x.ToString())).ToArray());
            }
            else if (propertyType == typeof(TimeSpan))
            {
                property = new PropertyTimeSpan(thing, getter, setter, isNullable,
                    propertyAttribute?.Enum?.Select(x=> TimeSpan.Parse(x.ToString())).ToArray());
            }
            else if (propertyType == typeof(DateTime))
            {
                property = new PropertyDateTime(thing, getter, setter, isNullable,
                    propertyAttribute?.Enum?.Select(Convert.ToDateTime).ToArray());
            }
            else if (propertyType == typeof(DateTimeOffset))
            {
                property = new PropertyDateTimeOffset(thing, getter, setter, isNullable,
                    propertyAttribute?.Enum?.Select(x => DateTimeOffset.Parse(x.ToString())).ToArray());
            }
            
            Properties.Add(propertyName, property);
        }
        
        private static Func<object, object> GetGetMethod(PropertyInfo property)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var instanceCast = property.DeclaringType.IsValueType ? 
                Expression.Convert(instance, property.DeclaringType) : Expression.TypeAs(instance, property.DeclaringType);
            
            var call = Expression.Call(instanceCast, property.GetGetMethod());
            var typeAs = Expression.TypeAs(call, typeof(object));

            return Expression.Lambda<Func<object, object>>(typeAs, instance).Compile();
        }
        
        private static Action<object, object> GetSetMethod(PropertyInfo property)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var value = Expression.Parameter(typeof(object), "value");

            // value as T is slightly faster than (T)value, so if it's not a value type, use that
            var instanceCast = property.DeclaringType.IsValueType ? 
                Expression.Convert(instance, property.DeclaringType) : Expression.TypeAs(instance, property.DeclaringType);
            
            var valueCast = property.PropertyType.IsValueType ? 
                Expression.Convert(value, property.PropertyType) : Expression.TypeAs(value, property.PropertyType);

            var call = Expression.Call(instanceCast, property.GetSetMethod(), valueCast);
            return Expression.Lambda<Action<object, object>>(call, new[] {instance, value}).Compile();
        }
    }
}
