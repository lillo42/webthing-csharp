using System;
using TypeCode = Mozilla.IoT.WebThing.Factories.TypeCode;

namespace Mozilla.IoT.WebThing.Convertibles.Number
{
    /// <summary>
    /// Convert value to <see cref="char"/>
    /// </summary>
    public class NumberConvertible : IConvertible
    {
        private readonly TypeCode _type;

        /// <summary>
        /// Initialize a new instance of <see cref="NumberConvertible"/>.
        /// </summary>
        /// <param name="type">The <see cref="TypeCode"/>.</param>
        public NumberConvertible(TypeCode type)
        {
            _type = type;
        }

        /// <inheritdoc/>
        public object? Convert(object? value)
        {
            if (value == null)
            {
                return null;
            }

            switch (_type)
            {
                case TypeCode.SByte:
                    if (value is sbyte sb)
                    {
                        return sb;
                    }
                    
                    return System.Convert.ToSByte(value);
                case TypeCode.Byte:
                    if (value is sbyte b)
                    {
                        return b;
                    }
                    
                    return System.Convert.ToByte(value);
                case TypeCode.Int16:
                    if (value is short s)
                    {
                        return s;
                    }
                    
                    return System.Convert.ToInt16(value);
                case TypeCode.UInt16:
                    if (value is ushort us)
                    {
                        return us;
                    }
                    
                    return System.Convert.ToUInt16(value);
                case TypeCode.Int32:
                    if (value is int i)
                    {
                        return i;
                    }
                    
                    return System.Convert.ToInt32(value);
                case TypeCode.UInt32: 
                    if (value is uint ui)
                    {
                        return ui;
                    }
                    
                    return System.Convert.ToUInt32(value);
                case TypeCode.Int64:
                    if (value is long l)
                    {
                        return l;
                    }
                    
                    return System.Convert.ToInt64(value);
                case TypeCode.UInt64:
                    if (value is ulong ul)
                    {
                        return ul;
                    }
                    
                    return System.Convert.ToUInt64(value);
                case TypeCode.Float:
                    if (value is float f)
                    {
                        return f;
                    }
                    
                    return System.Convert.ToSingle(value);
                case TypeCode.Double:
                    if (value is double d)
                    {
                        return d;
                    }
                    
                    return System.Convert.ToDouble(value);
                case TypeCode.Decimal:
                    if (value is decimal dd)
                    {
                        return dd;
                    }
                    
                    return System.Convert.ToDecimal(value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
