using System;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Properties
{
    /// <summary>
    /// Represent read only property.
    /// </summary>
    public readonly struct PropertyReadOnly : IProperty
    {
        private readonly Thing _thing;
        private readonly Func<Thing, object> _getter;

        /// <summary>
        /// Initialize a new instance of <see cref="PropertyReadOnly"/>.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/>.</param>
        /// <param name="getter">The method to get property.</param>
        public PropertyReadOnly(Thing thing, Func<Thing, object> getter)
        {
            _thing = thing ?? throw new ArgumentNullException(nameof(thing));
            _getter = getter ?? throw new ArgumentNullException(nameof(getter));
        }

        /// <inheritdoc/>
        public object? GetValue() 
            => _getter(_thing);

        /// <summary>
        /// Always return ReadOnly.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>Always return ReadOnly.</returns>
        public SetPropertyResult SetValue(JsonElement element) 
            => SetPropertyResult.ReadOnly;
    }
}
