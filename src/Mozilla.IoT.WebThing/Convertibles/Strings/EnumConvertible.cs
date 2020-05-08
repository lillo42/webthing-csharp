using System;

namespace Mozilla.IoT.WebThing.Convertibles.Strings
{
    /// <summary>
    /// Convert value to <see cref="Enum"/>
    /// </summary>
    public class EnumConvertible : IConvertible
    {
        private readonly Type _enum;

        /// <summary>
        /// Initialize a new instance of <see cref="EnumConvertible"/>.
        /// </summary>
        /// <param name="enum">The enum type</param>
        public EnumConvertible(Type @enum)
        {
            _enum = @enum ?? throw new ArgumentNullException(nameof(@enum));
        }

        /// <inheritdoc/>
        public object? Convert(object? value)
        {
            return value == null ? null : Enum.Parse(_enum, value.ToString()!, true);
        }
    }
}
