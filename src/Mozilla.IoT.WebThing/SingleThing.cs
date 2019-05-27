using System.Collections.Generic;

namespace Mozilla.IoT.WebThing
{
    public class SingleThing : IThingType
    {
        private readonly Thing _thing;

        public SingleThing(Thing thing)
        {
            _thing = thing;
        }
        
        public Thing this[int index] => _thing;

        public IEnumerable<Thing> Things => new[] {_thing};

        public string Name => _thing.Name;
    }
}
