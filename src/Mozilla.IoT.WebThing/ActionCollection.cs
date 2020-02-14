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

        public event EventHandler<ActionInfo>? Change;

        public ActionCollection()
        {
            _actions = new ConcurrentDictionary<Guid, ActionInfo>();
        }

        public void Add(Guid id, ActionInfo actionInfo)
        {
            _actions.TryAdd(id, actionInfo);

            actionInfo.StatusChanged += OnStatusChange;
            
            var change = Change;
            change?.Invoke(this, actionInfo);
        }

        public bool TryGetValue(Guid id, out ActionInfo? action)
            => _actions.TryGetValue(id, out action);
        
        public bool TryRemove(Guid id, out ActionInfo action)
        {
            var result =_actions.TryRemove(id, out action);
            if (result && action != null)
            {
             
                action.StatusChanged -= OnStatusChange;   
            }
            
            return result;
        }

        private void OnStatusChange(object? sender, EventArgs args)
        {
            var change = Change;
            change?.Invoke(this, (ActionInfo)sender);
        }

        public IEnumerator<ActionInfo> GetEnumerator()
            => _actions.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
