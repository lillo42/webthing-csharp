using System;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Properties
{
    public readonly struct PropertyReadOnly : IProperty
    {
        private readonly Thing _thing;
        private readonly Func<Thing, object> _getter;

        public PropertyReadOnly(Thing thing, Func<Thing, object> getter)
        {
            _thing = thing ?? throw new ArgumentNullException(nameof(thing));
            _getter = getter ?? throw new ArgumentNullException(nameof(getter));
        }

        public object GetValue() 
            => _getter(_thing);

        public SetPropertyResult SetValue(JsonElement element) 
            => SetPropertyResult.ReadOnly;
    }
}
