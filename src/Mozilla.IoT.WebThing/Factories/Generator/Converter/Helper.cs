using System;
using System.Collections;
using System.Linq;

namespace Mozilla.IoT.WebThing.Factories.Generator.Converter
{
    internal static class Helper
    {
        public static JsonType? GetJsonType(Type? type)
        {
            if (type == null)
            {
                return null;
            }
            
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(string)
                || type == typeof(char)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset)
                || type == typeof(Guid)
                || type == typeof(TimeSpan))
            {
                return JsonType.String;
            }

            if (type == typeof(bool))
            {
                return JsonType.Boolean;
            }
            
            if (type == typeof(int)
                || type == typeof(sbyte)
                || type == typeof(byte)
                || type == typeof(short)
                || type == typeof(long)
                || type == typeof(uint)
                || type == typeof(ulong)
                || type == typeof(ushort))
            {
                return JsonType.Integer;
            }
            
            if (type == typeof(double)
                || type == typeof(float)
                || type == typeof(decimal))
            {
                return JsonType.Number;
            }

            if (type.IsArray || type.GetInterfaces().Any(x => x == typeof(IEnumerable)))
            {
                return JsonType.Array;
            }

            return null;
        }
    }
}
