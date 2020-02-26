using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;
using Mozilla.IoT.WebThing.Mapper;

namespace Mozilla.IoT.WebThing.Factories.Generator.Properties
{
    internal class PropertiesIntercept : IPropertyIntercept
    {
        public Dictionary<string, Property> Properties { get; }
        private readonly ThingOption _option;

        public PropertiesIntercept(ThingOption option)
        {
            _option = option ?? throw new ArgumentNullException(nameof(option));
            Properties = option.IgnoreCase ? new Dictionary<string, Property>(StringComparer.InvariantCultureIgnoreCase) 
                : new Dictionary<string, Property>();
        }

        public void Before(Thing thing)
        {

        }

        public void Intercept(Thing thing, PropertyInfo propertyInfo, ThingPropertyAttribute? thingPropertyAttribute)
        {
            var propertyName =  thingPropertyAttribute?.Name ?? propertyInfo.Name;
            Properties.Add(_option.PropertyNamingPolicy.ConvertName(propertyName), new Property(GetGetMethod(propertyInfo),
                GetSetMethod(propertyInfo, thingPropertyAttribute),
                CreateValidator(propertyInfo, thingPropertyAttribute),
                CreateMapper(propertyInfo.PropertyType)));
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
        
        private static Action<object, object> GetSetMethod(PropertyInfo property, ThingPropertyAttribute? thingPropertyAttribute)
        {
            if ((thingPropertyAttribute != null && thingPropertyAttribute.IsReadOnly)
                || !property.CanWrite || !property.SetMethod.IsPublic) 
            {
                return null;
            }
            
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

        private static IPropertyValidator CreateValidator(PropertyInfo propertyInfo, ThingPropertyAttribute? thingPropertyAttribute)
        {
            return new PropertyValidator(
                thingPropertyAttribute?.IsReadOnly ?? !propertyInfo.CanWrite,
                thingPropertyAttribute?.MinimumValue,
                thingPropertyAttribute?.MaximumValue,
                thingPropertyAttribute?.MultipleOfValue,
                Cast(thingPropertyAttribute?.Enum, propertyInfo.PropertyType),
                propertyInfo.PropertyType == typeof(string) || Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null,
                thingPropertyAttribute?.ExclusiveMinimumValue, thingPropertyAttribute?.ExclusiveMaximumValue);
        }

        private static IJsonMapper CreateMapper(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type == typeof(string))
            {
                return StringJsonMapper.Instance;
            }

            if(type == typeof(bool))
            {
                return BoolJsonMapper.Instance;
            }

            if (type == typeof(int))
            {
                return IntJsonMapper.Instance;
            }
            
            if (type == typeof(uint))
            {
                return UIntJsonMapper.Instance;
            }

            if (type == typeof(long))
            {
                return LongJsonMapper.Instance;
            }

            if (type == typeof(ulong))
            {
                return ULongJsonMapper.Instance;
            }

            if (type == typeof(short))
            {
                return ShortJsonMapper.Instance;
            }

            if (type == typeof(ushort))
            {
                return UShortJsonMapper.Instance;
            }
            
            if (type == typeof(double))
            {
                return DoubleJsonMapper.Instance;
            }

            if (type == typeof(float))
            {
                return FloatJsonMapper.Instance;
            }
            
            if (type == typeof(byte))
            {
                return ByteJsonMapper.Instance;
            }
            
            if (type == typeof(sbyte))
            {
                return SByteJsonMapper.Instance;
            }
            
            if (type == typeof(decimal))
            {
                return DecimalJsonMapper.Instance;
            }
            
            if (type == typeof(DateTime))
            {
                return DateTimeJsonMapper.Instance;
            }
            
            if (type == typeof(DateTimeOffset))
            {
                return DateTimeOffsetJsonMapper.Instance;
            }
            
            
            throw new Exception();
        }
        
        private static object[] Cast(object?[] enums, Type type)
        {
            if (enums == null)
            {
                return null;
            }

            type = Nullable.GetUnderlyingType(type) ?? type;

            var result = new object?[enums.Length];
            if (type == typeof(string))
            {
                for (var i = 0; i < enums.Length; i++)
                {
                    if (enums[i] == null)
                    {
                        result[i] = null;
                        continue;
                    }
                    
                    result[i] = Convert.ToString(enums[i]);
                }
            }

            if(type == typeof(bool))
            {
                for (var i = 0; i < enums.Length; i++)
                {
                    if (enums[i] == null)
                    {
                        result[i] = null;
                        continue;
                    }
                    
                    result[i] = Convert.ToBoolean(enums[i]);
                }
            }

            if (type == typeof(int))
            {
                for (var i = 0; i < enums.Length; i++)
                {
                    if (enums[i] == null)
                    {
                        result[i] = null;
                        continue;
                    }
                    
                    result[i] = Convert.ToInt32(enums[i]);
                }
            }
            
            if (type == typeof(uint))
            {
                for (var i = 0; i < enums.Length; i++)
                {
                    if (enums[i] == null)
                    {
                        result[i] = null;
                        continue;
                    }
                    
                    result[i] = Convert.ToUInt32(enums[i]);
                }
            }

            if (type == typeof(long))
            {
                for (var i = 0; i < enums.Length; i++)
                {
                    if (enums[i] == null)
                    {
                        result[i] = null;
                        continue;
                    }
                    
                    result[i] = Convert.ToInt64(enums[i]);
                }
            }

            if (type == typeof(ulong))
            {
                for (var i = 0; i < enums.Length; i++)
                {
                    if (enums[i] == null)
                    {
                        result[i] = null;
                        continue;
                    }
                    
                    result[i] = Convert.ToUInt64(enums[i]);
                }
            }

            if (type == typeof(short))
            {
                for (var i = 0; i < enums.Length; i++)
                {
                    if (enums[i] == null)
                    {
                        result[i] = null;
                        continue;
                    }
                    
                    result[i] = Convert.ToInt16(enums[i]);
                }
            }

            if (type == typeof(ushort))
            {
                for (var i = 0; i < enums.Length; i++)
                {
                    if (enums[i] == null)
                    {
                        result[i] = null;
                        continue;
                    }
                    
                    result[i] = Convert.ToUInt16(enums[i]);
                }
            }
            
            if (type == typeof(double))
            {
                for (var i = 0; i < enums.Length; i++)
                {
                    if (enums[i] == null)
                    {
                        result[i] = null;
                        continue;
                    }
                    
                    result[i] = Convert.ToDouble(enums[i]);
                }
            }

            if (type == typeof(float))
            {
                for (var i = 0; i < enums.Length; i++)
                {
                    if (enums[i] == null)
                    {
                        result[i] = null;
                        continue;
                    }
                    
                    result[i] = Convert.ToSingle(enums[i]);
                }
            }
            
            if (type == typeof(byte))
            {
                for (var i = 0; i < enums.Length; i++)
                {
                    if (enums[i] == null)
                    {
                        result[i] = null;
                        continue;
                    }
                    
                    result[i] = Convert.ToByte(enums[i]);
                }
            }
            
            if (type == typeof(sbyte))
            {
                for (var i = 0; i < enums.Length; i++)
                {
                    if (enums[i] == null)
                    {
                        result[i] = null;
                        continue;
                    }
                    
                    result[i] = Convert.ToSByte(enums[i]);
                }
            }
            
            if (type == typeof(decimal))
            {
                for (var i = 0; i < enums.Length; i++)
                {
                    if (enums[i] == null)
                    {
                        result[i] = null;
                        continue;
                    }
                    
                    result[i] = Convert.ToDecimal(enums[i]);
                }
            }
            
            if (type == typeof(DateTime))
            {
                for (var i = 0; i < enums.Length; i++)
                {
                    if (enums[i] == null)
                    {
                        result[i] = null;
                        continue;
                    }
                    
                    result[i] = Convert.ToDateTime(enums[i]);
                }
            }

            return result;
        }
        
        public void After(Thing thing)
        {
        }
    }
}
