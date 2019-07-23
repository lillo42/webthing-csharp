using System.Collections;
using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Collections
{
    public class MultipleThingsCollections : IReadOnlyList<Thing>
    {
        private readonly List<Thing> _things;

        public MultipleThingsCollections(List<Thing> things)
        {
            _things = things;
            
            
        }

        public IEnumerator<Thing> GetEnumerator()
            => _things.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        public int Count => _things.Count;

        public Thing this[int index] => _things[index];
    }
}