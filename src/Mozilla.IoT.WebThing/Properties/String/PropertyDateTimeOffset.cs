using System;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Mozilla.IoT.WebThing.Properties.String
{
    public readonly struct PropertyDateTimeOffset : IProperty
    {
        private readonly Thing _thing;
        private readonly Func<Thing, object> _getter;
        private readonly Action<Thing, object> _setter;

        private readonly bool _isNullable;
        private readonly DateTimeOffset[]? _enums;

        public PropertyDateTimeOffset(Thing thing, Func<Thing, object> getter, Action<Thing, object> setter, 
             bool isNullable, DateTimeOffset[]? enums)
        {
            _thing = thing ?? throw new ArgumentNullException(nameof(thing));
            _getter = getter ?? throw new ArgumentNullException(nameof(getter));
            _setter = setter ?? throw new ArgumentNullException(nameof(setter));
            _isNullable = isNullable;
            _enums = enums;
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

            if (!element.TryGetDateTimeOffset(out var value))
            {
                return SetPropertyResult.InvalidValue;
            }

            if (_enums != null && _enums.Length > 0 &&  !_enums.Contains(value))
            {
                return SetPropertyResult.InvalidValue;
            }

            _setter(_thing, value);
            return SetPropertyResult.Ok;
        }
    }
}
