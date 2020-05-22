using System;

namespace Mozilla.IoT.WebThing.Convertibles.Strings
{
    /// <summary>
    /// Convert value to <see cref="Guid"/>
    /// </summary>
    public class GuidConvertible : IConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="GuidConvertible"/>
        /// </summary>
        public static GuidConvertible Instance { get; } = new GuidConvertible();

        /// <inheritdoc/>
        public object? Convert(object? value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is Guid guid)
            {
                return guid;
            }

            return Guid.Parse(value.ToString()!);
        }
    }
}
