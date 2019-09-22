using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Collections
{
    internal sealed class ActionCollection : IEnumerable<KeyValuePair<string, LinkedList<Action>>>, IEquatable<ActionCollection>
    {
        
        private readonly ConcurrentDictionary<string, LinkedList<Action>> _actions = new ConcurrentDictionary<string, LinkedList<Action>>();
        private readonly object _locker = new object();

        public LinkedList<Action> this[string index] => _actions[index];

        public void Add(Action item)
        {
            var actions = _actions.GetOrAdd(item.Name, name => new LinkedList<Action>());
            lock (_locker)
            {
                actions.AddLast(item);
                item.ActionStatusChanged += (sender, args) =>
                {
                    var @event = ActionStatusChanged;
                    @event?.Invoke(this, args);
                };
            }
        }

        public bool Contains(string actionName)
            => _actions.ContainsKey(actionName);

        public bool Remove(Action item) 
            => _actions.ContainsKey(item.Name) && _actions[item.Name].Remove(item);

        public IEnumerator<KeyValuePair<string, LinkedList<Action>>> GetEnumerator() 
            => _actions.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        public bool Equals(ActionCollection other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            return ReferenceEquals(this, other) || Equals(_actions, other._actions);
        }

        public override bool Equals(object obj) 
            => ReferenceEquals(this, obj) || obj is ActionCollection other && Equals(other);

        public override int GetHashCode()
            => (_actions != null ? _actions.GetHashCode() : 0);
        public event EventHandler<ActionStatusChangedEventArgs> ActionStatusChanged;
    }
}
