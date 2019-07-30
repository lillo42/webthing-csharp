using System;
using System.Collections.Generic;
using System.Diagnostics;
using Mozilla.IoT.WebThing.Collections;

namespace Mozilla.IoT.WebThing.DebugView
{
    internal sealed class ICollectionDebugView<T>
    {
        private readonly ICollection<T> _collection;

        public ICollectionDebugView(ICollection<T> collection)
        {
            this._collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                var array = new T[_collection.Count];
                _collection.CopyTo(array, 0);
                return array;
            }
        }
    }
}
