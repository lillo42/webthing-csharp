using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Mozilla.IoT.WebThing.Collections
{
    [DebuggerTypeProxy(typeof (ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class DefaultObservableCollection<T> : IObservableCollection<T>
    {
        private readonly LinkedList<T> _events = new LinkedList<T>();

        public IEnumerator<T> GetEnumerator()
            => _events.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public void Add(T item)
        {
            _events.AddLast(item);
            Notify(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Clear()
        {
            _events.Clear();
            Notify(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        } 

        public bool Contains(T item)
            => _events.Contains(item);

        public void CopyTo(T[] array, int arrayIndex)
            => _events.CopyTo(array, arrayIndex);

        public bool Remove(T item)
        {
            var result = _events.Remove(item);
            
            Notify(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private void Notify(NotifyCollectionChangedEventArgs eventArgs)
        {
            var change = CollectionChanged;
            change?.Invoke(this, eventArgs);
        }

        public int Count => _events.Count;
        public bool IsReadOnly => false;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
