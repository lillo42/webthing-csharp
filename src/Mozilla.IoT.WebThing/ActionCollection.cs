using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;

namespace Mozilla.IoT.WebThing
{
    public abstract class ActionCollection : IEnumerable<ActionInfo>
    {
        private readonly ConcurrentDictionary<Guid, ActionInfo> _actions;
        public event EventHandler<ActionInfo>? Change;
        protected abstract Type GetActionInfoType();
        
        protected ActionCollection()
        {
            _actions = new ConcurrentDictionary<Guid, ActionInfo>();
        }


        protected abstract bool IsValid(ActionInfo info);

        private ActionInfo Convert(JsonElement element) 
            => (ActionInfo)JsonSerializer.Deserialize(element.GetRawText(), GetActionInfoType());

        public ActionInfo? Add(JsonElement element)
        {
            if (!element.TryGetProperty("input", out var input))
            {
                return null;
            }
            
            var action = Convert(input);
            
            if (IsValid(action))
            {
                _actions.TryAdd(action.GetId(), action);
                return action;
            }

            return null;
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
