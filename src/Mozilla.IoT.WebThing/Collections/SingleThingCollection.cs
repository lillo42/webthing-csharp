using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Mozilla.IoT.WebThing.DebugView;

namespace Mozilla.IoT.WebThing.Collections
{
    [DebuggerTypeProxy(typeof (IThingReadOnlyCollectionDebugView))]
    [DebuggerDisplay("Count = {Count}")]
    public class SingleThingCollection : IThingReadOnlyCollection
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

        public Thing this[string name] => _thing;
    }
}
