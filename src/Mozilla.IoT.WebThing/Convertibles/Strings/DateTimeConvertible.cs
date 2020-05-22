using System;

namespace Mozilla.IoT.WebThing.Convertibles.Strings
{
    /// <summary>
    /// Convert value to <see cref="DateTime"/>
    /// </summary>
    public class DateTimeConvertible : IConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="DateTimeConvertible"/>
        /// </summary>
        public static DateTimeConvertible Instance { get; } = new DateTimeConvertible();

        /// <inheritdoc/>
        public object? Convert(object? value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is DateTime dateTime)
            {
                return dateTime;
            }

            return DateTime.Parse(value.ToString()!);
        }
    }
}
