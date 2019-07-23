namespace System.Collections.Generic
{
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