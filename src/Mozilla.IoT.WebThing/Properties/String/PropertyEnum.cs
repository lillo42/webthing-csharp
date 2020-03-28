using System;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Properties.String
{
    /// <summary>
    /// Represent <see cref="Enum"/> property.
    /// </summary>
    public class PropertyEnum : IProperty
    {
        private readonly Thing _thing;
        private readonly Func<Thing, object?> _getter;
        private readonly Action<Thing, object?> _setter;

        private readonly bool _isNullable;
        private readonly Type _enumType;

        /// <summary>
        /// Initialize a new instance of <see cref="PropertyGuid"/>.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/>.</param>
        /// <param name="getter">The method to get property.</param>
        /// <param name="setter">The method to set property.</param>
        /// <param name="isNullable">If property accepted null value.</param>
        /// <param name="enumType">The enum type.</param>
        public PropertyEnum(Thing thing, Func<Thing, object?> getter, Action<Thing, object?> setter, 
            bool isNullable, Type enumType)
        {
            _thing = thing ?? throw new ArgumentNullException(nameof(thing));
            _getter = getter ?? throw new ArgumentNullException(nameof(getter));
            _setter = setter ?? throw new ArgumentNullException(nameof(setter));
            _isNullable = isNullable;
            _enumType = enumType ?? throw new ArgumentNullException(nameof(enumType));
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

            if(!Enum.TryParse(_enumType, element.GetString(), true, out var value))
            {
                return SetPropertyResult.InvalidValue;
            }

            _setter(_thing, value);
            return SetPropertyResult.Ok;
        }
    }
}
