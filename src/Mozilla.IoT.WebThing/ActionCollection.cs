using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Mozilla.IoT.WebThing.Actions;

namespace Mozilla.IoT.WebThing
{
    public class ActionCollection : IEnumerable<ActionInfo>
    {
        private readonly ConcurrentDictionary<Guid, ActionInfo> _actions;

        public event EventHandler<ActionInfo> Added;

        public ActionCollection()
        {
            _actions = new ConcurrentDictionary<Guid, ActionInfo>();
        }

        public void Add(Guid id, ActionInfo actionInfo)
        {
            _actions.TryAdd(id, actionInfo);

            var added = Added;
            added?.Invoke(this, actionInfo);
        }

        public bool TryGetValue(Guid id, out ActionInfo action)
            => _actions.TryGetValue(id, out action);
        
        public bool TryRemove(Guid id, out ActionInfo action)
            => _actions.TryRemove(id, out action);

        public IEnumerator<ActionInfo> GetEnumerator()
            => _actions.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
