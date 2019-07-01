using System.Diagnostics.CodeAnalysis;

namespace System
{
    [ExcludeFromCodeCoverage]
    internal static class StringExtensions
    {
        internal static string JoinUrl(this string left, string right)
        {
            var start = string.Empty;

            if (!left.StartsWith('/'))
            {
                start = "/";
            }

            if ((left.EndsWith('/') && !right.StartsWith('/'))
                || (!left.EndsWith('/') && right.StartsWith('/')))
            {
                return $"{start}{left}{right}";
            }

            if (left.EndsWith('/') && right.StartsWith('/'))
            {
                if (left.Length == 1 && right.Length == 1)
                {
                    return $"{start}/";        
                }

                if (left.Length == 1 && right.Length > 1)
                {
                    return $"{start}{left}{right.Remove(0, 1)}";
                }

                return $"{start}{left.Remove(left.Length - 2)}{right}";
            }
            
            return $"{start}{left}/{right}";
        }
    }
}
