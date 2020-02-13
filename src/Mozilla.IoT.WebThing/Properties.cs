using System;
using System.Collections.Generic;
using System.Linq;

namespace Mozilla.IoT.WebThing
{
    internal class Properties : IProperties
    {
        private readonly Thing _thing;
        private readonly Dictionary<string, Property> _properties;

        public Properties(Thing thing,
            Dictionary<string, Property> properties)
        {
            _thing = thing ?? throw new ArgumentNullException(nameof(thing));
            _properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }

        public IEnumerable<string> PropertiesNames => _properties.Keys;

        public Dictionary<string, object>? GetProperties(string? propertyName = null)
        {
            if (propertyName == null)
            {
                return _properties.ToDictionary(getter => getter.Key, 
                    getter => getter.Value.Getter(_thing));
            }

            if (_properties.TryGetValue(propertyName, out var property))
            {
                return new Dictionary<string, object>
                {
                    [propertyName] = property.Getter(_thing)
                };
            }

            return null;
        }

        public SetPropertyResult SetProperty(string propertyName, object value)
        {
            if (_properties.TryGetValue(propertyName, out var property))
            {
                value = property.Mapper.Map(value);
                if (property.Validator.IsValid(value))
                {
                    property.Setter(_thing, value);
                    return SetPropertyResult.Ok;
                }

                return SetPropertyResult.InvalidValue;
            }

            return SetPropertyResult.NotFound;
        }
    }
}
