using System;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Properties.Boolean
{
    public readonly struct PropertyBoolean : IProperty
    {
        private readonly Thing _thing;
        private readonly Func<Thing, object> _getter;
        private readonly Action<Thing, object> _setter;

        private readonly bool _isNullable;

        public PropertyBoolean(Thing thing, Func<Thing, object> getter, Action<Thing, object> setter, bool isNullable)
        {
            _thing = thing ?? throw new ArgumentNullException(nameof(thing));
            _getter = getter ?? throw new ArgumentNullException(nameof(getter));
            _setter = setter ?? throw new ArgumentNullException(nameof(setter));
            _isNullable = isNullable;
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
            
            if (element.ValueKind == JsonValueKind.True)
            {
                _setter(_thing, true);
                return SetPropertyResult.Ok;
            }
            
            if (element.ValueKind == JsonValueKind.False)
            {
                _setter(_thing, false);
                return SetPropertyResult.Ok;
            }

            return SetPropertyResult.InvalidValue;
        }
    }
}
