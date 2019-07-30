using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mozilla.IoT.WebThing.DebugView;

namespace Mozilla.IoT.WebThing.Collections
{
    [DebuggerTypeProxy(typeof (IThingReadOnlyCollectionDebugView))]
    [DebuggerDisplay("Count = {Count}")]
    public class MultipleThingsCollections : IThingReadOnlyCollection
    {
        private readonly LinkedList<Thing> _things;

        public MultipleThingsCollections(IEnumerable<Thing> things)
        {
            _things = new LinkedList<Thing>(things);
            
        }

        public IEnumerator<Thing> GetEnumerator()
            => _things.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        public int Count => _things.Count;
        
        public Thing this[string name] => _things.FirstOrDefault(x => x.Name == name);
    }
}
