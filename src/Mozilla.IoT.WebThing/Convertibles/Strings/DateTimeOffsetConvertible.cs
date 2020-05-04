using System;

namespace Mozilla.IoT.WebThing.Convertibles.Strings
{
    /// <summary>
    /// Convert value to <see cref="DateTimeOffset"/>
    /// </summary>
    public class DateTimeOffsetConvertible : IConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="DateTimeOffsetConvertible"/>
        /// </summary>
        public static DateTimeOffsetConvertible Instance { get; } = new DateTimeOffsetConvertible();

        /// <inheritdoc/>
        public object? Convert(object? value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is DateTimeOffset dateTimeOffset)
            {
                return dateTimeOffset;
            }

            return DateTimeOffset.Parse(value.ToString()!);
        }
    }
}
