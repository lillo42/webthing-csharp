using System.Diagnostics.CodeAnalysis;

namespace System
{
    [ExcludeFromCodeCoverage]
    internal static class StringExtensions
    {
        internal static string JoinUrl(this string left, string right)
        {
            if (left == null)
            {
                left = string.Empty;
            }

            if (right == null)
            {
                right = string.Empty;
            }

            var start = string.Empty;
            

            if (!left.StartsWith('/'))
            {
                start = "/";
            }

            if (string.IsNullOrEmpty(left))
            {
                if (right.StartsWith('/'))
                {
                    return right;
                }

                return string.Concat(start, right);
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
