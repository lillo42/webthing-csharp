namespace System.Reflection
{
    internal static class TypeExtensions
    {
        public static bool IsNullable(this Type type)
            => Nullable.GetUnderlyingType(type) != null;

        public static Type GetUnderlyingType(this Type type)
            => Nullable.GetUnderlyingType(type) ?? type;
    }
}
