using System;
using System.Collections.Generic;
using System.Linq;

namespace Mozilla.IoT.WebThing.Convertibles
{
    /// <summary>
    /// Convert data for input
    /// </summary>
    public class InputConvertible : IConvertible
    {
        private readonly Dictionary<string, IConvertible?> _convertibles;

        /// <summary>
        /// Initialize a new instance of <see cref="InputConvertible"/>.
        /// </summary>
        /// <param name="convertibles">The convertibles</param>
        public InputConvertible(Dictionary<string, IConvertible?> convertibles)
        {
            _convertibles = convertibles ?? throw new ArgumentNullException(nameof(convertibles));
        }

        /// <inheritdoc/>
        public object? Convert(object? value)
        {
            if (_convertibles == null)
            {
                return value;
            }
            
            var dic = (Dictionary<string, object?>)value!;

            foreach (var (propertyName, propertyValue) in dic.ToArray())
            {
                if (_convertibles.TryGetValue(propertyName, out var convertible) && convertible != null)
                {
                    dic[propertyName] = convertible.Convert(propertyValue);
                }
            }

            return dic;
        }
    }
}
