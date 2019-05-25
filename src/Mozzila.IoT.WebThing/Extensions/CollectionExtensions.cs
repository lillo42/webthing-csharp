using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    [ExcludeFromCodeCoverage]
    internal static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (T item in source)
            {
                action(item);
            }
        }
        
        
        public static async Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (T item in source)
            {
                await action(item);
            }
        }
        
        public static async Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, CancellationToken, Task> action, CancellationToken cancellation)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (T item in source)
            {
                await action(item, cancellation);
            }
        }
    }
}
