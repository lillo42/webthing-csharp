using System;
using System.Linq;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Properties.String
{
    /// <summary>
    /// Represent <see cref="DateTimeOffset"/> property.
    /// </summary>
    public readonly struct PropertyDateTimeOffset : IProperty
    {
        private readonly Thing _thing;
        private readonly Func<Thing, object?> _getter;
        private readonly Action<Thing, object?> _setter;

        private readonly bool _isNullable;
        private readonly DateTimeOffset[]? _enums;

        /// <summary>
        /// Initialize a new instance of <see cref="PropertyDateTimeOffset"/>.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/>.</param>
        /// <param name="getter">The method to get property.</param>
        /// <param name="setter">The method to set property.</param>
        /// <param name="isNullable">If property accepted null value.</param>
        /// <param name="enums">The possible values this property could have.</param>
        public PropertyDateTimeOffset(Thing thing, Func<Thing, object?> getter, Action<Thing, object?> setter, 
             bool isNullable, DateTimeOffset[]? enums)
        {
            _thing = thing ?? throw new ArgumentNullException(nameof(thing));
            _getter = getter ?? throw new ArgumentNullException(nameof(getter));
            _setter = setter ?? throw new ArgumentNullException(nameof(setter));
            _isNullable = isNullable;
            _enums = enums;
        }

        /// <inheritdoc/>
        public object? GetValue() 
            => _getter(_thing);

        /// <inheritdoc/>
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
