using System;
using System.Collections.Generic;
using Mozilla.IoT.WebThing;

namespace Microsoft.AspNetCore.Builder
{
    public class ThingBindingOption
    {
        internal List<Type> ThingsType { get; } = new List<Type>();
        internal List<Thing> Things { get; } = new List<Thing>();

        public bool IsMultiThing { get; set; }
        public void AddThing(Thing thing)
            => Things.Add(thing);

        public void AddThing<T>()
            where T : Thing
            => ThingsType.Add(typeof(T));
    }
}
