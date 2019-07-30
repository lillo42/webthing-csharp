using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.Collections
{
    [DebuggerTypeProxy(typeof (ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    internal sealed class PropertyCollection : ICollection<Property>, IEquatable<PropertyCollection>
    {
        private readonly LinkedList<Property> _properties = new LinkedList<Property>();
        private readonly Thing _thing;
        
        public IJsonSchemaValidator JsonSchemaValidator { get; set; }

        public PropertyCollection(Thing thing)
        {
            _thing = thing;
        }

        public IEnumerator<Property> GetEnumerator() 
            => _properties.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        public void Add(Property item)
        {
            if (item.Thing == null)
            {
                item.Thing = _thing;
            }

            item.HrefPrefix = _thing.HrefPrefix;
            _properties.AddLast(new PropertyProxy(item, JsonSchemaValidator));
        }

        public void Clear() 
            => _properties.Clear();

        public bool Contains(Property item) 
            => _properties.Contains(item);

        public void CopyTo(Property[] array, int arrayIndex)
            => _properties.CopyTo(array, arrayIndex);

        public bool Remove(Property item) 
            => _properties.Remove(item);

        public int Count => _properties.Count;
        public bool IsReadOnly => false;

        public bool Equals(PropertyCollection other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            return ReferenceEquals(this, other) || Equals(_properties, other._properties);
        }

        public override bool Equals(object obj) 
            => ReferenceEquals(this, obj) || obj is PropertyCollection other && Equals(other);

        public override int GetHashCode() 
            => (_properties != null ? _properties.GetHashCode() : 0);
    }
}
