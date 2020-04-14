using System;

namespace Mozilla.IoT.WebThing.Convertibles.Strings
{
    /// <summary>
    /// Convert value to <see cref="TimeSpan"/>
    /// </summary>
    public class TimeSpanConvertible : IConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="TimeSpanConvertible"/>
        /// </summary>
        public static TimeSpanConvertible Instance { get; } = new TimeSpanConvertible();

        /// <inheritdoc/>
        public object? Convert(object? value)
        {
            if (value == null)
            {
                return null;
            }

            return TimeSpan.Parse(value.ToString()!);
        }
    }
}
