using System;

namespace Mozilla.IoT.WebThing.Factories.Generator.Converter
{
    internal static class Helper
    {
        public static string? GetJsonType(Type? type)
        {
            if (type == null)
            {
                return null;
            }
            
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(string)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset))
            {
                return "string";
            }

            if (type == typeof(bool))
            {
                return "boolean";
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
                return "integer";
            }
            
            if (type == typeof(double)
                || type == typeof(float)
                || type == typeof(decimal))
            {
                return "number";
            }

            return null;
        }
    }
}
