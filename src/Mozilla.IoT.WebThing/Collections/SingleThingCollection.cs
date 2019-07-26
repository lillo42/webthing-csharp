using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mozilla.IoT.WebThing.Collections
{
    [DebuggerTypeProxy(typeof (ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class SingleThingCollection : IReadOnlyList<Thing>
    {
        private readonly Thing _thing;

        public SingleThingCollection(Thing thing)
        {
            _thing = thing;
        }


        public IEnumerator<Thing> GetEnumerator()
        {
            yield return _thing;
        }

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        public int Count => 1;

        public Thing this[int index] => _thing;
    }
}
