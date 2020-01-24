using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator.Properties
{
    internal class PropertiesPropertyIntercept : IPropertyIntercept
    {
        private readonly JsonSerializerOptions _options;
        private readonly Dictionary<string, Func<object, object>> _getters = new Dictionary<string, Func<object, object>>();
        private readonly Dictionary<string, Action<object, object>> _setters = new Dictionary<string, Action<object, object>>();

        public Dictionary<string, Func<object, object>> Getters => _getters;

        public PropertiesPropertyIntercept(JsonSerializerOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void BeforeVisit(Thing thing)
        {

        }

        public void Intercept(Thing thing, PropertyInfo propertyInfo, ThingPropertyAttribute? thingPropertyAttribute)
        {
            var propertyName =  thingPropertyAttribute?.Name ?? _options.GetPropertyName(propertyInfo.Name);
            _getters.Add(propertyName, GetGetMethod(propertyInfo));
            //_setters.Add(propertyName, GetSetMethod(propertyInfo));
        }
        
        private static Func<object, object> GetGetMethod(PropertyInfo property)
        {
            var instance = Expression.Parameter(typeof (object), "instance");
            var instanceCast = property.DeclaringType.IsValueType switch
            {
                true => Expression.Convert(instance, property.DeclaringType),
                false => Expression.TypeAs(instance, property.DeclaringType)
            };
            
            var call = Expression.Call(instanceCast, property.GetGetMethod());
            var typeAs = Expression.TypeAs(call, typeof (object));

            return Expression.Lambda<Func<object, object>>(typeAs, instance).Compile();
        }
        
        private static Action<object, object> GetSetMethod(PropertyInfo property)
        {
            var instance = Expression.Parameter(typeof (object), "instance");
            var value = Expression.Parameter(typeof (object), "value");

            // value as T is slightly faster than (T)value, so if it's not a value type, use that
            var instanceCast = property.DeclaringType.IsValueType switch
            {
                true => Expression.Convert(instance, property.DeclaringType),
                false => Expression.TypeAs(instance, property.DeclaringType)
            };
            
            var valueCast = property.DeclaringType.IsValueType switch
            {
                true => Expression.Convert(instance, property.PropertyType),
                false => Expression.TypeAs(instance, property.PropertyType)
            };
           

            var call = Expression.Call(instanceCast, property.GetSetMethod(), valueCast);
            return Expression.Lambda<Action<object, object>>(call, new[] {instance, value}).Compile();
        }

        public void AfterVisit(Thing thing)
        {
        }
        
    }
}
