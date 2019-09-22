using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Generic
{
    [ExcludeFromCodeCoverage]
    internal static class CollectionExtensions
    {
        public static bool Remove<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            foreach (T item in collection)
            {
                if (predicate(item))
                {
                    return collection.Remove(item);
                }
            }

            return false;
        }
        
    }
}
