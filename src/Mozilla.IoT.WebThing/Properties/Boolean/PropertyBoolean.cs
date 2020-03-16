using System;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Properties.Boolean
{
    /// <summary>
    /// Represent <see cref="bool"/> property.
    /// </summary>
    public readonly struct PropertyBoolean : IProperty
    {
        private readonly Thing _thing;
        private readonly Func<Thing, object?> _getter;
        private readonly Action<Thing, object?> _setter;

        private readonly bool _isNullable;

        /// <summary>
        /// Initialize a new instance of <see cref="PropertyBoolean"/>.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/>.</param>
        /// <param name="getter">The method to get property.</param>
        /// <param name="setter">The method to set property.</param>
        /// <param name="isNullable">If property accepted null value.</param>
        public PropertyBoolean(Thing thing, Func<Thing, object?> getter, Action<Thing, object?> setter, bool isNullable)
        {
            _thing = thing ?? throw new ArgumentNullException(nameof(thing));
            _getter = getter ?? throw new ArgumentNullException(nameof(getter));
            _setter = setter ?? throw new ArgumentNullException(nameof(setter));
            _isNullable = isNullable;
        }

        /// <summary>
        /// Get value of thing
        /// </summary>
        /// <returns>Value of property thing</returns>
        public object? GetValue() 
            => _getter(_thing);

        /// <summary>
        /// Set value of thing
        /// </summary>
        /// <param name="element">Input value, from buffer</param>
        /// <returns>The <see cref="SetPropertyResult"/>></returns>
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
