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
                    return System.Convert.ToSByte(value);
                case TypeCode.Byte:
                    return System.Convert.ToByte(value);
                case TypeCode.Int16:
                    return System.Convert.ToInt16(value);
                case TypeCode.UInt16:
                    return System.Convert.ToUInt16(value);
                case TypeCode.Int32:
                    return System.Convert.ToInt32(value);
                case TypeCode.UInt32: 
                    return System.Convert.ToUInt32(value);
                case TypeCode.Int64:
                    return System.Convert.ToInt64(value);
                case TypeCode.UInt64:
                    return System.Convert.ToUInt64(value);
                case TypeCode.Float:
                    return System.Convert.ToSingle(value);
                case TypeCode.Double:
                    return System.Convert.ToDouble(value);
                case TypeCode.Decimal:
                    return System.Convert.ToDecimal(value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
