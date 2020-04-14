using System;

namespace Mozilla.IoT.WebThing.Convertibles.Collection
{
    /// <summary>
    /// Convert value to array of T
    /// </summary>
    /// <typeparam name="T">The value.</typeparam>
    public class ArrayConvertible<T> : IConvertible
    {
        private readonly IConvertible _convertible;

        /// <summary>
        /// Initialize a new instance of <see cref="ArrayConvertible{T}"/>.
        /// </summary>
        /// <param name="convertible">The <see cref="IConvertible"/>.</param>
        public ArrayConvertible(IConvertible convertible)
        {
            _convertible = convertible;
        }

        /// <inheritdoc/>
        public object? Convert(object? value)
        {
            if (value == null)
            {
                return null;
            }
            
            if (value is object[] array)
            {
                var result = new T[array.Length];

                for (var i = 0; i < array.Length; i++)
                {
                    result[i] = (T)_convertible.Convert(array[i])!;
                }
                
                return result;
            }

            throw new ArgumentException("Invalid value", nameof(value));
        }
    }
}
