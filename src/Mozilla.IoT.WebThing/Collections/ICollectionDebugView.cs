using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mozilla.IoT.WebThing.Collections
{
    internal sealed class ICollectionDebugView<T>
    {
        private readonly ICollection<T> _collection;

        public ICollectionDebugView(ICollection<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            this._collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                var array = new T[this._collection.Count];
                this._collection.CopyTo(array, 0);
                return array;
            }
        }
    }
}
