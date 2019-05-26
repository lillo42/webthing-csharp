using System.Collections.Generic;
using Mozilla.IoT.WebThing;

namespace Microsoft.AspNetCore.Builder
{
    public class ThingBindingOption
    {
        private readonly IList<Thing> _things = new List<Thing>();

        internal IList<Thing> Things => _things;
        
        public void AddThing(Thing thing) 
            => _things.Add(thing);
    }
}
