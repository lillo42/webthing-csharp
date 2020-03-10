using System;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Mozilla.IoT.WebThing.Properties.String
{
    public readonly struct PropertyString : IProperty
    {
        private readonly Thing _thing;
        private readonly Func<Thing, object> _getter;
        private readonly Action<Thing, object> _setter;

        private readonly bool _isNullable;
        private readonly int? _minimum;
        private readonly int? _maximum;
        private readonly string[]? _enums;
        private readonly Regex? _pattern;

        public PropertyString(Thing thing, Func<Thing, object> getter, Action<Thing, object> setter, 
             bool isNullable, int? minimum, int? maximum, string? pattern, string[]? enums)
        {
            _thing = thing ?? throw new ArgumentNullException(nameof(thing));
            _getter = getter ?? throw new ArgumentNullException(nameof(getter));
            _setter = setter ?? throw new ArgumentNullException(nameof(setter));
            _isNullable = isNullable;
            _minimum = minimum;
            _maximum = maximum;
            _enums = enums;
            _pattern = new Regex?(pattern, RegexOptions.Compiled);
        }

        public object GetValue() 
            => _getter(_thing);

        public SetPropertyResult SetValue(JsonElement element)
        {
            if (_isNullable && element.ValueKind == JsonValueKind.Null)
            {
                _setter(_thing, null);
                return SetPropertyResult.Ok;
            }
            
            if (element.ValueKind != JsonValueKind.String)
            {
                return SetPropertyResult.InvalidValue;
            }

            var value = element.GetString();
            
            if (_minimum.HasValue && value.Length < _minimum.Value)
            {
                return SetPropertyResult.InvalidValue;
            }
            
            if (_maximum.HasValue && value.Length > _maximum.Value)
            {
                return SetPropertyResult.InvalidValue;
            }

            if (_enums != null && _enums.Length > 0 && !_enums.Contains(value))
            {
                return SetPropertyResult.InvalidValue;
            }

            if (_pattern != null && !_pattern.IsMatch(value))
            {
                return SetPropertyResult.InvalidValue;
            }

            _setter(_thing, value);
            return SetPropertyResult.Ok;
        }
    }
}
