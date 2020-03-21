using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;
using Mozilla.IoT.WebThing.Properties;
using Mozilla.IoT.WebThing.Properties.Boolean;
using Mozilla.IoT.WebThing.Properties.String;
using Mozilla.IoT.WebThing.Properties.Number;

namespace Mozilla.IoT.WebThing.Factories.Generator.Properties
{
    /// <inheritdoc /> 
    public class PropertiesIntercept : IPropertyIntercept
    {
        /// <summary>
        /// The <see cref="IProperty"/> created, map by action name.
        /// </summary>
        public Dictionary<string, IProperty> Properties { get; }
        
        /// <summary>
        /// Initialize a new instance of <see cref="PropertiesIntercept"/>.
        /// </summary>
        /// <param name="option">The <see cref="ThingOption"/>.</param>
        public PropertiesIntercept(ThingOption option)
        {
            Properties = option.IgnoreCase ? new Dictionary<string, IProperty>(StringComparer.InvariantCultureIgnoreCase) 
                : new Dictionary<string, IProperty>();
        }
        
        /// <inheritdoc /> 
        public void Before(Thing thing)
        {
            
        }
        
        /// <inheritdoc /> 
        public void After(Thing thing)
        {
            
        }
        
        /// <inheritdoc /> 
        public void Visit(Thing thing, PropertyInfo propertyInfo, ThingPropertyAttribute? propertyAttribute)
        {
            var propertyName = propertyAttribute?.Name ?? propertyInfo.Name;

            var isReadOnly = !propertyInfo.CanWrite || !propertyInfo.SetMethod!.IsPublic ||
                             (propertyAttribute != null && propertyAttribute.IsReadOnly);

            var getter = GetGetMethod(propertyInfo);

            if (isReadOnly)
            {
                Properties.Add(propertyName, new PropertyReadOnly(thing, getter));
                return;
            }

            var validation = ToValidation(propertyAttribute);

            var setter = GetSetMethod(propertyInfo);
            var propertyType = propertyInfo.PropertyType.GetUnderlyingType();
            var isNullable = propertyType ==  typeof(string) || propertyInfo.PropertyType.IsNullable() || validation.HasNullValueOnEnum;

            IProperty property;
            
            if(propertyType == typeof(bool))
            {
                property = new PropertyBoolean(thing, getter, setter, isNullable);
            }
            else if (propertyType == typeof(string))
            {
                property = new PropertyString(thing, getter, setter, isNullable,
                    validation.MinimumLength, validation.MaximumLength, validation.Pattern,
                    validation.Enums?.Where(x => x != null).Select(Convert.ToString).ToArray()!);
            }
            else if (propertyType == typeof(char))
            {
                property = new PropertyChar(thing, getter, setter, isNullable, validation.Enums?.Where(x => x != null).Select(Convert.ToChar).ToArray());
            }
            else if (propertyType == typeof(Guid))
            {
                property = new PropertyGuid(thing, getter, setter, isNullable,
                    validation.Enums?.Where(x => x != null).Select(x=> Guid.Parse(x.ToString()!)).ToArray());
            }
            else if (propertyType == typeof(TimeSpan))
            {
                property = new PropertyTimeSpan(thing, getter, setter, isNullable,
                    validation.Enums?.Where(x => x != null).Select(x=> TimeSpan.Parse(x.ToString()!)).ToArray());
            }
            else if (propertyType == typeof(DateTime))
            {
                property = new PropertyDateTime(thing, getter, setter, isNullable,
                    validation.Enums?.Where(x => x != null).Select(Convert.ToDateTime).ToArray());
            }
            else if (propertyType == typeof(DateTimeOffset))
            {
                property = new PropertyDateTimeOffset(thing, getter, setter, isNullable,
                    validation.Enums?.Where(x => x != null).Select(x => DateTimeOffset.Parse(x.ToString()!)).ToArray());
            }
            else
            {
                var minimum = validation.Minimum;
                var maximum = validation.Maximum;
                var multipleOf = validation.MultipleOf;
                var enums = validation.Enums?.Where(x => x != null);
                
                if(validation.ExclusiveMinimum.HasValue)
                {
                    minimum = validation.ExclusiveMinimum.Value + 1;
                }

                if(validation.ExclusiveMaximum.HasValue)
                {
                    maximum = validation.ExclusiveMaximum.Value - 1;
                }
                

                if(propertyType == typeof(byte))
                {
                    var min = minimum.HasValue ? new byte?(Convert.ToByte(minimum.Value)) : null;
                    var max = maximum.HasValue ? new byte?(Convert.ToByte(maximum.Value)) : null;
                    var multi = multipleOf.HasValue ? new byte?(Convert.ToByte(multipleOf.Value)) : null;

                    property = new PropertyByte(thing, getter, setter, isNullable, 
                        min, max, multi, enums?.Select(Convert.ToByte).ToArray());
                }
                else if(propertyType == typeof(sbyte))
                {
                    var min = minimum.HasValue ? new sbyte?(Convert.ToSByte(minimum.Value)) : null;
                    var max = maximum.HasValue ? new sbyte?(Convert.ToSByte(maximum.Value)) : null;
                    var multi = multipleOf.HasValue ? new sbyte?(Convert.ToSByte(multipleOf.Value)) : null;

                    property = new PropertySByte(thing, getter, setter, isNullable, 
                        min, max, multi, enums?.Select(Convert.ToSByte).ToArray());
                }
                else if(propertyType == typeof(short))
                {
                    var min = minimum.HasValue ? new short?(Convert.ToInt16(minimum.Value)) : null;
                    var max = maximum.HasValue ? new short?(Convert.ToInt16(maximum.Value)) : null;
                    var multi = multipleOf.HasValue ? new short?(Convert.ToInt16(multipleOf.Value)) : null;
                    
                    property = new PropertyShort(thing, getter, setter, isNullable, 
                        min, max, multi, enums?.Select(Convert.ToInt16).ToArray());
                }
                else if(propertyType == typeof(ushort))
                {
                    var min = minimum.HasValue ? new ushort?(Convert.ToUInt16(minimum.Value)) : null;
                    var max = maximum.HasValue ? new ushort?(Convert.ToUInt16(maximum.Value)) : null;
                    var multi = multipleOf.HasValue ? new byte?(Convert.ToByte(multipleOf.Value)) : null;

                    property = new PropertyUShort(thing, getter, setter, isNullable, 
                        min, max, multi, enums?.Select(Convert.ToUInt16).ToArray());
                }
                else if(propertyType == typeof(int))
                {
                    var min = minimum.HasValue ? new int?(Convert.ToInt32(minimum.Value)) : null;
                    var max = maximum.HasValue ? new int?(Convert.ToInt32(maximum.Value)) : null;
                    var multi = multipleOf.HasValue ? new int?(Convert.ToInt32(multipleOf.Value)) : null;

                    property = new PropertyInt(thing, getter, setter, isNullable, 
                        min, max, multi, enums?.Select(Convert.ToInt32).ToArray());
                }
                else if(propertyType == typeof(uint))
                {
                    var min = minimum.HasValue ? new uint?(Convert.ToUInt32(minimum.Value)) : null;
                    var max = maximum.HasValue ? new uint?(Convert.ToUInt32(maximum.Value)) : null;
                    var multi = multipleOf.HasValue ? new uint?(Convert.ToUInt32(multipleOf.Value)) : null;

                    property = new PropertyUInt(thing, getter, setter, isNullable, 
                        min, max, multi, enums?.Select(Convert.ToUInt32).ToArray());
                }
                else if(propertyType == typeof(long))
                {
                    var min = minimum.HasValue ? new long?(Convert.ToInt64(minimum.Value)) : null;
                    var max = maximum.HasValue ? new long?(Convert.ToInt64(maximum.Value)) : null;
                    var multi = multipleOf.HasValue ? new long?(Convert.ToInt64(multipleOf.Value)) : null;

                    property = new PropertyLong(thing, getter, setter, isNullable, 
                        min, max, multi, enums?.Select(Convert.ToInt64).ToArray());
                }
                else if(propertyType == typeof(ulong))
                {
                    var min = minimum.HasValue ? new ulong?(Convert.ToUInt64(minimum.Value)) : null;
                    var max = maximum.HasValue ? new ulong?(Convert.ToUInt64(maximum.Value)) : null;
                    var multi = multipleOf.HasValue ? new byte?(Convert.ToByte(multipleOf.Value)) : null;

                    property = new PropertyULong(thing, getter, setter, isNullable, 
                        min, max, multi, enums?.Select(Convert.ToUInt64).ToArray());
                }
                else if(propertyType == typeof(float))
                {
                    var min = minimum.HasValue ? new float?(Convert.ToSingle(minimum.Value)) : null;
                    var max = maximum.HasValue ? new float?(Convert.ToSingle(maximum.Value)) : null;
                    var multi = multipleOf.HasValue ? new float?(Convert.ToSingle(multipleOf.Value)) : null;

                    property = new PropertyFloat(thing, getter, setter, isNullable, 
                        min, max, multi, enums?.Select(Convert.ToSingle).ToArray());
                }
                else if(propertyType == typeof(double))
                {
                    var min = minimum.HasValue ? new double?(Convert.ToDouble(minimum.Value)) : null;
                    var max = maximum.HasValue ? new double?(Convert.ToDouble(maximum.Value)) : null;
                    var multi = multipleOf.HasValue ? new double?(Convert.ToDouble(multipleOf.Value)) : null;

                    property = new PropertyDouble(thing, getter, setter, isNullable, 
                        min, max, multi, enums?.Select(Convert.ToDouble).ToArray());
                }
                else
                {
                    var min = minimum.HasValue ? new decimal?(Convert.ToDecimal(minimum.Value)) : null;
                    var max = maximum.HasValue ? new decimal?(Convert.ToDecimal(maximum.Value)) : null;
                    var multi = multipleOf.HasValue ? new decimal?(Convert.ToDecimal(multipleOf.Value)) : null;

                    property = new PropertyDecimal(thing, getter, setter, isNullable, 
                        min, max, multi, enums?.Select(Convert.ToDecimal).ToArray());
                }
            }
            
            Properties.Add(propertyName, property);
            
            static Validation ToValidation(ThingPropertyAttribute? validation)
            {
                return new Validation(validation?.MinimumValue, validation?.MaximumValue,
                    validation?.ExclusiveMinimumValue, validation?.ExclusiveMaximumValue,
                    validation?.MultipleOfValue,
                    validation?.MinimumLengthValue, validation?.MaximumLengthValue,
                    validation?.Pattern, validation?.Enum);
            }
        }
        
        private static Func<object, object> GetGetMethod(PropertyInfo property)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var instanceCast = property.DeclaringType!.IsValueType ? 
                Expression.Convert(instance, property.DeclaringType) : Expression.TypeAs(instance, property.DeclaringType);
            
            var call = Expression.Call(instanceCast, property.GetGetMethod());
            var typeAs = Expression.TypeAs(call, typeof(object));

            return Expression.Lambda<Func<object, object>>(typeAs, instance).Compile();
        }
        
        private static Action<object, object?> GetSetMethod(PropertyInfo property)
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
}
