using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace Mozilla.IoT.WebThing.Collections
{
    internal sealed class ObservableActionCollection : INotifyCollectionChanged, IEnumerable<KeyValuePair<string, LinkedList<Action>>>, IEquatable<ObservableActionCollection>
    {
        private readonly ConcurrentDictionary<string, LinkedList<Action>> _actions = new ConcurrentDictionary<string, LinkedList<Action>>();
        private readonly object _locker = new object();

        public LinkedList<Action> this[string index] => _actions[index];

        public void Add(Action item)
        {
            var actions = _actions.GetOrAdd(item.Name, name => new LinkedList<Action>());
            lock (_locker)
            {
                
                _actions[item.Name].AddLast(item);
                Notify(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }

        public bool Contains(string actionName)
            => _actions.ContainsKey(actionName);

        public bool Remove(Action item)
        {
            if (_actions.ContainsKey(item.Name) && _actions[item.Name].Remove(item))
            {
                Notify(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
                return true;
            }
            return false;
        }
        
        public IEnumerator<KeyValuePair<string, LinkedList<Action>>> GetEnumerator() 
            => _actions.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();
        
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private void Notify(NotifyCollectionChangedEventArgs eventArgs)
        {
            var change = CollectionChanged;
            change?.Invoke(this, eventArgs);
        }
        
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public bool Equals(ObservableActionCollection other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            return ReferenceEquals(this, other) || Equals(_actions, other._actions);
        }

        public override bool Equals(object obj) 
            => ReferenceEquals(this, obj) || obj is ObservableActionCollection other && Equals(other);

        public override int GetHashCode()
            => (_actions != null ? _actions.GetHashCode() : 0);
    }
}
