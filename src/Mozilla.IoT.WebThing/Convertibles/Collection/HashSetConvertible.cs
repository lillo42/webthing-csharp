using System;
using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Convertibles.Collection
{
    /// <summary>
    /// Convert value to <see cref="HashSet{T}"/>
    /// </summary>
    public class HashSetConvertible<T> : IConvertible
    {
        private readonly IConvertible _convertible;

        /// <summary>
        /// Initialize a new instance of <see cref="HashSetConvertible{T}"/>.
        /// </summary>
        /// <param name="convertible">The <see cref="IConvertible"/>.</param>
        public HashSetConvertible(IConvertible convertible)
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
                var result = new HashSet<T>();

                foreach (var item in array)
                {
                    result.Add((T)_convertible.Convert(item)!);
                }
                
                return result;
            }
            
            throw new ArgumentException("Invalid value", nameof(value));
        }
    }
}
