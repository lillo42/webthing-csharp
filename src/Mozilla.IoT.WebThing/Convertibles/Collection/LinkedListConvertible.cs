using System;
using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Convertibles.Collection
{
    /// <summary>
    /// Convert value to <see cref="LinkedList{T}"/>
    /// </summary>
    /// <typeparam name="T">The value.</typeparam>
    public class LinkedListConvertible<T> : IConvertible
    {
        private readonly IConvertible _convertible;

        /// <summary>
        /// Initialize a new instance of <see cref="LinkedListConvertible{T}"/>.
        /// </summary>
        /// <param name="convertible">The <see cref="IConvertible"/>.</param>
        public LinkedListConvertible(IConvertible convertible)
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
                var result = new LinkedList<T>();

                foreach (var item in array)
                {
                    result.AddLast((T)_convertible.Convert(item)!);
                }
                
                return result;
            }
            
            throw new ArgumentException("Invalid value", nameof(value));
        }
    }
}
