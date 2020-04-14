using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mozilla.IoT.WebThing.Convertibles;
using Mozilla.IoT.WebThing.Convertibles.Collection;
using Mozilla.IoT.WebThing.Convertibles.Number;
using Mozilla.IoT.WebThing.Convertibles.Strings;
using IConvertible = Mozilla.IoT.WebThing.Convertibles.IConvertible;

namespace Mozilla.IoT.WebThing.Factories
{
    /// <inheritdoc />
    public class SystemTextJsonConvertible : IConvertibleFactory
    {
        /// <inheritdoc />
        public IConvertible? Create(TypeCode typeCode, Type type)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return BooleanConvertible.Instance;
                case TypeCode.String:
                    return StringConvertible.Instance;
                case TypeCode.Char:
                    return CharConvertible.Instance;
                case TypeCode.DateTime:
                    return DateTimeConvertible.Instance;
                case TypeCode.DateTimeOffset:
                    return DateTimeOffsetConvertible.Instance;
                case TypeCode.Guid:
                    return GuidConvertible.Instance;
                case TypeCode.TimeSpan:
                    return TimeSpanConvertible.Instance;
                case TypeCode.Enum:
                    return new EnumConvertible(type);
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Float:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return new NumberConvertible(typeCode);
                case TypeCode.Array:
                    var arrayType = type.GetCollectionType();
                    var convertible = Create(arrayType.ToTypeCode(), arrayType);

                    var interfaces = type.GetInterfaces();
                    Type? convertibleType = null;
                    if (interfaces.Any(x => x == typeof(ISet<>) || x == typeof(HashSet<>)))
                    {
                        convertibleType  = typeof(HashSetConvertible<>).MakeGenericType(arrayType);
                    }
                    else if (interfaces.Any(x => x == typeof(ICollection<>) 
                                                 || x == typeof(IEnumerable<>)
                                                 || x == typeof(LinkedList<>)))
                    {
                        convertibleType  = typeof(LinkedListConvertible<>).MakeGenericType(arrayType);
                    }
                    else if (interfaces.Any(x => x == typeof(IList<>) 
                                                 || x == typeof(List<>)))
                    {
                        convertibleType  = typeof(ListConvertible<>).MakeGenericType(arrayType);
                    }
                    else
                    {
                        convertibleType  = typeof(ArrayConvertible<>).MakeGenericType(arrayType);
                    }

                    if (convertibleType != null)
                    {
                        return (IConvertible)Activator.CreateInstance(convertibleType, convertible)!;
                    }

                    return null;
                
                case TypeCode.Object:
                    return ObjectConvertible.Instance;
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeCode), typeCode, null);
            }
        }
    }
}
