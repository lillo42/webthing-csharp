using System;
using System.Collections.Generic;
using System.Diagnostics;
using Mozilla.IoT.WebThing.Collections;

namespace Mozilla.IoT.WebThing.DebugView
{
    internal sealed class IThingReadOnlyCollectionDebugView
    {
        private readonly IThingReadOnlyCollection _collection;

        public IThingReadOnlyCollectionDebugView(IThingReadOnlyCollection collection)
        {
            this._collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public Thing[] Items
        {
            get
            {
                var array = new Thing[_collection.Count];
                
                var i = 0;
                foreach (var item in _collection)
                {
                    array[i++] = item;
                }

                return array;
            }
        }
    }
}
