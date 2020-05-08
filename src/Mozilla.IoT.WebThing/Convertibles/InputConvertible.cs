using System;
using System.Collections.Generic;
using System.Linq;

namespace Mozilla.IoT.WebThing.Convertibles
{
    /// <summary>
    /// 
    /// </summary>
    public class InputConvertible : IConvertible
    {
        private readonly Dictionary<string, IConvertible> _convertibles;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="convertibles"></param>
        public InputConvertible(Dictionary<string, IConvertible?> convertibles)
        {
            _convertibles = convertibles;
        }

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
