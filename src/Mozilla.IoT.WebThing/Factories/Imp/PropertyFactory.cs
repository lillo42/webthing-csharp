using System;
using System.Linq;
using System.Reflection;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Properties;
using Mozilla.IoT.WebThing.Properties.Boolean;
using Mozilla.IoT.WebThing.Properties.Number;
using Mozilla.IoT.WebThing.Properties.String;

namespace Mozilla.IoT.WebThing.Factories
{
    /// <inheritdoc />
    public class PropertyFactory : IPropertyFactory
    {
        /// <inheritdoc />
        public IProperty Create(Type propertyType, Information information, Thing thing, 
            Action<object, object?> setter, Func<object, object?> getter)
        {
            propertyType = propertyType.GetUnderlyingType();
            if(propertyType == typeof(bool))
            { 
                return new PropertyBoolean(thing, getter, setter, information.IsNullable);
            }

            if (propertyType == typeof(string))
            {
                return new PropertyString(thing, getter, setter, information.IsNullable,
                    information.MinimumLength, information.MaximumLength, information.Pattern,
                    information.Enums?.Where(x => x != null).Select(Convert.ToString).ToArray()!);
            }
            
            if (propertyType == typeof(char))
            {
                return new PropertyChar(thing, getter, setter, information.IsNullable,
                    information.Enums?.Where(x => x != null).Select(Convert.ToChar).ToArray());
            }
            
            if(propertyType.IsEnum)
            {
                return new PropertyEnum(thing, getter, setter, information.IsNullable, propertyType);
            }
            
            if (propertyType == typeof(Guid))
            {
                return new PropertyGuid(thing, getter, setter, information.IsNullable,
                    information.Enums?.Where(x => x != null).Select(x=> Guid.Parse(x.ToString()!)).ToArray());
            }
            
            if (propertyType == typeof(TimeSpan))
            {
                return new PropertyTimeSpan(thing, getter, setter, information.IsNullable,
                    information.Enums?.Where(x => x != null).Select(x=> TimeSpan.Parse(x.ToString()!)).ToArray());
            }
            
            if (propertyType == typeof(DateTime))
            {
                return new PropertyDateTime(thing, getter, setter, information.IsNullable,
                    information.Enums?.Where(x => x != null).Select(Convert.ToDateTime).ToArray());
            }
            
            if (propertyType == typeof(DateTimeOffset))
            {
                return new PropertyDateTimeOffset(thing, getter, setter, information.IsNullable,
                    information.Enums?.Where(x => x != null).Select(x => DateTimeOffset.Parse(x.ToString()!)).ToArray());
            }
            
            var minimum = information.Minimum;
            var maximum = information.Maximum;
            var multipleOf = information.MultipleOf;
            var enums = information.Enums?.Where(x => x != null);
                
            if(information.ExclusiveMinimum.HasValue)
            {
                minimum = information.ExclusiveMinimum.Value + 1;
            }

            if(information.ExclusiveMaximum.HasValue)
            {
                maximum = information.ExclusiveMaximum.Value - 1;
            }
                

            if(propertyType == typeof(byte))
            {
                var min = minimum.HasValue ? new byte?(Convert.ToByte(minimum.Value)) : null;
                var max = maximum.HasValue ? new byte?(Convert.ToByte(maximum.Value)) : null;
                var multi = multipleOf.HasValue ? new byte?(Convert.ToByte(multipleOf.Value)) : null;

                return  new PropertyByte(thing, getter, setter, information.IsNullable, 
                    min, max, multi, enums?.Select(Convert.ToByte).ToArray());
            }

            if(propertyType == typeof(sbyte))
            {
                var min = minimum.HasValue ? new sbyte?(Convert.ToSByte(minimum.Value)) : null;
                var max = maximum.HasValue ? new sbyte?(Convert.ToSByte(maximum.Value)) : null;
                var multi = multipleOf.HasValue ? new sbyte?(Convert.ToSByte(multipleOf.Value)) : null;

                return  new PropertySByte(thing, getter, setter, information.IsNullable, 
                    min, max, multi, enums?.Select(Convert.ToSByte).ToArray());
            }
            
            if(propertyType == typeof(short))
            {
                var min = minimum.HasValue ? new short?(Convert.ToInt16(minimum.Value)) : null;
                var max = maximum.HasValue ? new short?(Convert.ToInt16(maximum.Value)) : null;
                var multi = multipleOf.HasValue ? new short?(Convert.ToInt16(multipleOf.Value)) : null;
                    
                return  new PropertyShort(thing, getter, setter, information.IsNullable, 
                    min, max, multi, enums?.Select(Convert.ToInt16).ToArray());
            }
            
            if(propertyType == typeof(ushort))
            {
                var min = minimum.HasValue ? new ushort?(Convert.ToUInt16(minimum.Value)) : null;
                var max = maximum.HasValue ? new ushort?(Convert.ToUInt16(maximum.Value)) : null;
                var multi = multipleOf.HasValue ? new byte?(Convert.ToByte(multipleOf.Value)) : null;

                return  new PropertyUShort(thing, getter, setter, information.IsNullable, 
                    min, max, multi, enums?.Select(Convert.ToUInt16).ToArray());
            }
            
            if(propertyType == typeof(int))
            {
                var min = minimum.HasValue ? new int?(Convert.ToInt32(minimum.Value)) : null;
                var max = maximum.HasValue ? new int?(Convert.ToInt32(maximum.Value)) : null;
                var multi = multipleOf.HasValue ? new int?(Convert.ToInt32(multipleOf.Value)) : null;

                return  new PropertyInt(thing, getter, setter, information.IsNullable, 
                    min, max, multi, enums?.Select(Convert.ToInt32).ToArray());
            }
            
            if(propertyType == typeof(uint))
            {
                var min = minimum.HasValue ? new uint?(Convert.ToUInt32(minimum.Value)) : null;
                var max = maximum.HasValue ? new uint?(Convert.ToUInt32(maximum.Value)) : null;
                var multi = multipleOf.HasValue ? new uint?(Convert.ToUInt32(multipleOf.Value)) : null;

                return  new PropertyUInt(thing, getter, setter, information.IsNullable, 
                    min, max, multi, enums?.Select(Convert.ToUInt32).ToArray());
            }
            
            if(propertyType == typeof(long))
            {
                var min = minimum.HasValue ? new long?(Convert.ToInt64(minimum.Value)) : null;
                var max = maximum.HasValue ? new long?(Convert.ToInt64(maximum.Value)) : null;
                var multi = multipleOf.HasValue ? new long?(Convert.ToInt64(multipleOf.Value)) : null;

                return  new PropertyLong(thing, getter, setter, information.IsNullable, 
                    min, max, multi, enums?.Select(Convert.ToInt64).ToArray());
            }
            
            if(propertyType == typeof(ulong))
            {
                var min = minimum.HasValue ? new ulong?(Convert.ToUInt64(minimum.Value)) : null;
                var max = maximum.HasValue ? new ulong?(Convert.ToUInt64(maximum.Value)) : null;
                var multi = multipleOf.HasValue ? new byte?(Convert.ToByte(multipleOf.Value)) : null;

                return  new PropertyULong(thing, getter, setter, information.IsNullable, 
                    min, max, multi, enums?.Select(Convert.ToUInt64).ToArray());
            }
            
            if(propertyType == typeof(float))
            {
                var min = minimum.HasValue ? new float?(Convert.ToSingle(minimum.Value)) : null;
                var max = maximum.HasValue ? new float?(Convert.ToSingle(maximum.Value)) : null;
                var multi = multipleOf.HasValue ? new float?(Convert.ToSingle(multipleOf.Value)) : null;

                return  new PropertyFloat(thing, getter, setter, information.IsNullable, 
                    min, max, multi, enums?.Select(Convert.ToSingle).ToArray());
            }
            
            if(propertyType == typeof(double))
            {
                var min = minimum.HasValue ? new double?(Convert.ToDouble(minimum.Value)) : null;
                var max = maximum.HasValue ? new double?(Convert.ToDouble(maximum.Value)) : null;
                var multi = multipleOf.HasValue ? new double?(Convert.ToDouble(multipleOf.Value)) : null;

                return  new PropertyDouble(thing, getter, setter, information.IsNullable, 
                    min, max, multi, enums?.Select(Convert.ToDouble).ToArray());
            }
            else
            {
                var min = minimum.HasValue ? new decimal?(Convert.ToDecimal(minimum.Value)) : null;
                var max = maximum.HasValue ? new decimal?(Convert.ToDecimal(maximum.Value)) : null;
                var multi = multipleOf.HasValue ? new decimal?(Convert.ToDecimal(multipleOf.Value)) : null;

                return  new PropertyDecimal(thing, getter, setter, information.IsNullable, 
                    min, max, multi, enums?.Select(Convert.ToDecimal).ToArray());
            }
        }
    }
}
