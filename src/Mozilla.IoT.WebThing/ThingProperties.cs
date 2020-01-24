using System;
using System.Collections.Generic;
using System.Linq;

namespace Mozilla.IoT.WebThing
{
    internal class ThingProperties : IProperties
    {
        private readonly Thing _thing;
        private readonly Dictionary<string, Func<object, object>> _getters;

        public ThingProperties(Thing thing, 
            Dictionary<string, Func<object, object>> getters)
        {
            _thing = thing ?? throw new ArgumentNullException(nameof(thing));
            _getters = getters ?? throw new ArgumentNullException(nameof(getters));
        }

        public Dictionary<string, object>? GetProperties(string? propertyName = null)
        {
            if (propertyName == null)
            {
                return _getters.ToDictionary(getter => getter.Key, 
                    getter => getter.Value(_thing));
            }

            if (_getters.TryGetValue(propertyName, out var get))
            {
                return new Dictionary<string, object>
                {
                    [propertyName] = get(_thing)
                };
            }

            return null;
        }
    }
}
