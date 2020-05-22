using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories;

namespace Mozilla.IoT.WebThing.Builders
{
    /// <inheritdoc /> 
    public class PropertyBuilder : IPropertyBuilder
    {
        private readonly IPropertyFactory _factory;
        private Thing? _thing;
        private ThingOption? _option;
        private Dictionary<string, IThingProperty>? _properties;

        /// <summary>
        /// Initialize a new instance of <see cref="PropertyBuilder"/>.
        /// </summary>
        /// <param name="factory">The <see cref="IPropertyFactory"/>.</param>
        public PropertyBuilder(IPropertyFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <inheritdoc /> 
        public IPropertyBuilder SetThing(Thing thing)
        {
            _thing = thing;
            return this;
        }
        
        /// <inheritdoc /> 
        public IPropertyBuilder SetThingOption(ThingOption option)
        {
            _option = option;
            _properties = new Dictionary<string, IThingProperty>(option.IgnoreCase ? StringComparer.OrdinalIgnoreCase : null);
            return this;
        }

        /// <inheritdoc /> 
        public void Add(PropertyInfo property, JsonSchema jsonSchema)
        {
            if (_thing == null)
            {
                throw new InvalidOperationException($"Thing is null, call {nameof(SetThing)} before build");
            }
            
            if (_properties == null || _option == null)
            {
                throw new InvalidOperationException($"ThingOption is null, call {nameof(SetThingOption)} before add");
            }

            var getter = GetGetMethod(property);
            var propertyName = _option.PropertyNamingPolicy.ConvertName(jsonSchema.Name);
            
            if (jsonSchema.IsReadOnly.GetValueOrDefault())
            {
                _properties.Add(propertyName, 
                    new ThingProperty(_thing, jsonSchema.IsReadOnly.GetValueOrDefault(), jsonSchema.IsWriteOnly.GetValueOrDefault(), 
                        getter, null, null, null, null, property.Name));
                return;
            }
            
            var setter = GetSetMethod(property);
            
            _properties.Add(_option.PropertyNamingPolicy.ConvertName(jsonSchema.Name), 
                _factory.Create(property.PropertyType, jsonSchema, _thing, setter, getter, property.Name));

            static Func<object, object?> GetGetMethod(PropertyInfo property)
            {
                if (!property.CanRead || !property.GetMethod!.IsPublic)
                {
                    return _ => null;
                }
                
                var instance = Expression.Parameter(typeof(object), "instance");
                var instanceCast = property.DeclaringType!.IsValueType ? 
                    Expression.Convert(instance, property.DeclaringType) : Expression.TypeAs(instance, property.DeclaringType);
            
                var call = Expression.Call(instanceCast, property.GetGetMethod());
                var typeAs = Expression.TypeAs(call, typeof(object));

                return Expression.Lambda<Func<object, object>>(typeAs, instance).Compile();
            }
            
            static Action<object, object?>? GetSetMethod(PropertyInfo property)
            {
               
                var instance = Expression.Parameter(typeof(object), "instance");
                var value = Expression.Parameter(typeof(object), "value");

                // value as T is slightly faster than (T)value, so if it's not a value type, use that
                var instanceCast = property.DeclaringType!.IsValueType ? 
                    Expression.Convert(instance, property.DeclaringType) : Expression.TypeAs(instance, property.DeclaringType);
            
                var valueCast = property.PropertyType.IsValueType ? 
                    Expression.Convert(value, property.PropertyType) : Expression.TypeAs(value, property.PropertyType);

                var call = Expression.Call(instanceCast, property.GetSetMethod(), valueCast);
                return Expression.Lambda<Action<object, object>>(call, new[] {instance, value}).Compile()!;
            }
        }

        /// <inheritdoc /> 
        public Dictionary<string, IThingProperty> Build()
        {
            if (_properties == null || _option == null)
            {
                throw new InvalidOperationException($"ThingOption is null, call {nameof(SetThingOption)} before add");
            }
            
            return _properties!;
        }
    }
}
